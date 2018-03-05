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
    public class VcManager : IPlugin
    {
        // Accesses the application itself, initialize to null for avoiding compiling errors
        [Import]
        IApplication _application = null;

        void IPlugin.Exit()
        {

        }

        void IPlugin.Initialize()
        {
            IMessageService ms = IoC.Get<IMessageService>();
            string message = "VC2OPCUA plugin loaded";
            ms.AppendMessage(message, MessageLevel.Warning);
        }
    }
}
