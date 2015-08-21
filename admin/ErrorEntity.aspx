<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ErrorEntity.aspx.cs" Inherits="admin.ErrorEntity" ValidateRequest="false" %>

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
            <li class="active">异常信息</li>
        </ol>
        <div class="row">
            <div class="col-lg-12">
                <table class="table table-striped table-responsive">
                    <thead>
                        <tr>
                            <td>站点名称
                            </td>
                            <td>机器名
                            </td>
                             <td>ExceptionMessage
                            </td>
                            <td>操作</td>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rep_main" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("WebName") %></td>
                                    <td><%# Eval("IP") %></td>
                                    <td><%# Eval("ExceptionMessage") %></td>
                                    <td>
                                        <a class="btn btn-default" target="_blank" href='/ErrorEntityDetail.aspx?ErrorId=<%# Eval("Id") %>'>查看</a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</body>
</html>