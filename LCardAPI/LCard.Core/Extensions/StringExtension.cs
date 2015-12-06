using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCard.Core.Extensions
{
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string FixEncoding(this string str)
        {
            Encoding iso = Encoding.GetEncoding("windows-1251");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = GetBytes(str);
            byte[] isoBytes = Encoding.Convert(iso, utf8, utfBytes);
            string msg = utf8.GetString(isoBytes);
            str = msg.Replace("\0", "");
            return str;
        }


        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
