# StaticHttpServer
Simplest Windows Static HTTP Server

Run it with a directory as an argument, and it answers HTTP GET requests for static files from that directory.

(RECOMMENDED) Put a shell command in the registry so you can right click on a directory:

Create the following key:
HKEY_CURRENT_USER\SOFTWARE\Classes\Directory\shell\StaticHttpServer\command
Set the default value of that key to:
"C:\Users\me\Projects\StaticHttpServer\StaticHttpServer\bin\Release\StaticHttpServer.exe" "%1"

Replace with the real path to your StaticHttpServer.exe
