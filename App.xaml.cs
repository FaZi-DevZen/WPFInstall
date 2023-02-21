using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MusicTeachingInstall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutexName = "MusicTeachingInstall";
            Mutex singleInstanceWatcher = new Mutex(false, mutexName, out createdNew);
            if (!createdNew)
            {
                Environment.Exit(-1);
            }
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName("MusicTeachingWindow");
            if (p != null && p.Count() > 0)
            {
                MessageBox.Show("请退出正在运行的“音乐教学PC端”程序");
                Environment.Exit(0);
            }

            // UI Thread Exception Processing
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            // 当前 AppDomain 内的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // 异步操作的异常
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            
        }
    }
}
