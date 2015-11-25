<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmissionsYearSetting.aspx.cs" Inherits="ReportingServices.EmissionsYearSetting" MasterPageFile="~/Site1.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" href="JsPlug/AmazeUI/css/amazeui.min.css" />
    <link rel="stylesheet" href="JsPlug/AmazeUI/css/admin.css" />
    <script type="text/javascript" src="JsPlug/AmazeUI/js/amazeui.min.js"></script>
</asp:Content>

<asp:Content ID="content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        DateTime now = DateTime.Now;
        DateTime d1 = new DateTime(now.Year, now.Month, 1);

        DateTime d2 = d1.AddMonths(1).AddDays(-1);
         %>
    <div style="min-width: 1000px">
        <p>参数：[pi:10DEH03MCS]小时数据 [pi:10DEH03MCS_MONTH]月数据累计 [pi:10DEH03MCS_MONTHAVG]月数据平均数</p>
        
        <div class="am-g am-g-fixed" style="margin-bottom: 10px; margin-left: -16px">
            <div class="am-u-sm-3">
                <input type="text" class="am-form-field" placeholder="更新开始时间" id="update-start" value="<%=d1.ToString("yyyy-MM-dd") %>" readonly />
            </div>
            <div class="am-u-sm-3">
                <input type="text" class="am-form-field" placeholder="更新结束时间" id="update-end" value="<%=d2.ToString("yyyy-MM-dd") %>" readonly />
            </div>
            <div class="am-u-sm-3"></div>
            <div class="am-u-sm-3"></div>
        </div>
        <table class="am-table am-table-bordered am-table-centered">
            <tr>
                <th>名称</th>
                <th>公式</th>
                <th>备注</th>
                <th>操作</th>
            </tr>
            <asp:Repeater runat="server" ID="rpt_table" DataSourceID="SqlDataSource1">
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("Name") %><input type="hidden" name="Id" value='<%#Eval("Id") %>' /></td>
                        <td>
                            <textarea name="Formula" disabled="disabled" cols="40" rows="2"><%# Eval("Formula") %></textarea>
                        </td>
                        <td>
                            <textarea name="Remarks" disabled="disabled" cols="40" rows="2"><%# Eval("Remarks") %></textarea>
                        </td>
                        <td>
                            <div class="am-btn-group am-btn-group-xs">
                                <button name="btn-edit" class="am-btn am-btn-secondary am-center">编辑</button>
                                <button class="am-btn am-btn-success am-radius btn-setting" style="display: none" type="button">配置</button>
                                <button class="am-btn am-btn-primary am-radius btn-updateData" type="button">更新数据</button>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <asp:SqlDataSource runat="server" ID="SqlDataSource1" ConnectionString='<%$ ConnectionStrings:dbconn %>' SelectCommand="SELECT 
      [Id]
      ,[Name]
      ,[Formula]
      ,[Remarks]
      ,[UpdateTime]
      ,[DataUpdateTime]
  FROM [Center].[dbo].[tn_FormulaSetting] ORDER BY Display"></asp:SqlDataSource>
    </div>




    <!-- modal开始 -->
    <div class="am-modal am-modal-alert" tabindex="-1" id="alert">
        <div class="am-modal-dialog">
            <div class="am-modal-hd">保存成功</div>
            <%--<div class="am-modal-bd">
                Hello world！
            </div>--%>
            <div class="am-modal-footer">
                <span class="am-modal-btn">确定</span>
            </div>
        </div>
    </div>

    <div class="am-modal am-modal-loading am-modal-no-btn" tabindex="-1" id="modal-loading">
        <div class="am-modal-dialog">
            <div class="am-modal-hd">正在保存...</div>
            <div class="am-modal-bd">
                <span class="am-icon-spinner am-icon-spin"></span>
            </div>
        </div>
    </div>

    <div class="am-modal am-modal-confirm" tabindex="-1" id="my-confirm">
        <div class="am-modal-dialog">
            <div class="am-modal-hd">你，确定要更新数据吗？</div>
            <div class="am-modal-bd">
            </div>
            <div class="am-modal-footer">
                <span class="am-modal-btn" data-am-modal-cancel>取消</span>
                <span class="am-modal-btn" data-am-modal-confirm>确定</span>
            </div>
        </div>
    </div>
    <!-- modal结束 -->


    <script type="text/javascript">
        $(function () {
            $alert = $("#alert");
            $modal = $("#modal-loading");
            $updateStart = $("#update-start");
            $updateEnd = $("#update-end");
            var nowTemp = new Date();
            var now = new Date(1990, 1, 1, 0, 0, 0, 0);
            var checkin = $updateStart.datepicker({
                onRender: function (date) {
                    return date.valueOf() < now.valueOf() ? 'am-disabled' : '';
                }
            }).on('changeDate.datepicker.amui', function (ev) {
                if (ev.date.valueOf() > checkout.date.valueOf()) {
                    var newDate = new Date(ev.date)
                    //newDate.setDate(newDate.getDate() + 1);
                    newDate.setMonth(newDate.getMonth() + 1);
                    checkout.setValue(newDate);
                }
                checkin.close();
                $updateEnd.focus();
            }).data('amui.datepicker');

            var checkout = $updateEnd.datepicker({
                onRender: function (date) {
                    return date.valueOf() <= checkin.date.valueOf() ? 'am-disabled' : '';
                }
            }).on('changeDate.datepicker.amui', function (ev) {
                checkout.close();
            }).data('amui.datepicker');


            $("button[name='btn-edit']").click(function () {
                $item = $(this).parents("tr").first();
                $item.find("textarea[name='Formula']").attr("disabled", false);
                $item.find("textarea[name='Remarks']").attr("disabled", false);
                $item.find(".btn-setting").show();
                $(this).hide();
                return false;
            })
            $(".btn-setting").click(function () {
                $item = $(this).parents("tr").first();
                id = $item.find("input[name='Id']").val();
                formula = $item.find("textarea[name='Formula']").val();
                remarks = $item.find("textarea[name='Remarks']").val();
                $.ajax({
                    type: 'POST',
                    url: '/api/FormulaAllocation/Post',
                    data: { id: id, formula: formula, remarks: remarks },
                    datatype: "json",
                    beforeSend: function () {
                        $alert.find(".am-modal-hd").text("正在保存...");
                        $modal.modal();
                    },
                    success: function (data) {
                        $alert.find(".am-modal-hd").text("保存成功");
                        $alert.modal();
                    },
                    complete: function (XMLHttpRequest, textStatus) {
                        $modal.modal('close');
                    },
                    error: function () {
                        alert('error')
                        $alert.find(".am-modal-hd").text("保存失败");
                        $alert.modal();
                        //请求出错处理
                    }
                });
                $item.find("textarea[name='Formula']").attr("disabled", true);
                $item.find("textarea[name='Remarks']").attr("disabled", true);
                $(this).hide();
                $item.find("button[name='btn-edit']").show();
                return false;
            })

            $(".btn-updateData").click(function () {
                $item = $(this).parents("tr").first();
                $('#my-confirm').modal({
                    relatedTarget: this,
                    onConfirm: function (options) {
                        id = $item.find("input[name='Id']").val();
                        updateStart = $updateStart.val();
                        updateEnd = $updateEnd.val();
                        $.ajax({
                            type: 'POST',
                            url: '/api/FormulaAllocation/UpdateData?id=' + id + "&updateStart=" + updateStart + "&updateEnd=" + updateEnd,
                            //data: { id: id},
                            datatype: "json",
                            beforeSend: function () {
                                $modal.find(".am-modal-hd").text("正在更新数据...");
                                $modal.modal();
                            },
                            success: function (data) {
                                if (data.state == 1)
                                    $alert.find(".am-modal-hd").text("更新成功");
                                else
                                    $alert.find(".am-modal-hd").text("更新失败");
                                $alert.modal();
                            },
                            complete: function (XMLHttpRequest, textStatus) {
                                $modal.modal('close');
                            },
                            error: function () {
                                alert('error')
                                $alert.find(".am-modal-hd").text("保存失败");
                                $alert.modal();
                                //请求出错处理
                            }
                        });

                    },
                    // closeOnConfirm: false,
                    onCancel: function () {

                    }
                });

                return false;
            });
        })
    </script>
</asp:Content>


