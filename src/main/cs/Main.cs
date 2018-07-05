using System;
using org.dark_archives.NetUPnP;

public static class MainClass {
	public static void Main(params string[] args) {
		GatewayDiscover gatewayDiscover;
		addLogLine("Starting NetUPnP.");
		gatewayDiscover = new DefaultGatewayDiscover();
		addLogLine("Looking for Gateway Devices...");
	}

	public static void addLogLine(string line) {
		string timeStamp = DateTime.Now.ToString("T");
		string logline = timeStamp + ": " + line + "\n";
		Console.Write(logline);
	}
}
