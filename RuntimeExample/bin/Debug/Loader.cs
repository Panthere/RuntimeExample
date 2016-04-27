using System;
using System.Reflection;
using System.IO;
using System.Text;
namespace LoaderExample
{
    public static class Program
    {
        private static Assembly ourAssembly = typeof(Program).Assembly;
        const string RES_NAME = "RESOURCE_NAME";
        const string DEC_KEY = "DECRYPTION_KEY";
        public static void Main(string[] args)
        {

            // Get our resource and decrypt it
            byte[] encCode = GetResource(RES_NAME);
            byte[] decCode = Decrypt(encCode, DEC_KEY);
            
            // Load our decrypted assembly
            Assembly codeAssembly = Assembly.Load(decCode);

            // Invoke our runtime code assembly
            // Note that we do NOT need to create an instance using Activator as it is STATIC (shared).
            codeAssembly.EntryPoint.Invoke(null, new object[] { new string[] { RES_NAME, DEC_KEY } });
        }
        private static byte[] GetResource(string name)
        {
            // Read the stream from the resources of our assembly
            using (Stream s = ourAssembly.GetManifestResourceStream(name))
            using (MemoryStream ms = new MemoryStream())
            {           
                // Small buffer
                byte[] buf = new byte[2048];
                int byteCount = 0;
                while ((byteCount = s.Read(buf, 0, buf.Length)) > 0)
                {
                    ms.Write(buf, 0, byteCount);
                }
                return ms.ToArray();
            }
        }
        private static byte[] Decrypt(byte[] data, string key)
        {
            // Simple XOR process to reverse encryption
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            int keyIndex = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (keyIndex >= keyBytes.Length)
                    keyIndex = 0;
                data[i] = (byte)(keyBytes[keyIndex] ^ data[i]);
                keyIndex++;
            }
            return data;
        }
    }
}