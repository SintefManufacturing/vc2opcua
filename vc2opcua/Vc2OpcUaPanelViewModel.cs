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
        private string _host = "0.0.0.0";
        private string _port = "4840";

        public Vc2OpcUaPanelViewModel()
        {
            this.DisplayName = "VC2OPCUA Panel";
            this.IsPinned = true;
            this.PaneLocation = DesiredPaneLocation.DockedRight;
        }

        public void Start()
        {
            // Automatically related to element in *View.xaml having x:Name = this method's name

            string message = String.Format("Starting OPCUA server: \n  Host: {0}\n  Port: {1}",
                                           Host, Port);

            _vcmanager.VcWriteWarningMsg(message);

            _vcmanager.GetComponentProperties();
        }
        public void Stop()
        {
            string message = "Stop Button Clicked";
            _vcmanager.VcWriteWarningMsg(message);
        }

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

    }
}
