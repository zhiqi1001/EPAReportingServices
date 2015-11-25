
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Outputtotal_Hour_Confirm.aspx.cs" Inherits="ReportingServices.Outputtotal_Hour_Confirm"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <script src="script/jquery-2.1.3.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" media="screen" href="themes/jquery-ui.css"/>
    <link rel="stylesheet" type="text/css" media="screen" href="themes/ui.jqgrid.css" />

    <script src="js/trirand/i18n/grid.locale-cn.js" type="text/javascript"></script>
    <script src="js/trirand/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="js/trirand/jquery.jqDatePicker.min.js" type="text/javascript"></script>
    <script src="js/trirand/jquery.jqMultiSelect.min.js" type="text/javascript"></script>
    <script src="js/trirand/jquery.jqDropDownList.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="easyui/themes/default/easyui.css"/>
    <link rel="stylesheet" type="text/css" href="easyui/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="easyui/demo/demo.css"/>
    <script type="text/javascript" src="easyui/jquery.easyui.min.js"></script>
    <title>小时排放数据</title>
</head>
<body style="background-color:silver">
            <script type="text/javascript">
                function refreshclick() {                  
                    $("input[name='ctl00$ContentPlaceHolder1$ReportViewer1$ctl05$ctl05$ctl00$ctl00$ctl00']",window.opener.document).click();
                }
		        function closeevent(){
			        window.onbeforeunload=refreshclick();
                }
                
            </script>
    <div>
        <form runat="server">
            <label>时间:</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox runat="server" ID="ts" Enabled="false"></asp:TextBox> <br /><br />
            <label>机组:</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox runat="server" ID="mi" Enabled="false"></asp:TextBox> <br /><br />
            <label>类型:</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList runat="server" ID="mtype" Enabled="false">
                <asp:ListItem Text="NOx" Value="1"></asp:ListItem>
                <asp:ListItem Text="SO2" Value="2"></asp:ListItem>
                <asp:ListItem Text="FLOW" Value="3"></asp:ListItem>
              </asp:DropDownList> <br /><br />
            <label>偏差处理:</label>&nbsp;&nbsp;
            <asp:DropDownList runat="server" ID="acceptoffset_d">
                <asp:ListItem Text="接受实测值" Value="1"></asp:ListItem>
                <asp:ListItem Text="接受物料平衡值" Value="0"></asp:ListItem>
                <asp:ListItem Text="隐式接受实测值" Value="-1"></asp:ListItem>
              </asp:DropDownList> <br /><br />
            <label>说明:</label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox runat="server" ID="comment_t" TextMode="MultiLine"></asp:TextBox>
            <br /><br /><br /><br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="output_submit" OnClick="output_submit_Click" runat="server" Text="确定" /><br /><br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<label id="status" runat="server" style="color:green"></label>
        </form>
    </div>
    <script type="text/javascript">
        $.ready(closeevent());
    </script>
</body>
</html>