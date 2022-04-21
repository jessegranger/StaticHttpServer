using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StaticHttpServer {
	class Server {
	
		public string DefaultDocument = "index.html";
		public string DefaultContentType = "application/octet-stream";

		private void ServerThread() {
			Debug.WriteLine("ServerThread: Starting...");
			while ( Started ) {
				try {
					var context = listener.GetContext();
					ThreadPool.QueueUserWorkItem(HandleRequest, context);
				} catch( Exception e ) {
					Debug.WriteLine(e.Message);
					break;
				}
			}
			Debug.WriteLine("ServerThread: Exiting...");
		}
		private void HandleRequest(Object stateInfo) {
			HttpListenerContext context = stateInfo as HttpListenerContext;
			var req = context.Request;
			var res = context.Response;


			var filename = req.Url.AbsolutePath.Substring(1);
			if ( string.IsNullOrWhiteSpace(filename) ) {
				filename = DefaultDocument;
			}
			filename = Path.Combine(RootDirectory, filename);

			if (req.HttpMethod != "GET" ) {
				res.StatusCode = (int)HttpStatusCode.NotImplemented;
			} else if ( !File.Exists(filename) ) {
				res.StatusCode = (int)HttpStatusCode.NotFound;
			} else try {
				Stream input = new FileStream(filename, FileMode.Open);
				res.ContentType = DefaultContentType;
				if ( mimeTypes.TryGetValue(Path.GetExtension(filename), out string mime) ) {
					res.ContentType = mime;
				}
				res.ContentLength64 = input.Length;
				// res.AddHeader("Date", DateTime.Now.ToString("r"));
				// res.AddHeader("Last-Modified", ...)
				byte[] buf = new byte[32 * 1024];
				int n;
				while ( (n = input.Read(buf, 0, buf.Length)) > 0 ) {
					res.OutputStream.Write(buf, 0, n);
				}
				input.Close();
				res.OutputStream.Flush();
				res.StatusCode = (int)HttpStatusCode.OK;
			} catch ( Exception err ) {
				Debug.WriteLine($"Error: {err.Message}");
				Debug.WriteLine($"Stack: {err.StackTrace}");
				res.StatusCode = (int)HttpStatusCode.InternalServerError;
				// TODO: set headers and send error body
			}
			Debug.WriteLine($"GET {req.RawUrl} {res.StatusCode}");
			res.OutputStream.Close();
			res.Close();
		}


		public string RootDirectory { get; private set; }
		public ushort Port {
			get => port;
			set {
				port = value;
				if( Started ) {
					Stop();
					Thread.Sleep(500);
					Start();
				}
			}
		}
		private ushort port;

		private HttpListener listener = new HttpListener();
		private Thread serverThread;
		public bool Started {
			get;
			private set;
		} = false;
		public void Start() {
			if ( Started ) return;
			try {
				listener.Prefixes.Clear();
				listener.Prefixes.Add($"http://localhost:{Port}/");
				listener.Start();
				Started = true;
				serverThread = new Thread(ServerThread);
				serverThread.SetApartmentState(ApartmentState.STA);
				serverThread.Start();
			} catch ( Exception err ) {
				Debug.WriteLine($"Listener failed to Start: {err.Message}");
				Started = false;
			}
		}
		public void Stop() {
			if ( !Started ) return;
			try {
				Started = false;
				listener.Stop();
			} catch ( Exception err ) {
				Debug.WriteLine($"Listener failed to Stop: {err.Message}");
			}
		}
		public Server(string path, ushort port) {
			RootDirectory = path;
			Port = port;
		}

		private static IDictionary<string, string> mimeTypes =
		 new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		 { // just the basic mime types
								{".css", "text/css"},
								{".ear", "application/java-archive"},
								{".gif", "image/gif"},
								{".htc", "text/x-component"},
								{".htm", "text/html"},
								{".html", "text/html"},
								{".ico", "image/x-icon"},
								{".jar", "application/java-archive"},
								{".jpeg", "image/jpeg"},
								{".jpg", "image/jpeg"},
								{".js", "application/x-javascript"},
								{".pdf", "application/pdf"},
								{".png", "image/png"},
								{".rar", "application/x-rar-compressed"},
								{".rss", "text/xml"},
								{".shtml", "text/html"},
								{".txt", "text/plain"},
								{".xml", "text/xml"},
								{".zip", "application/zip"},
		 };
	}

}
