using System; /* String */
using System.Net; /* IPAddress, NetworkInterface */
using System.Collections.Generic; /* IDictionary, ICollection, IEnumerator */
using org.dark_archives.NetUPnP;

public static class MainClass {
	public static void Main(params string[] args) {
		GatewayDiscover gatewayDiscover;
		IDictionary<IPAddress, GatewayDevice> gateways;
		IEnumerator<GatewayDevice> devices;
		int k;
		GatewayDevice dev;

		addLogLine("Starting NetUPnP.");
		gatewayDiscover = new DefaultGatewayDiscover();

		addLogLine("Looking for Gateway Devices...");
		gateways = gatewayDiscover.Discover();

		if (0 == gateways.Count) {
			addLogLine("No gateways found");
			addLogLine("Stopping netupnp");
			return;
		}

		addLogLine(String.Format("{0} gateway{1} found\n",
		                         gateways.Count,
		                         ((gateways.Count > 0) ? "s" : "")));

		devices = gateways.Values.GetEnumerator();
		for (k = 1; (gateways.Count >= k); ++k) {
			devices.MoveNext();
			dev = devices.Current;
			addLogLine(String.Format("Listing gateway details of device "
			                         	+ "#{0}\n"
			                         	+ "\tFriendly name: {1}\n"
			                         	+ "\tPresentation URL: {2}\n"
			                         	+ "\tModel name: {3}\n"
			                         	+ "\tModel number: {4}\n"
			                         	+ "\tLocal interface address: "
			                         	+ "{5}\n",
			                         k, dev.getFriendlyName(),
			                         dev.getPresentationURL(),
			                         dev.getModelName(),
			                         dev.getModelNumber(),
			                         dev.getLocalAddress().ToString()
			));
		}
	}

	public static void addLogLine(string line) {
		string timeStamp = DateTime.Now.ToString("T");
		string logline = timeStamp + ": " + line + "\n";
		Console.Write(logline);
	}
}
