# XiangJiang.Windows.Api

[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/XiangJiang.Windows.Api.svg)](https://www.nuget.org/packages/XiangJiang.Windows.Api) [![nuget](https://img.shields.io/nuget/dt/XiangJiang.Windows.Api.svg)](https://www.nuget.org/packages/XiangJiang.Windows.Api)

基于.NET 5.0,.NET Framework 4.6.1 的 Windows Api 开发辅助类

1. Windows Service 中创建进程；
2. Desktop 操作：隐藏任务栏，截图，显示任务栏，主屏幕截图；
3. 键盘鼠标操作：鼠标左键点击，粘贴，粘贴，按键，滚动，打字输入；
4. Window 操作：关闭窗口，设置关闭按钮不可用，获取坐标，获取矩形，获取大小，获取Title，隐藏，获取当前活动窗口，最前窗口，由此可判断一个窗口是否最前，根据句柄判断是否是个窗口，最大化窗口，最小化窗口，移动，调整大小，还原窗口，截图，设置该窗口句柄被激活并成为当前窗口，根据句柄判断一个窗口是否可见，强制窗口前置；
5. Windows 搜索：根据类名查询句柄，根据窗口标题查询句柄，根据句柄获取子句柄列表，获取所有窗口列表，根据类名查询句柄，不能查询子窗口，根据窗口名称查询句柄，不能查询子窗口

喜欢这个项目的话就 Star、Fork、Follow
项目开发模式：日常代码积累+网络搜集

## 本项目已得到[JetBrains](https://www.jetbrains.com/shop/eform/opensource)的支持！

<img src="https://www.jetbrains.com/shop/static/images/jetbrains-logo-inv.svg" height="100">

## 请注意：

一旦使用本开源项目以及引用了本项目或包含本项目代码的公司因为违反劳动法（包括但不限定非法裁员、超时用工、雇佣童工等）在任何法律诉讼中败诉的，项目作者有权利追讨本项目的使用费，或者直接不允许使用任何包含本项目的源代码！任何性质的`996公司`需要使用本类库，请联系作者进行商业授权！其他企业或个人可随意使用不受限。

## 建议开发环境

操作系统：Windows 10 1903 及以上版本  
开发工具：VisualStudio2019 v16.9 及以上版本  
SDK：.NET Framework 4.6.1 及以上版本

## 安装程序包

.NET ≥ .NET Framework 4.6.1

```shell
PM> Install-Package XiangJiang.Windows.Api
```

### Windows Service 中创建进程

在 Windows Service 中创建进程，实现 Session 0 穿透，并可以设置进程权限

```csharp
var childProc = Path.Combine(AppContext.BaseDirectory, "SampleWinFormsApp.exe");
var processId = Win32Process.Start(childProc, "YanZh", true);
_logService.Info($"Start SampleWinFormsApp.exe ==>{processId}");
```

### 根据窗口名字查找，并且聚焦，点击按钮

```csharp
var hwind = WindowSearch.GetWindows().FirstOrDefault(c => c.GetTitle() == "Form1");
var mainHandle = hwind.HWnd;
if (mainHandle != IntPtr.Zero)
{
    var btnHandle = WindowSearch.GetChildWindowByTitle(mainHandle, "button1");
    if (!Window.IsFocused(mainHandle))
        Window.SetFocused(mainHandle);
    MouseKeyboard.LeftClick(btnHandle);
}
```

### 获取 Windows Session 列表

```csharp
var sessions = WindowsCore.GetSessions();
if (sessions?.Any() ?? false)
    foreach (var session in sessions)
    {
        if (string.IsNullOrEmpty(session.Domain)) continue;
        Console.WriteLine(
            $"UserName：{session.UserName}  SessionId:{session.SessionId} State:{session.ConnectionState}");
    }
```

### 获取窗口列表

```csharp
var windows = WindowsCore.GetWindows();
foreach (var win in windows)
    if (win.IsVisible() && !string.IsNullOrEmpty(win.GetTitle()))
        Console.WriteLine(win.GetTitle());

Console.ReadLine();
```

### 获取 WindowStation 名称列表

```csharp
public static string DumpWinLogonDesktop()
{
    var builder = new StringBuilder();

    foreach (var station in WindowsCore.GetWindowStationNames())
    {
        builder.AppendLine("【WindowsStation】" + station + "");
        foreach (var desktop in WindowsCore.GetDesktopNames(station))
        {
            builder.AppendLine("        【Desktop】: " + desktop + "");
            try
            {
                foreach (var window in WindowsCore.GetWindows(desktop))
                    try
                    {
                        if (window.IsVisible())
                            builder.AppendLine(
                                $"                【Windows】 hWnd {window.HWnd}: Class={window.GetClassName()}, Text={window.GetTitle()}");
                    }
                    catch (Exception ex)
                    {
                        builder.AppendLine($"        hWnd {window.HWnd} Error: {ex.Message}\r\n");
                    }
            }
            catch (Exception ex)
            {
                builder.AppendLine("        " + ex.Message + "");
            }
        }
    }

    return builder.ToString();
}
```
