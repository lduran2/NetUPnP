using System; /* String */
using System.Net; /* IPAddress */
using System.Net.NetworkInformation; /* NetworkInterface */
using System.Net.Sockets; /* UdpClient, UdpReceiveResult */
using System.Threading; /* ThreadAbortException */
using System.Threading.Tasks; /* Task */
using System.Linq; /* Enumerable */
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
					IEnumerator<IPAddress> ips;
					IEnumerator<string> iType;
					string searchMessage;
					ICollection<Thread> threads;
					Thread newThread;

					ips = getLocalIpAddresses(IpAddressListType.onlyIp4);

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
						for (; ips.MoveNext(); ) {
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
								/* noop: if interrupted, continue with
								   the next thread */
							}

							if (0 < devices.Count) {
								break;
							}
						}

					}
					return devices;
				}

				private enum IpAddressListType { asIs, onlyIp4, onlyIp6, ip4BeforeIp6 };

				private IEnumerator<IPAddress> getLocalIpAddresses(IpAddressListType listType) {
					List<IPAddress> ipAddresses;
					IEnumerator<IPAddress> iAddresses;
					IPAddressCollection addresses;
					IPAddress ipAddress;
					int index;
					int iLastIp4 = 0;
					NetworkInterface[] networkInterfaces;
					bool isSuitableToSearchGateways = true;

					/* figure out insert in linked list, preferably */
					ipAddresses = new List<IPAddress>();

					networkInterfaces = null;
					try {
						networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					} catch (NetworkInformationException nie) {}

					if (networkInterfaces == null) {
						return Enumerable.Empty<IPAddress>().GetEnumerator();
					}

					foreach (NetworkInterface card in networkInterfaces) {
						try {
							isSuitableToSearchGateways &= (card.NetworkInterfaceType != NetworkInterfaceType.Loopback);
							isSuitableToSearchGateways &= (card.NetworkInterfaceType != NetworkInterfaceType.Ppp);
							isSuitableToSearchGateways &= !NetworkInterface.GetIsNetworkAvailable();
						} catch (SocketException se) {
							isSuitableToSearchGateways = false;
						}

						if (!isSuitableToSearchGateways) {
							continue;
						}

						addresses = card.GetIPProperties().WinsServersAddresses;
						iAddresses = addresses.GetEnumerator();

						for (; iAddresses.MoveNext(); ) {
							ipAddress = iAddresses.Current;
							index = ipAddresses.Count;

							switch (listType) {
								case IpAddressListType.onlyIp4:
									if (ipAddress.AddressFamily != AddressFamily.InterNetwork) {
										continue;
									}
									break;
								case IpAddressListType.onlyIp6:
									if (ipAddress.AddressFamily != AddressFamily.InterNetworkV6) {
										continue;
									}
									break;
								case IpAddressListType.ip4BeforeIp6:
									if (ipAddress.AddressFamily != AddressFamily.InterNetwork) {
										index = iLastIp4++;
									}
									break;
							}
							ipAddresses.Insert(index, ipAddress);
						}
					}

					return ipAddresses.GetEnumerator();
				}

			} /* end class GatewayDiscover */
		} /* end namespace org.dark_archives.NetUPnP */
	} /* end namespace org.dark_archives */
} /* end namespace org */
