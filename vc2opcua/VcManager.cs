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
}
