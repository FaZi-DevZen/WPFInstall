using Microsoft.Win32;
using MusicTeachingInstall.Helpers;
using MusicTeachingInstall.JsonModels;
using MusicTeachingInstall.WindowsBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicTeachingInstall
{
    public enum SetupState
    {
        Default,
        CustomPath,
        Agreement,
        SetupProgress,
        SetupComplete
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChildWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private SetupState _currentSetupState;

        /// <summary>
        /// Get or set CurrentSetupState value
        /// </summary>
        public SetupState CurrentSetupState
        {
            get { return _currentSetupState; }
            set
            {
                if (value == SetupState.Agreement)
                {
                    Agreement = false;
                    SetupProgress = false;
                    SetupComplete = false;
                }
                else
                {
                    Default = false;
                    CustomPath = false;
                    Agreement = false;
                    SetupProgress = false;
                    SetupComplete = false;
                }

                switch (value)
                {
                    case SetupState.Default:
                        Default = true;
                        break;
                    case SetupState.CustomPath:
                        CustomPath = true;
                        break;
                    case SetupState.Agreement:
                        Agreement = true;
                        break;
                    case SetupState.SetupProgress:
                        SetupProgress = true;
                        break;
                    case SetupState.SetupComplete:
                        SetupComplete = true;
                        break;
                }
                Set(ref _currentSetupState, value);
            }
        }
        private bool _default;

        /// <summary>
        /// Get or set Default value
        /// </summary>
        public bool Default
        {
            get { return _default; }
            set { Set(ref _default, value); }
        }
        private bool _customPath;

        /// <summary>
        /// Get or set CustomPath value
        /// </summary>
        public bool CustomPath
        {
            get { return _customPath; }
            set { Set(ref _customPath, value); }
        }
        private bool _agreement;

        /// <summary>
        /// Get or set Agreement value
        /// </summary>
        public bool Agreement
        {
            get { return _agreement; }
            set { Set(ref _agreement, value); }
        }
        private bool _setupProgress;

        /// <summary>
        /// Get or set SetupProgress value
        /// </summary>
        public bool SetupProgress
        {
            get { return _setupProgress; }
            set { Set(ref _setupProgress, value); }
        }
        private bool _setupComplete;

        /// <summary>
        /// Get or set SetupComplete value
        /// </summary>
        public bool SetupComplete
        {
            get { return _setupComplete; }
            set { Set(ref _setupComplete, value); }
        }
        private string _installPath = @"C:\Program Files (x86)\MusicTeachingWindow";

        /// <summary>
        /// Get or set InstallPath value
        /// </summary>
        public string InstallPath
        {
            get { return _installPath; }
            set { Set(ref _installPath, value); }
        }
        private bool _isAgree;

        /// <summary>
        /// Get or set IsAgree value
        /// </summary>
        public bool IsAgree
        {
            get { return _isAgree; }
            set { Set(ref _isAgree, value); }
        }
        private string _diskSize;

        /// <summary>
        /// Get or set DiskSize value
        /// </summary>
        public string DiskSize
        {
            get { return _diskSize; }
            set { Set(ref _diskSize, value); }
        }
        private SolidColorBrush _colorDisk;

        /// <summary>
        /// Get or set ColorDisk value
        /// </summary>
        public SolidColorBrush ColorDisk
        {
            get { return _colorDisk; }
            set { Set(ref _colorDisk, value); }
        }

        private void btnCustomSetup_Click(object sender, RoutedEventArgs e)
        {
            CurrentSetupState = SetupState.CustomPath;
        }

        private void btnUserAgreement_Click(object sender, RoutedEventArgs e)
        {
            CurrentSetupState = SetupState.Agreement;

        }
        string selectedPath = "";
        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = InstallPath;
            var result = dialog.ShowDialog();
            if (dialog.SelectedPath.EndsWith("\\"))
            {
                selectedPath = dialog.SelectedPath.Substring(dialog.SelectedPath.Length - 3, 2);
            }
            else
            {
                selectedPath = dialog.SelectedPath;
            }
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (!selectedPath.Contains("MusicTeachingWindow"))
                    InstallPath = selectedPath + @"\MusicTeachingWindow";
            }
            else
            {
                InstallPath = selectedPath;
            }
            long size = LocalInstallTesting.GetHardDiskFreeSpace(InstallPath.Substring(0, 1));
            if (Convert.ToDouble(size) >= 10)
            {
                ColorDisk = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF888888"));
                DiskSize = InstallPath.Substring(0, 1).ToUpper() + "盘可用空间：" + size.ToString() + "G";
            }
            else
            {
                ColorDisk = new SolidColorBrush(Colors.Red);
                DiskSize = InstallPath.Substring(0, 1).ToUpper() + "盘可用空间：" + size.ToString() + "G(尽量选择剩余空间10G以上的盘符)";
            }


        }
        /// <summary>
        /// 查询指定的 Windows 更新是否安装
        /// </summary>
        public bool IsKbInstalled(string kb)
        {
            var query = $"select * from Win32_QuickFixEngineering where HotFixID = '{kb}'";
            using (var searcher = new ManagementObjectSearcher(@"root\cimv2", query))
            {
                return searcher.Get().Count > 0;
            }
        }
        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region 用来判断win7sp1补丁是否安装
            //if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("ProductName").ToString().Contains("7"))//判断系统是不是win7版本
            //{
            //    if (!IsKbInstalled("KB2533552") && !LocalInstallTesting.GetDotNetRelease())//KB976932判断指定的Windows 更新是否已经安装和基础环境是否已经安装 ,用这个补丁判断是否已经安装SP1补丁
            //    {
            //        MessageBoxResult result = MessageBox.Show("请先安装win7sp1补丁,再安装基础环境。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        if (result == MessageBoxResult.OK)
            //        {
            //            this.Close();
            //        }
            //    }
            //    if (!IsKbInstalled("KB2533552") && LocalInstallTesting.GetDotNetRelease())//KB976932判断指定的Windows 更新是否已经安装，用这个补丁判断是否已经安装SP1补丁
            //    {
            //        MessageBoxResult result = MessageBox.Show("请先安装win7sp1补丁,无需在安装基础环境。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        if (result == MessageBoxResult.OK)
            //        {
            //            this.Close();
            //        }
            //    }
            //}
            if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("ProductName").ToString().Contains("7"))//判断系统是不是win7版本
            {
                try
                {
                    if (!Environment.OSVersion.ServicePack.ToString().Contains("Service Pack 1") && !LocalInstallTesting.GetDotNetRelease())//判断指定的Windows sp1更新是否已经安装和基础环境是否已经安装
                    {
                        MessageBoxResult result = MessageBox.Show("请先安装win7sp1补丁,再安装基础环境。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.OK)
                        {
                            this.Close();
                        }
                    }
                    if (!Environment.OSVersion.ServicePack.ToString().Contains("Service Pack 1") && LocalInstallTesting.GetDotNetRelease())//判断指定的Windows sp1更新是否已经安装
                    {
                        MessageBoxResult result = MessageBox.Show("请先安装win7sp1补丁,无需在安装基础环境。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.OK)
                        {
                            this.Close();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            #endregion

            #region 安装客户端之前检测运行环境是否安装
            if (!LocalInstallTesting.GetDotNetRelease())// || !LocalInstallTesting.IsVC2015Installed())//这里不需要了，因为内置VC++了
            {
                MessageBoxResult result = MessageBox.Show("请先安装发子基础环境。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }
            #endregion
            CurrentSetupState = SetupState.Default;

            LocalInstallTesting.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            long size = LocalInstallTesting.GetHardDiskFreeSpace(InstallPath.Substring(0, 1));
            if (Convert.ToDouble(size) >= 10)
            {
                ColorDisk = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF888888"));
                DiskSize = InstallPath.Substring(0, 1).ToUpper() + "盘可用空间：" + size.ToString() + "G";
            }
            else
            {
                ColorDisk = new SolidColorBrush(Colors.Red);
                DiskSize = InstallPath.Substring(0, 1).ToUpper() + "盘可用空间：" + size.ToString() + "G(尽量选择剩余空间10G以上的盘符)";
            }
        }

        private void btnCancl_Click(object sender, RoutedEventArgs e)
        {
            CurrentSetupState = SetupState.Default;
        }

        private void btnAgree_Click(object sender, RoutedEventArgs e)
        {
            Agreement = false;
            IsAgree = true;
        }
        BackgroundWorker worker = new BackgroundWorker();
        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (IsAgree)
            {
                try
                {
                    LocalInstallTesting.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    long size = LocalInstallTesting.GetHardDiskFreeSpace(InstallPath.Substring(0, 1));
                    FileInfo fileInfo = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
                    string fileSize = LocalInstallTesting.GetFileSize(fileInfo.Length);
                    if (Convert.ToDouble(fileSize) >= Convert.ToDouble(size))//导入的文件大小大于或等于磁盘剩余空间大小时，不允许导入资源
                    {
                        MessageBox.Show("磁盘空间不足,无法安装程序,请选择其它硬盘或者清理磁盘空间");
                        return;
                    }
                }
                catch (Exception)
                {
                }
                CurrentSetupState = SetupState.SetupProgress;
                worker.WorkerReportsProgress = true;
                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerAsync();
            }
        }
        private int _installProgress;

        /// <summary>
        /// Get or set InstallProgress value
        /// </summary>
        public int InstallProgress
        {
            get { return _installProgress; }
            set { Set(ref _installProgress, value); }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InstallProgress = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            worker.ReportProgress(2);
            string unzipPath = InstallPath.Replace('\\', '/');
            //try
            //{
            //    if (!LocalInstallTesting.GetDotNetRelease())
            //    {
            //        MessageBoxResult result = MessageBox.Show(" 1、将安装.Net基础环境，需要一点时间，请耐心等待。\n 2、安装完毕后，如果需要重新启动系统，请按提示重新启动电脑。\n 3、再次启动本安装程序完成剩下步骤安装。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        if (result == MessageBoxResult.OK)
            //        {
            //            LocalInstallTesting.CopyFile(unzipPath, @"Environmental/NDP452-KB2901907-x86-x64-AllOS-ENU.exe");
            //            Process.Start(unzipPath + @"/Environmental/NDP452-KB2901907-x86-x64-AllOS-ENU.exe").WaitForExit();
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("检测并安装.Net版本错误：" + ex.Message);
            //}

            #region 目前这个是开始安装之后检测，现在改成安装之前检测基础环境是否安装(注释掉20200912)
            //if (!LocalInstallTesting.GetDotNetRelease() || !LocalInstallTesting.IsVC2015Installed())
            //{
            //    MessageBoxResult result = MessageBox.Show("请先安装发子基础环境。", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //    if (result == MessageBoxResult.OK)
            //    {
            //        this.Close();
            //    }

            //}
            #endregion

            worker.ReportProgress(10);
            //try
            //{
            //    if (!LocalInstallTesting.IsInstallVC1() && !LocalInstallTesting.IsInstallVC2())
            //    {
            //        if (LocalInstallTesting.Is64Bit())
            //        {
            //            LocalInstallTesting.CopyFile(unzipPath, @"Environmental/vc_redist.x64.exe");
            //            Process.Start(unzipPath + @"/Environmental/vc_redist.x64.exe").WaitForExit();
            //        }
            //        else
            //        {
            //            LocalInstallTesting.CopyFile(unzipPath, @"Environmental/vc_redist.x86.exe");
            //            Process.Start(unzipPath + @"/Environmental/vc_redist.x86.exe").WaitForExit();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("检测并安装VC运行环境错误：" + ex.Message);
            //}

            //注册vcpp 86（32位）
            //try
            //{
            //    //if (!LocalInstallTesting.IsInstallVC1() && !LocalInstallTesting.IsInstallVC2())
            //    //{
            //    //    LocalInstallTesting.CopyFile(unzipPath, @"Environmental/vc_redist.x86.exe");
            //    //    Process.Start(unzipPath + @"/Environmental/vc_redist.x86.exe").WaitForExit();
            //    //}
            //    if (!LocalInstallTesting.IsVC2015Installed())
            //    {
            //        LocalInstallTesting.CopyFile(unzipPath, @"Environmental/vc_redist.x86.exe");
            //        Process.Start(unzipPath + @"/Environmental/vc_redist.x86.exe").WaitForExit();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("检测并安装VC运行环境错误：" + ex.Message);
            //}

            worker.ReportProgress(15);
            Process[] ExeList = Process.GetProcessesByName("MusicTeachingWindow");
            for (int i = 0; i < ExeList.Length; i++)
            {
                ExeList[i].Kill();
            }
            worker.ReportProgress(20);

            LocalInstallTesting.CopyAllFile(unzipPath, new Action<int>((p) =>
            {
                worker.ReportProgress(p);
            }));
            try
            {
                LocalInstallTesting.CreateShortcut(InstallPath);//创建桌面图标
                LocalInstallTesting.CreateProgramsShortcut(InstallPath, "发子音乐课堂");
                //LocalInstallTesting.CreateWebProgramsShortcut(InstallPath, "发子音乐课堂");发子备课端已经程程到客户端，所以当前不再开始菜单添加发子备课端
                LocalInstallTesting.CreateProgramsUninstallShortcut(InstallPath, "发子音乐课堂");
                // LocalInstallTesting.CreateWebShortcut(InstallPath);//创建发子备课端web快捷方式，目前注释掉,改为从从客户端访问
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成快捷方式错误：" + ex.Message);
            }
            worker.ReportProgress(95);
            try
            {
                //LocalInstallTesting.AddRegedit(InstallPath);
                AppconfigModel appconfig = new JsonModels.AppconfigModel();
                appconfig.Version = LocalInstallTesting.Version;
                appconfig.FingerPrint = LocalInstallTesting.sFingerPrint;
                appconfig.InstallGuid = LocalInstallTesting.InstallGuid;
                appconfig.InstallDateTime = LocalInstallTesting.InstallDateTime.ToString();
                appconfig.ClientUuid = LocalInstallTesting.ClientUuid;

                string json = JsonHelper.Serializer(appconfig);
                FileHelper.WriteFile(json, InstallPath + "\\appconfig.json");

                if (HttpHelper.Instance.IsInternet)
                {
                    string jsonStr = HttpHelper.Instance.HttpPost(ApplicationPath.JavaApiHttpAddress +
                        "api/app/install", "{\"instanceUuid\": \"" + LocalInstallTesting.InstallGuid + "\",\"clientUuid\": \"" + LocalInstallTesting.ClientUuid + "\",\"applicationCode\": \"MusicTeachingWindow\",\"releaseVersion\": \"" + LocalInstallTesting.Version + "\",\"clientCpu\": \"" + LocalInstallTesting.GetCPUName() + "\",\"clientMem\": \"" + LocalInstallTesting.GetPhysicalMemory() + "\",\"clientOs\": \"" + Environment.OSVersion.VersionString + "\",\"clientIp\": \"" + LocalInstallTesting.GetIPAddress() + "\",\"installDateTime\": \"" + LocalInstallTesting.InstallDateTime.ToString("yyyy-MM-dd hh:mm:ss") + "\"}");
                    QueryPageResult<object> queryPage = JsonHelper.Deserializer<QueryPageResult<object>>(jsonStr);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

            worker.ReportProgress(100);
            CurrentSetupState = SetupState.SetupComplete;
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Process.Start(InstallPath + @"/MusicTeachingWindow.exe");
        }
    }
}
