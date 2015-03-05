using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dto;
using utility;

namespace notifyservice
{
    public partial class Notify : ServiceBase
    {
        public Notify()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread notifyThread = new Thread(NotityUser);
            notifyThread.Start();
            notifyThread.IsBackground = true;
            LogHelper.Info("notity job start...");
        }

        public void NotityUser()
        {
            while (true)
            {
                string errorMessageId = RedisHelper.DequeueItemFromList("ErrorMessageQueue");
                if (errorMessageId != "")
                {
                    ErrorMessage errorMessage = RedisHelper.Get<ErrorMessage>(errorMessageId);
                    SendMail(errorMessage);
                }
                else
                {
                    Thread.Sleep(5000); //如果没有取到数据，则休息5s
                }
            }
        }


        private void SendMail(ErrorMessage errorMessage)
        {






            MailHelper.SendMail("[异常报警]" + errorMessage.Ip, "", "", null, "smtp.sohu.com", "mail.sohu.com",
                   "zhaoyabin");
        }

        protected override void OnStop()
        {
            LogHelper.Info("notity job stop...");
        }
    }
}
