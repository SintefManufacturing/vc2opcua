using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using Caliburn.Micro;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;

namespace vc2opcua
{

    [Export(typeof(IPlugin))]
    public class Vc2OpcUaPlugin : IPlugin
    {
        VcUtils _vcutils = new VcUtils();

        void IPlugin.Exit()
        {

        }

        void IPlugin.Initialize()
        {
            string message = "Plugin loaded";
            _vcutils.VcWriteWarningMsg(message);
        }
    }

    public class VcUtils
    {
        [Import]
        IApplication _application = null;

        IMessageService _ms = IoC.Get<IMessageService>();

        public VcUtils()
        {
            
        }

        /// <summary>
        /// Returns all components in current VC simulation
        /// </summary>
        public ReadOnlyCollection<ISimComponent>  GetComponents()
        {
            _application = IoC.Get<IApplication>();
            ReadOnlyCollection<ISimComponent> components = _application.World.Components;

            return components;
        }

        /// <summary>
        /// Returns a specific component if present on current VC simulation
        /// </summary>
        public ISimComponent GetComponent(string componentName)
        {
            var components = GetComponents();

            foreach (var component in components)
            {
                if (component.Name == componentName)
                {
                    return component;
                }
            }

            return null;
        }

        public void VcWriteWarningMsg(string message)
        {
            _ms.AppendMessage("[vc2opcua] " + message, MessageLevel.Warning);
        }

        public void VcWriteErrorMsg(string message)
        {
            _ms.AppendMessage("[vc2opcua] " + message, MessageLevel.Error);
        }
    }
}
