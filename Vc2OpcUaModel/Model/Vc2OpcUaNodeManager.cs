using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Sample;
using System.Reflection;

using VisualComponents.Create3D;
using System.Collections.ObjectModel;

namespace Vc2OpcUa
{
    /// <summary>
    /// A node manager the diagnostic information exposed by the server.
    /// </summary>
    public class Vc2OpcUaNodeManager : SampleNodeManager
    {

        private Collection<ISimComponent> components;

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public Vc2OpcUaNodeManager(
            Opc.Ua.Server.IServerInternal server, 
            ApplicationConfiguration configuration,
            Collection<ISimComponent> comps
            )
        :
            base(server)
        {
            List<string> namespaceUris = new List<string>();
            namespaceUris.Add(Namespaces.Vc2OpcUa);
            namespaceUris.Add(Namespaces.Vc2OpcUa + "/Instance");
            NamespaceUris = namespaceUris;

            m_typeNamespaceIndex = Server.NamespaceUris.GetIndexOrAppend(namespaceUris[0]);
            m_namespaceIndex = Server.NamespaceUris.GetIndexOrAppend(namespaceUris[1]);

            m_lastUsedId = 0;

            components = comps;
        }
        #endregion


        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, "Vc2OpcUaServer.Model.Vc2OpcUa.PredefinedNodes.uanodes", this.GetType().GetTypeInfo().Assembly, true);
            return predefinedNodes;
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);

                foreach (ISimComponent component in components)
                {
                    CreateNode(SystemContext, component.Name);
                }
            }
        }

        private void CreateNode(SystemContext context, string nodename)
        {
            ComponentState component = new ComponentState(null);

            component.Create(
                context, 
                null, 
                new QualifiedName(nodename, m_namespaceIndex),
                null, 
                true);

            NodeState folder = (NodeState)FindPredefinedNode(
                ExpandedNodeId.ToNodeId(ObjectIds.VisualComponents_Components, Server.NamespaceUris),
                typeof(NodeState));

            folder.AddReference(ReferenceTypeIds.Organizes, false, component.NodeId);
            component.AddReference(ReferenceTypeIds.Organizes, true, folder.NodeId);

            AddPredefinedNode(context, component);
        }

        #region Private Fields
        private ushort m_namespaceIndex;
        private ushort m_typeNamespaceIndex;
        private long m_lastUsedId;
        #endregion

    }

}