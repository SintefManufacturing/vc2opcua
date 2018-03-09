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

namespace Vc2OpcUa
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

    #region Variable Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <summary>
        /// The identifier for the ComponentType_Signal Variable.
        /// </summary>
        public const uint ComponentType_Signal = 15005;
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
        /// The identifier for the VisualComponents Object.
        /// </summary>
        public static readonly ExpandedNodeId VisualComponents = new ExpandedNodeId(Vc2OpcUa.Objects.VisualComponents, Vc2OpcUa.Namespaces.Vc2OpcUa);

        /// <summary>
        /// The identifier for the VisualComponents_VcApplication Object.
        /// </summary>
        public static readonly ExpandedNodeId VisualComponents_VcApplication = new ExpandedNodeId(Vc2OpcUa.Objects.VisualComponents_VcApplication, Vc2OpcUa.Namespaces.Vc2OpcUa);

        /// <summary>
        /// The identifier for the VisualComponents_Components Object.
        /// </summary>
        public static readonly ExpandedNodeId VisualComponents_Components = new ExpandedNodeId(Vc2OpcUa.Objects.VisualComponents_Components, Vc2OpcUa.Namespaces.Vc2OpcUa);
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
        public static readonly ExpandedNodeId ComponentType = new ExpandedNodeId(Vc2OpcUa.ObjectTypes.ComponentType, Vc2OpcUa.Namespaces.Vc2OpcUa);
    }
    #endregion

    #region Variable Node Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <summary>
        /// The identifier for the ComponentType_Signal Variable.
        /// </summary>
        public static readonly ExpandedNodeId ComponentType_Signal = new ExpandedNodeId(Vc2OpcUa.Variables.ComponentType_Signal, Vc2OpcUa.Namespaces.Vc2OpcUa);
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
        /// The BrowseName for the Signal component.
        /// </summary>
        public const string Signal = "Signal";

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
        /// The URI for the Vc2OpcUa namespace (.NET code namespace is 'Vc2OpcUa').
        /// </summary>
        public const string Vc2OpcUa = "vc2opcua:namespace";
    }
    #endregion
}