using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Sample;
using System.Reflection;

namespace Vc2OpcUa
{
    /// <summary>
    /// A node manager the diagnostic information exposed by the server.
    /// </summary>
    public class Vc2OpcUaNodeManager : SampleNodeManager
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public Vc2OpcUaNodeManager(
            Opc.Ua.Server.IServerInternal server, 
            ApplicationConfiguration configuration)
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

        #region Private Fields
        private ushort m_namespaceIndex;
        private ushort m_typeNamespaceIndex;
        private long m_lastUsedId;
        #endregion

    }

}