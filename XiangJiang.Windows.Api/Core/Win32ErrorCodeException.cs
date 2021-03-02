using System.ComponentModel;
using System.Runtime.InteropServices;

namespace XiangJiang.Windows.Api.Core
{
    /// <summary>
    ///     Windows Api自定义异常
    /// </summary>
    public class Win32ErrorCodeException : Win32Exception
    {
        #region Constructors

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="context">异常信息</param>
        public Win32ErrorCodeException(string context, Win32ErrorCode win32ErrorCode = Win32ErrorCode.Win32Error)
        {
            var error = Marshal.GetLastWin32Error();
            var innerException = new Win32Exception(error);
            Win32ErrorCode = win32ErrorCode;
            Message = $"{context}: (Error Code {error}) {innerException.Message}";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     详细异常信息
        /// </summary>
        public override string Message { get; }


        public readonly Win32ErrorCode Win32ErrorCode;

        #endregion Properties
    }
}