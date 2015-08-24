<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin.Default" %>

<%@ Register TagPrefix="HJ" TagName="Nav" Src="~/Controls/HeaderNav.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>异常日志报警系统</title>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="" />
    <script src="static/js/jquery.1.9.1.min.js"></script>
    <link rel="stylesheet" href="/static/css/bootstrap.min.css" />
    <script src="static/js/bootstrap.min.js"></script>
    <script src="Static/js/asset/js/esl/esl.js"></script>
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

    <script>
        function GetOptionStr(str) {
            var ops = str.split(',');
            var options = "";
            for (var i = 0; i < ops.length; i++) {
                options += "<option value='" + ops[i] + "'>" + ops[i] + "</option>";
            }
            return options;
        }
        function selectchange() {
            historycharts.clear();
            currentsitename = $("#siteinfo").val();
            InitCharts();
        }
    </script>
</head>
<body>
    <HJ:Nav ID="Nav" runat="server" />
    <div class="container">
        <ol class="breadcrumb">
            <li><a href="/">异常日志报警系统</a></li>
            <li class="active">报表</li>
            &nbsp;&nbsp;&nbsp;
            <select id="siteinfo" onchange="selectchange()" class="breadcrumb" style="padding: 0; margin: 0; width: 120px">
            </select>
            <input type="hidden" id="hid_sitename" runat="server" />
        </ol>
    </div>
    <div id="historycharts" class="container" style="height: 400px; width: 70%;"></div>


    <script type="text/javascript">

        var historycharts;
        var historychartsoption;

        var currentsitename;

        // 路径配置
        require.config({
            paths: {
                'echarts': 'Static/js/asset/js/esl/echarts',
                'echarts/chart/bar': 'Static/js/asset/js/esl/echarts'
            }
        });

        // 使用
        require(
            [
                'echarts',
                'echarts/chart/bar' // 使用柱状图就加载bar模块，按需加载
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                historycharts = ec.init(document.getElementById('historycharts'));
                historychartsoption = {
                    title: {
                        text: '站点异常量',
                        x: 'left',
                        textAlign:'left'
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: ['Error']
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            dataZoom: { show: true },
                            magicType: { show: true, type: ['line', 'bar'] },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: ['0时', '1时', '2时', '3时', '4时', '5时', '6时', '7时', '8时', '9时', '10时', '11时', '12时', '13时', '14时', '15时', '16时', '17时', '18时', '19时', '20时', '21时', '22时', '23时']
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',
                            scale: true,
                            name: "条",
                            splitArea: { show: true }
                        }
                    ],
                    series: [
                        {
                            name: 'Error',
                            type: 'line',
                        }
                    ]
                };
                $("#siteinfo").append(GetOptionStr($("#hid_sitename").val()));
                currentsitename = $("#siteinfo").val();
                InitCharts();

            });


        function InitCharts() {
            historycharts.showLoading({
                text: '正在加载',
                effect: 'dynamicLine',
                textStyle: {
                    fontSize: 20
                }
            });

            $.get("Report.ashx?WebName=" + currentsitename, function (result) {

                historychartsoption.series[0].data = JSON.parse("[" + result + "]");
                historycharts.hideLoading();
                historycharts.setOption(historychartsoption);
            });
        }
    </script>
</body>
</html>
