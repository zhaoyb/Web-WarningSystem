using AutoMapper;
using business;
using dto;
using model;
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
using utility;

namespace notifyservice
{
    public partial class HandleService : ServiceBase
    {
        private readonly ErrorEntityBusiness _errorEntityBusiness = new ErrorEntityBusiness();
        private readonly WebSiteBusiness _webSiteBusiness = new WebSiteBusiness();

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

        protected override void OnStop()
        {
            LogHelper.Info("handle job stop...");
        }

        public void Handle()
        {
            while (true)
            {
                string errorEntityId = RedisHelper.DequeueItemFromList("ErrorEntityQueue");   //从Redis中获取数据
                if (errorEntityId != "")
                {
                    ThreadPool.SetMaxThreads(10, 40);   //设置线程
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        //0. 从Redis中获取数据
                        var errorEntityDto = RedisHelper.Get<ErrorEntityDto>(errorEntityId);

                        //1. 信息持久到DB, 方便后续查看，统计
                        var errorEntity = Mapper.Map<ErrorEntity>(errorEntityDto);
                        errorEntity.NotityStatus = 0; //未通知
                        _errorEntityBusiness.Insert(errorEntity);
                        //2. 通知到相关责任人
                        bool issuccess = Notity(errorEntityDto);
                        if (issuccess)
                        {
                            //3. 更新DB中的状态
                            errorEntity.NotityStatus = 1; //已通知
                            _errorEntityBusiness.Update(new { NotityStatus = 1 }, errorEntity.Id);
                        }

                        //todo 这里应该以有个补救措施， 如果通知失败。 应该再另外一个job中对这些失败的通知重新发送。这里就不写了
                    }, errorEntityId);
                }
                else
                {
                    Thread.Sleep(5000); //如果没有取到数据，则暂停5s
                }
            }
        }

    

        #region 发送通知

        private bool Notity(ErrorEntityDto errorMessage)
        {
            WebSite webSite = _webSiteBusiness.GetSingleOrDefault(string.Format("select * from WebSite where WebToken='{0}'", errorMessage.WebToken));

            if (webSite == null)
            {
                LogHelper.Warn("根据webtoken未找到对应的配置，可能为非法请求!");
                return false;
            }

            return true;
        }

        private void SendMail(WebSite webSite, ErrorEntityDto errorMessage)
        {
            string context = string.Format(EmailTital, webSite.Manager, webSite.WebName, errorMessage.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));

            context += string.Format(EMailbody, webSite.WebName, errorMessage.MachineName,
                errorMessage.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                errorMessage.ExceptionType, errorMessage.ExceptionMessage, errorMessage.ExceptionSource,
                errorMessage.ExceptionDetail, errorMessage.HttpStatusCode,
                NameValueCollectionHelper.PrintNameValueCollection(errorMessage.ServerVariables),
                NameValueCollectionHelper.PrintNameValueCollection(errorMessage.QueryString),
                NameValueCollectionHelper.PrintNameValueCollection(errorMessage.Form),
                NameValueCollectionHelper.PrintNameValueCollection(errorMessage.Cookies),
                errorMessage.RequestUrl);

            MailHelper.SendMail("[异常报警]" + webSite.WebName + "(" + errorMessage.Ip + ")", context, "zhao_yabin@sohu.com", new List<string>() { webSite.ManagerEmail },
                "smtp.sohu.com", "zhao_yabinm",
                "zhaoyabin");
        }

        public void SendSms(WebSite webSite, ErrorEntityDto errorMessage)
        {
            //todo  发送短信需要连接短信网关， 这里不再实现
        }


        #endregion

        #region 邮件模板
        private const string EmailTital = @"<p>&nbsp;Hi  {0}:<br/> &nbsp;&nbsp;&nbsp; 你所负责的站点{1}在{2}发生一个异常,异常信息如下</p>";

        private const string EMailbody = @"<table style='border:#000000 1px solid;border-collapse:collapse;'>
                                         <tbody>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='100'>站点名</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{0}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='100'>机器名</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{1}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='100'>异常时间</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{2}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='100'>异常类型</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{3}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='100'>异常信息</td>
                                               <td style='border:#000000 1px solid;'   width='800'>{4}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>异常源</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{5}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>异常详细</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{6}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>Http状态码</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{7}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>ServerVariables信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{8}</td>
                                            </tr>
                                             <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>QueryString信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{9}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>Form信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{10}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>Cookies信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{11}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='100'>URL</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{12}</td>
                                            </tr>
                                        </tbody>
                                        </table>";
        #endregion

    }
}