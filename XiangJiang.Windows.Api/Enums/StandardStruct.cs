using System.Runtime.InteropServices;

namespace XiangJiang.Windows.Api.Enums
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTSINFOEX
    {
        public uint Level;
        public uint Reserved;
        public WTSINFOEX_LEVEL Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WTSINFOEX_LEVEL
    {
        public WTSINFOEX_LEVEL1 WTSInfoExLevel1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WTSINFOEX_LEVEL1
    {
        public uint SessionId;
        public WTS_CONNECTSTATE_CLASS SessionState;
        public int SessionFlags;
    }
}