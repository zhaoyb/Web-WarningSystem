using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace warning
{
    public class ErrorLogModule : IHttpModule
    {

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.Error += context_Error;
        }

        #endregion

        private void context_Error(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication) sender;
            ClientErrorEntity errorMessage = new ClientErrorEntity(application.Server.GetLastError().GetBaseException(),
                application.Context);
            SendErrorEntity.SendError(errorMessage);
        }
    }
}
