# VC2OPCUA

Visual Components Plugin that creates an [OPC UA](https://opcfoundation.org/about/opc-technologies/opc-ua/) Server exposing your model data.

**Server URI:** <opc.tcp://localhost:51210/UA/Vc2OpcUaServer/>

The server exposes the active components in a Visual Components model, together with their signals and properties.

When installed, a VC2OPCUA Panel will be added to Visual Components main window:

![Visual Components Screenshot](/screenshots/visual-components.png?raw=true "Visual Components Screenshot")

**Node structure:**

![OPCUA Client Screenshot](/screenshots/opcua-client.png?raw=true "OPCUA Client Screenshot")


## Requirements
 - [Visual Components](https://visualcomponents.com/)
 - [OPC UA .NET Stack](https://github.com/OPCFoundation/UA-.NETStandard) (only required if you want to compile this plugin yourself)

## License

This project is licensed under the GNU General Public License v2. You should have received a copy of the license along with this software. If not, see <https://opcfoundation.org/license/gpl.html> or <https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html>