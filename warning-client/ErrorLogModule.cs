using System;
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

        void context_Error(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            ErrorMessage errorMessage = new ErrorMessage(application.Server.GetLastError().GetBaseException(), application.Context);
            Utility.SendError(errorMessage);
        }
    }
}
