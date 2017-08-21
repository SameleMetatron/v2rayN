using System;
using System.Windows.Forms;
using v2rayN.Handler;

namespace v2rayN.Forms
{
    public partial class OptionSettingForm : BaseForm
    {
        public OptionSettingForm()
        {
            InitializeComponent();
        }

        private void OptionSettingForm_Load(object sender, EventArgs e)
        {
            InitBase();

            InitRouting();

            InitKCP();
        }

        /// <summary>
        /// 初始化基础设置
        /// </summary>
        private void InitBase()
        {
            //日志
            chklogEnabled.Checked = config.logEnabled;
            cmbloglevel.Text = config.loglevel;

            //Mux
            chkmuxEnabled.Checked = config.muxEnabled;

            //本地监听
            if (config.inbound.Count > 0)
            {
                txtlocalPort.Text = config.inbound[0].localPort.ToString();
                cmbprotocol.Text = config.inbound[0].protocol.ToString();
                chkudpEnabled.Checked = config.inbound[0].udpEnabled;
                if (config.inbound.Count > 1)
                {
                    txtlocalPort2.Text = config.inbound[1].localPort.ToString();
                    cmbprotocol2.Text = config.inbound[1].protocol.ToString();
                    chkudpEnabled2.Checked = config.inbound[1].udpEnabled;
                    chkAllowIn2.Checked = true;
                }
                else
                {
                    chkAllowIn2.Checked = false;
                }
                chkAllowIn2State();
            }

            //开机自动启动
            chkAutoRun.Checked = Utils.IsAutoRun();

            //自动从网络同步本地时间
            chkAutoSyncTime.Checked = config.autoSyncTime;

            //启用系统代理 
            chksysAgentEnabled.Checked = config.sysAgentEnabled;

            txtPACPort.Text = config.pacPort.ToString();
        }

        /// <summary>
        /// 初始化路由设置
        /// </summary>
        private void InitRouting()
        {
            //路由
            chkBypassChinasites.Checked = config.chinasites;
            chkBypassChinaip.Checked = config.chinaip;

            txtUseragent.Text = Utils.List2String(config.useragent);
            txtUserdirect.Text = Utils.List2String(config.userdirect);
            txtUserblock.Text = Utils.List2String(config.userblock);
        }

        /// <summary>
        /// 初始化KCP设置
        /// </summary>
        private void InitKCP()
        {
            txtKcpmtu.Text = config.kcpItem.mtu.ToString();
            txtKcptti.Text = config.kcpItem.tti.ToString();
            txtKcpuplinkCapacity.Text = config.kcpItem.uplinkCapacity.ToString();
            txtKcpdownlinkCapacity.Text = config.kcpItem.downlinkCapacity.ToString();
            txtKcpreadBufferSize.Text = config.kcpItem.readBufferSize.ToString();
            txtKcpwriteBufferSize.Text = config.kcpItem.writeBufferSize.ToString();
            chkKcpcongestion.Checked = config.kcpItem.congestion;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (SaveBase() != 0)
            {
                return;
            }

            if (SaveRouting() != 0)
            {
                return;
            }

            if (SaveKCP() != 0)
            {
                return;
            }

            if (ConfigHandler.SaveConfig(ref config) == 0)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                UI.Show("操作失败，请检查重试");
            }
        }

        /// <summary>
        /// 保存基础设置
        /// </summary>
        /// <returns></returns>
        private int SaveBase()
        {
            //日志
            bool logEnabled = chklogEnabled.Checked;
            string loglevel = cmbloglevel.Text;

            //Mux
            bool muxEnabled = chkmuxEnabled.Checked;

            //本地监听
            string localPort = txtlocalPort.Text;
            string protocol = cmbprotocol.Text;
            bool udpEnabled = chkudpEnabled.Checked;
            if (Utils.IsNullOrEmpty(localPort) || !Utils.IsNumberic(localPort))
            {
                UI.Show("请填写本地监听端口");
                return -1;
            }
            if (Utils.IsNullOrEmpty(protocol))
            {
                UI.Show("请选择协议");
                return -1;
            }
            config.inbound[0].localPort = Convert.ToInt32(localPort);
            config.inbound[0].protocol = protocol;
            config.inbound[0].udpEnabled = udpEnabled;

            //本地监听2
            string localPort2 = txtlocalPort2.Text;
            string protocol2 = cmbprotocol2.Text;
            bool udpEnabled2 = chkudpEnabled2.Checked;
            if (chkAllowIn2.Checked)
            {
                if (Utils.IsNullOrEmpty(localPort2) || !Utils.IsNumberic(localPort2))
                {
                    UI.Show("请填写本地监听端口2");
                    return -1;
                }
                if (Utils.IsNullOrEmpty(protocol2))
                {
                    UI.Show("请选择协议2");
                    return -1;
                }
                if (config.inbound.Count < 2)
                {
                    config.inbound.Add(new Mode.InItem());
                }
                config.inbound[1].localPort = Convert.ToInt32(localPort2);
                config.inbound[1].protocol = protocol2;
                config.inbound[1].udpEnabled = udpEnabled2;
            }
            else
            {
                if (config.inbound.Count > 1)
                {
                    config.inbound.RemoveAt(1);
                }
            }

            //日志     
            config.logEnabled = logEnabled;
            config.loglevel = loglevel;

            //Mux
            config.muxEnabled = muxEnabled;

            //开机自动启动
            Utils.SetAutoRun(chkAutoRun.Checked);

            //自动从网络同步本地时间
            config.autoSyncTime = chkAutoSyncTime.Checked;

            //启用系统代理 
            config.sysAgentEnabled = chksysAgentEnabled.Checked;

            if (Utils.IsNullOrEmpty(txtPACPort.Text) || !Utils.IsNumberic(txtPACPort.Text))
            {
                UI.Show("请填写PAC监听端口");
                return -1;
            }
            config.pacPort = Convert.ToInt32(txtPACPort.Text);

            return 0;
        }

        /// <summary>
        /// 保存路由设置
        /// </summary>
        /// <returns></returns>
        private int SaveRouting()
        {
            //路由
            bool bypassChinasites = chkBypassChinasites.Checked;
            bool bypassChinaip = chkBypassChinaip.Checked;

            string useragent = txtUseragent.Text;
            string userdirect = txtUserdirect.Text;
            string userblock = txtUserblock.Text;

            config.chinasites = bypassChinasites;
            config.chinaip = bypassChinaip;

            config.useragent = Utils.String2List(useragent);
            config.userdirect = Utils.String2List(userdirect);
            config.userblock = Utils.String2List(userblock);

            return 0;
        }

        /// <summary>
        /// 保存KCP设置
        /// </summary>
        /// <returns></returns>
        private int SaveKCP()
        {
            string mtu = txtKcpmtu.Text;
            string tti = txtKcptti.Text;
            string uplinkCapacity = txtKcpuplinkCapacity.Text;
            string downlinkCapacity = txtKcpdownlinkCapacity.Text;
            string readBufferSize = txtKcpreadBufferSize.Text;
            string writeBufferSize = txtKcpwriteBufferSize.Text;
            bool congestion = chkKcpcongestion.Checked;

            if (Utils.IsNullOrEmpty(mtu) || !Utils.IsNumberic(mtu)
                || Utils.IsNullOrEmpty(tti) || !Utils.IsNumberic(tti)
                || Utils.IsNullOrEmpty(uplinkCapacity) || !Utils.IsNumberic(uplinkCapacity)
                || Utils.IsNullOrEmpty(downlinkCapacity) || !Utils.IsNumberic(downlinkCapacity)
                || Utils.IsNullOrEmpty(readBufferSize) || !Utils.IsNumberic(readBufferSize)
                || Utils.IsNullOrEmpty(writeBufferSize) || !Utils.IsNumberic(writeBufferSize))
            {
                UI.Show("请正确填写KCP参数");
                return -1;
            }
            config.kcpItem.mtu = Convert.ToInt32(mtu);
            config.kcpItem.tti = Convert.ToInt32(tti);
            config.kcpItem.uplinkCapacity = Convert.ToInt32(uplinkCapacity);
            config.kcpItem.downlinkCapacity = Convert.ToInt32(downlinkCapacity);
            config.kcpItem.readBufferSize = Convert.ToInt32(readBufferSize);
            config.kcpItem.writeBufferSize = Convert.ToInt32(writeBufferSize);
            config.kcpItem.congestion = congestion;

            return 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void chkAllowIn2_CheckedChanged(object sender, EventArgs e)
        {
            chkAllowIn2State();
        }
        private void chkAllowIn2State()
        {
            bool blAllow2 = chkAllowIn2.Checked;
            txtlocalPort2.Enabled =
            cmbprotocol2.Enabled =
            chkudpEnabled2.Enabled = blAllow2;
        }
    }
}
