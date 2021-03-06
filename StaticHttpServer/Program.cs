using System;
using System.Windows.Forms;

namespace StaticHttpServer {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			if( args.Length == 0 ) {
				MessageBox.Show("Requires a directory.", "Command-line Error");
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1(args[0]));
		}
	}
}
