using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;
using log4net.Appender;

namespace vc2opcua
{
    class VcManager
    {
        // Accesses the application itself, initialize to null for avoiding compiling errors
        [Import]
        IApplication _application = null;

        ReadOnlyCollection<ISimComponent> _components;

        IMessageService _ms = IoC.Get<IMessageService>();
        log4net.ILog logger;

        public void GetComponentProperties()
        {
            _application = IoC.Get<IApplication>();
            _components = _application.World.Components;

            foreach (ISimComponent comp in _components)
            {
                Debug.WriteLine("Component: " + comp.Name);
                Debug.WriteLine("  Properties: ");
                foreach (IProperty property in comp.Properties)
                {
                    Debug.WriteLine("    " + property.Name);
                }
            }
        }

        public ISimComponent GetComponent(string name)
        {
            _application = IoC.Get<IApplication>();
            _components = _application.World.Components;

            foreach (ISimComponent component in _components)
            {
                if (component.Name == name)
                {
                    return component;
                }
            }

            // If we go through all the components and we do not find any match
            string message = String.Format("Component {0} not found", name);
            VcWriteWarningMsg(message);

            return null;
        }

        public IProperty GetProperty(ISimComponent component, string name)
        {

            foreach (IProperty property in component.Properties)
            {
                if (property.Name == name)
                {
                    return property;
                }
            }

            // If we go through all the properties and we do not find any match
            string message = String.Format("Property {0} not found", name);
            VcWriteWarningMsg(message);

            return null;
        }

        public void SetPropertyValueDouble(IProperty property, double value)
        {
            property.Value = value;
        }

        public void VcWriteWarningMsg(string message)
        {
            _ms.AppendMessage("[vc2opcua] " + message, MessageLevel.Warning);
        }
    }
}
