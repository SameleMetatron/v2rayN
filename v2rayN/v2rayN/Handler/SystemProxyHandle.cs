using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v2rayN.Mode;

namespace v2rayN.Handler
{
    class SystemProxyHandle
    {
        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        public static void ReSetPACProxy(Config config)
        {
            if (config.listenerType == 2)
            {
                SysProxyHandle.SetIEProxy(false, false, null, null);
                PACServerHandle.Stop();
            }
            Update(config, false);
        }

        public static bool Update(Config config, bool forceDisable)
        {
            int type = config.listenerType;

            if (forceDisable)
            {
                type = 0;
            }

            try
            {
                if (type != 0)
                {
                    var port = Utils.GetHttpPortNum(config);
                    if (port == -1)
                    {
                        return false;
                    }
                    if (type == 1)
                    {
                        
                        PACServerHandle.Stop();
                        PACFileWatcherHandle.StopWatch();
                        SysProxyHandle.SetIEProxy(true, true, "127.0.0.1:" + port, null);
                    }
                    else
                    {
                        string pacUrl = string.Format("http://127.0.0.1:{0}/pac/?t={1}", config.pacPort,
                            GetTimestamp(DateTime.Now));
                        SysProxyHandle.SetIEProxy(true, false, null, pacUrl);
                        PACServerHandle.Init(config);
                        PACFileWatcherHandle.StartWatch(config);
                    }
                }
                else
                {
                    SysProxyHandle.SetIEProxy(false, false, null, null);
                    PACServerHandle.Stop();
                    PACFileWatcherHandle.StopWatch();
                }
            }
            catch (Exception ex)
            {
                //Logging.LogUsefulException(ex);
            }
            return true;
        }
    }
}
