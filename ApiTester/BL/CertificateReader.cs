using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;

namespace ApiTester.BL
{
    public class CertificateReader
    {
        private byte[] _merchantIdentifier;
        private ECPrivateKeyParameters _ecPrivateKeyParameters;
        private ECPublicKeyParameters _ecPublicKeyParameters;

        /// <summary> Gets the <see cref="ECPrivateKeyParameters"/> private key parameters. </summary>
        public ECPrivateKeyParameters PrivateKeyParameters { get { return _ecPrivateKeyParameters; } }

        /// <summary> Gets a value indicating whether we found merchant identifier (OID) on the certificate. </summary>
        public bool FoundMerchantId { get; private set; }

        /// <summary> Gets the valid from. </summary>
        public string ValidFrom { get; private set; }

        /// <summary> Gets the valid to. </summary>
        public string ValidTo { get; private set; }

        /// <summary> Gets the certificate subject. </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Sets the private key from P12.
        /// </summary>
        /// <param name="pathToP12File">The path to P12 file.</param>
        /// <param name="passwordToP12File">The password to P12 file.</param>
        public void SetPrivateKeyFromP12(string pathToP12File, string passwordToP12File)
        {
            if (!File.Exists(pathToP12File)) throw new ArgumentException(string.Format("Invalid path specified for PFX/P12 file: {0}", pathToP12File));

            using (StreamReader reader = new StreamReader(pathToP12File))
            {
                var fs = reader.BaseStream;
                SetMerchantIdentifierField(fs, passwordToP12File);

                fs.Position = 0;
                SetPrivateKey(fs, passwordToP12File);
            }
        }

        /// <summary>
        /// Sets the private key.
        /// </summary>
        /// <param name="fs">The fs.</param>
        /// <param name="passWord">The pass word.</param>
        private void SetPrivateKey(Stream fs, string passWord)
        {
            Pkcs12Store store = new Pkcs12Store(fs, passWord.ToCharArray());

            foreach (string n in store.Aliases)
            {
                if (store.IsKeyEntry(n))
                {
                    AsymmetricKeyEntry asymmetricKey = store.GetKey(n);

                    if (asymmetricKey.Key.IsPrivate)
                    {
                        _ecPrivateKeyParameters = asymmetricKey.Key as ECPrivateKeyParameters;
                    }
                    else
                    {
                        _ecPublicKeyParameters = asymmetricKey.Key as ECPublicKeyParameters;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the merchant identifier field.
        /// </summary>
        /// <param name="fs">The <see cref="Stream"/>.</param>
        /// <param name="password">The password.</param>
        private void SetMerchantIdentifierField(Stream fs, string password)
        {
            byte[] rawTmp = new byte[fs.Length];
            fs.Read(rawTmp, 0, (Int32)fs.Length);

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            //NOTE: the subject name string can contain part of the subject name, whole but not more characters than the actual subject name
            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindBySubjectName, "merchant.com.ippayments.winkwink", false);

            X509Certificate2 x509Cert = certificates[0];

            //X509Certificate2 x509Cert = new X509Certificate2(
            //                            rawTmp,
            //                            password,
            //                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            rawTmp = null;

            // TODO: Check if using the Oid is better
            rawTmp = x509Cert.Extensions[7].RawData;
            ValidFrom = x509Cert.GetEffectiveDateString();
            ValidTo = x509Cert.GetExpirationDateString();
            Subject = GetCnSubjectName(x509Cert.SubjectName.Name);

            using (var stream = new MemoryStream(64))
            using (var sr = new BinaryWriter(stream))
            {
                sr.Write(rawTmp, 2, 64);
                sr.Flush();
                rawTmp = null;

                //var streamTmp = stream.GetBuffer();

                var ascStr = Encoding.ASCII.GetString(stream.GetBuffer());

                rawTmp = new byte[ascStr.Length / 2];
                for (int i = 0; i < rawTmp.Length; i++)
                {
                    rawTmp[i] = Convert.ToByte(ascStr.Substring(i * 2, 2), 16);
                }

                _merchantIdentifier = rawTmp;
                FoundMerchantId = true;
            }
        }

        private string GetCnSubjectName(string fullName)
        {
            var nameParts = fullName.Split(',').ToList();
            nameParts = nameParts.Select(s => s.Trim()).ToList();

            string result = nameParts.FirstOrDefault(item => item.StartsWith("CN"));
            if (result != null)
            {
                result = result.Substring(result.IndexOf(":", StringComparison.Ordinal) + 1).TrimStart();
            }

            return result;
        }


    }
}