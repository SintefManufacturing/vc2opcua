/*
 * Bridge between Visual Components and OPCUA  
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using Opc.Ua;
using Opc.Ua.Sample;
using Opc.Ua.Server;
using VisualComponents.Create3D;

namespace vc2opcua
{
    class Vc2OpcUaBridge
    {
        [Import]
        IApplication _application = null;

        VcUtils _vcUtils = new VcUtils();
        IServerInternal uaServer;
        public UaNodeManager nodeManager;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructors

        public Vc2OpcUaBridge(IServerInternal server,
            ApplicationConfiguration configuration)
        {
            uaServer = server;
            nodeManager = new UaNodeManager(uaServer, configuration);

            nodeManager.AddressSpaceCreated += NodeManager_AddressSpaceCreated;

            _application = IoC.Get<IApplication>();
            // Subscribe to added/removing component events
            _application.World.ComponentAdded += World_ComponentAdded;
            _application.World.ComponentRemoving += World_ComponentRemoving;
        }

        #endregion

        #region Properties

        // Correlates OPCUA BrowseNames to corresponding component names
        public Dictionary<string, string> UaBrowseName2VcComponentName { get; set; } = new Dictionary<string, string>();
        // Correlates types between VC Signal Types and OPCUA types
        Dictionary<BehaviorType, NodeId> VcSignal2OpcuaType { get; } = new Dictionary<BehaviorType, NodeId>
        {
            { BehaviorType.StringSignal, new NodeId(DataTypeIds.String) },
            { BehaviorType.BooleanSignal, new NodeId(DataTypeIds.Boolean) },
            { BehaviorType.RealSignal, new NodeId(DataTypeIds.Double) },
            { BehaviorType.IntegerSignal, new NodeId(DataTypeIds.Integer) },
            { BehaviorType.ComponentSignal, new NodeId(DataTypeIds.String) }
        };

        // Correlates types between VC Property Types and OPCUA types
        Dictionary<Type, NodeId> VcProperty2OpcuaType { get; } = new Dictionary<Type, NodeId>
        {
            { typeof(String), new NodeId(DataTypeIds.String) },
            { typeof(Boolean), new NodeId(DataTypeIds.Boolean) },
            { typeof(Double), new NodeId(DataTypeIds.Double) },
            { typeof(Int32), new NodeId(DataTypeIds.Integer) }
        };

        #endregion

        #region Methods

        /// <summary>
        /// Gets the components active in simulation when server is started
        /// </summary>
        private List<NodeState> GetInitialNodes()
        {
            List<NodeState> initialNodes = new List<NodeState>();

            foreach(ISimComponent vcComponent in _vcUtils.GetComponents())
            {
                initialNodes.Add(CreateNodeTree(vcComponent));
            }

            return initialNodes;
        }

        /// <summary>
        /// Returns a node tree with a component and all its signals
        /// </summary>
        private ComponentState CreateNodeTree(ISimComponent simComponent)
        {

            ComponentState componentNode = CreateComponentNode(simComponent.Name);

            VcComponent vcComponent = new VcComponent(simComponent);

            AddSignalNodes(componentNode, vcComponent);
            AddPropertyNodes(componentNode, vcComponent);

            return componentNode;
        }

        /// <summary>
        /// Add signals from VC component to component OPCUA node
        /// </summary>
        private void AddSignalNodes(ComponentState uaComponentNode, VcComponent vcComponent)
        {
            foreach (ISignal vcSignal in vcComponent.GetSignals())
            {
                // Add signal node
                BaseDataVariableState uaSignalNode = CreateVariableNode(uaComponentNode.Signals, vcSignal.Name, VcSignal2OpcuaType[vcSignal.Type]);
                uaComponentNode.Signals.AddChild(uaSignalNode);

                // Store names in UaBrowseName2VcComponentName
                UaBrowseName2VcComponentName.Add(uaSignalNode.BrowseName.Name, vcComponent.component.Name);

                // Subscribe to signal triggered events
                vcSignal.SignalTrigger += vc_SignalTriggered;
                uaSignalNode.StateChanged += ua_SignalTriggered;
            }

        }

        /// <summary>
        /// Add properties from VC component to component OPCUA node
        /// </summary>
        private void AddPropertyNodes(ComponentState uaComponentNode, VcComponent vcComponent)
        {
            foreach (IProperty vcProperty in vcComponent.component.Properties)
            {
                try
                {
                    // Add signal node
                    BaseDataVariableState uaPropertyNode = CreateVariableNode(uaComponentNode.Properties, vcProperty.Name, VcProperty2OpcuaType[vcProperty.PropertyType]);
                    uaComponentNode.Properties.AddChild(uaPropertyNode);
                    
                    // Store names in UaBrowseName2VcComponentName
                    UaBrowseName2VcComponentName.Add(uaPropertyNode.BrowseName.Name, vcComponent.component.Name);

                    uaPropertyNode.Value = vcProperty.Value;

                    // Subscribe to property changed events
                    vcProperty.PropertyChanged += vc_PropertyChanged;
                    uaPropertyNode.StateChanged += ua_PropertyChanged;

                }
                catch (Exception ex)
                {
                    logger.Warn("Error adding property", ex);
                }
            }

        }

        /// <summary>
        /// Creates node for Component in Visual Components.
        /// </summary>
        private ComponentState CreateComponentNode(string nodeName)
        {
            string namespaceUri = Namespaces.vc2opcua;

            ComponentState componentNode = new ComponentState(null);
            NodeId componentNodeId = NodeId.Create(nodeName, namespaceUri, uaServer.NamespaceUris);
            NodeId signalsNodeId = NodeId.Create("Signals-" + nodeName, namespaceUri, uaServer.NamespaceUris);
            NodeId propertiesNodeId = NodeId.Create("Properties-" + nodeName, namespaceUri, uaServer.NamespaceUris);

            componentNode.Create(
                nodeManager.context,
                componentNodeId,
                new QualifiedName(nodeName, (ushort)uaServer.NamespaceUris.GetIndex(namespaceUri)),
                null,
                true);

            componentNode.Signals.NodeId = signalsNodeId;
            componentNode.Properties.NodeId = propertiesNodeId;

            componentNode.AddChild(componentNode.Signals);
            componentNode.AddChild(componentNode.Properties);

            nodeManager.baseFolder.AddReference(ReferenceTypeIds.Organizes, false, componentNode.NodeId);
            componentNode.AddReference(ReferenceTypeIds.Organizes, true, nodeManager.baseFolder.NodeId);

            return componentNode;
        }

        /// <summary>
        /// Creates node for Signal associated to Component in Visual Components.
        /// </summary>
        private BaseDataVariableState CreateVariableNode(FolderState parentFolder, string nodeName, NodeId nodeDataType)
        {
            string namespaceUri = Namespaces.vc2opcua;
            string nodeNameParent = String.Format("{0}-{1}", nodeName, parentFolder.Parent.DisplayName);

            BaseDataVariableState variableNode = new BaseDataVariableState(parentFolder);
            NodeId nodeId = NodeId.Create(nodeNameParent, namespaceUri, uaServer.NamespaceUris);

            variableNode.Create(
                nodeManager.context,
                nodeId,
                new QualifiedName(nodeNameParent, (ushort)uaServer.NamespaceUris.GetIndex(namespaceUri)),
                new LocalizedText(nodeName),
                true);
            
            variableNode.DataType = nodeDataType;

            parentFolder.AddReference(ReferenceTypeIds.Organizes, false, variableNode.NodeId);
            variableNode.AddReference(ReferenceTypeIds.Organizes, true, parentFolder.NodeId);

            return variableNode;
        }

        #endregion

        #region Event Handlers

        private void NodeManager_AddressSpaceCreated(object source, EventArgs args)
        {
            foreach (NodeState node in GetInitialNodes())
            {
                nodeManager.AddNode(node);
            }
        }

        private void World_ComponentAdded(object sender, ComponentAddedEventArgs e)
        {
            // Add component node
            ComponentState componentNode = CreateNodeTree(e.Component);

            nodeManager.AddNode(componentNode);

            _vcUtils.VcWriteWarningMsg("Component added: " + e.Component.Name);
        }

        private void World_ComponentRemoving(object sender, ComponentRemovingEventArgs e)
        {
            NodeId componentId = NodeId.Create(e.Component.Name, Namespaces.vc2opcua, uaServer.NamespaceUris);

            ComponentState componentNode = (ComponentState)nodeManager.FindPredefinedNode(componentId, 
                                                                                    typeof(ComponentState));

            if (componentNode != null)
            {
                nodeManager.RemoveNode(nodeManager.baseFolder, componentNode, ReferenceTypeIds.Organizes);
            }

            // Remove signals from SignalComponents property
            VcComponent vcComponent = new VcComponent(e.Component);
            foreach (ISignal signal in vcComponent.GetSignals())
            {
                string nodeNameParent = String.Format("{0}-{1}", signal.Name, vcComponent.component.Name);

                UaBrowseName2VcComponentName.Remove(nodeNameParent);
            }

            _vcUtils.VcWriteWarningMsg("Component removed: " + e.Component.Name);
        }

        /// <summary>
        /// Sets value of OPCUA node signal to corresponding VC signal
        /// </summary>
        private void ua_SignalTriggered(ISystemContext context, NodeState node, NodeStateChangeMasks changes)
        {

            if (!UaBrowseName2VcComponentName.ContainsKey(node.BrowseName.Name))
            {
                _vcUtils.VcWriteWarningMsg(String.Format("Component with signal {0} not found", node.BrowseName.Name));
                return;
            }

            // Cast signal OPCUA node to BaseDataVariableState type
            BaseDataVariableState uaSignal = (BaseDataVariableState)node;

            // Get signal component as VC object
            ISimComponent vcComponent = _vcUtils.GetComponent(UaBrowseName2VcComponentName[node.BrowseName.Name]);
            ISignal vcSignal = (ISignal)vcComponent.FindBehavior(node.DisplayName.ToString());

            if (uaSignal.DataType == new NodeId(DataTypeIds.String))
            {
                if ((string)uaSignal.Value == (string)vcSignal.Value)
                {
                    return;
                }
                vcSignal.Value = (string)uaSignal.Value;
            }
            else if (uaSignal.DataType == new NodeId(DataTypeIds.Boolean))
            {
                if ((bool)uaSignal.Value == (bool)vcSignal.Value)
                {
                    return;
                }
                vcSignal.Value = (bool)uaSignal.Value;
            }
            else if (uaSignal.DataType == new NodeId(DataTypeIds.Double))
            {
                if ((double)uaSignal.Value == (double)vcSignal.Value)
                {
                    return;
                }
                vcSignal.Value = (double)uaSignal.Value;
            }
            else if (uaSignal.DataType == new NodeId(DataTypeIds.Integer))
            {
                if ((int)uaSignal.Value == (int)vcSignal.Value)
                {
                    return;
                }
                vcSignal.Value = (int)uaSignal.Value;
            }
            else
            {
                _vcUtils.VcWriteWarningMsg("OPCUA signal type not supported" + uaSignal.DataType.ToString());
                return;
            }
        }

        /// <summary>
        /// Sets value of VC signal to OPCUA node
        /// </summary>
        private void vc_SignalTriggered(object sender, SignalTriggerEventArgs e)
        {
            // Get OPCUA node that will get its value updated
            string nodeNameParent = String.Format("{0}-{1}", e.Signal.Name, e.Signal.Node.Name);
            NodeId nodeId = NodeId.Create(nodeNameParent, Namespaces.vc2opcua, uaServer.NamespaceUris);
            BaseDataVariableState uaSignal = (BaseDataVariableState)nodeManager.FindPredefinedNode(nodeId, typeof(BaseDataVariableState));

            if (uaSignal == null)
            {
                // Unsubscribe to events
                e.Signal.SignalTrigger -= vc_SignalTriggered;
                return;
            }
            
            if (e.Signal.Type == BehaviorType.StringSignal)
            {
                if ((string)uaSignal.Value == (string)e.Signal.Value)
                {
                    return;
                }
                uaSignal.Value = (string)e.Signal.Value;
            }
            else if (e.Signal.Type == BehaviorType.ComponentSignal)
            {
                ISimComponent component = (ISimComponent)e.Signal.Value;

                if ((string)uaSignal.Value == component.Name)
                {
                    return;
                }
                uaSignal.Value = component.Name;
            }
            else if (e.Signal.Type == BehaviorType.BooleanSignal)
            {
                if ((bool)uaSignal.Value == (bool)e.Signal.Value)
                {
                    return;
                }
                uaSignal.Value = (bool)e.Signal.Value;
            }
            else if (e.Signal.Type == BehaviorType.RealSignal)
            {
                if ((double)uaSignal.Value == (double)e.Signal.Value)
                {
                    return;
                }
                uaSignal.Value = (double)e.Signal.Value;
            }
            else if (e.Signal.Type == BehaviorType.IntegerSignal)
            {
                if ((int)uaSignal.Value == (int)e.Signal.Value)
                {
                    return;
                }
                uaSignal.Value = (int)e.Signal.Value;
            }
            else
            {
                _vcUtils.VcWriteWarningMsg("VC signal type not supported" + e.Signal.Type.ToString());
                return;
            }

            uaSignal.Timestamp = DateTime.UtcNow;
            uaSignal.ClearChangeMasks(nodeManager.context, true);
        }

        /// <summary>
        /// Sets value of OPCUA node property to corresponding VC property
        /// </summary>
        private void ua_PropertyChanged(ISystemContext context, NodeState node, NodeStateChangeMasks changes)
        {

            if (!UaBrowseName2VcComponentName.ContainsKey(node.BrowseName.Name))
            {
                _vcUtils.VcWriteWarningMsg(String.Format("Component with property {0} not found", node.BrowseName.Name));
                return;
            }

            // Cast property OPCUA node to BaseDataVariableState type
            BaseDataVariableState uaProperty = (BaseDataVariableState)node;

            // Get property component as VC object
            ISimComponent vcComponent = _vcUtils.GetComponent(UaBrowseName2VcComponentName[node.BrowseName.Name]);
            IProperty vcProperty = vcComponent.GetProperty(node.DisplayName.ToString());

            if (uaProperty.DataType == new NodeId(DataTypeIds.String))
            {
                if ((string)uaProperty.Value == (string)vcProperty.Value)
                {
                    return;
                }
                vcProperty.Value = (string)uaProperty.Value;
            }
            else if (uaProperty.DataType == new NodeId(DataTypeIds.Boolean))
            {
                if ((bool)uaProperty.Value == (bool)vcProperty.Value)
                {
                    return;
                }
                vcProperty.Value = (bool)uaProperty.Value;
            }
            else if (uaProperty.DataType == new NodeId(DataTypeIds.Double))
            {
                if ((double)uaProperty.Value == (double)vcProperty.Value)
                {
                    return;
                }
                vcProperty.Value = (double)uaProperty.Value;
            }
            else if (uaProperty.DataType == new NodeId(DataTypeIds.Integer))
            {
                if ((int)uaProperty.Value == (int)vcProperty.Value)
                {
                    return;
                }
                vcProperty.Value = (int)uaProperty.Value;
            }
            else
            {
                _vcUtils.VcWriteWarningMsg("OPCUA property type not supported" + uaProperty.DataType.ToString());
                return;
            }
        }

        /// <summary>
        /// Sets value of VC property to OPCUA node
        /// </summary>
        private void vc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IProperty vcProperty = (IProperty)sender;

            // Get OPCUA node that will get its value updated
            string nodeNameParent = String.Format("{0}-{1}", vcProperty.Name, vcProperty.Container);
            NodeId nodeId = NodeId.Create(nodeNameParent, Namespaces.vc2opcua, uaServer.NamespaceUris);
            BaseDataVariableState uaProperty = (BaseDataVariableState)nodeManager.FindPredefinedNode(nodeId, typeof(BaseDataVariableState));

            if (uaProperty == null)
            {
                // Unsubscribe to events
                vcProperty.PropertyChanged -= vc_PropertyChanged;
                return;
            }

            if (vcProperty.PropertyType == typeof(String))
            {
                if ((string)uaProperty.Value == (string)vcProperty.Value)
                {
                    return;
                }
                uaProperty.Value = (string)vcProperty.Value;
            }
            else if (vcProperty.PropertyType == typeof(Boolean))
            {
                if ((bool)uaProperty.Value == (bool)vcProperty.Value)
                {
                    return;
                }
                uaProperty.Value = (bool)vcProperty.Value;
            }
            else if (vcProperty.PropertyType == typeof(Double))
            {
                if ((double)uaProperty.Value == (double)vcProperty.Value)
                {
                    return;
                }
                uaProperty.Value = (double)vcProperty.Value;
            }
            else if (vcProperty.PropertyType == typeof(Int32))
            {
                if ((int)uaProperty.Value == (int)vcProperty.Value)
                {
                    return;
                }
                uaProperty.Value = (int)vcProperty.Value;
            }
            else
            {
                _vcUtils.VcWriteWarningMsg("VC property type not supported" + vcProperty.PropertyType.ToString());
                return;
            }

            uaProperty.Timestamp = DateTime.UtcNow;
            uaProperty.ClearChangeMasks(nodeManager.context, true);

        }

        #endregion
    }
}
