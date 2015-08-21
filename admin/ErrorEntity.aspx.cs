using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Web.UI;
using business;

namespace admin
{
    public partial class ErrorEntity : Page
    {
        private readonly ErrorEntitiesBusiness _errorEntitiesBusiness = new ErrorEntitiesBusiness();


        protected void Page_Load(object sender, EventArgs e)
        {
            rep_main.DataSource = _errorEntitiesBusiness.GetList("select WebSite.WebName,ErrorEntity.* from ErrorEntity inner join WebSite on WebSite.WebToken = ErrorEntity.WebToken order by ErrorEntity.DateTime desc");
            rep_main.DataBind();
        }
    }
}