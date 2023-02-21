using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MusicTeachingInstall
{
    public class LocalInstallTesting
    {
        #region 本地环境检测

        public static bool IsInstallVC1()
        {
            const string subkey = @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\";
            try
            {
                //if (Is64Bit())
                //{
                //    RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(subkey + @"X64\");

                //    if (ndpKey != null && ndpKey.GetValue("Version") != null)
                //    {
                //        string version = ndpKey.GetValue("Version").ToString();
                //        if (string.IsNullOrWhiteSpace(version))
                //            return false;
                //        int versionNo = Convert.ToInt32(version.Substring(1, version.IndexOf('.') - 1));
                //        if (versionNo >= 14)
                //            return true;
                //    }
                //    ndpKey.Close();
                //}
                //else
                //{
                RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey + @"X86\");

                if (ndpKey != null && ndpKey.GetValue("Version") != null)
                {
                    string version = ndpKey.GetValue("Version").ToString();
                    if (string.IsNullOrWhiteSpace(version))
                        return false;
                    int versionNo = Convert.ToInt32(version.Substring(1, version.IndexOf('.') - 1));
                    if (versionNo >= 14)
                        return true;
                }
                ndpKey.Close();
                //}
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }

        }
        public static bool IsInstallVC2()
        {
            const string subkey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\";
            try
            {
                //if (Is64Bit())
                //{
                //    RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(subkey + @"X64\");

                //    if (ndpKey != null && ndpKey.GetValue("Version") != null)
                //    {
                //        string version = ndpKey.GetValue("Version").ToString();
                //        if (string.IsNullOrWhiteSpace(version))
                //            return false;
                //        int versionNo = Convert.ToInt32(version.Substring(1, version.IndexOf('.') - 1));
                //        if (versionNo >= 14)
                //            return true;
                //    }
                //    ndpKey.Close();
                //}
                //else
                //{
                RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey + @"X86\");

                if (ndpKey != null && ndpKey.GetValue("Version") != null)
                {
                    string version = ndpKey.GetValue("Version").ToString();
                    if (string.IsNullOrWhiteSpace(version))
                        return false;
                    int versionNo = Convert.ToInt32(version.Substring(1, version.IndexOf('.') - 1));
                    if (versionNo >= 14)
                        return true;
                }
                ndpKey.Close();
                //}
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }

        }

        public static bool IsVC2015Installed()
        {
            string dependenciesPath = @"SOFTWARE\Classes\Installer\Dependencies";

            using (RegistryKey dependencies = Registry.LocalMachine.OpenSubKey(dependenciesPath))
            {
                if (dependencies == null) return false;

                foreach (string subKeyName in dependencies.GetSubKeyNames().Where(n => !n.ToLower().Contains("dotnet") && !n.ToLower().Contains("microsoft")))
                {
                    using (RegistryKey subDir = Registry.LocalMachine.OpenSubKey(dependenciesPath + "\\" + subKeyName))
                    {
                        var value = subDir.GetValue("DisplayName")?.ToString() ?? null;
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }
                        if (Regex.IsMatch(value, @"C\+\+ (2015|2017).*\(x86\)"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public enum INSTALLSTATE
        {
            INSTALLSTATE_NOTUSED = -7,  // component disabled
            INSTALLSTATE_BADCONFIG = -6,  // configuration data corrupt
            INSTALLSTATE_INCOMPLETE = -5,  // installation suspended or in progress
            INSTALLSTATE_SOURCEABSENT = -4,  // run from source, source is unavailable
            INSTALLSTATE_MOREDATA = -3,  // return buffer overflow
            INSTALLSTATE_INVALIDARG = -2,  // invalid function argument
            INSTALLSTATE_UNKNOWN = -1,  // unrecognized product or feature
            INSTALLSTATE_BROKEN = 0,  // broken
            INSTALLSTATE_ADVERTISED = 1,  // advertised feature
            INSTALLSTATE_REMOVED = 1,  // component being removed (action state, not settable)
            INSTALLSTATE_ABSENT = 2,  // uninstalled (or action state absent but clients remain)
            INSTALLSTATE_LOCAL = 3,  // installed on local drive
            INSTALLSTATE_SOURCE = 4,  // run from source, CD or net
            INSTALLSTATE_DEFAULT = 5,  // use default, local or source
        }
        public static bool Is64Bit()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool GetDotNetRelease(int release = 379893)
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return (int)ndpKey.GetValue("Release") >= release ? true : false;
                }
                return false;
            }
        }
        #endregion


        #region 获取本机硬件信息
        public static string RegeditGuid { get; set; }
        /// <summary>
        /// 计算机唯一标识
        /// </summary>
        private static string _sFingerPrint;

        /// <summary>
        /// 计算机唯一标识
        /// </summary>
        public static string sFingerPrint
        {
            get
            {
                if (string.IsNullOrEmpty(_sFingerPrint))
                {
                    InstallDateTime = DateTime.Now;
                    _sFingerPrint = "{" + UUID() + "+" + GetBIOSSerialNumber() + "}";
                    ClientUuid = LocalInstallTesting.Hash_MD5_32(_sFingerPrint);
                    //InstallGuid = Guid.NewGuid().ToString();
                    InstallGuid = UUID();


                }
                return _sFingerPrint;
            }
        }
        public static string GetBIOSSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_BIOS");
                string sBIOSSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sBIOSSerialNumber = mo["SerialNumber"].ToString().Trim();
                }
                return sBIOSSerialNumber;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="filesize">文件传入大小</param>
        /// <returns></returns>
        public static string GetFileSize(long filesize)
        {
            try
            {
                if (filesize < 0)
                {
                    return "0";
                }
                else//文件大小大于或等于1024MB    
                {
                    return string.Format("{0:0.0000}", (double)filesize / (1024 * 1024 * 1024));
                }
                //else if (filesize >= 1024 * 1024) //文件大小大于或等于1024KB    
                //{
                //    return string.Format("{0:0.00} MB", (double)filesize / (1024 * 1024));
                //}
                //else if (filesize >= 1024) //文件大小大于等于1024bytes    
                //{
                //    return string.Format("{0:0.00} KB", (double)filesize / 1024);
                //}
                //else
                //{
                //    return string.Format("{0:0.00} bytes", filesize);
                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// 获取指定驱动器的剩余空间总大小(单位为GB)  
        /// </summary>
        /// <param name="str_HardDiskName"></param>
        /// <returns></returns>
        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long freeSpace = new long();
            str_HardDiskName = str_HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace / (1024 * 1024 * 1024);
                }
            }
            return freeSpace;

        }
        public static DateTime InstallDateTime { get; set; }
        public static string InstallGuid { get; set; }
        /// <summary>
        /// 客户端id 哈希值
        /// </summary>
        public static string ClientUuid { get; set; }

        /// <summary>
        /// 获取UUID
        /// </summary>
        /// <returns></returns>
        public static string UUID()
        {
            string code = null;
            SelectQuery query = new SelectQuery("select * from Win32_ComputerSystemProduct");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (var item in searcher.Get())
                {
                    using (item)
                        code = item["UUID"].ToString().ToLower();
                }
            }
            return code;
        }

        /// <summary>  
        /// CPU名称信息  
        /// </summary>  
        public static string GetCPUName()
        {
            string st = "";
            ManagementObjectSearcher driveID = new ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (ManagementObject mo in driveID.Get())
            {
                st = mo["Name"].ToString();
            }
            return st;
        }
        /// <summary>  
        /// 物理内存  
        /// </summary>  
        public static string GetPhysicalMemory()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                st = mo["TotalPhysicalMemory"].ToString();
            }
            return st;
        }
        /// <summary>  
        /// 获取IP地址  
        /// </summary>  
        public static string GetIPAddress()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    //st=mo["IpAddress"].ToString();   
                    System.Array ar;
                    ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    st = ar.GetValue(0).ToString();
                    break;
                }
            }
            return st;
        }
        #endregion

        /// <summary>
        /// 程序图标
        /// </summary>
        public const string AppIco = "logo.ico";
        /// <summary>
        /// 控制面板显示名称
        /// </summary>
        public const string ControlPanelDisplayName = "发子音乐课堂";
        /// <summary>
        /// 程序版本
        /// </summary>
        public static string Version { get; set; }
        /// <summary>
        /// 程序发行公司
        /// </summary>
        public const string Publisher = "发子智能科技有限公司";
        /// <summary>
        /// 卸载程序名称（必须包含在压缩包内）
        /// </summary>
        public const string UninstallExe = "MusicTeachingUninstall.exe";
        /// <summary>
        /// 显示名称
        /// </summary>
        //public const string DisplayName = "发子音乐教学云客户端";
        public const string DisplayName = "发子音乐课堂";
        /// <summary>
        /// 注册应用信息
        /// </summary>
        /// <param name="setupPath">安装路径</param>
        public static void AddRegedit(string setupPath)
        {
            try
            {
                RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);
                RegistryKey software = key.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", true).CreateSubKey(sFingerPrint);
                software.SetValue("DisplayIcon", setupPath + "\\" + AppIco);
                software.SetValue("DisplayName", ControlPanelDisplayName);
                software.SetValue("DisplayVersion", Version);
                software.SetValue("Publisher", Publisher);
                software.SetValue("InstallLocation", setupPath);
                software.SetValue("InstallSource", setupPath);
                // software.SetValue("HelpTelephone", "123456789");
                software.SetValue("UninstallString", setupPath + "\\" + UninstallExe);
                software.SetValue("InstallGuid", InstallGuid);
                software.SetValue("InstallDateTime", InstallDateTime.ToString());
                software.Flush();
                software.Close();
                key.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"计算机标识码{sFingerPrint}Error:{ex.Message}");
            }
        }
        public static void CopyFile(string targetPath, string sourceFile)
        {
            Stream stream = null;
            try
            {
                stream = System.Windows.Application.GetResourceStream(new Uri(@"/MusicTeachingInstall;component/Resources/" + sourceFile, UriKind.Relative)).Stream;
                if (!Directory.Exists(targetPath + "/" + Path.GetDirectoryName(sourceFile)))
                {
                    Directory.CreateDirectory(targetPath + "/" + Path.GetDirectoryName(sourceFile));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            try
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                FileStream fs = new FileStream(targetPath + "/" + sourceFile, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public static void CopyAllFile(string targetPath, Action<int> progress)
        {
            try
            {
                CopyFile(targetPath, "Images/White.png");
                CopyFile(targetPath, "Images/avatar.png");

                #region 集成的调音器               
                CopyFile(targetPath, "AudioTuner/JinDuoTuner.exe");
                #endregion

                #region x86 cefsharp
                progress(32);
                CopyFile(targetPath, "x86/locales/en-US.pak");
                progress(40);
                CopyFile(targetPath, "x86/locales/zh-CN.pak");
                CopyFile(targetPath, "x86/swiftshader/libEGL.dll");
                CopyFile(targetPath, "x86/swiftshader/libGLESv2.dll");
                progress(52);
                CopyFile(targetPath, "x86/cef.pak");
                CopyFile(targetPath, "x86/cef_100_percent.pak");
                CopyFile(targetPath, "x86/cef_200_percent.pak");
                CopyFile(targetPath, "x86/cef_extensions.pak");
                progress(56);
                CopyFile(targetPath, "x86/CefSharp.BrowserSubprocess.Core.dll");
                CopyFile(targetPath, "x86/CefSharp.BrowserSubprocess.exe");
                CopyFile(targetPath, "x86/CefSharp.Core.dll");
                progress(60);
                CopyFile(targetPath, "x86/CefSharp.Core.xml");
                CopyFile(targetPath, "x86/CefSharp.dll");
                CopyFile(targetPath, "x86/CefSharp.Wpf.dll");
                progress(62);
                CopyFile(targetPath, "x86/CefSharp.Wpf.XML");
                CopyFile(targetPath, "x86/CefSharp.XML");
                CopyFile(targetPath, "x86/chrome_elf.dll");
                CopyFile(targetPath, "x86/d3dcompiler_47.dll");
                progress(64);
                CopyFile(targetPath, "x86/devtools_resources.pak");
                CopyFile(targetPath, "x86/icudtl.dat");
                CopyFile(targetPath, "x86/libcef.dll");
                CopyFile(targetPath, "x86/libEGL.dll");
                progress(66);
                CopyFile(targetPath, "x86/libGLESv2.dll");
                CopyFile(targetPath, "x86/natives_blob.bin");
                CopyFile(targetPath, "x86/snapshot_blob.bin");
                CopyFile(targetPath, "x86/v8_context_snapshot.bin");
                progress(68);
                CopyFile(targetPath, "x86/api-ms-win-core-console-l1-1-0.dll");//1
                CopyFile(targetPath, "x86/api-ms-win-core-datetime-l1-1-0.dll");//2
                CopyFile(targetPath, "x86/api-ms-win-core-debug-l1-1-0.dll");//3
                CopyFile(targetPath, "x86/api-ms-win-core-errorhandling-l1-1-0.dll");//4
                CopyFile(targetPath, "x86/api-ms-win-core-file-l1-1-0.dll"); //5
                CopyFile(targetPath, "x86/api-ms-win-core-file-l1-2-0.dll");//6
                CopyFile(targetPath, "x86/api-ms-win-core-file-l2-1-0.dll");//7
                CopyFile(targetPath, "x86/api-ms-win-core-handle-l1-1-0.dll");//8
                CopyFile(targetPath, "x86/api-ms-win-core-heap-l1-1-0.dll"); //9
                CopyFile(targetPath, "x86/api-ms-win-core-interlocked-l1-1-0.dll");//10
                CopyFile(targetPath, "x86/api-ms-win-core-libraryloader-l1-1-0.dll");//11
                CopyFile(targetPath, "x86/api-ms-win-core-localization-l1-2-0.dll");//12
                CopyFile(targetPath, "x86/api-ms-win-core-memory-l1-1-0.dll");//13
                CopyFile(targetPath, "x86/api-ms-win-core-namedpipe-l1-1-0.dll");//14
                CopyFile(targetPath, "x86/api-ms-win-core-processenvironment-l1-1-0.dll");//15
                CopyFile(targetPath, "x86/api-ms-win-core-processthreads-l1-1-0.dll");//16
                CopyFile(targetPath, "x86/api-ms-win-core-processthreads-l1-1-1.dll");//17
                CopyFile(targetPath, "x86/api-ms-win-core-profile-l1-1-0.dll");//18
                CopyFile(targetPath, "x86/api-ms-win-core-rtlsupport-l1-1-0.dll"); //19
                CopyFile(targetPath, "x86/api-ms-win-core-string-l1-1-0.dll");//20
                CopyFile(targetPath, "x86/api-ms-win-core-synch-l1-1-0.dll"); //21
                CopyFile(targetPath, "x86/api-ms-win-core-synch-l1-2-0.dll");//22
                CopyFile(targetPath, "x86/api-ms-win-core-sysinfo-l1-1-0.dll"); //23
                CopyFile(targetPath, "x86/api-ms-win-core-timezone-l1-1-0.dll");//24
                CopyFile(targetPath, "x86/api-ms-win-core-util-l1-1-0.dll"); //25
                CopyFile(targetPath, "x86/api-ms-win-crt-conio-l1-1-0.dll");//26
                CopyFile(targetPath, "x86/api-ms-win-crt-convert-l1-1-0.dll");//27
                CopyFile(targetPath, "x86/api-ms-win-crt-environment-l1-1-0.dll");//28
                CopyFile(targetPath, "x86/api-ms-win-crt-filesystem-l1-1-0.dll");//29
                CopyFile(targetPath, "x86/api-ms-win-crt-heap-l1-1-0.dll");//30
                CopyFile(targetPath, "x86/api-ms-win-crt-locale-l1-1-0.dll"); //31
                CopyFile(targetPath, "x86/api-ms-win-crt-math-l1-1-0.dll");//32
                CopyFile(targetPath, "x86/api-ms-win-crt-multibyte-l1-1-0.dll");//33
                CopyFile(targetPath, "x86/api-ms-win-crt-private-l1-1-0.dll");//34
                CopyFile(targetPath, "x86/api-ms-win-crt-process-l1-1-0.dll");//35 
                CopyFile(targetPath, "x86/api-ms-win-crt-runtime-l1-1-0.dll");//36
                CopyFile(targetPath, "x86/api-ms-win-crt-stdio-l1-1-0.dll");//37
                CopyFile(targetPath, "x86/api-ms-win-crt-string-l1-1-0.dll");//38
                CopyFile(targetPath, "x86/api-ms-win-crt-time-l1-1-0.dll");//39
                CopyFile(targetPath, "x86/api-ms-win-crt-utility-l1-1-0.dll");//40
                CopyFile(targetPath, "x86/ucrtbase.dll");//41
                CopyFile(targetPath, "x86/msvcp140.dll");//42
                CopyFile(targetPath, "x86/vccorlib140.dll");//43
                CopyFile(targetPath, "x86/vcruntime140.dll");//44
                progress(79);
                #endregion


                progress(81);
                CopyFile(targetPath, "CommonLib.dll");
                CopyFile(targetPath, "ConfigInfo.dll");
                CopyFile(targetPath, "ControlsResource.dll");
                CopyFile(targetPath, "logo.ico");
                progress(83);
                CopyFile(targetPath, "ICSharpCode.SharpZipLib.dll");
                CopyFile(targetPath, "Microsoft.Expression.Interactions.dll");
                CopyFile(targetPath, "Microsoft.Practices.ServiceLocation.dll");
                CopyFile(targetPath, "MusicTeachingModel.dll");
                progress(85);
                CopyFile(targetPath, "MusicTeachingServicesModel.dll");
                CopyFile(targetPath, "MusicTeachingView.dll");
                CopyFile(targetPath, "MusicTeachingViewModel.dll");
                CopyFile(targetPath, "MusicTeachingWindow.exe");
                CopyFile(targetPath, "MusicTeachUpdateWindow.exe");
                CopyFile(targetPath, "ResourceTemplate/video-js.min.css");
                CopyFile(targetPath, "ResourceTemplate/jquery.min.js");
                CopyFile(targetPath, "ResourceTemplate/video.html");
                CopyFile(targetPath, "ResourceTemplate/video.min.js");
                CopyFile(targetPath, "ResourceTemplate/defaultResource.png");
                CopyFile(targetPath, "ResourceTemplate/teachinggrade.json");
                CopyFile(targetPath, "ResourceTemplate/resourceType.json");
                CopyFile(targetPath, "ResourceTemplate/videoPlay.html");
                CopyFile(targetPath, "ResourceTemplate/mp3play.png");

                #region 主题课程资源
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/3e99/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/3e99/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/3e99/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/3e99/身体打击乐四拍子.jpg");

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/5bef/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/5bef/身体打击乐六拍子.jpg");

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/7f12/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/7f12/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/7f12/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/7f12/4.json");//身体打击乐

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/9b39/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/9b39/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/9b39/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/9b39/乐曲欣赏.jpg");

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/476e/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/476e/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/476e/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/476e/4.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/476e/5.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/476e/五线谱教学.jpg");

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/ce1b/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/ce1b/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/ce1b/乐器介绍.jpg");


                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f16f/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f16f/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f16f/稳定速度训练.jpg");


                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/a3b1/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/a3b1/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/a3b1/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/a3b1/声势游戏.jpg");

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/2569/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/2569/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/2569/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/2569/4.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/2569/5.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/2569/节奏游戏.jpg");

                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f2f9/1.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f2f9/2.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f2f9/3.json");
                CopyFile(targetPath, "ResourceTemplate/ThmeLesson/f2f9/音准练习.jpg");

                #endregion


                progress(88);
                CopyFile(targetPath, "Newtonsoft.Json.dll");
                CopyFile(targetPath, "System.Windows.Interactivity.dll");
                CopyFile(targetPath, "WPFCommonLib.dll");
                CopyFile(targetPath, "zxing.dll");
                CopyFile(targetPath, "zxing.presentation.dll");
                CopyFile(targetPath, "MusicTeachingUninstall.exe");
                CopyFile(targetPath, "MusicTeachingWindow.exe.config");
                CopyFile(targetPath, "logoWeb.ico");
                RegeditGuid = Guid.NewGuid().ToString();
                Configuration config = ConfigurationManager.OpenExeConfiguration(targetPath + "\\MusicTeachingWindow.exe");
                config.AppSettings.Settings["StartPath"].Value = targetPath;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                progress(90);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static void CreateShortcut(string targetFile)
        {
            string privateDesktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);//旧的私人桌面目录
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonDesktopDirectory);
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);

            #region 删除之前的老图标
            if (System.IO.File.Exists(privateDesktop + "\\发子学院.lnk"))
            {
                System.IO.File.Delete(privateDesktop + "\\发子学院.lnk");
            }
            if (System.IO.File.Exists(privateDesktop + "\\发子音乐课堂.lnk"))
            {
                System.IO.File.Delete(privateDesktop + "\\发子音乐课堂.lnk");
            }
            if (System.IO.File.Exists(privateDesktop + "\\发子教学端客户端.lnk"))
            {
                System.IO.File.Delete(privateDesktop + "\\发子教学端客户端.lnk");
            }
            if (System.IO.File.Exists(privateDesktop + "\\发子备课端.lnk"))
            {
                System.IO.File.Delete(privateDesktop + "\\发子备课端.lnk");
            }
            if (System.IO.File.Exists(privateDesktop + "\\发子教学端.lnk"))
            {
                System.IO.File.Delete(privateDesktop + "\\发子教学端.lnk");
            }


            if (System.IO.File.Exists(desktop + "\\发子教学端客户端.lnk"))
            {
                System.IO.File.Delete(desktop + "\\发子教学端客户端.lnk");
            }
            if (System.IO.File.Exists(desktop + "\\发子教学端.lnk"))
            {
                System.IO.File.Delete(desktop + "\\发子教学端.lnk");
            }
            if (System.IO.File.Exists(desktop + "\\发子备课端.lnk"))
            {
                System.IO.File.Delete(desktop + "\\发子备课端.lnk");
            }
            if (System.IO.File.Exists(desktop + "\\发子学院.lnk"))
            {
                System.IO.File.Delete(desktop + "\\发子学院.lnk");
            }
            if (System.IO.File.Exists(desktop + "\\发子音乐课堂.lnk"))
            {
                System.IO.File.Delete(desktop + "\\发子音乐课堂.lnk");
            }

            #endregion

            var shortcut = shell.CreateShortcut(desktop + "\\" + DisplayName + ".lnk");
            shortcut.TargetPath = targetFile + "\\MusicTeachingWindow.exe";
            shortcut.WorkingDirectory = targetFile;
            shortcut.Save();


        }


        public static void CreateWebShortcut(string targetFile)
        {
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            var shortcut = shell.CreateShortcut(desktop + "\\发子备课端.lnk");
            shortcut.TargetPath = "https://jinduo.art";
            shortcut.IconLocation = targetFile + "\\logoWeb.ico";
            shortcut.Save();
        }

        /// <summary>
        /// 创建程序菜单快捷方式
        /// </summary>
        /// <param name="targetPath">可执行文件路径</param>
        /// <param name="menuName">程序菜单中子菜单名称，为空则不创建子菜单</param>
        /// <returns></returns>
        public static void CreateProgramsShortcut(string targetPath, string menuName)
        {
            string startMenu = System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu);
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            if (!string.IsNullOrEmpty(menuName))
            {
                startMenu += "\\" + menuName;
                if (!System.IO.Directory.Exists(startMenu))
                {
                    System.IO.Directory.CreateDirectory(startMenu);

                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(startMenu);
                    di.Delete(true);
                    System.IO.Directory.CreateDirectory(startMenu);
                }
            }
            targetPath = targetPath + "\\MusicTeachingWindow.exe";
            var shortcut = shell.CreateShortcut(startMenu + "\\" + DisplayName + ".lnk");
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.Save();
        }
        public static void CreateWebProgramsShortcut(string targetPath, string menuName)
        {
            string startMenu = System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu);
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            if (!string.IsNullOrEmpty(menuName))
            {
                startMenu += "\\" + menuName;
                if (!System.IO.Directory.Exists(startMenu))
                {
                    System.IO.Directory.CreateDirectory(startMenu);

                }
            }
            //var shortcut = shell.CreateShortcut(startMenu + "\\音乐教学云平台.lnk");
            var shortcut = shell.CreateShortcut(startMenu + "\\发子备课端.lnk");
            shortcut.TargetPath = "https://jinduo.art";
            shortcut.IconLocation = targetPath + "\\logoWeb.ico";
            shortcut.Save();
        }
        public static void CreateProgramsUninstallShortcut(string targetPath, string menuName)
        {
            string startMenu = System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu);
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            if (!string.IsNullOrEmpty(menuName))
            {
                startMenu += "\\" + menuName;
                if (!System.IO.Directory.Exists(startMenu))
                {
                    System.IO.Directory.CreateDirectory(startMenu);
                }
            }
            targetPath = targetPath + "\\" + UninstallExe;
            var shortcut = shell.CreateShortcut(startMenu + "\\卸载发子音乐课堂客户端.lnk");
            shortcut.TargetPath = targetPath;
            //shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.Save();
        }
        /// <summary>
        /// 计算32位MD5码
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回哈希值格式 true：英文大写，false：英文小写</param>
        /// <returns></returns>
        public static string Hash_MD5_32(string word, bool toUpper = true)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider MD5CSP
                    = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                byte[] bytHash = MD5CSP.ComputeHash(bytValue);
                MD5CSP.Clear();

                //根据计算得到的Hash码翻译为MD5码
                string sHash = "", sTemp = "";
                for (int counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }
                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根据大小写规则决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
