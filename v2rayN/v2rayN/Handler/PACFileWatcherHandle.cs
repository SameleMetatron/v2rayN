using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using v2rayN.Mode;

namespace v2rayN.Handler
{
    class PACFileWatcherHandle
    {
        private static FileSystemWatcher fileSystemWatcher;

        private static long fileSize;

        public static void StartWatch(Config config)
        {
            if (fileSystemWatcher == null)
            {
                fileSystemWatcher = new FileSystemWatcher(Application.StartupPath);
                fileSystemWatcher.Filter = "pac.txt";
                fileSystemWatcher.NotifyFilter = NotifyFilters.Size;
                fileSystemWatcher.Changed += (sender, args) =>
                {
                    var fileInfo = new FileInfo(args.FullPath);
                    if (fileSize != fileInfo.Length)
                    {
                        fileSize = fileInfo.Length;
                        SystemProxyHandle.ReSetPACProxy(config);
                    }
                    
                };
            }
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public static void StopWatch()
        {
            if (fileSystemWatcher != null)
            {
                fileSystemWatcher.EnableRaisingEvents = false;
            }
        }
    }
}
