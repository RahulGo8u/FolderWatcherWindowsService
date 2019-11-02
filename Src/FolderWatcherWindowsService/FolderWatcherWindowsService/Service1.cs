using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderWatcherWindowsService
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Timer timer;
        private readonly string incomingPath = ConfigurationManager.AppSettings["IncomingPath"].ToString();
        private int counter;
        private readonly string outputPath = ConfigurationManager.AppSettings["OutputPath"].ToString();
        public Service1()
        {
            InitializeComponent();            
        }
        public void RunOnDebugMode()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            log.Debug("Service Started");
            counter++;
            ProcessService();
        }
        protected override void OnStop()
        {
            log.Debug("Service Stopped");
            timer.Dispose();
        }
        private void SchedulerCallBack(object o)
        {
            log.Debug("Scheduler Callback");            
            ProcessService();
        }
        private void ProcessService()
        {
            log.Debug("Process Service Count =" + counter);
            timer = new Timer(new TimerCallback(SchedulerCallBack));
            MoveFiles();
            counter++;
            timer.Change(10000, Timeout.Infinite);
        }
        private void MoveFiles()
        {
            DirectoryInfo d = new DirectoryInfo(incomingPath);
            FileInfo[] Files = d.GetFiles("*.*");
            if (Files.Count() > 0)
            {
                foreach (FileInfo file in Files)
                {
                    if (File.Exists(outputPath + file.Name))
                    {
                        File.Delete(outputPath + file.Name);
                    }
                    File.Move(incomingPath + file.Name, outputPath + file.Name);
                }
            }            
        }
    }
}
