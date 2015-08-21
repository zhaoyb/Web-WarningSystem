<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebSite.aspx.cs" Inherits="admin.WebSite" ValidateRequest="false" %>

<%@ Register TagPrefix="Warning" TagName="Nav" Src="~/Controls/HeaderNav.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <title>异常报警系统</title>
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
    <Warning:Nav ID="Nav" runat="server" />
    <div class="container">
        <ol class="breadcrumb">
            <li><a href="/">异常报警系统</a></li>
            <li class="active">监控站点( <a id="newplatform" data-toggle="modal" data-target="#myModal">添加新站点</a>)</li>
        </ol>
        <div class="row">
            <div class="col-lg-12">
                <table class="table table-striped table-responsive">
                    <thead>
                        <tr>
                            <td>Id
                            </td>
                            <td>站点名称
                            </td>
                            <td>Host
                            </td>
                            <td>负责人
                            </td>
                             <td>负责人手机
                            </td>
                             <td>负责人邮箱
                            </td>
                              <td>CheckUrl
                            </td>
                            <td>是否启用
                            </td>
                            <td>操作</td>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rep_main" runat="server">
                            <ItemTemplate>
                                <tr <%# Eval("Enable").ToString()=="0" ? "class=\"danger\"":string.Empty %>>
                                    <td><%# Eval("Id") %></td>
                                    <td><%# Eval("WebName") %></td>
                                    <td><%# Eval("Host") %></td>
                                    <td><%# Eval("Manager") %></td>
                                    <td><%# Eval("ManagerPhone") %></td>
                                    <td><%# Eval("ManagerEmail") %></td>
                                    <td><%# Eval("CheckUrl") %></td>
                                    <td><%#  Eval("Enable").ToString()=="1" ? "<span class=\"label label-success\">启用</span>":"<span class=\"label label-default\">关闭</span>" %></td>
                                    <td>
                                        <a data-toggle="modal"  class="btn btn-default" href="#1" data-target="#myModal" data-id='<%# Eval("Id") %>'>修改</a>
                                        <a class="btn btn-default" target="_blank" href='/WebService.aspx?WebId=<%# Eval("Id") %>'>服务器列表</a>
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
                <label for="add_WebNamee" class="col-sm-2 control-label">站点名称</label>
                <div class="col-xs-8">
                    <input  type="text" class="form-control" id="add_WebNamee" name="WebName" value="" placeholder="站点名称">
                </div>
            </div>
            <div class="form-group">
                <label for="add_Host" class="col-sm-2 control-label">域名</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_Host" name="Host" value="" placeholder="域名">
                </div>
            </div>
            <div class="form-group">
                <label for="add_WebToken" class="col-sm-2 control-label">站点密钥</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_WebToken" name="WebToken" value="<%=Clientkey %>" readonly="readonly" placeholder="站点密钥">
                </div>
            </div>
            
              <div class="form-group">
                <label for="add_Manager" class="col-sm-2 control-label">负责人</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_Manager" name="Manager" placeholder="负责人">
                </div>
            </div>
            
              <div class="form-group">
                <label for="add_ManagerPhone" class="col-sm-2 control-label">负责人手机</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_ManagerPhone" name="ManagerPhone"  placeholder="负责人手机">
                </div>
            </div>
            
              <div class="form-group">
                <label for="add_ManagerEmailn" class="col-sm-2 control-label">负责人邮箱</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_ManagerEmailn" name="ManagerEmail"  placeholder="负责人邮箱">
                </div>
            </div>
            
              <div class="form-group">
                <label for="add_CheckUrl" class="col-sm-2 control-label">监控页面</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="add_CheckUrl" name="CheckUrl" placeholder="监控页面">
                </div>
            </div>
            

            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" name="Enable" checked="checked" value="1" />
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
                <label for="edit_WebName" class="col-sm-2 control-label">站点名称</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_WebName" name="WebName"  value="${data.WebName}" placeholder="站点名称">
                </div>
            </div>
            <div class="form-group">
                <label for="edit_Host" class="col-sm-2 control-label">域名</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_Host" name="Host" value="${data.Host}" placeholder="域名">
                </div>
            </div>
            <div class="form-group">
                <label for="edit_SiteToken" class="col-sm-2 control-label">站点密钥</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_SiteToken" name="SiteToken" readonly="readonly" value="${data.WebToken}" placeholder="站点密钥">
                </div>
            </div>
             <div class="form-group">
                <label for="edit_Manager" class="col-sm-2 control-label">负责人</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_Manager" name="Manager"  value="${data.Manager}" placeholder="负责人">
                </div>
            </div>
             <div class="form-group">
                <label for="edit_ManagerPhone" class="col-sm-2 control-label">负责人手机</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_ManagerPhone" name="ManagerPhone"  value="${data.ManagerPhone}" placeholder="负责人手机">
                </div>
            </div>
             <div class="form-group">
                <label for="edit_ManagerEmail" class="col-sm-2 control-label">负责人邮箱</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_ManagerEmail" name="ManagerEmail"  value="${data.ManagerEmail}" placeholder="负责人邮箱">
                </div>
            </div>
             <div class="form-group">
                <label for="edit_CheckUrl" class="col-sm-2 control-label">监控页面</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_CheckUrl" name="CheckUrl"  value="${data.CheckUrl}" placeholder="监控页面">
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" name="Enable"  {{if data.Enable}}checked="checked"{{/if}}>
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