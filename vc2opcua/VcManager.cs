using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Sample;
using Opc.Ua.Server;
using VisualComponents.Create3D;

namespace vc2opcua
{
    class VcManager : SampleNodeManager
    {
        VcUtils _vcutils = new VcUtils();

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public VcManager(
            Opc.Ua.Server.IServerInternal server,
            ApplicationConfiguration configuration
            )
        :
            base(server)
        {
            List<string> namespaceUris = new List<string>();
            namespaceUris.Add(Namespaces.vc2opcua);
            namespaceUris.Add(Namespaces.vc2opcua + "/Instance");
            NamespaceUris = namespaceUris;

            m_typeNamespaceIndex = Server.NamespaceUris.GetIndexOrAppend(namespaceUris[0]);
            m_namespaceIndex = Server.NamespaceUris.GetIndexOrAppend(namespaceUris[1]);
        }
        #endregion

        #region Properties

        // Dictionary that relates active signal names to their corresponding components
        public Dictionary<string, string> ActiveSignals { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Overrides

        /// <summary>
        /// Loads a node set from a file or resource and adds them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, "vc2opcua.opcuamodel.vc2opcua.PredefinedNodes.uanodes", this.GetType().GetTypeInfo().Assembly, true);
            return predefinedNodes;
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);
                CreateComponentNodes();
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates OPCUA nodes based on current components active in Visual Components.
        /// </summary>
        private void CreateComponentNodes()
        {
            string namespaceUri = Namespaces.vc2opcua;
            SystemContext context = SystemContext;
            ReadOnlyCollection<ISimComponent> vcComponents = _vcutils.GetComponents();

            NodeState baseFolder = (NodeState)FindPredefinedNode(
                ExpandedNodeId.ToNodeId(ObjectIds.VisualComponents_Components, Server.NamespaceUris),
                typeof(NodeState));

            foreach (ISimComponent vcComponent in vcComponents)
            {
                VcComponent vcComp = new VcComponent(vcComponent);
                var vcSignals = vcComp.GetStringSignals();

                ComponentState componentNode = CreateComponentNode(context, baseFolder, namespaceUri, vcComponent.Name);

                componentNode.EventNotifier = EventNotifiers.SubscribeToEvents;

                foreach (IStringSignal vcSignal in vcSignals)
                {
                    BaseDataVariableState<string> uaSignal = CreateVariableNode(context, componentNode.Signals, namespaceUri, vcSignal.Name);

                    componentNode.Signals.AddChild(uaSignal);

                    ActiveSignals.Add(vcSignal.Name, vcComponent.Name);

                    SetSignals(uaSignal, vcSignal);
                }
                AddPredefinedNode(context, componentNode);
            }
        }

        /// <summary>
        /// Set signal values and mutually subscribe to signal changes
        /// </summary>
        private void SetSignals(BaseDataVariableState<string> uaSignal, IStringSignal vcSignal)
        {
            // Start setting value of VC signal to OPCUA signal
            uaSignal.Value = (string)vcSignal.Value;

            // Subscribe to signal triggered events
            vcSignal.SignalTrigger += vc_SignalTriggered;
            uaSignal.StateChanged += ua_SignalTriggered;
        }

        /// <summary>
        /// Sets value of OPCUA node signal to corresponding VC signal
        /// </summary>
        private void ua_SignalTriggered(ISystemContext context, NodeState node, NodeStateChangeMasks changes)
        {

            if (ActiveSignals.ContainsKey(node.BrowseName.Name))
            {
                ISimComponent vcComponent = _vcutils.GetComponent(ActiveSignals[node.BrowseName.Name]);

                if (vcComponent != null)
                {
                    IStringSignal vcSignal = (IStringSignal)vcComponent.FindBehavior(node.BrowseName.Name);
                    BaseDataVariableState<string> uaSignal = (BaseDataVariableState<string>)node;

                    if ((string)vcSignal.Value != uaSignal.Value)
                        vcSignal.Value = uaSignal.Value;
                }
            }
            else
            {
                _vcutils.VcWriteWarningMsg(String.Format("Component with signal {0} not found", node.BrowseName.Name));
            }
        }

        /// <summary>
        /// Sets value of VC signal to OPCUA node
        /// </summary>
        private void vc_SignalTriggered(object sender, SignalTriggerEventArgs e)
        {
            NodeId nodeId = NodeId.Create(e.Signal.Name, Namespaces.vc2opcua, Server.NamespaceUris);
            BaseDataVariableState<string> uaSignal = (BaseDataVariableState<string>)FindPredefinedNode(nodeId, typeof(BaseDataVariableState<string>));

            if (uaSignal != null)
            {
                if (uaSignal.Value != (string)e.Signal.Value)
                {
                    uaSignal.Value = (string)e.Signal.Value;
                    uaSignal.Timestamp = DateTime.UtcNow;
                    uaSignal.ClearChangeMasks(SystemContext, true);
                }
            }
            else
            {
                // Unsubscribe to events
                e.Signal.SignalTrigger -= vc_SignalTriggered;
            }
        }

        /// <summary>
        /// Creates node for Component in Visual Components.
        /// </summary>
        private ComponentState CreateComponentNode(SystemContext context, NodeState baseFolder, string namespaceUri, string nodeName)
        {
            ComponentState uaComponent = new ComponentState(null);
            NodeId componentNodeId = NodeId.Create(nodeName, namespaceUri, context.NamespaceUris);
            NodeId signalsNodeId = NodeId.Create("Signals_" + nodeName, namespaceUri, context.NamespaceUris);

            uaComponent.Create(
                context,
                componentNodeId,
                new QualifiedName(nodeName, m_namespaceIndex),
                null,
                true);

            uaComponent.Signals.NodeId = signalsNodeId;

            uaComponent.AddChild(uaComponent.Signals);

            baseFolder.AddReference(ReferenceTypeIds.Organizes, false, uaComponent.NodeId);
            uaComponent.AddReference(ReferenceTypeIds.Organizes, true, baseFolder.NodeId);

            return uaComponent;
        }
        
        /// <summary>
        /// Creates node for Signal associated to Component in Visual Components.
        /// </summary>
        private BaseDataVariableState<string> CreateVariableNode(SystemContext context, FolderState baseFolder, string namespaceUri, string nodeName)
        {
            BaseDataVariableState<string> uaSignal = new BaseDataVariableState<string>(baseFolder);
            NodeId nodeId = NodeId.Create(nodeName, namespaceUri, context.NamespaceUris);

            uaSignal.Create(
                context,
                nodeId,
                new QualifiedName(nodeName, m_namespaceIndex),
                null,
                true);

            baseFolder.AddReference(ReferenceTypeIds.Organizes, false, uaSignal.NodeId);
            uaSignal.AddReference(ReferenceTypeIds.Organizes, true, baseFolder.NodeId);

            return uaSignal;
        }

        #endregion

        #region Private Fields
        private ushort m_namespaceIndex;
        private ushort m_typeNamespaceIndex;
        #endregion
    }
}
