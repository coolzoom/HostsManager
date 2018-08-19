using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostsManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // 全部变量，基本的配置信息
        // Host文件的绝对路径
        private const string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        // 域名和IP对应关系字典
        private static Dictionary<string,List<string>> hostsDic;
        // 域名对应的最终生效的IP地址
        private static Dictionary<string, string> domainToIP;
        // 本地主机
        private const string localhost = "localhost";
        // 不绑定任何IP，走域名解析DNS
        private const string disabledIP = "不指定IP地址";
        // 换行符
        private static string lineBreak = "\r\n";
        private static bool isNeedSwitch = false;
        //是否退出程序，false-仅关闭窗口、true-退出主程序
        private static bool isExitMain = false;

        /// <summary>
        /// 窗口加载入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            initLoad();
        }

        /// <summary>
        /// 手动重新加载hosts文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadHosts_Click(object sender, EventArgs e)
        {
            initLoad();
        }

        /// <summary>
        /// 定时器触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 每个60秒，重新加载一次Hosts文件，重新渲染托盘图标的左键菜单
            initLoad();
        }

        private void initLoad()
        {
            lbHosts.Items.Clear();
            hostsDic = new Dictionary<string, List<string>>();
            domainToIP = new Dictionary<string, string>();
            // 1-加载hosts文件
            this.LoadHosts(hostsFilePath);

            foreach (KeyValuePair<string, List<string>> item in hostsDic)
            {
                // 2-加载主界面域名列表
                lbHosts.Items.Add(item.Key);

                // 3-加载通知栏菜单
                ToolStripMenuItem itemTool = (ToolStripMenuItem)contextMenuStrip1.Items.Add(item.Key);
                ToolStripMenuItem itemDropDisabledIP = (ToolStripMenuItem)itemTool.DropDownItems.Add(disabledIP);
                itemDropDisabledIP.ToolTipText = item.Key;
                itemDropDisabledIP.Click += new EventHandler(tsmiClick1);//绑定方法
                itemDropDisabledIP.ForeColor = Color.Red;

                string ipSetted = domainToIP.ContainsKey(item.Key) ? domainToIP[item.Key] : "";
                foreach (string ip in item.Value)
                {
                    ToolStripMenuItem itemDrop = (ToolStripMenuItem)itemTool.DropDownItems.Add(ip);
                    itemDrop.ToolTipText = item.Key;
                    if (ip == ipSetted)
                    {
                        itemDrop.ForeColor = Color.Red;
                        itemDropDisabledIP.ForeColor = Color.Black;
                    }
                    itemDrop.Click += new EventHandler(tsmiClick1);//绑定方法
                }
            }

            // 4-增加退出功能
            contextMenuStrip1.Items.Add("退出");
        }

        /// <summary>
        /// 单击托盘图标的菜单项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiClick1(object sender, EventArgs e)
        {
            ToolStripMenuItem itemDrop = (ToolStripMenuItem)sender;
            ToolStrip parentStrip = itemDrop.GetCurrentParent();
            foreach(ToolStripItem item in parentStrip.Items)
            {
                item.ForeColor = Color.Black;
            }
            itemDrop.ForeColor = Color.Red;
            bindDomainToIp(itemDrop.ToolTipText, itemDrop.Text);
        }

        /// <summary>
        /// 加载hosts文件内容，生成字典
        /// </summary>
        /// <param name="path">hosts文件的绝对路径</param>
        private void LoadHosts(string path)
        {
            domainToIP = new Dictionary<String, String>();
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            String lineString = "";
            List<String> ipList = new List<String>();
            while ((line = sr.ReadLine()) != null)
            {
                lineString  = line.ToString().Replace("#", "").Trim();
                //lbHosts.Items.Add(line.ToString());
                // trim之后，第一个字符不是数据则跳过
                if (lineString.Length>0 && Common.isNumberic(lineString.Substring(0,1)))
                {
                    List<string> lineDomain = Common.matchsDomain(lineString);
                    // 兼容localhost
                    if (lineDomain.Count == 1 && lineString.IndexOf(localhost) >0)
                    {
                        generateHostsDic(lineDomain[0], localhost);
                        //lbHosts.Items.Add(lineDomain[0]);
                        //lbHosts.Items.Add(localhost);

                        if (line.ToString().IndexOf("#") < 0)
                        {
                            //设置每个域名对应的IP地址，判断是否含有字符#
                            if (domainToIP.ContainsKey(localhost))
                            {
                                domainToIP[localhost] = lineDomain[0];
                            }
                            else
                            {
                                domainToIP.Add(localhost, lineDomain[0]);
                            }
                        }
                    }
                    // 格式化出域名和IP
                    // 添加到hosts列表中
                    if (lineDomain.Count == 2)
                    {
                        generateHostsDic(lineDomain[0], lineDomain[1]);
                        //lbHosts.Items.Add(lineDomain[0]);
                        //lbHosts.Items.Add(lineDomain[1]);

                        if (line.ToString().IndexOf("#") < 0)
                        {
                            //设置每个域名对应的IP地址，判断是否含有字符#
                            if (domainToIP.ContainsKey(lineDomain[1]))
                            {
                                domainToIP[lineDomain[1]] = lineDomain[0];
                            }
                            else
                            {
                                domainToIP.Add(lineDomain[1], lineDomain[0]);
                            }
                        }
                    }
                }
            }
            sr.Close();
        }

        /// <summary>
        /// 生成HostsDic，添加新的绑定关系到HostsDic字典中
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="domain">域名</param>
        private static void generateHostsDic(string ip,string domain)
        {
            List<string> ipList = new List<string>();
            if (hostsDic.ContainsKey(domain))
            {
                ipList = hostsDic[domain];
            }
            ipList.Add(ip);
            hostsDic[domain] = ipList;
        }

        /// <summary>
        /// 切换域名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            isNeedSwitch = false;
            if(lbHosts.SelectedIndex<0)
            {
                return;
            }
            string domain = lbHosts.SelectedItem.ToString();
            if(domain.Trim().Length<=0)
            {
                return;
            }
            if(!hostsDic.ContainsKey(domain))
            {
                return;
            }
            lbIPS.Items.Clear();
            int index = 0;
            lbIPS.Items.Add(disabledIP);
            lbIPS.SelectedIndex = index;
            List<string> ipList = hostsDic[domain];
            string ipSetted = domainToIP.ContainsKey(domain) ? domainToIP[domain] : "";
            foreach(string ip in ipList)
            {
                lbIPS.Items.Add(ip);
                index++;
                if (ipSetted == ip)
                {
                    lbIPS.SelectedIndex = index;
                }
            }
            // 设置选中
            isNeedSwitch = true;
        }

        /// <summary>
        /// 选中指定ip，将选中的域名绑定到选中的ip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbIPS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(isNeedSwitch == false)
            {
                return;
            }
            string domain = lbHosts.SelectedItem.ToString();
            if(domain.Length<=0)
            {
                return;
            }
            bindDomainToIp(domain, lbIPS.SelectedItem.ToString());
        }

        /// <summary>
        /// 绑定域名到指定的IP
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="ip">IP</param>
        private void bindDomainToIp(string domain, string ip)
        {
            if (domain.Length <= 0)
            {
                return;
            }
            bool isSetted = false;

            StreamReader sr = new StreamReader(hostsFilePath, Encoding.Default);
            String line;
            string content = "";
            string lineString = "";
            List<string> ipList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                lineString = line.ToString().Replace("#", "").Trim();
                //lbHosts.Items.Add(line.ToString());
                // trim之后，第一个字符不是数据则跳过
                if (lineString.Length > 0 && Common.isNumberic(lineString.Substring(0, 1)))
                {
                    List<string> lineDomain = Common.matchsDomain(lineString);
                    // 兼容localhost
                    if (lineDomain.Count == 1 && lineString.IndexOf(localhost) > 0)
                    {
                        if (domain == localhost)
                        {
                            if (lineDomain[0] == ip)
                            {
                                content += ip + " " + domain + lineBreak;
                            }
                            else
                            {
                                content += "#" + lineDomain[0] + " " + domain + lineBreak;
                            }
                            isSetted = true;
                            continue;
                        }
                    }
                    // 格式化出域名和IP
                    // 添加到hosts列表中
                    if (lineDomain.Count == 2)
                    {
                        //genHostsDic(lineDomain[0], lineDomain[1]);
                        //lbHosts.Items.Add(lineDomain[0]);
                        //lbHosts.Items.Add(lineDomain[1]);
                        if (domain == lineDomain[1])
                        {
                            if (lineDomain[0] == ip)
                            {
                                content += ip + " " + domain + lineBreak;
                            }
                            else
                            {
                                content += "#" + lineDomain[0] + " " + domain + lineBreak;
                            }
                            isSetted = true;
                            continue;
                        }
                    }
                }
                content += line.ToString() + lineBreak;
            }
            if (!isSetted)
            {
                content += ip + " " + domain + lineBreak;
            }
            sr.Close();
            Common.write(hostsFilePath, content, Encoding.Default);
        }

        /// <summary>
        /// 添加一个或者多个域名绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddHosts_Click(object sender, EventArgs e)
        {
            string ipHosts = tbHosts.Text.ToString().Replace("#", "").Trim();
            List<string> lineDomain = Common.matchsDomain(ipHosts);
            // 判断数据是否合法，格式是否正确

            string domain = lineDomain[1];
            if (domain.Length <= 0)
            {
                return;
            }
            string ip = lineDomain[0];
            bool isSetted = false;

            StreamReader sr = new StreamReader(hostsFilePath, Encoding.Default);
            String line;
            string content = "";
            string lineString = "";
            List<string> ipList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                lineString = line.ToString().Replace("#", "").Trim();
                //lbHosts.Items.Add(line.ToString());
                // trim之后，第一个字符不是数据则跳过
                if (lineString.Length > 0 && Common.isNumberic(lineString.Substring(0, 1)))
                {
                    lineDomain = Common.matchsDomain(lineString);
                    // 兼容localhost
                    if (lineDomain.Count == 1 && lineString.IndexOf(localhost) > 0)
                    {
                        if (domain == localhost && lineDomain[0] == ip)
                        {
                            isSetted = true;
                        }
                    }
                    // 格式化出域名和IP
                    // 添加到hosts列表中
                    if (lineDomain.Count == 2)
                    {
                        //genHostsDic(lineDomain[0], lineDomain[1]);
                        //lbHosts.Items.Add(lineDomain[0]);
                        //lbHosts.Items.Add(lineDomain[1]);
                        if (domain == lineDomain[1] && lineDomain[0] == ip)
                        {
                            isSetted = true;
                        }
                    }
                }
                content += line.ToString() + lineBreak;
            }
            if (!isSetted)
            {
                content += ip + " " + domain + lineBreak;
            }
            sr.Close();
            Common.write(hostsFilePath, content, Encoding.Default);

            // 1-加载hosts文件，重新加载Hosts
            this.LoadHosts(MainForm.hostsFilePath);

            lbHosts.Items.Clear();
            foreach (KeyValuePair<string, List<string>> item in hostsDic)
            {
                lbHosts.Items.Add(item.Key);
            }
        }

        /// <summary>
        /// 记事本打开hosts文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenHosts_Click(object sender, EventArgs e)
        {
            // 打开文件
            System.Diagnostics.Process.Start("notepad.exe", hostsFilePath);
        }

        /// <summary>
        /// 退出主程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {            
            if (e.ClickedItem.Text == "退出")
            {
                isExitMain = true;
                this.Close();
            }
        }

        /// <summary>
        /// 关闭主程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isExitMain)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 主程序窗口缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
        }

        /// <summary>
        /// 双击托盘图标，显示程序主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            if(this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        /// 输入框成为活动控件是，清空示例内容，以便输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbHosts_Enter(object sender, EventArgs e)
        {
            if(tbHosts.Text == "示例：127.0.0.1 localhost;127.0.0.1 local")
            {
                tbHosts.Text = "";
            }
        }

        /// <summary>
        /// 输入框失去焦点后，如果内容为空，重新设置提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbHosts_Leave(object sender, EventArgs e)
        {
            if (tbHosts.Text.Trim() == "")
            {
                tbHosts.Text = "示例：127.0.0.1 localhost;127.0.0.1 local";
            }
        }
    }
}
