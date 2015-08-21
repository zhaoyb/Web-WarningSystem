using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using business;

namespace admin
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly WebSiteBusiness _webSiteBusiness = new WebSiteBusiness();
        protected void Page_Load(object sender, EventArgs e)
        {
            hid_sitename.Value = string.Join(",", _webSiteBusiness.GetList("select * from WebSite where Enable = 1").Select(x => x.WebName).ToArray());
        }
    }
}