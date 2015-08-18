using System;
using System.Web.UI;
using YesHJ.Log.Model;

namespace YesHJ.Log.Web.Admin.Controls
{
    public partial class ControlsHeaderNav : UserControl
    {
        protected bool IsSuperAdmin
        {
            get
            {
                var bp = Page as BasePage;
                if (bp == null) return false;
                return bp.CurrentUser.IsManager;
            }
        }

        protected bool IsLogin
        {
            get
            {
                var bp = Page as BasePage;
                if (bp == null) return false;
                return bp.CurrentUser != null;
            }
        }

        protected UserInfo CurrentUser
        {
            get
            {
                var basePage = Page as BasePage;
                if (basePage != null)
                    return basePage.CurrentUser;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}