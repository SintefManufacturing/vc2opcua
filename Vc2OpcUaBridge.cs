/*
 * Bridge between Visual Components and OPCUA  
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // Correlates signal names to corresponding component names
        public Dictionary<string, string> SignalComponents { get; set; } = new Dictionary<string, string>();
        // Correlates types between VisualComponents and OPCUA types
        Dictionary<BehaviorType, NodeId> Vc2OpcuaTypeCorrelations { get; } = new Dictionary<BehaviorType, NodeId>
        {
            { BehaviorType.StringSignal, new NodeId(DataTypeIds.String) },
            { BehaviorType.BooleanSignal, new NodeId(DataTypeIds.Boolean) },
            { BehaviorType.RealSignal, new NodeId(DataTypeIds.Double) },
            { BehaviorType.IntegerSignal, new NodeId(DataTypeIds.Integer) },
            { BehaviorType.ComponentSignal, new NodeId(DataTypeIds.String) }
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

            // Add signals to SignalComponents property
            VcComponent vcComponent = new VcComponent(simComponent);

            foreach (ISignal vcSignal in vcComponent.GetSignals())
            {
                SignalComponents.Add(vcSignal.Name, simComponent.Name);

                // Add signal node
                BaseDataVariableState signalNode = CreateVariableNode(componentNode.Signals, vcSignal.Name, vcSignal.Type);

                componentNode.Signals.AddChild(signalNode);

                SetSignals(signalNode, vcSignal);
            }

            return componentNode;
        }

        /// <summary>
        /// Creates node for Component in Visual Components.
        /// </summary>
        private ComponentState CreateComponentNode(string nodeName)
        {
            string namespaceUri = Namespaces.vc2opcua;

            ComponentState componentNode = new ComponentState(null);
            NodeId componentNodeId = NodeId.Create(nodeName, namespaceUri, uaServer.NamespaceUris);
            NodeId signalsNodeId = NodeId.Create("Signals_" + nodeName, namespaceUri, uaServer.NamespaceUris);
            NodeId propertiesNodeId = NodeId.Create("Properties_" + nodeName, namespaceUri, uaServer.NamespaceUris);

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
        private BaseDataVariableState CreateVariableNode(FolderState parentFolder, string nodeName, BehaviorType signalType)
        {
            string namespaceUri = Namespaces.vc2opcua;

            BaseDataVariableState variableNode = new BaseDataVariableState(parentFolder);
            NodeId nodeId = NodeId.Create(nodeName, namespaceUri, uaServer.NamespaceUris);

            variableNode.Create(
                nodeManager.context,
                nodeId,
                new QualifiedName(nodeName, (ushort)uaServer.NamespaceUris.GetIndex(namespaceUri)),
                null,
                true);

            variableNode.DataType = Vc2OpcuaTypeCorrelations[signalType];

            parentFolder.AddReference(ReferenceTypeIds.Organizes, false, variableNode.NodeId);
            variableNode.AddReference(ReferenceTypeIds.Organizes, true, parentFolder.NodeId);

            return variableNode;
        }

        /// <summary>
        /// Set signal values and mutually subscribe to signal changes
        /// </summary>
        private void SetSignals(BaseDataVariableState uaSignal, ISignal vcSignal)
        {
            // Subscribe to signal triggered events
            vcSignal.SignalTrigger += vc_SignalTriggered;
            uaSignal.StateChanged += ua_SignalTriggered;
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
                SignalComponents.Remove(signal.Name);
            }

            _vcUtils.VcWriteWarningMsg("Component removed: " + e.Component.Name);
        }

        /// <summary>
        /// Sets value of OPCUA node signal to corresponding VC signal
        /// </summary>
        private void ua_SignalTriggered(ISystemContext context, NodeState node, NodeStateChangeMasks changes)
        {

            if (!SignalComponents.ContainsKey(node.BrowseName.Name))
            {
                _vcUtils.VcWriteWarningMsg(String.Format("Component with signal {0} not found", node.BrowseName.Name));
                return;
            }

            // Cast signal OPCUA node to BaseDataVariableState type
            BaseDataVariableState uaSignal = (BaseDataVariableState)node;

            // Get signal component as VC object
            ISimComponent vcComponent = _vcUtils.GetComponent(SignalComponents[node.BrowseName.Name]);
            ISignal vcSignal = (ISignal)vcComponent.FindBehavior(node.BrowseName.Name);

            if (uaSignal.DataType == new NodeId(DataTypeIds.String))
            {
                if ((string)uaSignal.Value == (string)vcSignal.Value)
                {
                    return;
                }
                else
                {
                    vcSignal.Value = (string)uaSignal.Value;
                }
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
            NodeId nodeId = NodeId.Create(e.Signal.Name, Namespaces.vc2opcua, uaServer.NamespaceUris);
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

        #endregion
    }
}
