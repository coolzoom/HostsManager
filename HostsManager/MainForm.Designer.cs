namespace HostsManager
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lbHosts = new System.Windows.Forms.ListBox();
            this.lbIPS = new System.Windows.Forms.ListBox();
            this.btnLoadHosts = new System.Windows.Forms.Button();
            this.tbHosts = new System.Windows.Forms.TextBox();
            this.btnAddHosts = new System.Windows.Forms.Button();
            this.btnOpenHosts = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbHosts
            // 
            this.lbHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbHosts.BackColor = System.Drawing.SystemColors.Window;
            this.lbHosts.Font = new System.Drawing.Font("宋体", 11F);
            this.lbHosts.FormattingEnabled = true;
            this.lbHosts.ItemHeight = 15;
            this.lbHosts.Location = new System.Drawing.Point(0, 30);
            this.lbHosts.Margin = new System.Windows.Forms.Padding(0);
            this.lbHosts.Name = "lbHosts";
            this.lbHosts.Size = new System.Drawing.Size(300, 424);
            this.lbHosts.TabIndex = 0;
            this.lbHosts.SelectedIndexChanged += new System.EventHandler(this.lbHosts_SelectedIndexChanged);
            // 
            // lbIPS
            // 
            this.lbIPS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbIPS.Font = new System.Drawing.Font("宋体", 11F);
            this.lbIPS.FormattingEnabled = true;
            this.lbIPS.ItemHeight = 15;
            this.lbIPS.Location = new System.Drawing.Point(300, 30);
            this.lbIPS.Margin = new System.Windows.Forms.Padding(0);
            this.lbIPS.Name = "lbIPS";
            this.lbIPS.Size = new System.Drawing.Size(506, 424);
            this.lbIPS.TabIndex = 3;
            this.lbIPS.SelectedIndexChanged += new System.EventHandler(this.lbIPS_SelectedIndexChanged);
            // 
            // btnLoadHosts
            // 
            this.btnLoadHosts.Location = new System.Drawing.Point(0, 2);
            this.btnLoadHosts.Name = "btnLoadHosts";
            this.btnLoadHosts.Size = new System.Drawing.Size(75, 23);
            this.btnLoadHosts.TabIndex = 4;
            this.btnLoadHosts.Text = "加载Hosts";
            this.btnLoadHosts.UseVisualStyleBackColor = true;
            this.btnLoadHosts.Click += new System.EventHandler(this.btnLoadHosts_Click);
            // 
            // tbHosts
            // 
            this.tbHosts.Location = new System.Drawing.Point(181, 4);
            this.tbHosts.Name = "tbHosts";
            this.tbHosts.Size = new System.Drawing.Size(286, 21);
            this.tbHosts.TabIndex = 5;
            this.tbHosts.Text = "示例：127.0.0.1 localhost;127.0.0.1 local";
            this.tbHosts.Enter += new System.EventHandler(this.tbHosts_Enter);
            this.tbHosts.Leave += new System.EventHandler(this.tbHosts_Leave);
            // 
            // btnAddHosts
            // 
            this.btnAddHosts.Location = new System.Drawing.Point(473, 4);
            this.btnAddHosts.Name = "btnAddHosts";
            this.btnAddHosts.Size = new System.Drawing.Size(75, 23);
            this.btnAddHosts.TabIndex = 6;
            this.btnAddHosts.Text = "添加绑定";
            this.btnAddHosts.UseVisualStyleBackColor = true;
            this.btnAddHosts.Click += new System.EventHandler(this.btnAddHosts_Click);
            // 
            // btnOpenHosts
            // 
            this.btnOpenHosts.Location = new System.Drawing.Point(81, 2);
            this.btnOpenHosts.Name = "btnOpenHosts";
            this.btnOpenHosts.Size = new System.Drawing.Size(75, 23);
            this.btnOpenHosts.TabIndex = 7;
            this.btnOpenHosts.Text = "打开Hosts";
            this.btnOpenHosts.UseVisualStyleBackColor = true;
            this.btnOpenHosts.Click += new System.EventHandler(this.btnOpenHosts_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "HostsManager";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 6000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 471);
            this.Controls.Add(this.btnOpenHosts);
            this.Controls.Add(this.btnAddHosts);
            this.Controls.Add(this.tbHosts);
            this.Controls.Add(this.btnLoadHosts);
            this.Controls.Add(this.lbIPS);
            this.Controls.Add(this.lbHosts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "HostsManager";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbHosts;
        private System.Windows.Forms.ListBox lbIPS;
        private System.Windows.Forms.Button btnLoadHosts;
        private System.Windows.Forms.TextBox tbHosts;
        private System.Windows.Forms.Button btnAddHosts;
        private System.Windows.Forms.Button btnOpenHosts;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Timer timer1;
    }
}

