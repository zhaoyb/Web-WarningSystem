using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using YesHJ.Log.Business;
using YesHJ.Log.Model;

namespace YesHJ.Log.Web.Admin
{
    /// <summary>
    ///     BasePage 的摘要说明
    /// </summary>
    public class BasePage : Page
    {
        private UserInfo _currUser;
        private List<UserSiteMapping> _userSites;

        public UserInfo CurrentUser
        {
            get { return _currUser; }
        }


        public List<UserSiteMapping> UserSites
        {
            get { return _userSites ?? (_userSites = UserSiteMappingBusiness.GetUserSiteMappingByUserId(CurrentUser.Id)); }
        }

        protected override void OnInit(EventArgs e)
        {
            string currUserName = null;
            if (Request.IsAuthenticated)
            {
                string userData = ((FormsIdentity)User.Identity).Ticket.UserData;
                int hjID;
                currUserName = int.TryParse(userData.Split('|')[0], out hjID)
                    ? userData.Split('|')[1]
                    : userData.Split(',')[0];
            }


            _currUser = Session["curruser"] as UserInfo;
            if (_currUser == null)
            {
                if (!string.IsNullOrEmpty(currUserName))
                {
                    UserInfo user = UserBusiness.GetUserInfoByName(currUserName);
                    if (user == null || user.UserStatus == false)
                    {
                        //没有该用户或被禁用

                        BadAccess("您没有登录这个后台的权限或您的帐号被禁用");
                    }
                    else
                    {
                        _currUser = new UserInfo
                        {
                            UserName = user.UserName,
                            Id = user.Id,
                            IsManager = user.IsManager,
                            UserStatus = user.UserStatus,
                            UserEmail = user.UserEmail,
                            UserPhone = user.UserPhone,
                            RealName = user.RealName
                        };
                        Session["curruser"] = _currUser;
                    }
                }
                else
                {
                    Response.Redirect(FormsAuthentication.LoginUrl + "?url=" +
                                      HttpUtility.UrlEncode(Request.Url.AbsoluteUri));
                    Response.End();
                }
            }
            base.OnInit(e);
        }


        protected bool HasPermission(int siteId)
        {
            if (CurrentUser.IsManager) return true;
            return UserSites.Exists(usersite => usersite.SiteId == siteId);
        }


        protected virtual void BadAccess(string msg)
        {
            Server.Transfer("~/AccessError.aspx?msg=" + HttpUtility.UrlEncode(msg), true);
        }

        protected virtual void BadAccess()
        {
            BadAccess(string.Empty);
        }


        protected override void OnLoad(EventArgs e)
        {
            if (Request.QueryString["action"] == "logout")
            {
                Session.RemoveAll();
                Session.Abandon();
                const string script = @"
<script type=""text/javascript"">
window.top.location.href='http://pass.hujiang.com/uc/handler/logout.ashx?returnurl=http://pass.hujiang.com';
</script>
";
                Response.Write(script);
                Response.End();
            }
            base.OnLoad(e);
        }
    }
}