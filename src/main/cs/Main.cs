using System; /* String */
using System.Net; /* IPAddress, NetworkInterface */
using System.Collections.Generic; /* IDictionary, ICollection, IEnumerator */
using org.dark_archives.NetUPnP;

public static class MainClass {
	public static void Main(params string[] args) {
		GatewayDevice active;
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

		addLogLine(
			String.Format("{0} gateway{1} found\n",
				gateways.Count,
				((gateways.Count > 0) ? "s" : "")
			)
		);

		devices = gateways.Values.GetEnumerator();
		for (k = 1; (gateways.Count >= k); ++k) {
			devices.MoveNext();
			dev = devices.Current;
			addLogLine(
				dev.Format(
					String.Format(
						"Listing gateway details of device #{0}\n"
						+ "\tFriendly name: {{0}}\n"
						+ "\tPresentation URL: {{1}}\n"
						+ "\tModel name: {{2}}\n"
						+ "\tModel number: {{3}}\n"
						+ "\tLocal interface address: {{4}}\n",
						k
					)
				)
			);
		}

		/* choose the first active gateway to test */
		active = gatewayDiscover.ValidGateway(null);

		if (null != active) {
			addLogLine(active.Format("Using gateway: {0}"));
		}
		else {
			addLogLine("No active gateway device found");
			addLogLine("Stopping weupnp");
		}
	}

	public static void addLogLine(string line) {
		string timeStamp = DateTime.Now.ToString("T");
		string logline = timeStamp + ": " + line + "\n";
		Console.Write(logline);
	}
}
