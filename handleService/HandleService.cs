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
    public partial class HandleService : ServiceBase
    {
        public HandleService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var notifyThread = new Thread(Handle);
            notifyThread.Start();
            notifyThread.IsBackground = true;
            LogHelper.Info("handle job start...");
        }

        public void Handle()
        {
            while (true)
            {
                string errorEntityId = RedisHelper.DequeueItemFromList("ErrorEntityQueue");
                if (errorEntityId != "")
                {
                    var errorMessage = RedisHelper.Get<ErrorEntity>(errorEntityId);

                    //1. 信息持久到DB, 方便后续查看，统计


                    //2. 根据异常信息， 查找对应的责任人 ， 并得到对应的通知类型 

                    SendMail(errorMessage);


                    //3. 更新DB中的状态
                }
                else
                {
                    Thread.Sleep(5000); //如果没有取到数据，则暂停5s
                }
            }
        }


        private void SendMail(ErrorEntity errorMessage)
        {
            MailHelper.SendMail("[异常报警]" + errorMessage.Ip, "", "", null, "smtp.sohu.com", "mail.sohu.com",
                   "zhaoyabin");
        }

        protected override void OnStop()
        {
            LogHelper.Info("handle job stop...");
        }
    }
}
