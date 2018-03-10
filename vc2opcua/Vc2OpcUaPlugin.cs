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
using log4net.Appender;
using log4net.Core;

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

    public class VcUtils //: IAppender
    {
        [Import]
        IApplication _application = null;

        IMessageService _ms = IoC.Get<IMessageService>();

        //private log4net.ILog log;
        //private int LogMaxSize = 5000;

        public VcUtils()
        {
            /*
            //Set up logging
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);
            */
        }

        public ReadOnlyCollection<ISimComponent>  GetComponents()
        {
            _application = IoC.Get<IApplication>();
            ReadOnlyCollection<ISimComponent> components = _application.World.Components;

            return components;
        }

        public void VcWriteWarningMsg(string message)
        {
            _ms.AppendMessage("[vc2opcua] " + message, MessageLevel.Warning);
        }

/*
        // Logger stuff that is not set
        public string Name { get; set; }

        public void Close()
        {
            throw new NotImplementedException();
            
        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            throw new NotImplementedException();
        }
*/
    }
}
