using System; /* String */
using System.Net; /* IPAddress */

namespace org {
	namespace dark_archives {
 		namespace NetUPnP {
			public sealed class GatewayDevice {

				public GatewayDevice() {
					this.friendlyName = "";
					this.presentationUrl = "";
					this.modelName = "";
					this.modelNumber = "";
					this.localAddress = new IPAddress(0);
				}

				public boolean isConnected() {
					string connectionStatus;

					connectionStatus = simpleUpnpCommand(controlUrl, serviceType, "GetStatusInfo", null);
					return (
						(connectionStatus != null)
						&& connectionStatus.EqualsIgnoreCase("Connected")
					);
				}

				private IDictionary<string, string> simpleUpnpCommand(string url, string service, string action, IDictionary<string, string> args) {
					return new Dictionar<string, string>();
				}

				public string Format(string format) {
					return String.Format(format,
					                     this.friendlyName,
					                     this.presentationUrl,
					                     this.modelName,
					                     this.modelNumber,
					                     this.localAddress.ToString()
					);
				}

				private readonly string friendlyName;
				private readonly string presentationUrl;
				private readonly string modelName;
				private readonly string modelNumber;
				private readonly IPAddress localAddress;

			}
		} /* end namespace org.dark_archives.NetUPnP */
	} /* end namespace org.dark_archives */
} /* end namespace org */
