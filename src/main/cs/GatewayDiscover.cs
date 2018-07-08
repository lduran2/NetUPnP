using System; /* String */
using System.Net; /* IPAddress */
using System.Net.NetworkInformation; /* NetworkInterface */
using System.Collections.Generic; /* IDictionary, ICollection */
using System.Threading; /* ThreadAbortException */
// using Windows.Networking.Sockets; /* DatagramSocket */

namespace org {
	namespace dark_archives {
		namespace NetUPnP {
			/**
			 * A gateway discover that searches for the 3 default
			 * types.
			 */
			public class DefaultGatewayDiscover : GatewayDiscover {
				private static readonly string[] DEFAULT_SEARCH_TYPES = new string[] {
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

				/** Default timeout */
				private const uint DEFAULT_TIMEOUT = 3000;

				/** 
				 * The gateway types for which the discover will
				 * search
				 */
				private string[] searchTypes;

				// /** Timeout for the broadcase request. */
				// private uint timeout;

				/**
				 * GatewayDevices discovered so far.
				 * The relationship of IPAddress to GatewayDevice is functional.
				 */
				private readonly IDictionary<IPAddress, GatewayDevice> devices;

				/**
				 * Constructor.
				 * @param types for which the discover will search
				 */
				public GatewayDiscover(params string[] types) {
					this.searchTypes = types;
					// this.timeout = DEFAULT_TIMEOUT;
					this.devices = new Dictionary<IPAddress, GatewayDevice>();
				} /* end GatewayDiscovery(params string[]) */

				public IDictionary<IPAddress, GatewayDevice> Discover() {
					ICollection<IPAddress> ips;
					uint k;
					string searchMessage;

					ips = getLocalIpAddresses(true, false, false);

					/* look through the search types until a device is
					   found, or no more search types */
					for (k = 0; ((0 == devices.Count) && (searchTypes.Length > k)); ++k) {
						searchMessage = String.Format(
							"M-SEARCH * HTTP/1.1\r\n"
							+ "HOST: {0}:{1}\r\n"
							+ "ST: {2}\r\n"
							+ "MAN: \"{3}\"\r\n"
							+ "MX: {4}\r\n" /* response delay in seconds */
							+ "\r\n",
							IP, PORT, searchTypes[k], "ssdp:discover", 2
						);

						/* perform search requests for multiple network adapters concurrently */
						ICollection<SendDiscoveryThread> threads = new LinkedList<SendDiscoveryThread>();
						foreach (IPAddress ip in ips) {
							SendDiscoveryThread thread = new SendDiscoveryThread(ip, searchMessage);
							threads.Add(thread);
							thread.Start();
						}

						/* wait for all search threads to finish */
						foreach (SendDiscoveryThread thread in threads) {
							try {
								thread.Join();
							}
							catch (ThreadAbortException taex) {
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
					catch (NetworkInformationException niex) {
						areNoIpAddresses = true;
					}

					areNoIpAddresses |= (networkInterfaces == null);
					if (areNoIpAddresses) {
						return ipAddresses;
					}

					/* For every suitable network interface,
					   get all IP addresses */
					foreach (NetworkInterface card in networkInterfaces) {
						try {
							
						}
						catch (Exception ex) {
							
						}
					}
					return null;
				}

				private class SendDiscoveryThread {
					IPAddress ip;
					string searchMessage;

					public SendDiscoveryThread(IPAddress localIp, string initSearchMessage) {
						this.ip = localIp;
						this.searchMessage = initSearchMessage;
					}

					public void Start() {}
					public void Join() {}

					public void Run() {
						// IDatagramSocket ssdp = null;

						try {
							/* Create socket bound to specified local address */
							// ssdp = new DatagramSocket();
							//ssdp.BindEndpointAsync(new HostName(String.Format("{0}:{1}", ip, 0)), );
						}
						catch (ThreadAbortException taex) {
							
						}
					}
				}
			} /* end class GatewayDiscover */
		} /* end namespace org.dark_archives.NetUPnP */
	} /* end namespace org.dark_archives */
} /* end namespace org */
