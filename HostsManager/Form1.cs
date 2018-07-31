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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //this.WindowState = FormWindowState.Minimized;
            //this.Hide();
        }

        private const string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        private static Dictionary<string,List<string>> hostsDic;
        private static Dictionary<string, string> hosts2IP;
        private const string localhost = "localhost";
        private const string disabledIP = "不指定IP地址";
        private static string lineBreak = "\r\n";
        private static bool isNeedSwitch = false;
        private static bool isClose = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            lbHosts.Items.Clear();
            hostsDic = new Dictionary<string, List<string>>();
            hosts2IP = new Dictionary<string, string>();
            // 1-读取hosts文件
            string hostsContent = this.Read(Form1.hostsFilePath);
            //label1.Text = hostsContent;
            
            foreach (KeyValuePair<string,List<string>> item in hostsDic)
            {
                // 2-加载主界面域名列表
                lbHosts.Items.Add(item.Key);
                
                // 3-加载通知栏菜单
                ToolStripMenuItem itemTool = (ToolStripMenuItem)contextMenuStrip1.Items.Add(item.Key);
                ToolStripMenuItem itemDropDisabledIP = (ToolStripMenuItem)itemTool.DropDownItems.Add(disabledIP);
                itemDropDisabledIP.ToolTipText = item.Key;
                itemDropDisabledIP.Click += new EventHandler(tsmiClick1);//绑定方法
                itemDropDisabledIP.ForeColor = Color.Red;

                string ipSetted = hosts2IP.ContainsKey(item.Key) ? hosts2IP[item.Key] : "";
                foreach (string ip in item.Value)
                {
                    ToolStripMenuItem itemDrop = (ToolStripMenuItem)itemTool.DropDownItems.Add(ip);
                    itemDrop.ToolTipText = item.Key;
                    if(ip == ipSetted)
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

        private void tsmiClick1(object sender, EventArgs e)
        {
            ToolStripMenuItem itemDrop = (ToolStripMenuItem)sender;
            ToolStrip parentStrip = itemDrop.GetCurrentParent();
            foreach(ToolStripItem item in parentStrip.Items)
            {
                item.ForeColor = Color.Black;
            }
            itemDrop.ForeColor = Color.Red;
            //MessageBox.Show(itemDrop.ToolTipText + itemDrop.Text);
            setHostIP(itemDrop.ToolTipText, itemDrop.Text);
        }

        private string Read(string path)
        {
            hosts2IP = new Dictionary<string, string>();
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            string content = "";
            string lineString = "";
            List<string> ipList = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                lineString  = line.ToString().Replace("#", "").Trim();
                //lbHosts.Items.Add(line.ToString());
                // trim之后，第一个字符不是数据则跳过
                if (lineString.Length>0 && this.isNumberic(lineString.Substring(0,1)))
                {
                    List<string> lineDomain = MatchsDomain(lineString);
                    // 兼容localhost
                    if (lineDomain.Count == 1 && lineString.IndexOf(localhost) >0)
                    {
                        genHostsDic(lineDomain[0], localhost);
                        //lbHosts.Items.Add(lineDomain[0]);
                        //lbHosts.Items.Add(localhost);

                        if (line.ToString().IndexOf("#") < 0)
                        {
                            //设置每个域名对应的IP地址，判断是否含有字符#
                            if (hosts2IP.ContainsKey(localhost))
                            {
                                hosts2IP[localhost] = lineDomain[0];
                            }
                            else
                            {
                                hosts2IP.Add(localhost, lineDomain[0]);
                            }
                        }
                    }
                    // 格式化出域名和IP
                    // 添加到hosts列表中
                    if (lineDomain.Count == 2)
                    {
                        genHostsDic(lineDomain[0], lineDomain[1]);
                        //lbHosts.Items.Add(lineDomain[0]);
                        //lbHosts.Items.Add(lineDomain[1]);

                        if (line.ToString().IndexOf("#") < 0)
                        {
                            //设置每个域名对应的IP地址，判断是否含有字符#
                            if (hosts2IP.ContainsKey(lineDomain[1]))
                            {
                                hosts2IP[lineDomain[1]] = lineDomain[0];
                            }
                            else
                            {
                                hosts2IP.Add(lineDomain[1], lineDomain[0]);
                            }
                        }
                    }
                }
            }
            sr.Close();
            return content;
        }

        private static void genHostsDic(string ip,string domain)
        {
            List<string> ipList = new List<string>();
            if (hostsDic.ContainsKey(domain))
            {
                ipList = hostsDic[domain];
                ipList.Add(ip);
                hostsDic[domain] = ipList;
            }
            else
            {
                ipList.Add(ip);
                hostsDic[domain] = ipList;
            }
        }

        private void Write(string path,string content, System.Text.Encoding encoding)
        {
            File.WriteAllText(path, content);
            //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            //sw.Write(content);
            //sw.Close();
            //fs.Close();
        }

        private bool isNumberic(string message)
        {
            System.Text.RegularExpressions.Regex rex =
            new System.Text.RegularExpressions.Regex(@"^\d+$");
            int result = -1;
            if (rex.IsMatch(message))
            {
                result = int.Parse(message);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>  
        /// 匹配获取字符串中所有的域名  
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public static List<string> MatchsDomain(string input)
        {
            string pattern = @"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+";
            return Matchs(input, pattern);
        }
        /// <summary>  
        /// 匹配结果  返回匹配结果的数组  
        /// </summary>  
        /// <param name="input"></param>  
        /// <param name="expression"></param>  
        /// <returns></returns>  
        public static List<string> Matchs(string input, string expression)
        {
            List<string> list = new List<string>();
            MatchCollection collection = Regex.Matches(input, expression, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (Match item in collection)
            {
                if (item.Success)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }

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
            string ipSetted = hosts2IP.ContainsKey(domain) ? hosts2IP[domain] : "";
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
            setHostIP(domain, lbIPS.SelectedItem.ToString());
        }

        private void setHostIP(string domain, string ip)
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
                if (lineString.Length > 0 && this.isNumberic(lineString.Substring(0, 1)))
                {
                    List<string> lineDomain = MatchsDomain(lineString);
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
            Write(hostsFilePath, content, Encoding.Default);
        }

        private void btnLoadHosts_Click(object sender, EventArgs e)
        {
            hostsDic = new Dictionary<string, List<string>>();
            // 1-读取hosts文件
            string hostsContent = this.Read(Form1.hostsFilePath);
            //label1.Text = hostsContent;

            lbHosts.Items.Clear();
            foreach (KeyValuePair<string, List<string>> item in hostsDic)
            {
                lbHosts.Items.Add(item.Key);
            }
        }

        private void btnAddHosts_Click(object sender, EventArgs e)
        {
            string ipHosts = tbHosts.Text.ToString().Replace("#", "").Trim();
            List<string> lineDomain = MatchsDomain(ipHosts);
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
                if (lineString.Length > 0 && this.isNumberic(lineString.Substring(0, 1)))
                {
                    lineDomain = MatchsDomain(lineString);
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
            Write(hostsFilePath, content, Encoding.Default);

            // 重新加载Hosts
            hostsDic = new Dictionary<string, List<string>>();
            // 1-读取hosts文件
            string hostsContent = this.Read(Form1.hostsFilePath);
            //label1.Text = hostsContent;

            lbHosts.Items.Clear();
            foreach (KeyValuePair<string, List<string>> item in hostsDic)
            {
                lbHosts.Items.Add(item.Key);
            }
        }

        private void btnOpenHosts_Click(object sender, EventArgs e)
        {
            // 打开文件
            System.Diagnostics.Process.Start("notepad.exe", hostsFilePath);
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {            
            if (e.ClickedItem.Text == "退出")
            {
                isClose = true;
                this.Close();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            if(this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
            }
            //this.notifyIcon1.Visible = false;
            //this.ShowInTaskbar = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!isClose)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
                e.Cancel = true;
            }
        }
    }
}
