using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetReadData.Lusb
{
    public class Lusbapi
    {
        //library version
        public const UInt32 VERSION_MAJOR_LUSBAPI = 0x03;
        public const UInt32 VERSION_MINOR_LUSBAPI = 0x04;
        public const UInt32 CURRENT_VERSION_LUSBAPI = ((VERSION_MAJOR_LUSBAPI << 0x10) | VERSION_MINOR_LUSBAPI);
        public const Int32 INVALID_HANDLE_VALUE = -1;

        //extern "C" DWORD WINAPI GetDllVersion(void)
        [DllImport("Lusbapi.dll")]
        internal static extern UInt32 GetDllVersion();

        [DllImport("Lusbapi.dll", CharSet = CharSet.Ansi)]
        internal static extern IntPtr CreateLInstance(string DeviceName);


        public const short MAX_VIRTUAL_SLOTS_QUANTITY_LUSBAPI = 127;

    }
}
