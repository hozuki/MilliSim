using System.Runtime.InteropServices;

namespace OpenMLTD.MilliSim.Theater.Interop {
    internal static class NativeStructures {

        [StructLayout(LayoutKind.Sequential)]
        internal struct LVGROUP {

            public int cbSize;
            public int mask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHeader;
            public int cchHeader;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszFooter;
            public int cchFooter;
            public int iGroupId;
            public int stateMask;
            public int state;
            public int uAlign;

        }

    }
}
