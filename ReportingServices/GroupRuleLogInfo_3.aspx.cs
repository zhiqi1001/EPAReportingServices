﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;

//database
using System.Data.SqlClient;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ReportingServices
{
    public partial class GroupRuleLogInfo_3 : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["authority"] != "3" && Session["ManagementFlowAuthorityCtl"] == "1")
                {
                    Response.Redirect("logon.aspx");
                }
                if ((ed.Text == "") && (sd.Text == ""))
                {
                    DataSet ds;
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("select starttime,endtime from dynamicschedule where ");
                        sb.Append("starttime <= getdate() and ");
                        sb.Append("endtime > getdate()");
                        Database db = DatabaseFactory.CreateDatabase("dbconn");
                        System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                        ds = db.ExecuteDataSet(dbc);

                        ed.Text = DateTime.Parse(ds.Tables[0].Rows[0]["endtime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        sd.Text = DateTime.Parse(ds.Tables[0].Rows[0]["starttime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        DateTime enddate = DateTime.Now;
                        ed.Text = enddate.ToString("yyyy-MM-dd HH:mm:ss");
                        sd.Text = enddate.AddHours(-8).ToString("yyyy-MM-dd HH:mm:ss");
                    }    
                }

                PointsDownList.Items.Clear();
                DataSet ds2;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("select * from point_machine_map where pointname not like '%SIM%'");
                    Database db = DatabaseFactory.CreateDatabase("dbconn");
                    System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                    ds2 = db.ExecuteDataSet(dbc);

                    foreach (DataRow dr in ds2.Tables[0].Rows)
                    {
                        PointsDownList.Items.Add(new ListItem(dr["description"].ToString(), dr["pointname"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    ds2 = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_rptqry_Click(object sender, EventArgs e)
        {
            Session["sd_GroupRuleLogInfo_3"] = sd.Text;
            Session["ed_GroupRuleLogInfo_3"] = ed.Text;
            //add 2015-05-05
            Session["machineid_GroupRuleLogInfo_3"] = machine.SelectedItem.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Jqgrid1_DataRequesting(object sender, Trirand.Web.UI.WebControls.JQGridDataRequestEventArgs e)
        {
            string ssd = (string)Session["sd_GroupRuleLogInfo_3"];
            string sed = (string)Session["ed_GroupRuleLogInfo_3"];
            string machineid = (string)Session["machineid_GroupRuleLogInfo_3"];
            if ((ssd == null) || (sed == null) || (machineid == null))
            {
                ssd = sd.Text;
                sed = ed.Text;
                machineid = machine.SelectedItem.Value;
            }
            if ((ssd != null) && (sed != null) && (ssd != "") && (sed != ""))
            {
                DataSet ds;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("select t.id,iif(s.description is null,t.rulename,s.description) as rulename,t.alarmlog,t.timelog,t.timelogend,t.alarmdis,t.cemstype,iif(t.confirmedgroup is null, t.alarmlog,t.confirmedgroup) as confirmedgroup,iif(t.validatedgroup is null,iif(t.confirmedgroup is null, t.alarmlog,t.confirmedgroup),t.validatedgroup) as validatedgroup ,iif(k.statusname is null,'未确认',k.statusname) as groupstatus,s.machineid from t_RulelogS t left join Point_Machine_Map s on t.RuleName = s.pointname left join grouprulestatus k on t.groupstatus=k.id where ");
                    sb.Append("t.timelog <= '" + sed + "' and ");
                    sb.Append("t.timelog >= '" + ssd + "' and (t.cemstype='FGD' or t.cemstype='DUST')  and s.machineid=" + machineid + "  order by t.timelog");
                    Database db = DatabaseFactory.CreateDatabase("dbconn");
                    System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                    ds = db.ExecuteDataSet(dbc);
                }
                catch (Exception ex)
                {
                    ds = null;
                }
                #region
                //if (!CheckBox_validvalue.Checked)
                //{
                //    Jqgrid1.Columns[5].Width = 0;
                //}
                //if (!CheckBox_load.Checked)
                //{
                //    Jqgrid1.Columns[6].Visible = false;
                //}
                #endregion
                Jqgrid1.DataSource = ds.Tables[0];
                Jqgrid1.DataBind();
                //Session["gridFilterPageState_exceptioninfo"] = Jqgrid1.GetState();
            }
        }

        /// <summary>
        /// edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Jqgrid1_RowEditing(object sender, Trirand.Web.UI.WebControls.JQGridRowEditEventArgs e)
        {
            try
            {
                //get confirmed group text and validated group text
                string cg = e.RowData["confirmedgroup"];
               // string vg = e.RowData["validatedgroup"];
                string cgt = GroupType_ddl.Items.FindByValue(cg).Text;
               // string vgt = GroupType_ddl.Items.FindByValue(vg).Text;

                string dis = e.RowData["alarmdis"];

                //get  group status
                string gs = e.RowData["groupstatus"];

                //get row key
                string id = e.RowKey;

                StringBuilder sb = new StringBuilder();
                sb.Append("update t_rulelogs set confirmedgroup = '" + cgt + "', validatedgroup='" + cgt + "', groupstatus=" + gs + ", alarmdis='" + dis + "', confirmeduser='单元长', validateduser='专工', confirmedtime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', validatedtime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where id = " + id);
                Database db = DatabaseFactory.CreateDatabase("dbconn");
                System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                db.ExecuteNonQuery(dbc);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                //string a = "s";
            }
        }

        /// <summary>
        /// cell binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Jqgrid1_CellBinding(object sender, Trirand.Web.UI.WebControls.JQGridCellBindEventArgs e)
        {
            string editLink = "<a onclick='javascript:jQuery(\"#{0}\").editRow(\"{1}\")' href=\"#\"><span class=\"str\">编辑</span></a>";
            string saveLink = "<a onclick='javascript:jQuery(\"#{0}\").saveRow(\"{1}\")' href=\"#\"><span class=\"str\">保存</span></a>";
            string cancelLink = "<a onclick='javascript:jQuery(\"#{0}\").restoreRow(\"{1}\")' href=\"#\"><span class=\"str\">取消</span></a>";

            if (e.RowValues[9].ToString() != "已审核")
            {
                if (e.ColumnIndex == 11) // edit
                {
                    e.CellHtml = String.Format(editLink, Jqgrid1.ClientID, e.RowKey);
                }
                if (e.ColumnIndex == 12) // save
                {
                    e.CellHtml = String.Format(saveLink, Jqgrid1.ClientID, e.RowKey);
                }
                if (e.ColumnIndex == 13) // cancel
                {
                    e.CellHtml = String.Format(cancelLink, Jqgrid1.ClientID, e.RowKey);
                }
            }
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Jqgrid1_RowAdding(object sender, Trirand.Web.UI.WebControls.JQGridRowAddEventArgs e)
        {
            try
            {
                string rn = e.RowData["rulename"];
                string tl = e.RowData["timelog"];
                string ad = e.RowData["alarmdis"];
                string ct = e.RowData["cemstype"];
                string cg = e.RowData["confirmedgroup"];
                //string vg = e.RowData["validatedgroup"];
                string gs = e.RowData["groupstatus"];

                string cgt = GroupType_ddl.Items.FindByValue(cg).Text;
                //string vgt = GroupType_ddl.Items.FindByValue(vg).Text;

                StringBuilder sb = new StringBuilder();
                sb.Append("insert into t_rulelogs(rulename,timelog,alarmdis,cemstype,confirmedgroup,validatedgroup,groupstatus,confirmeduser,validateduser,edittime,confirmedtime,validatedtime) values('");
                sb.Append(rn);
                sb.Append("','");
                sb.Append(tl);
                sb.Append("','");
                sb.Append(ad);
                sb.Append("','");
                sb.Append(ct);
                sb.Append("','");
                sb.Append(cgt);
                sb.Append("','");
                sb.Append(cgt);
                sb.Append("',");
                sb.Append(gs);
                sb.Append(",'单元长','专工','");
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("','");
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("','");
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("')");

                Database db = DatabaseFactory.CreateDatabase("dbconn");
                System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
            }

        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Jqgrid1_RowDeleting(object sender, Trirand.Web.UI.WebControls.JQGridRowDeleteEventArgs e)
        {
            try
            {
                string id = e.RowKey;

                StringBuilder sb = new StringBuilder();
                sb.Append("delete from t_rulelogs where id = " + id);
                Database db = DatabaseFactory.CreateDatabase("dbconn");
                System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {

            }
        }

        protected void btn_validate_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Export_Click(object sender, EventArgs e)
        {
            Trirand.Web.UI.WebControls.JQGridState gridState = Session["gridFilterPageState_exceptioninfo"] as Trirand.Web.UI.WebControls.JQGridState;
            Jqgrid1.ExportSettings.ExportDataRange = Trirand.Web.UI.WebControls.ExportDataRange.Filtered;
            Jqgrid1.ExportToExcel("export.xls", gridState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ChartDetail_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Jqgrid1.SelectedRow))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>$.messager.alert('提示','未选中一行日志明细');</script>");
                return;
            }
            //DateTime timestamps;
            string st = null, et = null, pn = null, pt = null, mi = null;
            try
            {
                DataSet ds;
                StringBuilder sb = new StringBuilder();
                sb.Append("select t.timelog,t.cemstype,t.rulename,s.machineid from t_rulelogs t inner join point_machine_map s on t.rulename=s.pointname where ");
                sb.Append("t.id = " + Jqgrid1.SelectedRow);
                Database db = DatabaseFactory.CreateDatabase("dbconn");
                System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                //timestamps = DateTime.Parse(db.ExecuteScalar(dbc).ToString());
                ds = db.ExecuteDataSet(dbc);
                st = DateTime.Parse(ds.Tables[0].Rows[0]["timelog"].ToString()).AddHours(-3).ToString("yyyy-MM-dd HH:mm:ss");
                et = DateTime.Parse(ds.Tables[0].Rows[0]["timelog"].ToString()).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss");
                pn = ds.Tables[0].Rows[0]["rulename"].ToString();
                mi = ds.Tables[0].Rows[0]["machineid"].ToString();
                if (ds.Tables[0].Rows[0]["cemstype"].ToString() == "SCR")
                {
                    pt = "nox";
                }
                else if (ds.Tables[0].Rows[0]["cemstype"].ToString() == "FGD")
                {
                    pt = "so2";
                }
                else if (ds.Tables[0].Rows[0]["cemstype"].ToString() == "DUST")
                {
                    pt = "dust";
                }
            }
            catch (Exception ex)
            {
                return;
            }
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "detail chart", "window.open('http://10.150.124.140:8088?xyz=iahbhy&tagtypeid=3&TimeFrom=" + timestamps.AddHours(-3.0).ToString("yyyy/MM/dd HH:mm:ss") + "&TimeTo=" + timestamps.AddHours(3.0).ToString("yyyy/MM/dd HH:mm:ss") + "')", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "detail chart", "window.open('reports_client.aspx?pname=" + pn + "&pointtype=" + pt + "&machineid=" + mi + "&starttime=" + st + "&endtime=" + et + "')", true);
        }
    }
}