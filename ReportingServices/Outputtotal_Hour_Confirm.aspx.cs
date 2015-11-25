using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

using System.Text;

using System.Data.SqlClient;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ReportingServices
{
    /// <summary>
    /// for trend chart
    /// </summary>
    public partial class Outputtotal_Hour_Confirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string times = null,/* compare_item = null, mi = null,*/ pn = null;
                foreach (string k in Request.QueryString.AllKeys)
                {
                    if (k.ToLower() == "timestamps")
                    {
                        times = Request.QueryString[k];
                    }
                    else if (k.ToLower()== "pname")
                    {
                        pn = Request.QueryString[k];
                    }
                    //else if (k == "CompareItem")
                    //{
                    //    compare_item = Request.QueryString["CompareItem"];
                    //}
                    //else if (k == "MachineId")
                    //{
                    //    mi = Request.QueryString["MachineId"];
                    //}
                }

                if ((ts != null) && (pn != null))
                {
                    DataSet ds;
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("select c.pname,c.ptype,c.seq,c.ud1,c.ud2,c.ud3,c.ud5,c.aggregatetype,v.acceptoffset,v.comment,b.pvalue,b.timestamps, iif(d.machineid is null, c.machineid, d.machineid) as machineid, iif(c.des is null, d.description, c.des) as des, v.compareitem_type, v.id, v.statis_selection from outputtotalconfig c ");
                        sb.Append(" left join (select * from OutputDataCombined where timestamps='" + times + "') b on c.pname=b.pname left join point_machine_map d on c.pname = d.pointname left join Output_Total_Hour_BiasConfirm v on c.ud5 = v.compareitem_type and b.timestamps = v.timestamps and v.machineid = iif(d.machineid is null, c.machineid, d.machineid) ");
                        sb.Append(" where c.pname='" + pn + "'");
                        Database db = DatabaseFactory.CreateDatabase("dbconn");
                        System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                        ds = db.ExecuteDataSet(dbc);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        output_submit.Enabled = false;
                        status.InnerText = ex.Message.ToString();
                        return;
                    }

                    ts.Text = ds.Tables[0].Rows[0]["timestamps"].ToString();
                    mi.Text = ds.Tables[0].Rows[0]["machineid"].ToString();
                    mtype.SelectedValue = ds.Tables[0].Rows[0]["ud5"].ToString();
                    comment_t.Text = ds.Tables[0].Rows[0]["comment"].ToString();
                    //mtype.SelectedValue = "2";
                    if (ds.Tables[0].Rows[0]["acceptoffset"] == null || ds.Tables[0].Rows[0]["acceptoffset"].ToString() == "")
                    {
                        acceptoffset_d.SelectedValue = "-1";
                    }
                    else
                        acceptoffset_d.SelectedValue = ds.Tables[0].Rows[0]["acceptoffset"].ToString();

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void output_submit_Click(object sender, EventArgs e)
        {
            if (ts.Text != "" && mi.Text != "")
            {
                DataSet ds;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("select count(*) from Output_Total_Hour_BiasConfirm where machineid=" + mi.Text);
                    sb.Append(" and compareitem_type="+mtype.SelectedValue.ToString());
                    sb.Append(" and timestamps='" + ts.Text.ToString() + "'");
                    Database db = DatabaseFactory.CreateDatabase("dbconn");
                    System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                    ds = db.ExecuteDataSet(dbc);
                }
                catch (Exception ex)
                {
                    ds = null;
                    status.InnerText = ex.Message.ToString();
                    return;
                }

                try
                {
                    if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) == 0)
                    {
                        if (acceptoffset_d.SelectedValue != "-1")
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("insert into Output_Total_Hour_BiasConfirm(machineid,compareitem_type,timestamps,comment,statis_selection,acceptoffset) values(");
                            sb.Append(mi.Text + "," + mtype.SelectedValue + ",'" + ts.Text + "','" + comment_t.Text + "',1," + acceptoffset_d.Text + ")");
                            Database db = DatabaseFactory.CreateDatabase("dbconn");
                            System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                            db.ExecuteNonQuery(dbc);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("insert into Output_Total_Hour_BiasConfirm(machineid,compareitem_type,timestamps,comment,statis_selection) values(");
                            sb.Append(mi.Text + "," + mtype.SelectedValue + ",'" + ts.Text + "','" + comment_t.Text + "',1" + ")");
                            Database db = DatabaseFactory.CreateDatabase("dbconn");
                            System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                            db.ExecuteNonQuery(dbc);
                        }
                    }
                    else
                    {
                        if (acceptoffset_d.SelectedValue != "-1")
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("update Output_Total_Hour_BiasConfirm set comment='" + comment_t.Text + "',statis_selection=1,acceptoffset=" + acceptoffset_d.SelectedValue);
                            sb.Append(" where machineid = " + mi.Text);
                            sb.Append(" and compareitem_type=" + mtype.SelectedValue.ToString());
                            sb.Append(" and timestamps='" + ts.Text.ToString() + "'");
                            Database db = DatabaseFactory.CreateDatabase("dbconn");
                            System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                            db.ExecuteNonQuery(dbc);
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("update Output_Total_Hour_BiasConfirm set comment='" + comment_t.Text + "',statis_selection=1,acceptoffset=NULL");
                            sb.Append(" where machineid = " + mi.Text);
                            sb.Append(" and compareitem_type=" + mtype.SelectedValue.ToString());
                            sb.Append(" and timestamps='" + ts.Text.ToString() + "'");
                            Database db = DatabaseFactory.CreateDatabase("dbconn");
                            System.Data.Common.DbCommand dbc = db.GetSqlStringCommand(sb.ToString());
                            db.ExecuteNonQuery(dbc);
                        }
                    }
                }
                catch(Exception ex)
                {
                    status.InnerText = ex.Message.ToString();
                    return;      
                }
                status.InnerText = "操作成功";

            }
        }
    }
}