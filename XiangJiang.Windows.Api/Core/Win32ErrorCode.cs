namespace XiangJiang.Windows.Api.Core
{
    public enum Win32ErrorCode : int
    {
        UserNotFound = 3,
        UserNotSignIn = 2,
        GetUserTokenFailed = 1,
        Win32Error = 0,
        Win32ErrorProcessStartFailed = -1
    }
}