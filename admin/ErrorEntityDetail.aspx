<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ErrorEntityDetail.aspx.cs" Inherits="admin.ErrorEntityDetail" ValidateRequest="false" %>

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
            <li class="active">异常信息</li>
        </ol>
        <div class="row">
            <div class="col-lg-12">
                <asp:Repeater ID="rep_main" runat="server">
                    <ItemTemplate>
                        <table class="table  table-responsive">
                            <tbody>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>服务名称</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("WebName") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>机器名称</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("MachineName") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>IP地址</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("Ip") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>URL</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("RequestUrl") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>异常时间</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Convert.ToDateTime( Eval("DateTime")).ToString("yyyy-MM-dd HH:mm:ss") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>异常类型</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("ExceptionType") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>异常信息</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("ExceptionMessage") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>异常源</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("ExceptionSource") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>异常详细</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("ExceptionDetail") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>Http状态码</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# Eval("HttpStatusCode") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>ServerVariables信息</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# (Eval("ServerVariables")??"").ToString().Replace("\r\n","<br />") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>QueryString信息</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# (Eval("QueryString")??"").ToString().Replace("\r\n","<br />") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>Form信息</td>
                                    <td style='border: #000000 1px solid;' width='800'><%#  (Eval("Form")??"").ToString().Replace("\r\n","<br />") %></td>
                                </tr>
                                <tr>
                                    <td style='border: #000000 1px solid;' valign='middle' width='120'>Cookies信息</td>
                                    <td style='border: #000000 1px solid;' width='800'><%# (Eval("Cookies")??"").ToString().Replace("\r\n","<br />") %></td>
                                </tr>
                            </tbody>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</body>
</html>