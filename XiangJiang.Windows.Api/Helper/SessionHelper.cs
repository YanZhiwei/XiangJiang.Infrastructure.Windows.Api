using System;
using System.Runtime.InteropServices;
using XiangJiang.Core;
using XiangJiang.Windows.Api.Core;
using XiangJiang.Windows.Api.Enums;
using XiangJiang.Windows.Api.Models;

namespace XiangJiang.Windows.Api.Helper
{
    public static class SessionHelper
    {
        public static WindowsLockState GetLockState(this SessionInfo session)
        {
            Checker.Begin().NotNull(session, nameof(session));
            return GetLockState(session.SessionId);
        }

        public static WindowsLockState GetLockState(int sessionId)
        {
            var osVersion = Environment.OSVersion;
            var isWindows7 = osVersion.Platform == PlatformID.Win32NT && osVersion.Version.Major == 6 &&
                             osVersion.Version.Minor == 1;

            var serverHandle = Win32Api.WTSOpenServer(Environment.MachineName);
            try
            {
                var result = Win32Api.WTSQuerySessionInformation(
                    serverHandle,
                    sessionId,
                    WTS_INFO_CLASS.WTSSessionInfoEx,
                    out var ppBuffer,
                    out var pBytesReturned
                );

                if (!result)
                    return WindowsLockState.Unknown;

                var sessionInfoEx = Marshal.PtrToStructure<WTSINFOEX>(ppBuffer);

                if (sessionInfoEx.Level != 1)
                    return WindowsLockState.Unknown;

                var lockState = sessionInfoEx.Data.WTSInfoExLevel1.SessionFlags;
                Win32Api.WTSFreeMemoryEx(WTS_TYPE_CLASS.WTSTypeSessionInfoLevel1, ppBuffer, pBytesReturned);
                return GetLockState(lockState, isWindows7);
            }
            finally
            {
                if (serverHandle != IntPtr.Zero)
                    Win32Api.WTSCloseServer(serverHandle);
            }
        }

        private static WindowsLockState GetLockState(int lockState, bool isWindows7)
        {
            const int wtsSessionStateLock = 0;
            const int wtsSessionStateUnlock = 1;

            switch (lockState)
            {
                case wtsSessionStateLock when isWindows7:
                    return WindowsLockState.Unlocked;

                case wtsSessionStateLock:
                    return WindowsLockState.Locked;

                case wtsSessionStateUnlock when isWindows7:
                    return WindowsLockState.Locked;

                case wtsSessionStateUnlock:
                    return WindowsLockState.Unlocked;

                default:
                    return WindowsLockState.Unknown;
            }
        }
    }
}