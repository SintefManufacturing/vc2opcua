using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net.Appender;
using log4net.Core;

namespace vc2opcua
{
    public partial class mainForm : Form, IAppender
    {
        public static VCManager vcapp = null;
        private log4net.ILog log;
        private int LogMaxSize = 5000;

        public mainForm()
        {
            InitializeComponent();

            // Set up logging
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(typeof(Program));
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);
        }

        string IAppender.Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Start()
        {
            StartButton.Enabled = false;
            // Parse command line
            string host = ServerBox.Text;
            int port = int.Parse(PortBox.Text);

            log.Info(String.Format("Using {0}:{1} as OPCUA discovery server", host, port.ToString()));

            // Create objects and start
            try
            {
                log.Info("Connected to OPCUA");
                // TODO: Connect to OPCUA
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message + ": Could not connect to OPCUA Server. Are the servers running?");
                StartButton.Enabled = true;
                return;
            }
            try
            {
                log.Info("Connected to Visual Components");
                vcapp = new VCManager();
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message + ": Could not connect to Visual Components");
                StopButton.Enabled = true;
                return;
            }

            log.Info("VC2OPCUA running");
            StopButton.Enabled = true;
        }

        public void Stop()
        {
            StopButton.Enabled = false;
            log.Warn("Shutting down VCManager\n");
        }

        void IAppender.Close()
        {
            throw new NotImplementedException();
        }

        void IAppender.DoAppend(LoggingEvent loggingEvent)
        {
            throw new NotImplementedException();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }
    }
}
