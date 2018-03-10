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

            m_lastUsedId = 0;
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, "vc2opcua.opcuamodel.vc2opcua.PredefinedNodes.uanodes", this.GetType().GetTypeInfo().Assembly, true);
            return predefinedNodes;
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            ReadOnlyCollection<ISimComponent> components = _vcutils.GetComponents();

            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);

                foreach (ISimComponent component in components)
                {
                    CreateNode(SystemContext, component.Name);
                }

            }
        }
        #endregion

        #region Methods

        private void CreateNode(SystemContext context, string nodename)
        {
            ComponentState component = new ComponentState(null);

            string namespaceuri = "vc2opcua:namespace";

            NodeId nodeid = NodeId.Create(nodename, namespaceuri, context.NamespaceUris);

            component.Create(
                context,
                nodeid,
                new QualifiedName(nodename, m_namespaceIndex),
                null,
                true);

            NodeState folder = (NodeState)FindPredefinedNode(
                ExpandedNodeId.ToNodeId(ObjectIds.VisualComponents_Components, Server.NamespaceUris),
                typeof(NodeState));

            folder.AddReference(ReferenceTypeIds.Organizes, false, component.NodeId);
            component.AddReference(ReferenceTypeIds.Organizes, true, folder.NodeId);

            AddPredefinedNode(context, component);

            Debug.WriteLine(SystemContext.StringTable);
        }

        public ISimComponent GetComponent(Collection<ISimComponent> components, string name)
        {
            foreach (ISimComponent component in components)
            {
                if (component.Name == name)
                {
                    return component;
                }
            }

            // If we go through all the components and we do not find any match
            string message = String.Format("Component {0} not found", name);
            _vcutils.VcWriteWarningMsg(message);

            return null;
        }

        public void PrintComponents(Collection<ISimComponent> components)
        {
            Debug.WriteLine("Components: ");
            foreach (ISimComponent component in components)
            {
                Debug.WriteLine("  " + component.Name);
            }
        }

        #endregion

        #region Private Fields
        private ushort m_namespaceIndex;
        private ushort m_typeNamespaceIndex;
        private long m_lastUsedId;
        #endregion
    }
}
