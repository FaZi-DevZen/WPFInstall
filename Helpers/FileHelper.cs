using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicTeachingInstall.Helpers
{
    public class FileHelper
    {
        public static void WriteFile(string str, string file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }
            System.IO.StreamWriter fileWrite = new System.IO.StreamWriter(file, false);
            fileWrite.WriteLine(str);
            fileWrite.Flush();
            fileWrite.Close();

        }
    }
}
