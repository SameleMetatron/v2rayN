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

        public static void Update(Config config, bool forceDisable)
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
                    if (type == 1)
                    {
                        var localHttp = config.inbound.FirstOrDefault(x => x.protocol == "http");
                        if (localHttp == null)
                        {
                            return;
                        }
                        PACServerHandle.Stop();
                        PACFileWatcherHandle.StopWatch();
                        SysProxyHandle.SetIEProxy(true, true, "127.0.0.1:" + localHttp.localPort.ToString(), null);
                    }
                    else
                    {
                        string pacUrl = $"http://127.0.0.1:{config.sysListenerPort}/pac/?t={GetTimestamp(DateTime.Now)}";
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
        }
    }
}
