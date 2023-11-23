namespace StreamingClientWin.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connect = new System.Windows.Forms.Button();
            this.disconnect = new System.Windows.Forms.Button();
            this.start = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.servers = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.getServers = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.TextBox();
            this.getlog = new System.Windows.Forms.Button();
            this.clearlog = new System.Windows.Forms.Button();
            this.clearservers = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(15, 13);
            this.connect.Margin = new System.Windows.Forms.Padding(4);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(213, 32);
            this.connect.TabIndex = 0;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // disconnect
            // 
            this.disconnect.Enabled = false;
            this.disconnect.Location = new System.Drawing.Point(15, 53);
            this.disconnect.Margin = new System.Windows.Forms.Padding(4);
            this.disconnect.Name = "disconnect";
            this.disconnect.Size = new System.Drawing.Size(213, 32);
            this.disconnect.TabIndex = 1;
            this.disconnect.Text = "Disconnect";
            this.disconnect.UseVisualStyleBackColor = true;
            this.disconnect.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // start
            // 
            this.start.Enabled = false;
            this.start.Location = new System.Drawing.Point(15, 94);
            this.start.Margin = new System.Windows.Forms.Padding(4);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(213, 32);
            this.start.TabIndex = 2;
            this.start.Text = "Start Streaming";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // stop
            // 
            this.stop.Enabled = false;
            this.stop.Location = new System.Drawing.Point(15, 134);
            this.stop.Margin = new System.Windows.Forms.Padding(4);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(213, 32);
            this.stop.TabIndex = 3;
            this.stop.Text = "Stop Streaming";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // servers
            // 
            this.servers.Location = new System.Drawing.Point(266, 38);
            this.servers.Margin = new System.Windows.Forms.Padding(4);
            this.servers.Name = "servers";
            this.servers.Size = new System.Drawing.Size(206, 173);
            this.servers.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(266, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server State";
            // 
            // getServers
            // 
            this.getServers.Location = new System.Drawing.Point(376, 221);
            this.getServers.Margin = new System.Windows.Forms.Padding(4);
            this.getServers.Name = "getServers";
            this.getServers.Size = new System.Drawing.Size(96, 32);
            this.getServers.TabIndex = 6;
            this.getServers.Text = "Refresh";
            this.getServers.UseVisualStyleBackColor = true;
            this.getServers.Click += new System.EventHandler(this.getServers_Click);
            // 
            // log
            // 
            this.log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.log.Location = new System.Drawing.Point(494, 38);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(401, 173);
            this.log.TabIndex = 7;
            // 
            // getlog
            // 
            this.getlog.Location = new System.Drawing.Point(685, 222);
            this.getlog.Name = "getlog";
            this.getlog.Size = new System.Drawing.Size(102, 31);
            this.getlog.TabIndex = 8;
            this.getlog.Text = "Get";
            this.getlog.UseVisualStyleBackColor = true;
            this.getlog.Click += new System.EventHandler(this.getlog_Click);
            // 
            // clearlog
            // 
            this.clearlog.Location = new System.Drawing.Point(793, 222);
            this.clearlog.Name = "clearlog";
            this.clearlog.Size = new System.Drawing.Size(102, 31);
            this.clearlog.TabIndex = 9;
            this.clearlog.Text = "Clear";
            this.clearlog.UseVisualStyleBackColor = true;
            this.clearlog.Click += new System.EventHandler(this.clearlog_Click);
            // 
            // clearservers
            // 
            this.clearservers.Location = new System.Drawing.Point(266, 221);
            this.clearservers.Name = "clearservers";
            this.clearservers.Size = new System.Drawing.Size(102, 31);
            this.clearservers.TabIndex = 10;
            this.clearservers.Text = "Clear";
            this.clearservers.UseVisualStyleBackColor = true;
            this.clearservers.Click += new System.EventHandler(this.clearservers_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(494, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 21);
            this.label2.TabIndex = 11;
            this.label2.Text = "Log";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 273);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clearservers);
            this.Controls.Add(this.clearlog);
            this.Controls.Add(this.getlog);
            this.Controls.Add(this.log);
            this.Controls.Add(this.getServers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.servers);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.start);
            this.Controls.Add(this.disconnect);
            this.Controls.Add(this.connect);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button connect;
        private Button disconnect;
        private Button start;
        private Button stop;
        private TreeView servers;
        private Label label1;
        private Button getServers;
        private TextBox log;
        private Button getlog;
        private Button clearlog;
        private Button clearservers;
        private Label label2;
    }
}