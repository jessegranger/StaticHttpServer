using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StaticHttpServer {
	public partial class Form1 : Form {
		public Form1(string path) {
			InitializeComponent();
			this.Text = path;
			server = new Server(path, (ushort)Math.Round(numericUpDown1.Value));
		}

		private Server server;

		private void btnStart_Click(object sender, EventArgs e) {
			if ( server.Started ) {
				server.Stop();
				btnStart.Text = "Start";
				this.BackColor = Color.Black;
			} else {
				server.Start();
				btnStart.Text = "Stop";
				this.BackColor = Color.Green;
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
			server.Port = (ushort)(sender as NumericUpDown).Value;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			server.Stop();
		}
	}
}
