/* ========================================================================
 * Copyright (c) 2005-2016 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace vc2opcua
{
    #region ComponentState Class
    #if (!OPCUA_EXCLUDE_ComponentState)
    /// <summary>
    /// Stores an instance of the ComponentType ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ComponentState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public ComponentState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(vc2opcua.ObjectTypes.ComponentType, vc2opcua.Namespaces.vc2opcua, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        /// <summary>
        /// Initializes the instance with a node.
        /// </summary>
        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAABIAAAB2YzJvcGN1YTpuYW1lc3BhY2X/////hGCAAAEAAAABABUAAABDb21wb25lbnRUeXBlSW5z" +
           "dGFuY2UBAZw6AQGcOgH/////AgAAAIRggAoBAAAAAQAHAAAAU2lnbmFscwEBnToALwA9nToAAAH/////" +
           "AAAAAIRggAoBAAAAAQAKAAAAUHJvcGVydGllcwEBnjoALwA9njoAAAH/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Signals Object.
        /// </summary>
        public FolderState Signals
        {
            get
            {
                return m_signals;
            }

            set
            {
                if (!Object.ReferenceEquals(m_signals, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_signals = value;
            }
        }

        /// <summary>
        /// A description for the Properties Object.
        /// </summary>
        public FolderState Properties
        {
            get
            {
                return m_properties;
            }

            set
            {
                if (!Object.ReferenceEquals(m_properties, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_properties = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_signals != null)
            {
                children.Add(m_signals);
            }

            if (m_properties != null)
            {
                children.Add(m_properties);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case vc2opcua.BrowseNames.Signals:
                {
                    if (createOrReplace)
                    {
                        if (Signals == null)
                        {
                            if (replacement == null)
                            {
                                Signals = new FolderState(this);
                            }
                            else
                            {
                                Signals = (FolderState)replacement;
                            }
                        }
                    }

                    instance = Signals;
                    break;
                }

                case vc2opcua.BrowseNames.Properties:
                {
                    if (createOrReplace)
                    {
                        if (Properties == null)
                        {
                            if (replacement == null)
                            {
                                Properties = new FolderState(this);
                            }
                            else
                            {
                                Properties = (FolderState)replacement;
                            }
                        }
                    }

                    instance = Properties;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private FolderState m_signals;
        private FolderState m_properties;
        #endregion
    }
    #endif
    #endregion
}