<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebSite.aspx.cs" Inherits="admin.WebSite" ValidateRequest="false" %>

<%@ Register TagPrefix="HJ" TagName="Nav" Src="~/Controls/HeaderNav.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <title>沪江站点日志系统</title>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <script src="/static/js/jquery.1.9.1.min.js"></script>
    <link rel="stylesheet" href="/static/css/bootstrap.min.css" />
    <script src="/static/js/bootstrap.min.js"></script>
    <script src="/static/js/jquery.tmpl.js"></script>
    <script src="Static/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript">
        var platform = '';
        $(function () {
            $('table a').click(function () {
                edit($(this).attr('data-id'));
            });

            $('#newplatform').click(function () {

                $('div.modal-body').html('');
                $('#add_tmpl').tmpl().appendTo('div.modal-body');

                $('#myModalLabel').text('添加新站点');
            });
        });

        function edit(id) {

            $.getJSON('?id=' + id, function (data) {
                $('div.modal-body').html('');
                $('#edit_tmpl').tmpl(data).appendTo('div.modal-body');
                $('#myModalLabel').text('修改站点');
            });
        }
        function save() {
            if ($('#delete_checkbox').prop('checked')) {
                if (!confirm('确定要删除?')) {
                    return false;
                }
            }

            var data = $('#edit_form').serialize() || $('#form_add').serialize();
            $.post('', data, function (res) {
                if (res == 'ok') {
                    window.location.reload();
                }
                if (res == 'exists') {
                    alert('指定的站点名称已存在');
                } else if (res == 'IntervalFormatError') {
                    alert("备份频率数值不正确");
                } else if (res == 'TimeFormatError') {
                    alert("备份时间数值不正确");
                } else if (res == 'keyword') {
                    alert("站点名称为关键字");
                } else if (res == 'TimeError') {
                    alert("备份时间必须大于当前时间");
                }
            });
        }
    </script>
    <style type="text/css">
        div.easyDialog_footer {
            display: none;
        }

        .table > thead > tr > th, .table > tbody > tr > th, .table > tfoot > tr > th, .table > thead > tr > td, .table > tbody > tr > td, .table > tfoot > tr > td {
            vertical-align: middle;
        }

        body {
            padding-top: 60px;
        }

        a {
            cursor: pointer;
        }
    </style>
</head>
<body>
    <HJ:Nav ID="Nav" runat="server" />
    <div class="container">
        <ol class="breadcrumb">
            <li><a href="/">日志中心</a></li>
            <li class="active">站点列表( <a id="newplatform" data-toggle="modal" data-target="#myModal">添加新站点</a>)</li>
        </ol>



        <form class="well form-search ">
            <div class="col-xs-6">
                <div class="input-group">
                    <span class="input-group-addon">搜索</span>
                    <input type="text" class="form-control glyphicon-search" id="exampleInputEmail2" name="keyword" placeholder="站点名称或URL或密钥" value="<%=HttpUtility.HtmlEncode(Keyword) %>" />
                </div>

            </div>
            <button type="submit" class="btn btn-default">查询</button>
        </form>
        <div class="row">
            <div class="col-lg-12">
                <table class="table table-striped table-responsive">
                    <thead>
                        <tr>
                            <td>Id
                            </td>
                            <td>站点名称
                            </td>
                            <td>URL
                            </td>
                            <td>级别
                            </td>
                             <td>备份频率
                            </td>
                             <td>下次备份时间
                            </td>
                            <td>是否启用
                            </td>
                            <td>操作</td>
                            <td>创建人</td>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rep_main" runat="server">
                            <ItemTemplate>
                                <tr <%# !(bool)Eval("SiteStatus") ? "class=\"danger\"":string.Empty %>>
                                    <td><%# Eval("Id") %></td>
                                    <td><%# Eval("SiteName") %></td>
                                    <td><%# Eval("SiteUrl") %></td>
                                    <td><%# Eval("SiteLevel") %></td>
                                    <td><%# Eval("SiteBackUpInterval") %><%# Eval("SiteBackUpIntervalUnit") %></td>
                                    <td><%# Eval("SiteNextBackUpTime") %></td>
                                    <td><%# (bool)Eval("SiteStatus") ? "<span class=\"label label-success\">有效</span>":"<span class=\"label label-default\">无效</span>" %></td>
                                    <td>
                                        <% if (CurrentUser.IsManager)
                                           { %>
                                        <a data-toggle="modal"  class="btn btn-default" href="#1" data-target="#myModal" data-id='<%# Eval("Id") %>'>修改</a>
                                         <a class="btn btn-default" target="_blank" href='/BackUpLog.aspx?siteId=<%# Eval("Id") %>'>查看备份情况</a>
                                        <a class="btn btn-default" target="_blank" href='/JobList.aspx?siteId=<%# Eval("Id") %>'>查看Job情况</a>
                                        <% }
                                           else
                                           {
                                        %>
                                        <a data-toggle="modal"  class="btn btn-default" href="#1" data-target="#myModal" data-id='<%# Eval("Id") %>'>查看</a>
                                        <% } %>
                                    </td>
                                    <td>
                                        <%# Eval("SiteAuthor") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>

    </div>




    <!-- Modal -->
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="myModalLabel">Modal title</h4>
                </div>
                <div class="modal-body">
                </div>

            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->


    <script type="text/x-jquery-tmpl" id="add_tmpl">
        <form id="form_add" class="form-horizontal" role="form">
            <div class="form-group">
                <label for="add_sitename" class="col-sm-2 control-label">站点名称</label>
                <div class="col-xs-8">
                    <input onkeyup="value=value.replace(/[^\a-\z\A-\Z]/g,'')" onpaste="value=value.replace(/[^\a-\z\A-\Z]/g,'')" oncontextmenu = "value=value.replace(/[^\a-\z\A-\Z]/g,'')" type="text" class="form-control" id="add_sitename" name="sitename" value="" placeholder="站点名称(英文)">
                </div>
            </div>
            <div class="form-group">
                <label for="add_siteurl" class="col-sm-2 control-label">站点URL</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_siteurl" name="siteUrl" value="" placeholder="站点URL">
                </div>
            </div>
            <div class="form-group">
                <label for="add_SiteLevel" class="col-sm-2 control-label">站点级别</label>
                <div class="col-xs-8">
                    <select id="add_SiteLevel" name="SiteLevel" class="form-control">
                         <option value="高">高</option>
                         <option value="中" selected="selected">中</option>
                         <option value="低">低</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="add_sitebackupinterval" class="col-sm-2 control-label">备份频率</label>
                <div class="col-xs-8">
                    <input type="text"  id="add_sitebackupinterval" class="form-control-custom" name="sitebackupinterval" value="" placeholder="备份频率" style="width: 100px">&nbsp;&nbsp;
                    <select id="add_SiteBackUpIntervalUnit" name="SiteBackUpIntervalUnit"  style="width: 100px" class="form-control-custom">
                         <option value="秒">秒</option>
                         <option value="分" selected="selected">分</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="add_SiteNextBackUpTime" class="col-sm-2 control-label">下次备份时间</label>
                <div class="col-xs-8">
                    <input type="text" id="add_SiteNextBackUpTime" name="SiteNextBackUpTime" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" class="Wdate" style="width:210px"/>
                </div>
            </div>
            <div class="form-group">
                <label for="add_siteToken" class="col-sm-2 control-label">站点密钥</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_siteToken" name="SiteToken" value="<%=Clientkey %>" readonly="readonly" placeholder="站点密钥">
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" name="SiteStatus" checked="checked" value="1" />
                            启用
                        </label>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <button type="button" class="btn btn-primary" onclick="save();">添加</button>

                </div>
            </div>
        </form>
    </script>

    <script type="text/x-jquery-tmpl" id="edit_tmpl">
        <form id="edit_form" class="form-horizontal" role="form">
            <div class="form-group">
                <label for="edit_SiteName" class="col-sm-2 control-label">站点名称</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_SiteName" name="SiteName" readonly="readonly" value="${data.SiteName}" placeholder="站点名称">
                </div>
            </div>
            <div class="form-group">
                <label for="edit_SiteUrl" class="col-sm-2 control-label">站点URL</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_SiteUrl" name="SiteUrl" value="${data.SiteUrl}" placeholder="站点URL">
                </div>
            </div>
                <div class="form-group">
                <label for="edit_SiteLevel" class="col-sm-2 control-label">站点级别</label>
                <div class="col-xs-8">
                    <select id="edit_SiteLevel" name="SiteLevel" class="form-control">
                         <option value="高" {{if '高'==data.SiteLevel}} selected="selected"{{/if}} >高</option>
                         <option value="中" {{if '中'==data.SiteLevel}} selected="selected"{{/if}} >中</option>
                         <option value="低" {{if '低'==data.SiteLevel}} selected="selected"{{/if}} >低</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="edit_sitebackupinterval" class="col-sm-2 control-label">备份频率</label>
                <div class="col-xs-8">
                    <input type="text" id="edit_sitebackupinterval" name="sitebackupinterval" value="${data.SiteBackUpInterval}" placeholder="备份频率" style="width: 100px">&nbsp;&nbsp;

                    <select id="edit_SiteBackUpIntervalUnit" name="SiteBackUpIntervalUnit" style="width: 100px" >
                         <option value="秒" {{if '秒'==data.SiteBackUpIntervalUnit}} selected="selected"{{/if}}>秒</option>
                         <option value="分" {{if '分'==data.SiteBackUpIntervalUnit}} selected="selected"{{/if}}>分</option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label for="editd_SiteNextBackUpTime" class="col-sm-2 control-label">下次备份时间</label>
                <div class="col-xs-8">
                    <input type="text" id="editd_SiteNextBackUpTime" name="SiteNextBackUpTime" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" value="${data.SiteNextBackUpTime}" class="Wdate" style="width:210px"/>
                </div>
            </div>

            <div class="form-group">
                <label for="edit_SiteToken" class="col-sm-2 control-label">站点密钥</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_SiteToken" name="SiteToken" readonly="readonly" value="${data.SiteToken}" placeholder="站点密钥">
                </div>
            </div>
            <div class="form-group">
                <label for="edit_SiteAuthor" class="col-sm-2 control-label">创建人</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_SiteAuthor" name="SiteAuthor" readonly="readonly" value="${data.SiteAuthor}" placeholder="创建人">
                </div>
            </div>

            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" name="SiteStatus"  {{if data.SiteStatus}}checked="checked"{{/if}}>
                            有效
                        </label>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" name="delete" id="delete_checkbox">
                            <span class="label label-danger">删除</span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <button type="button" class="btn btn-primary" onclick="save();">提交</button>
                    <input type="hidden" name="id" value="${data.Id}" />
                </div>
            </div>
        </form>
    </script>
</body>
</html>
