using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommonHelper
{
    public static class HashHelper
    {
        private static string ToHashString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
        public static string ComputeSha256Hash(Stream stream)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(stream);
                return ToHashString(bytes);
            }
        }
    }
}