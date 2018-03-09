using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Diagnostics;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;

namespace vc2opcua
{
    [Export(typeof(IDockableScreen))]
    class Vc2OpcUaPanelViewModel : DockableScreen
    {
        // Accesses the application itself, initialize to null for avoiding compiling errors
        [Import]
        IApplication _application = null;

        [Import]
        IRenderService _renderService = null;

        VcUtils _vcutils = new VcUtils();
        VcManager _vcmanager = new VcManager();

        Server _server;
        Thread _serverthread;

        public Vc2OpcUaPanelViewModel()
        {
            this.DisplayName = "VC2OPCUA Panel";
            this.IsPinned = true;
            this.PaneLocation = DesiredPaneLocation.DockedRight;
        }

        #region Properties

        public string Text1 { get; set; } = "";
        public string Text2 { get; set; } = "";
        public string Text3 { get; set; } = "";

        #endregion

        #region Methods

        // Add methods for listener to execute when component is added or removed
        protected override void OnActivate()
        {
            _application.World.ComponentAdded += World_ComponentAdded;
            _application.World.ComponentRemoving += World_ComponentRemoving;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _application.World.ComponentAdded -= World_ComponentAdded;
            _application.World.ComponentRemoving -= World_ComponentRemoving;
        }

        private void World_ComponentAdded(object sender, ComponentAddedEventArgs e)
        {
            _vcutils.VcWriteWarningMsg("Component added: " + e.Component.Name);
            _vcmanager.Components.Add(e.Component);
        }

        private void World_ComponentRemoving(object sender, ComponentRemovingEventArgs e)
        {
            _vcutils.VcWriteWarningMsg("Component removed: " + e.Component.Name);
            _vcmanager.Components.Remove(e.Component);
        }

        public void Start()
        {
            _vcutils.VcWriteWarningMsg("Start Button Clicked");

            // Start Server
            _server = new Server(false, 0, _vcmanager.Components);
            _serverthread = new Thread(new ThreadStart(_server.Run));

            _serverthread.Start();
            CanStart = false;
            CanStop = true;
        }


        public void Stop()
        {
            _vcutils.VcWriteWarningMsg("Stop Button Clicked");
            _server.Stop();
            _serverthread.Abort();

            CanStart = true;
            CanStop = false;
        }

        private bool _canstart = true;
        public bool CanStart
        {
            get { return _canstart; }
            private set
            {
                _canstart = value;
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        private bool _canstop = false;
        public bool CanStop
        {
            get { return _canstop; }
            private set
            {
                _canstop = value;
                NotifyOfPropertyChange(() => CanStop);
            }
        }

        /*
        public void Move()
        {
            // Automatically related to element in *View.xaml having x:Name = this method's name
            try
            {
                VcComponent component = new VcComponent(_vcmanager.GetComponent(Text1));
                IProperty property = component.GetProperty(Text2);

                component.SetPropertyValueDouble(property, double.Parse(Text3));

                _renderService.RequestRender();

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: {0}", e);
            }
        }*/

        #endregion

    }
}
