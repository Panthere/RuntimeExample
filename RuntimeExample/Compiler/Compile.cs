using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace RuntimeExample.Compiler
{
    public class Compile
    {
        public static bool FromSource(string codeFile, string outFile)
        {
            // Start by defining some variables we will need
            // Provider options sets CompilerVersion to v2.0 (.NET 2.0)
            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v2.0" } });
            CompilerParameters cp = new CompilerParameters();

            string encKey = "MyVerySecurePassword";
            string resPath = Environment.CurrentDirectory + "\\RuntimeCode";
            

            // Change settings
            string mySource = File.ReadAllText(codeFile);
            // Set the resource name
            mySource = mySource.Replace("RESOURCE_NAME", Path.GetFileNameWithoutExtension(resPath));
            // Set the decryption key
            mySource = mySource.Replace("DECRYPTION_KEY", encKey);

            // Create a resource (relies on paths, get over it)
            byte[] runtimeStub = File.ReadAllBytes("..\\..\\..\\RuntimeCode\\bin\\Debug\\RuntimeCode.exe");
            byte[] encStub = Encrypt(runtimeStub, encKey);

            // Write the resource to disk at the resPath for easy embedding later on
            File.WriteAllBytes(resPath, encStub);

            // Start compiling the stub code
            // We want to generate an exe
            cp.GenerateExecutable = true;
            // We want to add our resource to our file
            cp.EmbeddedResources.Add(resPath);
            // We want to output our assembly to this file
            cp.OutputAssembly = outFile;
            // We don't want warnings to stop compilation
            cp.TreatWarningsAsErrors = false;

            // Compile as a windows forms application
            // More info: https://msdn.microsoft.com/en-us/library/6ds95cz0.aspx
            cp.CompilerOptions += "/target:winexe";

            CompilerResults cr = provider.CompileAssemblyFromSource(cp, mySource);

            if (cr.Errors.Count > 0)
            {
                foreach (CompilerError err in cr.Errors)
                {
                    Console.WriteLine("Loader Code Error at line {0}, message {1}", err.Line, err.ErrorText);
                }

                // Clean up the resource we created
                File.Delete(resPath);
                return false;
            }

            // Clean up the resource we created
            File.Delete(resPath);
            return true;
        }
        private static byte[] Encrypt(byte[] data, string key)
        {
            // Simple XOR process to somewhat encrypt the data (very weak)

            // Declare the key as bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            // Declare a new index for it (probably not needed)
            int keyIndex = 0;

            // Loop data
            for (int i = 0; i < data.Length; i++ )
            {
                // If our key index is greater than the length of key bytes we reset it to zero
                if (keyIndex >= keyBytes.Length)
                    keyIndex = 0;
                // Set the byte at the current data position to the data xor'd by the current byte of the key
                data[i] = (byte)(data[i] ^ keyBytes[keyIndex]);
                // Increase our key index (probably a better way to do the index)
                keyIndex++;
            }
            return data;
        }
        private static byte[] Decrypt(byte[] data, string key)
        {
            // Simple XOR process to reverse the above
            // Read above to understand
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
