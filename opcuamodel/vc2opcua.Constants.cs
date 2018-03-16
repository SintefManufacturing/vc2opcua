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
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace vc2opcua
{
    #region Object Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Objects
    {
        /// <summary>
        /// The identifier for the ComponentType_Signals Object.
        /// </summary>
        public const uint ComponentType_Signals = 15005;

        /// <summary>
        /// The identifier for the VisualComponents Object.
        /// </summary>
        public const uint VisualComponents = 15001;

        /// <summary>
        /// The identifier for the VisualComponents_VcApplication Object.
        /// </summary>
        public const uint VisualComponents_VcApplication = 15002;

        /// <summary>
        /// The identifier for the VisualComponents_Components Object.
        /// </summary>
        public const uint VisualComponents_Components = 15003;
    }
    #endregion

    #region ObjectType Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <summary>
        /// The identifier for the ComponentType ObjectType.
        /// </summary>
        public const uint ComponentType = 15004;
    }
    #endregion

    #region Object Node Identifiers
    /// <summary>
    /// A class that declares constants for all Objects in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectIds
    {
        /// <summary>
        /// The identifier for the ComponentType_Signals Object.
        /// </summary>
        public static readonly ExpandedNodeId ComponentType_Signals = new ExpandedNodeId(vc2opcua.Objects.ComponentType_Signals, vc2opcua.Namespaces.vc2opcua);

        /// <summary>
        /// The identifier for the VisualComponents Object.
        /// </summary>
        public static readonly ExpandedNodeId VisualComponents = new ExpandedNodeId(vc2opcua.Objects.VisualComponents, vc2opcua.Namespaces.vc2opcua);

        /// <summary>
        /// The identifier for the VisualComponents_VcApplication Object.
        /// </summary>
        public static readonly ExpandedNodeId VisualComponents_VcApplication = new ExpandedNodeId(vc2opcua.Objects.VisualComponents_VcApplication, vc2opcua.Namespaces.vc2opcua);

        /// <summary>
        /// The identifier for the VisualComponents_Components Object.
        /// </summary>
        public static readonly ExpandedNodeId VisualComponents_Components = new ExpandedNodeId(vc2opcua.Objects.VisualComponents_Components, vc2opcua.Namespaces.vc2opcua);
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <summary>
        /// The identifier for the ComponentType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId ComponentType = new ExpandedNodeId(vc2opcua.ObjectTypes.ComponentType, vc2opcua.Namespaces.vc2opcua);
    }
    #endregion

    #region BrowseName Declarations
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class BrowseNames
    {
        /// <summary>
        /// The BrowseName for the Components component.
        /// </summary>
        public const string Components = "Components";

        /// <summary>
        /// The BrowseName for the ComponentType component.
        /// </summary>
        public const string ComponentType = "ComponentType";

        /// <summary>
        /// The BrowseName for the Signals component.
        /// </summary>
        public const string Signals = "Signals";

        /// <summary>
        /// The BrowseName for the VcApplication component.
        /// </summary>
        public const string VcApplication = "VC Application";

        /// <summary>
        /// The BrowseName for the VisualComponents component.
        /// </summary>
        public const string VisualComponents = "VisualComponents";
    }
    #endregion

    #region Namespace Declarations
    /// <summary>
    /// Defines constants for all namespaces referenced by the model design.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Namespaces
    {
        /// <summary>
        /// The URI for the OpcUa namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUa = "http://opcfoundation.org/UA/";

        /// <summary>
        /// The URI for the OpcUaXsd namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUaXsd = "http://opcfoundation.org/UA/2008/02/Types.xsd";

        /// <summary>
        /// The URI for the vc2opcua namespace (.NET code namespace is 'vc2opcua').
        /// </summary>
        public const string vc2opcua = "vc2opcua:namespace";
    }
    #endregion
}