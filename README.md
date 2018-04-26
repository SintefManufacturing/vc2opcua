# VC2OPCUA

Visual Components Plugin that creates an [OPC UA](https://opcfoundation.org/about/opc-technologies/opc-ua/) Server exposing your model data.

The server exposes the active components in a Visual Components model, together with their signals and properties.
![Visual Components Screenshot](/screenshots/visual-components.png?raw=true "Visual Components Screenshot")

Node structure:
![OPCUA Client Screenshot](/screenshots/opcua-client.png?raw=true "OPCUA Client Screenshot")

Server URI: opc.tcp://localhost:51210/UA/Vc2OpcUaServer/

#### Requirements:
 - [Visual Components](https://visualcomponents.com/)
 - [OPC UA .NET Stack](https://github.com/OPCFoundation/UA-.NETStandard)
