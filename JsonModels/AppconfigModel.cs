using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicTeachingInstall.JsonModels
{
    public class AppconfigModel
    {
        /// <summary>
        /// 记录当前安装的uuid
        /// </summary>
        public string InstallGuid { get; set; }
        /// <summary>
        /// 当前版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 安装时间
        /// </summary>
        public string InstallDateTime { get; set; }
        /// <summary>
        /// 唯一标识，记录用户行为
        /// </summary>

        public string FingerPrint { get; set; }
        /// <summary>
        /// 客户端id 哈希值
        /// </summary>
        public string ClientUuid { get; set; }
    }

}
