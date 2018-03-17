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

        // Contains all present components, and their corresponding ISimComponent object
        public Dictionary<string, ISimComponent> Components { get; set; } = new Dictionary<string, ISimComponent>();
        // Correlates signal names to corresponding component names
        public Dictionary<string, string> SignalComponents { get; set; } = new Dictionary<string, string>();

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
                initialNodes.Add(CreateCompleteNode(vcComponent));
            }

            return initialNodes;
        }

        /// <summary>
        /// Returns a complete node with a compoenent and all its signals
        /// </summary>
        private ComponentState CreateCompleteNode(ISimComponent simComponent)
        {
            // Add component to Components property
            Components.Add(simComponent.Name, simComponent);

            ComponentState componentNode = CreateComponentNode(simComponent.Name);

            // Add signals to SignalComponents property
            VcComponent vcComponent = new VcComponent(simComponent);

            foreach (ISignal vcSignal in vcComponent.GetComponentSignals())
            {
                SignalComponents.Add(vcSignal.Name, simComponent.Name);

                // Add signal node
                BaseDataVariableState signalNode = CreateVariableNode(componentNode.Signals, vcSignal.Name);

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

            componentNode.Create(
                nodeManager.context,
                componentNodeId,
                new QualifiedName(nodeName, (ushort)uaServer.NamespaceUris.GetIndex(namespaceUri)),
                null,
                true);

            componentNode.Signals.NodeId = signalsNodeId;

            componentNode.AddChild(componentNode.Signals);

            nodeManager.baseFolder.AddReference(ReferenceTypeIds.Organizes, false, componentNode.NodeId);
            componentNode.AddReference(ReferenceTypeIds.Organizes, true, nodeManager.baseFolder.NodeId);

            return componentNode;
        }

        /// <summary>
        /// Creates node for Signal associated to Component in Visual Components.
        /// </summary>
        private BaseDataVariableState CreateVariableNode(FolderState parentFolder, string nodeName)
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

            parentFolder.AddReference(ReferenceTypeIds.Organizes, false, variableNode.NodeId);
            variableNode.AddReference(ReferenceTypeIds.Organizes, true, parentFolder.NodeId);

            return variableNode;
        }

        /// <summary>
        /// Set signal values and mutually subscribe to signal changes
        /// </summary>
        private void SetSignals(BaseDataVariableState uaSignal, ISignal vcSignal)
        {
            // Start setting value of VC signal to OPCUA signal
            uaSignal.Value = (string)vcSignal.Value;

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
            _vcUtils.VcWriteWarningMsg("Component added: " + e.Component.Name);

            // Add component node
            ComponentState componentNode = CreateCompleteNode(e.Component);

            nodeManager.AddNode(componentNode);
        }

        private void World_ComponentRemoving(object sender, ComponentRemovingEventArgs e)
        {
            _vcUtils.VcWriteWarningMsg("Component removed: " + e.Component.Name);
            // Remove component from components property
            Components.Remove(e.Component.Name);

            // Remove signals from SignalComponents property
            VcComponent vcComponent = new VcComponent(e.Component);
            foreach (ISignal signal in vcComponent.GetComponentSignals())
            {
                SignalComponents.Remove(signal.Name);
            }

            //nodeManager.RemoveNode(node);
        }

        /// <summary>
        /// Sets value of OPCUA node signal to corresponding VC signal
        /// </summary>
        private void ua_SignalTriggered(ISystemContext context, NodeState node, NodeStateChangeMasks changes)
        {

            if (SignalComponents.ContainsKey(node.BrowseName.Name))
            {
                ISimComponent vcComponent = _vcUtils.GetComponent(SignalComponents[node.BrowseName.Name]);

                if (vcComponent != null)
                {
                    IStringSignal vcSignal = (IStringSignal)vcComponent.FindBehavior(node.BrowseName.Name);
                    BaseDataVariableState uaSignal = (BaseDataVariableState)node;

                    if ((string)vcSignal.Value != (string)uaSignal.Value)
                    {
                        vcSignal.Value = uaSignal.Value;
                    }
                }
            }
            else
            {
                _vcUtils.VcWriteWarningMsg(String.Format("Component with signal {0} not found", node.BrowseName.Name));
            }
        }

        /// <summary>
        /// Sets value of VC signal to OPCUA node
        /// </summary>
        private void vc_SignalTriggered(object sender, SignalTriggerEventArgs e)
        {
            NodeId nodeId = NodeId.Create(e.Signal.Name, Namespaces.vc2opcua, uaServer.NamespaceUris);
            BaseDataVariableState uaSignal = (BaseDataVariableState)nodeManager.FindPredefinedNode(nodeId, typeof(BaseDataVariableState));

            if (uaSignal != null)
            {
                if ((string)uaSignal.Value != (string)e.Signal.Value)
                {
                    uaSignal.Value = (string)e.Signal.Value;
                    uaSignal.Timestamp = DateTime.UtcNow;
                    uaSignal.ClearChangeMasks(nodeManager.context, true);
                }
            }
            else
            {
                // Unsubscribe to events
                e.Signal.SignalTrigger -= vc_SignalTriggered;
            }
        }

        #endregion
    }
}
