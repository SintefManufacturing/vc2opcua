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
            ReadOnlyCollection<ISimComponent> components = _vcutils.GetComponents();

            NodeState baseFolder = (NodeState)FindPredefinedNode(
                ExpandedNodeId.ToNodeId(ObjectIds.VisualComponents_Components, Server.NamespaceUris),
                typeof(NodeState));

            foreach (ISimComponent component in components)
            {
                VcComponent vcComp = new VcComponent(component);
                var vcSignals = vcComp.GetStringSignals();

                ComponentState componentNode = CreateNode(context, baseFolder, namespaceUri, component.Name);

                foreach (IBehavior signal in vcSignals)
                {
                    PropertyState<string> uaSignal = CreateNode(context, componentNode, namespaceUri, signal.Name);
                    componentNode.AddChild(uaSignal);
                }
                AddPredefinedNode(context, componentNode);
            }
        }

        /// <summary>
        /// Creates node for Component in Visual Components.
        /// </summary>
        private ComponentState CreateNode(SystemContext context, NodeState baseNode, string namespaceUri, string nodeName)
        {
            ComponentState uaComponent = new ComponentState(null);
            NodeId nodeId = NodeId.Create(nodeName, namespaceUri, context.NamespaceUris);

            uaComponent.Create(
                context,
                nodeId,
                new QualifiedName(nodeName, m_namespaceIndex),
                null,
                true);

            baseNode.AddReference(ReferenceTypeIds.Organizes, false, uaComponent.NodeId);
            uaComponent.AddReference(ReferenceTypeIds.Organizes, true, baseNode.NodeId);

            return uaComponent;
        }

        /// <summary>
        /// Creates node for Signal associated to Component in Visual Components.
        /// </summary>
        private PropertyState<string> CreateNode(SystemContext context, ComponentState baseNode, string namespaceUri, string nodeName)
        {
            PropertyState<string> uaSignal = new PropertyState<string>(null);
            NodeId nodeId = NodeId.Create(nodeName, namespaceUri, context.NamespaceUris);

            uaSignal.Create(
                context,
                nodeId,
                new QualifiedName(nodeName, m_namespaceIndex),
                null,
                true);

            baseNode.AddReference(ReferenceTypeIds.Organizes, false, uaSignal.NodeId);
            uaSignal.AddReference(ReferenceTypeIds.Organizes, true, baseNode.NodeId);

            return uaSignal;
        }
        #endregion

        #region Private Fields
        private ushort m_namespaceIndex;
        private ushort m_typeNamespaceIndex;
        #endregion
    }
}
