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

        public Vc2OpcUaBridge(IServerInternal uaServer,
            ApplicationConfiguration configuration)
        {
            nodeManager = new UaNodeManager(uaServer, configuration);

            // Subscribe to added/removing component events
            //_application.World.ComponentAdded += World_ComponentAdded;
            //_application.World.ComponentRemoving += World_ComponentRemoving;
        }

        #endregion

        #region Properties

        //TODO: create event when properties are changed, and listen from nodemanager

        // Contains all present components, and their corresponding ISimComponent object
        public Dictionary<string, ISimComponent> Components { get; set; } = new Dictionary<string, ISimComponent>();
        // Correlates signal names to corresponding component names
        public Dictionary<string, string> SignalComponents { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Methods

        /// <summary>
        /// Creates node for Component in Visual Components.
        /// </summary>
        public ComponentState CreateComponentNode(string nodeName)
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

        private void World_ComponentAdded(object sender, ComponentAddedEventArgs e)
        {
            string namespaceUri = Namespaces.vc2opcua;

            _vcUtils.VcWriteWarningMsg("Component added: " + e.Component.Name);
            // Add component to Components property
            Components.Add(e.Component.Name, e.Component);

            // Add component node
            ComponentState componentNode = CreateComponentNode(e.Component.Name);

            nodeManager.AddNode(componentNode);

            // Add signals to SignalComponents property
            VcComponent vcComponent = new VcComponent(e.Component);
            foreach (ISignal vcSignal in vcComponent.GetComponentSignals())
            {
                SignalComponents.Add(vcSignal.Name, e.Component.Name);

                // Add signal node
                BaseDataVariableState signalNode = CreateVariableNode(componentNode.Signals, vcSignal.Name);

                nodeManager.AddNode(signalNode);

                SetSignals(signalNode, vcSignal);
            }
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
            BaseDataVariableState<string> uaSignal = (BaseDataVariableState<string>)nodeManager.FindPredefinedNode(nodeId, typeof(BaseDataVariableState<string>));

            if (uaSignal != null)
            {
                if (uaSignal.Value != (string)e.Signal.Value)
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
