#nullable enable
using System;
using System.Text;

namespace BinarySerializer
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] bytes, int? align = null, string? newLinePrefix = null, int? maxLines = null)
        {
            if (bytes == null) 
                throw new ArgumentNullException(nameof(bytes));
            
            StringBuilder result = new();
            const string hexAlphabet = "0123456789ABCDEF";
            
            int curLine = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0 && align.HasValue && i % align == 0)
                {
                    curLine++;
                    if (curLine >= maxLines)
                    {
                        result.Append("...");
                        return result.ToString();
                    }
                    result.Append(Environment.NewLine + newLinePrefix);
                }
                byte B = bytes[i];
                result.Append(hexAlphabet[B >> 4]);
                result.Append(hexAlphabet[B & 0xF]);

                if (i < bytes.Length - 1) 
                    result.Append(' ');
            }

            return result.ToString();
        }
    }
}