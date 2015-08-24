<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebService.aspx.cs" Inherits="admin.WebService" ValidateRequest="false" %>

<%@ Register TagPrefix="Warning" TagName="Nav" Src="~/Controls/HeaderNav.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <title>异常日志报警系统</title>
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

                $('#myModalLabel').text('添加新服务器');
            });
        });

        function edit(id) {

            $.getJSON('?id=' + id, function (data) {
                $('div.modal-body').html('');
                $('#edit_tmpl').tmpl(data).appendTo('div.modal-body');
                $('#myModalLabel').text('修改服务器');
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
            <li><a href="/">异常日志报警系统</a></li>
            <li class="active">站点服务器( <a id="newplatform" data-toggle="modal" data-target="#myModal">添加新服务器</a>)</li>
        </ol>
        <div class="row">
            <div class="col-lg-12">
                <table class="table table-striped table-responsive">
                    <thead>
                        <tr>
                            <td>Id
                            </td>
                            <td>服务器IP
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
                                    <td><%# Eval("ServerIp") %></td>
                                    <td><%#  Eval("Enable").ToString()=="1" ? "<span class=\"label label-success\">启用</span>":"<span class=\"label label-default\">关闭</span>" %></td>
                                    <td>
                                        <a data-toggle="modal"  class="btn btn-default" href="#1" data-target="#myModal" data-id='<%# Eval("Id") %>'>修改</a>
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
                <label for="add_ServerIp" class="col-sm-2 control-label">服务器IP</label>
                <div class="col-xs-8">
                    <input  type="text" class="form-control" id="add_ServerIp" name="ServerIp" value="" placeholder="服务器IP">
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
                     <input type="hidden" name="WebId" value="<%= WebId %>" />
                </div>
            </div>
        </form>
    </script>

    <script type="text/x-jquery-tmpl" id="edit_tmpl">
        <form id="edit_form" class="form-horizontal" role="form">
            <div class="form-group">
                <label for="edit_ServerIp" class="col-sm-2 control-label">服务器IP</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_ServerIp" name="ServerIp"  value="${data.ServerIp}" placeholder="服务器IP">
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
                     <input type="hidden" name="WebId" value="<%= WebId %>" />
                </div>
            </div>
        </form>
    </script>
</body>
</html>