using OtpNet;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Ayalon.MFA.Core
{
    public class TwoFactorAuth
    {
        private string secretKey;

        public TwoFactorAuth()
        {
            secretKey = GenerateSecretKey();
        }

        public string GenerateCode()
        {
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            return totp.ComputeTotp();
        }

        public bool ValidateCode(string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            return totp.VerifyTotp(code, out long timeStepMatched, new VerificationWindow(0, 0));
        }

        public string GenerateSecretKey()
        {
            byte[] key = new byte[20];
            RandomNumberGenerator.Fill(key);
            return Base32Encoding.ToString(key);
        }

        //public string EncryptSecretKey(string secretKey)
        //{
        //    return Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey));
        //}

        //public string DecryptSecretKey(string encryptedSecretKey)
        //{
        //    return Encoding.UTF8.GetString(Convert.FromBase64String(encryptedSecretKey));
        //}

        public string EncryptSecretKey(string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("The secret key cannot be null or empty!");
            }

            byte[] dataBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] encryptedData = ProtectedData.Protect(dataBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public string DecryptSecretKey(string encryptedSecretKey)
        {
            if (string.IsNullOrEmpty(encryptedSecretKey))
            {
                throw new Exception("The encrypted secret key cannot be null or empty!");
            }

            byte[] encryptedData = Convert.FromBase64String(encryptedSecretKey);
            byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedData);
        }

        public void SetSecretKey(string base32Secret)
        {
            secretKey = base32Secret;
        }
    }
}