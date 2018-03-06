using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.ComponentModel.Composition;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;

namespace vc2opcua
{
    [Export(typeof(IDockableScreen))]
    class Vc2OpcUaPanelViewModel : DockableScreen
    {
        VcManager _vcmanager = new VcManager();

        public Vc2OpcUaPanelViewModel()
        {
            this.DisplayName = "VC2OPCUA Panel";
            this.IsPinned = true;
            this.PaneLocation = DesiredPaneLocation.DockedRight;
        }

        public void Start()
        {
            // Automatically related to element in *View.xaml having x:Name = this method's name
            string message = "Start Button Clicked";
            _vcmanager.VcWriteWarningMsg(message);

            _vcmanager.GetComponentProperties();
        }
        public void Stop()
        {
             string message = "Stop Button Clicked";
             _vcmanager.VcWriteWarningMsg(message);
        }
    }
}
