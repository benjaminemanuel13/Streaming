using StreamConsole;
using StreamingClientWin.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamingClientWin.Forms
{
    public partial class MainForm : Form
    {
        private Controller controller = new Controller();

        public MainForm()
        {
            InitializeComponent();
        }

        private async void connect_Click(object sender, EventArgs e)
        {
            connect.Enabled = false;
            disconnect.Enabled = true;
            start.Enabled = true;
            stop.Enabled = false;

            await controller.Connect();
        }

        private async void disconnect_Click(object sender, EventArgs e)
        {
            connect.Enabled = true;
            disconnect.Enabled = false;
            start.Enabled = false;
            stop.Enabled = false;

            await controller.Stop();
            await controller.Disconnect();
        }

        private async void start_Click(object sender, EventArgs e)
        {
            start.Enabled = false;
            stop.Enabled = true;

            await controller.Start();
        }

        private async void stop_Click(object sender, EventArgs e)
        {
            start.Enabled = true;
            stop.Enabled = false;

            await controller.Stop();
        }

        private async void getServers_Click(object sender, EventArgs e)
        {
            AdminService admin = new AdminService();
            var list = await admin.GetServers();

            servers.Nodes.Clear();
            foreach (var server in list)
            {
                var root = servers.Nodes.Add(server.Id);
                int count = 1;
                foreach (var client in server.Clients)
                {
                    root.Nodes.Add("Client:" + count++);
                }
            }
        }

        private async void getlog_Click(object sender, EventArgs e)
        {
            AdminService admin = new AdminService();

            log.Text = await admin.GetLog();
        }

        private async void clearlog_Click(object sender, EventArgs e)
        {
            AdminService admin = new AdminService();

            await admin.ClearLog();

            log.Text = "";
        }

        private async void clearservers_Click(object sender, EventArgs e)
        {
            AdminService admin = new AdminService();

            await admin.ClearServers();
        }
    }
}
