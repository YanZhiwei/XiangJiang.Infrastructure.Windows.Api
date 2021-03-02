using System;
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public readonly struct ProcessInformation
    {
        public readonly IntPtr hProcess;
        public readonly IntPtr hThread;
        public readonly int dwProcessId;
        public readonly int dwThreadId;
    }

    public struct TokenUser
    {
        public SidAndAttributes User;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SidAndAttributes
    {
        public IntPtr Sid;
        public int Attributes;
    }
}