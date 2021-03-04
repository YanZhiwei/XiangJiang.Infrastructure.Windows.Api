using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using XiangJiang.Core;
using XiangJiang.Windows.Api.Core;
using XiangJiang.Windows.Api.Enums;
using XiangJiang.Windows.Api.Models;

namespace XiangJiang.Windows.Api
{
    public sealed class Win32Process
    {
        public static int Start(string processPath, string targetWindowsUser, bool runAsAdmin)
        {
            return Start(processPath, targetWindowsUser, string.Empty, runAsAdmin);
        }

        public static int Start(string processPath, string targetWindowsUser, string args = "",
            bool runAsAdmin = false)
        {
            Checker.Begin().CheckFileExists(processPath)
                .NotNullOrEmpty(targetWindowsUser, nameof(targetWindowsUser));

            var targetSession = GetSessionId(targetWindowsUser);
            if (targetSession.ConnectionState != WTS_CONNECTSTATE_CLASS.WTSActive)
                throw new Win32ErrorCodeException($"TargetWindowsUser:{targetWindowsUser} not active",
                    Win32ErrorCode.UserNotActive);

            if (!Win32Api.WTSQueryUserToken(targetSession.SessionId, out var hToken))
                throw new Win32ErrorCodeException($"TargetWindowsUser:{targetWindowsUser} get token failed",
                    Win32ErrorCode.GetUserTokenFailed);

            var environmentPtr = IntPtr.Zero;
            if (!Win32Api.CreateEnvironmentBlock(ref environmentPtr, hToken, false))
                throw new Win32ErrorCodeException("CreateEnvironmentBlock('" + hToken + "')");

            var dwCreationFlags = StandardDef.NormalPriorityClass | StandardDef.CreateUnicodeEnvironment |
                                  StandardDef.ExtendedStartupInfoPresent;

            if (args != null && !args.StartsWith(" "))
                args = $" {args}";

            var startupInfo = new StartupInfoEx();
            var token = runAsAdmin ? GetProcessElevation(hToken) : hToken;
            startupInfo.StartupInfo.cb = Marshal.SizeOf(startupInfo);
            var childProcStarted = Win32Api.CreateProcessAsUser(
                token,
                processPath,
                args,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                dwCreationFlags,
                environmentPtr,
                Path.GetDirectoryName(processPath),
                ref startupInfo,
                out var tProcessInfo
            );

            if (!childProcStarted)
                throw new Win32ErrorCodeException($"TargetWindowsUser:{targetWindowsUser} start failed",
                    Win32ErrorCode.Win32ErrorProcessStartFailed);

            return tProcessInfo.dwProcessId;
        }

        private static SessionInfo GetSessionId(string targetWindowsUser)
        {
            var sessions = WindowsCore.GetSessions();

            if (!(sessions?.Any() ?? false))
                throw new Win32ErrorCodeException($"TargetWindowsUser:{targetWindowsUser} does not exist",
                    Win32ErrorCode.UserNotFound);
            var targetSession = sessions.FirstOrDefault(c =>
                c.UserName.Equals(targetWindowsUser, StringComparison.OrdinalIgnoreCase));
            return targetSession;
        }

        private static IntPtr GetProcessElevation(IntPtr hToken)
        {
            var tokenInfLength = 0;
            Win32Api.GetTokenInformation(hToken, TokenInformationClass.TokenLinkedToken, IntPtr.Zero,
                tokenInfLength, out tokenInfLength);
            var tokenInformation = Marshal.AllocHGlobal(tokenInfLength);
            var result = Win32Api.GetTokenInformation(hToken, TokenInformationClass.TokenLinkedToken, tokenInformation,
                tokenInfLength, out tokenInfLength);

            try
            {
                if (!result) throw new Win32ErrorCodeException("GetProcessElevation failed");

                var tokenUser = (TokenUser)Marshal.PtrToStructure(tokenInformation, typeof(TokenUser));
                return tokenUser.User.Sid;
            }
            finally
            {
                Marshal.FreeHGlobal(tokenInformation);
            }
        }
    }
}