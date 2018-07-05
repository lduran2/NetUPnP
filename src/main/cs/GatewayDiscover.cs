using System.Collections.Generic; /* IDictionary, ICollection */
using System.Net; /* IPAddress */

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

				/** Default timeout. */
				private const uint DEFAULT_TIMEOUT = 3000;

				/** 
				 * The gateway types for which the discover will
				 * search.
				 */
				private string[] searchTypes;

				// /** Timeout for the broadcase request. */
				// private uint timeout;

				/**
				 * Constructor.
				 * @param types for which the discover will search
				 */
				public GatewayDiscover(params string[] types) {
					this.searchTypes = types;
					// this.timeout = DEFAULT_TIMEOUT;
				} /* end GatewayDiscovery(params string[]) */

				public IDictionary<IPAddress, GatewayDevice> Discover() {
					ICollection<IPAddress> ips;
					return null;
				}
			} /* end class GatewayDiscover */

			public class GatewayDevice {}
		} /* end namespace org.dark_archives.NetUPnP */
	} /* end namespace org.dark_archives */
} /* end namespace org */
