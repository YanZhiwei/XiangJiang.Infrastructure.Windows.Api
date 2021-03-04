using System;
using System.IO;
using XiangJiang.Logging.Abstractions;
using XiangJiang.Logging.File;
using XiangJiang.Windows.Abstractions;
using XiangJiang.Windows.Abstractions.Models;
using XiangJiang.Windows.Api;
using XiangJiang.Windows.Api.Core;

namespace WindowsSessionZeroSample
{
    public sealed class SampleService : IWindowsService
    {
        private readonly ILogService _logService;
        private ServiceOption _serviceOption;

        public SampleService()
        {
            _logService = new FileLogService();
        }

        public void Continued()
        {
            _logService.Debug($"Service Name:{_serviceOption.ServiceName} Continued");
        }

        public void Paused()
        {
            _logService.Debug($"Service Name:{_serviceOption.ServiceName} Paused");
        }

        public void Start(string[] args, ServiceOption option)
        {
            _serviceOption = option ?? throw new ArgumentNullException(nameof(option));
            _logService.Debug($"Service Name:{_serviceOption.ServiceName} Start");
            try
            {
                var childProc = Path.Combine(AppContext.BaseDirectory, "SampleWinFormsApp.exe");
                var processId = Win32Process.Start(childProc, "YanZh", true);
                _logService.Info($"Start SampleWinFormsApp.exe ==>{processId}");
            }
            catch (Win32ErrorCodeException ex)
            {
                _logService.Error($"{ex.Message}==》{ex.Win32ErrorCode}", ex);
            }
            catch (Exception ex)
            {
                _logService.Error($"{ex.Message}", ex);
            }
        }

        public void Stop()
        {
            _logService.Debug($"Service Name:{_serviceOption.ServiceName} Stop");
            _logService?.Dispose();
        }
    }
}