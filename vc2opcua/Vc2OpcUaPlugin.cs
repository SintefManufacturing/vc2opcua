using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.Composition;
using Caliburn.Micro;
using VisualComponents.Create3D;
using VisualComponents.UX.Shared;
using log4net.Appender;

namespace vc2opcua
{
    [Export(typeof(IPlugin))]
    public class Vc2OpcUaPlugin : IPlugin
    {
        VcManager _vcmanager = new VcManager();

        void IPlugin.Exit()
        {

        }

        void IPlugin.Initialize()
        {
            string message = "Plugin loaded";
            _vcmanager.VcWriteWarningMsg(message);
        }
    }
}
