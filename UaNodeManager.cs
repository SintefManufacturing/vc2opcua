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

namespace vc2opcua
{
    public class UaNodeManager : SampleNodeManager
    {
        public NodeState baseFolder;
        public SystemContext context;

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public UaNodeManager(
            Opc.Ua.Server.IServerInternal server,
            ApplicationConfiguration configuration
            )
        :
            base(server)
        {
            List<string> namespaceUris = new List<string>
            {
                Namespaces.vc2opcua,
                Namespaces.vc2opcua + "/Instance"
            };

            NamespaceUris = namespaceUris;
            context = SystemContext;

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
                CreateBaseFolder();

                // Raise event address space created
                OnAddressSpaceCreated();
            }
        }

        public delegate void AddressSpaceCreatedEventHandler(object source, EventArgs args);
        public event AddressSpaceCreatedEventHandler AddressSpaceCreated;
        protected virtual void OnAddressSpaceCreated()
        {
            if (AddressSpaceCreated != null)
            {
                AddressSpaceCreated(this, EventArgs.Empty);
            }
        }

        private void CreateBaseFolder()
        {
            // Base folder under which all components will be created
            baseFolder = (NodeState)FindPredefinedNode(
                ExpandedNodeId.ToNodeId(ObjectIds.VisualComponents_Components, Server.NamespaceUris),
                typeof(NodeState));

            AddNode(baseFolder);
        }

        /// <summary>
        /// AddPredefindedNode is private, we access it through this method
        /// </summary>
        public void AddNode(NodeState node) => AddPredefinedNode(SystemContext, node);

        public void RemoveNode(NodeState node)
        {
            //TODO
            //RemovePredefinedNode(SystemContext, node, references);
        }
        #endregion

        #region Private Fields
        private ushort m_namespaceIndex;
        private ushort m_typeNamespaceIndex;
        #endregion
    }
}
