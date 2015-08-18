<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HeaderNav.ascx.cs" Inherits="YesHJ.Log.Web.Admin.Controls.ControlsHeaderNav" %>
<div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
    <div class="container">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="/">Log Center</a>
        </div>
        <div class="collapse navbar-collapse">
            <ul class="nav navbar-nav">
                <li data-href="" class="active"><a href="/">日志报表</a></li>
                <li data-href="LogList.aspx"><a href="LogList.aspx">日志列表</a></li>
                <li data-href="Site.aspx"><a href="Site.aspx">站点列表</a></li>
                <%if (IsSuperAdmin)
                  { %>
                <li data-href="JobService.aspx"><a href="JobService.aspx">Job管理</a></li>
                <li data-href="user.aspx"><a href="User.aspx">用户管理</a></li>
                <%} %>
                <li data-href="http://wiki.yeshj.com/pages/viewpage.action?pageId=9701051"><a href="http://wiki.yeshj.com/pages/viewpage.action?pageId=9701051">帮助文档</a></li>
            </ul>

            <%if (IsLogin)
              { %>

            <ul class="nav navbar-nav navbar-right">

                <li class="dropdown">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown">您好：<%=  CurrentUser.UserName%><b class="caret"></b></a>
                    <ul class="dropdown-menu">
                        <li><a href="?action=logout">退出管理</a></li>

                    </ul>
                </li>
            </ul>
            <%} %>
        </div>
    </div>
</div>
<script type="text/javascript">
    var islogin = <%=IsLogin ? 1 : 0%>;
    $(function () {
        
        var path = location.pathname.toLocaleLowerCase().replace("/",'');
        if (path !='') {
            $('ul.nav li').removeClass('active');
            $('ul.nav li[data-href="'+path+'"]').addClass('active');
        }
    });
</script>
