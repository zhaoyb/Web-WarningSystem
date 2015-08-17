using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace webcheckservice
{
    public partial class HealthyCheckService : ServiceBase
    {
        public HealthyCheckService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TaskManager.AddTask(Handle, x => x.WithName("HealthyCheckService").NonReentrant().ToRunEvery(2).Seconds());
        }

        protected override void OnStop()
        {
            TaskManager.GetSchedule("HealthyCheckService").Disable();
        }

        private static void Handle()
        {
            //获取所有的网站， 依次检查返回的是否为ok, 或者检查http.status=200

            Parallel.ForEach()
        }
    }
}
