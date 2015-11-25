using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using Trirand.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Configuration;

using Envir_ExceptionGroup;

namespace ReportingServices
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService] 
    public class WSConfirm : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">所要提交的日数据日期</param>
        [WebMethod(Description = "提交日数据")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public opermsg submit(string datestr)
        {
            try
            {
                AppSettingsReader asr = new AppSettingsReader();
                string dbconn = (string)asr.GetValue("dbconn", typeof(string));
                using (SqlConnection conn = new SqlConnection(dbconn))
                {
                    conn.Open();
                    SqlCommand sqlcomm = new SqlCommand("insert into Exception_DayConfirm(timestamps,confirmed) values('" + datestr + "',1)", conn);
                    sqlcomm.ExecuteNonQuery();
                    conn.Close();
                }
                return new opermsg { status = 1, msg = "该日数据已提交" };
            }
            catch (Exception ex)
            {
                return new opermsg { status = 0, msg = ex.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datestr"></param>
        /// <returns></returns>
        [WebMethod(Description = "查看提交状态")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int checkdaysubmitstatus(string datestr)
        {
            DataSet ds = new DataSet();
            try
            {
                int count;
                AppSettingsReader asr = new AppSettingsReader();
                string dbconn = (string)asr.GetValue("dbconn", typeof(string));
                using (SqlConnection conn = new SqlConnection(dbconn))
                {
                    conn.Open();
                    SqlCommand sqlcomm = new SqlCommand("select count(*) from Exception_DayConfirm where timestamps='" + datestr + "'", conn);
                    count = (int)sqlcomm.ExecuteScalar();
                    conn.Close();
                }
                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="et"></param>
        /// <returns></returns>
        [WebMethod(Description = "小时均值分组操作")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int houravggroup(DateTime st, DateTime et)
        {
            try
            {
                System.Data.DataSet ds_expt = (new EnvExceptions()).GetEnvExceptionsDs(st, et);
                if (ds_expt != null)
                {
                    foreach (System.Data.DataRow dr_expt in ds_expt.Tables[0].Rows)
                    {
                        long envid = long.Parse(dr_expt["id"].ToString());
                        (new Exception_Group()).DeleteExceptionGroupData(envid);
                        (new Exception_Rulelog()).DeleteMatchData(envid);

                        //NOx
                        if (int.Parse(dr_expt["indicatorid"].ToString()) == 3)
                        {                          
                            //machine start and stop,typeid:15
                            int? startstopcount = (new Exception_Group()).GetStartStopCount(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (startstopcount > 0)
                            {
                                System.Data.DataSet ds_ss = (new Exception_Group()).GetStartStopDs(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_ss != null)
                                {
                                    foreach (System.Data.DataRow dr_ss in ds_ss.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_ss["id"].ToString()), 15);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 0);
                                continue;
                            }

                            //calibration,typeid:3
                            int? calibcount = (new Exception_Group()).GetCalibRelatedCount(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (calibcount > 0)
                            {
                                System.Data.DataSet ds_calib = (new Exception_Group()).GetCalibRelatedDs(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_calib != null)
                                {
                                    foreach (System.Data.DataRow dr_calib in ds_calib.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_calib["id"].ToString()), 3);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 0);
                                continue;
                            }

                            //scr,typeid:1
                            int? scrcount = (new Exception_Group()).GetScrRelatedCount(DateTime.Parse(dr_expt["timestamps"].ToString()), 3, 3, int.Parse(dr_expt["pointid"].ToString()));
                            if (scrcount > 0)
                            {
                                System.Data.DataSet ds_scr = (new Exception_Group()).GetScrRelatedDs(DateTime.Parse(dr_expt["timestamps"].ToString()), 3/*offset hour*/, 3, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_scr != null)
                                {
                                    foreach (System.Data.DataRow dr_scr in ds_scr.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_scr["id"].ToString()), 1);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 1, "", 0);
                                continue;
                            }

                            //special condition,typeid:2
                            int? sconcount = (new Exception_Group()).GetSconRelatedCount(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (sconcount > 0)
                            {
                                System.Data.DataSet ds_scon = (new Exception_Group()).GetSconRelatedDs(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_scon != null)
                                {
                                    foreach (System.Data.DataRow dr_scon in ds_scon.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_scon["id"].ToString()), 2);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 2, "", 0);
                                continue;
                            }
                            #region
                            //获取记录数                                                  
                            //if ((scrcount != null) && (sconcount != null) && (calibcount != null) && (startstopcount != null))
                            //{
                            //    //如果有两类及以上关联数据
                            //    if (((scrcount > 0) && (sconcount > 0)) || ((scrcount > 0) && (calibcount > 0)) || ((scrcount > 0) && (startstopcount > 0)) || ((sconcount > 0) && (calibcount > 0)) || ((sconcount > 0) && (startstopcount > 0)) || ((startstopcount > 0) && (calibcount > 0)))
                            //    {
                            //        if (startstopcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 1);
                            //            continue;
                            //        }
                            //        if (scrcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 1, "", 1);
                            //            continue;
                            //        }
                            //        if (sconcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 2, "", 1);
                            //            continue;
                            //        }
                            //        if (calibcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 1);
                            //            continue;
                            //        }
                            //    }
                            //    //有一类关联数据
                            //    if (scrcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 1, "", 0);
                            //        continue;
                            //    }
                            //    if (sconcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 2, "", 0);
                            //        continue;
                            //    }
                            //    if (calibcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 0);
                            //        continue;
                            //    }
                            //    if (startstopcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 0);
                            //        continue;
                            //    }
                            //}
                            #endregion
                        }
                        //SO2
                        if (int.Parse(dr_expt["indicatorid"].ToString()) == 1)
                        {
                            #region
                            //DataTable dt = new DataTable();
                            //dt.Columns.Add("a");
                            //DataRow dr = dt.NewRow();
                            //dr["a"] = "xyz";
                            //dt.Rows.Add(dr);        
                            #endregion
                            //long envid = long.Parse(dr_expt["id"].ToString());
                            #region
                            //fgd,typeid:16
                            //int? fgdcount = (new Exception_Group()).GetFGDRelatedCount(DateTime.Parse(dr_expt["timestamps"].ToString()), 3, 3, int.Parse(dr_expt["pointid"].ToString()));
                            //if (fgdcount > 0)
                            //{
                            //    System.Data.DataSet ds_fgd = (new Exception_Group()).GetFGDRelatedDs(DateTime.Parse(dr_expt["timestamps"].ToString()), 3/*offset hour*/, 3, int.Parse(dr_expt["pointid"].ToString()));
                            //    if (ds_fgd != null)
                            //    {
                            //        foreach (System.Data.DataRow dr_fgd in ds_fgd.Tables[0].Rows)
                            //        {
                            //            (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_fgd["id"].ToString()), 16);
                            //        }
                            //    }
                            //}
                            #endregion

                            //machine start and stop,typeid:15
                            int? startstopcount = (new Exception_Group()).GetStartStopCount_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (startstopcount > 0)
                            {
                                System.Data.DataSet ds_ss = (new Exception_Group()).GetStartStopDs_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_ss != null)
                                {
                                    foreach (System.Data.DataRow dr_ss in ds_ss.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_ss["id"].ToString()), 15);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 0);
                                continue;
                            }

                            //calibration,typeid:3
                            int? calibcount = (new Exception_Group()).GetCalibRelatedCount_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (calibcount > 0)
                            {
                                System.Data.DataSet ds_calib = (new Exception_Group()).GetCalibRelatedDs_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_calib != null)
                                {
                                    foreach (System.Data.DataRow dr_calib in ds_calib.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_calib["id"].ToString()), 3);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 0);
                                continue;
                            }
                            //special condition,typeid:2
                            int? sconcount = (new Exception_Group()).GetSconRelatedCount_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (sconcount > 0)
                            {
                                System.Data.DataSet ds_scon = (new Exception_Group()).GetSconRelatedDs_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_scon != null)
                                {
                                    foreach (System.Data.DataRow dr_scon in ds_scon.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_scon["id"].ToString()), 2);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 2, "", 0);
                                continue;
                            }
                            #region
                            //获取记录数
                                                      
                            //if ((fgdcount != null) && (sconcount != null) && (calibcount != null) && (startstopcount != null))
                            //{
                            //    //如果有两类及以上关联数据
                            //    if (((fgdcount > 0) && (sconcount > 0)) || ((fgdcount > 0) && (calibcount > 0)) || ((fgdcount > 0) && (startstopcount > 0)) || ((sconcount > 0) && (calibcount > 0)) || ((sconcount > 0) && (startstopcount > 0)) || ((startstopcount > 0) && (calibcount > 0)))
                            //    {
                            //        if (startstopcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 1);
                            //            continue;
                            //        }
                            //        if (fgdcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 16, "", 1);
                            //            continue;
                            //        }
                            //        if (sconcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 2, "", 1);
                            //            continue;
                            //        }
                            //        if (calibcount > 0)
                            //        {
                            //            (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 1);
                            //            continue;
                            //        }
                            //    }
                            //    //有一类关联数据
                            //    if (fgdcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 16, "", 0);
                            //        continue;
                            //    }
                            //    if (sconcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 2, "", 0);
                            //        continue;
                            //    }
                            //    if (calibcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 0);
                            //        continue;
                            //    }
                            //    if (startstopcount > 0)
                            //    {
                            //        (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 0);
                            //        continue;
                            //    }
                            //}
                            #endregion
                        }
                        //Dust
                        if (int.Parse(dr_expt["indicatorid"].ToString()) == 2)
                        {
                            //machine start and stop,typeid:15
                            int? startstopcount = (new Exception_Group()).GetStartStopCount_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (startstopcount > 0)
                            {
                                System.Data.DataSet ds_ss = (new Exception_Group()).GetStartStopDs_FGD(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_ss != null)
                                {
                                    foreach (System.Data.DataRow dr_ss in ds_ss.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_ss["id"].ToString()), 15);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 15, "", 0);
                                continue;
                            }

                            //calib type 3
                            int? calibcount = (new Exception_Group()).GetCalibRelatedCount_DUST(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                            if (calibcount > 0)
                            {
                                System.Data.DataSet ds_calib = (new Exception_Group()).GetCalibRelatedDs_DUST(DateTime.Parse(dr_expt["timestamps"].ToString()), 2, 2, int.Parse(dr_expt["pointid"].ToString()));
                                if (ds_calib != null)
                                {
                                    foreach (System.Data.DataRow dr_calib in ds_calib.Tables[0].Rows)
                                    {
                                        (new Exception_Rulelog()).AddMatchData(envid, long.Parse(dr_calib["id"].ToString()), 3);
                                    }
                                }
                                (new Exception_Group()).AddExceptionGroupData(envid, 3, "", 0);
                                continue;
                            }
                        }
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

    }
}
