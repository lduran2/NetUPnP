using System; /* String */
using System.Net; /* IPAddress */
using System.Net.NetworkInformation; /* NetworkInterface */
using System.Net.Sockets; /* UdpClient, UdpReceiveResult */
using System.Threading; /* ThreadAbortException */
using System.Threading.Tasks; /* Task */
using System.Collections.Generic; /* IDictionary, ICollection */
using System.Text; /* Encoding */

namespace org {
	namespace dark_archives {
		namespace NetUPnP {
			/**
			 * A gateway discover that searches for the 3 default
			 * types.
			 */
			public sealed class DefaultGatewayDiscover : GatewayDiscover {
				private static readonly IEnumerable<string> DEFAULT_SEARCH_TYPES = new string[] {
					"urn:schemas-upnp-org:device:InternetGatewayDevice:1",
					"urn:schemas-upnp-org:service:WANIPConnection:1",
					"urn:schemas-upnp-org:service:WANPPPConnection:1"
				};

				/**
				 * Constructor.
				 */
				public DefaultGatewayDiscover() : base(DEFAULT_SEARCH_TYPES)
				{} /* end DefaultGatewayDiscover() */
			} /* end class DefaultGatewayDiscover */

			public class GatewayDiscover {

				/** Broadcast address for contacting UPnP devices */
				private const string IP = "239.255.255.250";

				/** The SSDP port */
				private const int PORT = 1900;

				/** 
				 * The gateway types for which the discover will
				 * search
				 */
				private readonly IEnumerable<string> searchTypes;

				/**
				 * GatewayDevices discovered so far.
				 * The relationship of IPAddress to GatewayDevice is functional.
				 */
				private readonly IDictionary<IPAddress, GatewayDevice> devices;

				/**
				 * Constructor.
				 * @param types for which the discover will search
				 */
				public GatewayDiscover(IEnumerable<string> types) {
					this.searchTypes = types;
					this.devices = new Dictionary<IPAddress, GatewayDevice>();
				} /* end GatewayDiscovery(IEnumerable<string>) */

				public IDictionary<IPAddress, GatewayDevice> Discover() {
					ICollection<IPAddress> ips;
					IEnumerator<string> iType;
					string searchMessage;
					ICollection<Thread> threads;
					Thread newThread;

					ips = getLocalIpAddresses(true, false, false);

					/* look through the search types until a device is
					   found, or no more search types */
					for (iType = searchTypes.GetEnumerator(); ((0 == devices.Count) && iType.MoveNext()); ) {
						searchMessage = String.Format(
							"M-SEARCH * HTTP/1.1\r\n"
							+ "HOST: {0}:{1}\r\n"
							+ "ST: {2}\r\n"
							+ "MAN: \"{3}\"\r\n"
							+ "MX: {4}\r\n" /* response delay in seconds */
							+ "\r\n",
							IP, PORT, iType.Current, "ssdp:discover", 2
						);

						/* perform search requests for multiple network adapters concurrently */
						threads = new LinkedList<Thread>();
						foreach (IPAddress ip in ips) {
							newThread = new Thread((/* params here */) => {
								// Some code here which will run in another thread
							});
							threads.Add(newThread);
							newThread.Start();
						}

						/* wait for all search threads to finish */
						foreach (Thread thread in threads) {
							try {
								thread.Join();
							}
							catch (ThreadAbortException tae) {
								/* if interrupted, continue with the
								   next thread */
							}
						}

					}
					return devices;
				}

				private LinkedList<IPAddress> getLocalIpAddresses(bool getIp4, bool getIp6, bool shouldIp4BeforeIp6) {
					LinkedList<IPAddress> ipAddresses = new LinkedList<IPAddress>();
					int iLastIp4Address = 0;
					bool areNoIpAddresses = false;

					/* get all network interfaces */
					NetworkInterface[] networkInterfaces = null;
					try {
						networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					}
					catch (NetworkInformationException nie) {
						areNoIpAddresses = true;
					}

					areNoIpAddresses |= (networkInterfaces == null);
					if (areNoIpAddresses) {
						return ipAddresses;
					}

					/* For every suitable network interface,
					   get all IP addresses */
					
					foreach (NetworkInterface card in networkInterfaces) {
						bool isSuitableToSearchGateways = true;
						try {
							if ((card.NetworkInterfaceType == NetworkInterfaceType.Loopback) || (card.NetworkInterfaceType == NetworkInterfaceType.Ppp) || (!NetworkInterface.GetIsNetworkAvailable())) {
								isSuitableToSearchGateways = false;
							}
						}
						catch (Exception ex) {
							isSuitableToSearchGateways = false;
						}
						if (!isSuitableToSearchGateways) {
							continue;
						}
					}
					return null;
				}

			} /* end class GatewayDiscover */
		} /* end namespace org.dark_archives.NetUPnP */
	} /* end namespace org.dark_archives */
} /* end namespace org */
