<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JobList.aspx.cs" Inherits="YesHJ.Log.Web.Admin.JobList" ValidateRequest="false" %>

<%@ Register TagPrefix="HJ" TagName="Nav" Src="~/Controls/HeaderNav.ascx" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <title>沪江站点日志系统</title>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <script src="static/js/jquery.1.9.1.min.js"></script>
    <link rel="stylesheet" href="/static/css/bootstrap.min.css" />
    <script src="static/js/bootstrap.min.js"></script>
    <script src="Static/js/jquery.tmpl.js"></script>
    <script src="Static/My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript">
        var platform = '';
        $(function () {
            $('table a').click(function () {
                edit($(this).attr('data-id'));
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
            var data = $('#edit_form').serialize() || $('#form_add').serialize();
            $.post('', data, function (res) {
                if (res == 'ok') {
                    window.location.reload();
                }
                if (res == 'CountFormatError') {
                    alert('线程数格式错误');
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
            <li class="active">Job列表</li>
        </ol>
        <form class="well form-search " >
            <label>服务器名称：</label>
            <select id="jobservicename" name="jobservicename">
            </select>
            
            
            &nbsp;&nbsp;
            <label>站点名称：</label>
              <select id="sitename" name="sitename">
            </select> &nbsp;&nbsp;
          

            <button type="submit" class="btn btn-default">查询</button>
        </form>
          <input type="hidden" id="hid_sitename" runat="server"/>
          <input type="hidden" id="hid_jobservicename" runat="server"/>
        <div class="row">
            <div class="col-lg-12">
                <table class="table table-striped table-responsive">
                    <thead>
                        <tr>
                            <td>Id
                            </td>
                            <td>服务器名称
                            </td>
                            <td>站点名称
                            </td>
                            <td>线程数量
                            </td>
                            <td>是否有效
                            </td>
                            <td>操作</td>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rep_main" runat="server">
                            <ItemTemplate>
                                <tr <%# !(bool)Eval("JobStatus") ? "class=\"danger\"":string.Empty %>>
                                    <td><%# Eval("Id") %></td>
                                    <td><%# GetJobServiceName(Eval("JobServiceId")) %></td>
                                    <td><%# GetSiteName(Eval("SiteId")) %></td>
                                    <td><%# Eval("JobThreadCount") %></td>
                                    <td><%# (bool)Eval("JobStatus") ? "<span class=\"label label-success\">有效</span>":"<span class=\"label label-default\">无效</span>" %></td>
                                    <td>
                                        <a data-toggle="modal" class="btn btn-default" href="#1" data-target="#myModal" data-id='<%# Eval("Id") %>'>修改</a>
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


    <script type="text/x-jquery-tmpl" id="edit_tmpl">
        <form id="edit_form" class="form-horizontal" role="form">
            <div class="form-group">
                <label for="edit_JobServiceName" class="col-sm-2 control-label">服务器名称</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_JobServiceName" name="JobServiceName" value="${jobentity.JobServiceName}" readonly="readonly">
                </div>
            </div>
            <div class="form-group">
                <label for="edit_SiteName" class="col-sm-2 control-label">站点名称</label>
                <div class="col-xs-8">
                    <input type="text" class="form-control" id="edit_SiteName" name="SiteName" value="${jobentity.SiteName}" readonly="readonly">
                </div>
            </div>

            <div class="form-group">
                <label for="edit_JobThreadCount" class="col-sm-2 control-label">线程数</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="edit_JobThreadCount" name="JobThreadCount" value="${jobentity.JobThreadCount}" placeholder="线程数">
                </div>
            </div>


            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" name="JobStatus"  {{if jobentity.JobStatus}}checked="checked"{{/if}} />
                            有效
                        </label>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <button type="button" class="btn btn-primary" onclick="save();">提交</button>
                    <input type="hidden" name="id" value="${jobentity.Id}" />
                </div>
            </div>
        </form>
    </script>
    
    <script>
        $("#sitename").append(GetOptionStr($("#hid_sitename").val()));
        $("#jobservicename").append(GetOptionStr($("#hid_jobservicename").val()));
        function GetOptionStr(str) {
            var opsstr = str.split('|');
            var ops = opsstr[0].split(',');
            var seleted = opsstr[1];
            var options = "";
            for (var i = 0; i < ops.length; i++) {
                if (ops[i] == seleted) {
                    options += "<option value='" + ops[i] + "' selected='selected'>" + ops[i] + "</option>";
                } else {
                    options += "<option value='" + ops[i] + "'>" + ops[i] + "</option>";
                }
            }
            return options;
        }
    </script>
</body>
</html>