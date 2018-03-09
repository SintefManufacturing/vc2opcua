using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;

using Opc.Ua;
using Opc.Ua.Sample;

namespace vc2opcua
{
    class VcManager
    {

        VcUtils _vcutils = new VcUtils();

        #region Properties

        public Collection<ISimComponent> Components { get; set; } = new Collection<ISimComponent>();

        #endregion

        #region Methods

        public ISimComponent GetComponent(string name)
        {
            foreach (ISimComponent component in Components)
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

        public void PrintComponents()
        {
            Debug.WriteLine("Components: ");
            foreach (ISimComponent component in Components)
            {
                Debug.WriteLine("  " + component.Name);
            }
        }

        #endregion

    }


    public class OpcUaNodeManager : SampleNodeManager
    {

        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public OpcUaNodeManager(
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
            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);

                CreateNode(SystemContext, "test");

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
