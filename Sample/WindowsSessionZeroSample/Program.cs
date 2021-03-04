using System;
using XiangJiang.Windows.Abstractions.Enums;
using XiangJiang.Windows.Abstractions.Models;
using XiangJiang.Windows.Service;

namespace WindowsSessionZeroSample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceConfigure<SampleService>.Run(null, new ServiceOption
                {
                    ServiceName = "WindowsSessionZeroSample",
                    StartPattern = ServiceStartPattern.Manually
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
