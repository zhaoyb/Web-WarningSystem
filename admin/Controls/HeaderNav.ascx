<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HeaderNav.ascx.cs" Inherits="admin.ControlsHeaderNav" %>
<div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
    <div class="container">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="/">WarningSystem</a>
        </div>
        <div class="collapse navbar-collapse">
            <ul class="nav navbar-nav">
                <li data-href="" class="active"><a href="/">日志报表</a></li>
                <li data-href="LogList.aspx"><a href="LogList.aspx">日志列表</a></li>
                <li data-href="Site.aspx"><a href="Site.aspx">站点列表</a></li>
                <li data-href="http://wiki.yeshj.com/pages/viewpage.action?pageId=9701051"><a href="http://wiki.yeshj.com/pages/viewpage.action?pageId=9701051">帮助文档</a></li>
            </ul>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(function () {
        
        var path = location.pathname.toLocaleLowerCase().replace("/",'');
        if (path !='') {
            $('ul.nav li').removeClass('active');
            $('ul.nav li[data-href="'+path+'"]').addClass('active');
        }
    });
</script>
