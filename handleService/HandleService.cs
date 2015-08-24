using AutoMapper;
using business;
using dto;
using model;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
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
            ThreadPool.SetMaxThreads(10, 40);   //设置线程
            while (true)
            {
                string errorEntityId = RedisHelper.DequeueItemFromList("ErrorEntityQueue");   //从Redis中获取数据
                if (!string.IsNullOrEmpty(errorEntityId))
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            //0. 从Redis中获取数据
                            var errorEntityDto = RedisHelper.Get<ErrorEntityDto>(errorEntityId);

                            //1. 信息持久到DB, 方便后续查看，统计
                            ErrorEntity errorEntity = null;
                            errorEntity = Mapper.Map<ErrorEntity>(errorEntityDto);
                            errorEntity.NotifyStatus = 0; //未通知
                            _errorEntityBusiness.Insert(errorEntity, false);

                            //2. 通知到相关责任人
                            bool issuccess = Notity(errorEntityDto);
                            if (issuccess)
                            {
                                //3. 更新DB中的状态
                                _errorEntityBusiness.UpdateBySql(string.Format("set NotifyStatus = 1 where Id='{0}'", errorEntity.Id));
                            }

                            //4. 删除redis中的数据
                            RedisHelper.Del(errorEntityId);
                        }
                        catch (Exception exception)
                        {
                            LogHelper.Error("处理发生异常", exception);
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

            //SendMail(webSite, errorMessage);

            return true;
        }

        private void SendMail(WebSite webSite, ErrorEntityDto errorMessage)
        {
            string context = "";
            if (errorMessage.Type == 0)   //普通系统异常
            {
                context = string.Format(EmailTital, webSite.Manager, webSite.WebName, errorMessage.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                context += string.Format(EMailbody, webSite.WebName, errorMessage.MachineName, errorMessage.Ip, errorMessage.RequestUrl,
                    errorMessage.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    errorMessage.ExceptionType, errorMessage.ExceptionMessage, errorMessage.ExceptionSource,
                    errorMessage.ExceptionDetail.Replace("\r\n", "<br/>"), errorMessage.HttpStatusCode,
                    DictionaryHelper.PrintDictionary(errorMessage.ServerVariables).Replace("\r\n", "<br/>"),
                    DictionaryHelper.PrintDictionary(errorMessage.QueryString).Replace("\r\n", "<br/>"),
                    DictionaryHelper.PrintDictionary(errorMessage.Form).Replace("\r\n", "<br/>"),
                    DictionaryHelper.PrintDictionary(errorMessage.Cookies).Replace("\r\n", "<br/>")
              );
            }
            else   //监控异常， 这种异常较为严重，建议发短信
            {
                context = string.Format(EmailTital2, webSite.Manager, webSite.WebName, errorMessage.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                context += string.Format(EMailbody, webSite.WebName, errorMessage.MachineName, errorMessage.Ip, errorMessage.RequestUrl,
                    errorMessage.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    errorMessage.ExceptionType, errorMessage.ExceptionMessage, errorMessage.ExceptionSource,
                    errorMessage.ExceptionDetail.Replace("\r\n", "<br/>"), errorMessage.HttpStatusCode,
                    DictionaryHelper.PrintDictionary(errorMessage.ServerVariables).Replace("\r\n", "<br/>"),
                    DictionaryHelper.PrintDictionary(errorMessage.QueryString).Replace("\r\n", "<br/>"),
                    DictionaryHelper.PrintDictionary(errorMessage.Form).Replace("\r\n", "<br/>"),
                    DictionaryHelper.PrintDictionary(errorMessage.Cookies).Replace("\r\n", "<br/>")
              );
            }

            MailHelper.SendMail("[异常报警]" + webSite.WebName + "(" + errorMessage.Ip + ")", context, "", new List<string>() { webSite.ManagerEmail },
                "", "",
                "");
        }

        public void SendSms(WebSite webSite, ErrorEntityDto errorMessage)
        {
            //todo  发送短信需要连接短信网关， 这里不再实现
        }

        #endregion 发送通知

        #region 邮件模板

        private const string EmailTital = @"<p>&nbsp;Hi  {0}:<br/> &nbsp;&nbsp;&nbsp; 你所负责的站点{1}在{2}发生一个异常,异常信息如下</p><br/>";

        private const string EmailTital2 = @"<p>&nbsp;Hi  {0}:<br/> &nbsp;&nbsp;&nbsp; 系统监测到你所负责的站点{1}在{2}发生了访问异常 ,异常信息如下</p><br/>";

        private const string EMailbody = @"<table style='border:#000000 1px solid;border-collapse:collapse;'>
                                         <tbody>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='120'>服务名称</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{0}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='120'>机器名称</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{1}</td>
                                            </tr>
                                             <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='120'>IP地址</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{2}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>URL</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{3}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='120'>异常时间</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{4}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='120'>异常类型</td>
                                               <td style='border:#000000 1px solid;'  width='800'>{5}</td>
                                            </tr>
                                            <tr>
                                               <td style='border:#000000 1px solid;'  valign='middle' width='120'>异常信息</td>
                                               <td style='border:#000000 1px solid;'   width='800'>{6}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>异常源</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{7}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>异常详细</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{8}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>Http状态码</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{9}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>ServerVariables信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{10}</td>
                                            </tr>
                                             <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>QueryString信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{11}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>Form信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{12}</td>
                                            </tr>
                                            <tr>
                                                <td style='border:#000000 1px solid;'  valign='middle' width='120'>Cookies信息</td>
                                                <td style='border:#000000 1px solid;'  width='800'>{13}</td>
                                            </tr>
                                        </tbody>
                                        </table>";

        #endregion 邮件模板
    }
}