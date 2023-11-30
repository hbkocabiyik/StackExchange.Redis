using System.Security.Cryptography.X509Certificates;

namespace StackExchangeRedis.API.Helper
{
    public class x509
    {
        public X509Certificate2 FindCertificateByThumbprint(string storeName, string thumbprint)
        {
            if (string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(thumbprint))
                return null;

            X509Certificate2 certificate = null;

            using (var store = new X509Store(storeName, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);

                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                if (certs.Count > 0)
                    certificate = certs[0];

                store.Close();
            }

            return certificate;
        }

    }
}
