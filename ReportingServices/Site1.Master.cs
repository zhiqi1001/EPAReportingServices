using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

 using DevExpress.Web.ASPxTreeView;


namespace WebApplication1
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        //private bool nodeclicked = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckAuthority();
                
                //set management flow authority signal
                //Session["ManagementFlowAuthorityCtl"] = "0";

                TreeViewNode tvn2 = new TreeViewNode("浙能集团");
                TreeViewNode tvn = new TreeViewNode("乐清电厂");
                TreeViewNode tvn_wz = new TreeViewNode("温州电厂");

                //TreeViewNode tvn_icemsgroup = new TreeViewNode("环保工况信息管理流");
                //TreeViewNode tvn_minimum = new TreeViewNode("环保超低排放管理");
                //TreeViewNode tvn_overlimits = new TreeViewNode("月超限值明细分析");
                //TreeViewNode tvn_overlimits_wz = new TreeViewNode("月超限值明细分析");
                TreeViewNode tvn_VerificationElectricityPriceSubsidy = new TreeViewNode("火电环保排污电价核实");
                TreeViewNode tvn_VerificationElectricityPriceSubsidy_wz = new TreeViewNode("火电环保排污电价核实");
                TreeViewNode tvn_SewageStatisticsVerification = new TreeViewNode("排污(减排)统计核实");

                //tvn_fgdscr.Nodes.Add("脱硝工况分析", "scr_startstop_ab", null, "~/scr_startstop_ab.aspx");
                //tvn_fgdscr.Nodes.Add("脱硫工况分析", "machine_startstop", null, "~/machine_startstop.aspx");

                //yq
                tvn_VerificationElectricityPriceSubsidy.Nodes.Add("环保数据异常统计", "EnvStatisticYQ", null, "~/Reports2.aspx");
                tvn_VerificationElectricityPriceSubsidy.Nodes.Add("仪表标定及支撑材料", "EnvCalibSpanYQ", null, "~/CalibSpan.aspx");

                //for yq output
                tvn_SewageStatisticsVerification.Nodes.Add("年排放总量表", "Output_Year", null, "~/Output_Year.aspx");
                tvn_SewageStatisticsVerification.Nodes.Add("月排放总量表", "Output_Month", null, "~/Output_Month.aspx");
                tvn_SewageStatisticsVerification.Nodes.Add("日排放总量表", "Output_Day", null, "~/Output_Day.aspx");

                tvn_SewageStatisticsVerification.Nodes.Add("月NOx排放总量表", "Output_Month_SCR", null, "~/Output_Month_SCR.aspx");
                tvn_SewageStatisticsVerification.Nodes.Add("月SO2排放总量表", "Output_Month_SCR", null, "~/Output_Month_FGD.aspx");

                tvn_SewageStatisticsVerification.Nodes.Add("月减排衡算核准表", "Output_BalanceCheck_Month", null, "~/Output_BalanceCheck_Month.aspx");
                tvn_SewageStatisticsVerification.Nodes.Add("日减排衡算核准表", "Output_BalanceCheck_Day", null, "~/Output_BalanceCheck_Day.aspx");

                tvn_SewageStatisticsVerification.Nodes.Add("减排年报表公式配置", "EmissionsYearSetting", null, "~/EmissionsYearSetting.aspx");
                tvn_SewageStatisticsVerification.Nodes.Add("减排年报表(脱硝）", "EmissionReduction_Denitration_Year", null, "~/EmissionReduction_Denitration_Year.aspx");
                tvn_SewageStatisticsVerification.Nodes.Add("减排年报表(脱硫）", "EmissionReduction_Desulphurization_Year", null, "~/EmissionReduction_Desulphurization_Year.aspx");

                //wz
                tvn_VerificationElectricityPriceSubsidy_wz.Nodes.Add("环保数据异常统计", "EnvStatisticYQ", null, "~/Reports2_wz.aspx");
                tvn_VerificationElectricityPriceSubsidy_wz.Nodes.Add("仪表标定及支撑材料", "EnvCalibSpanYQ", null, "~/CalibSpan_wz.aspx");
                //yq
                tvn_VerificationElectricityPriceSubsidy.Nodes.Add("脱硝月明细表", "OverLimits_Nox", null, "~/OverLimits_Nox_Month.aspx");
                tvn_VerificationElectricityPriceSubsidy.Nodes.Add("脱硫月明细表", "OverLimits_So2", null, "~/OverLimits_So2_Month.aspx");
                tvn_VerificationElectricityPriceSubsidy.Nodes.Add("除尘月明细表", "OverLimits_Dust", null, "~/OverLimits_Dust_Month.aspx");
                //wz
                tvn_VerificationElectricityPriceSubsidy_wz.Nodes.Add("脱硝月明细表", "OverLimits_Nox", null, "~/OverLimits_Nox_Month_wz.aspx");
                tvn_VerificationElectricityPriceSubsidy_wz.Nodes.Add("脱硫月明细表", "OverLimits_So2", null, "~/OverLimits_So2_Month_wz.aspx");
                tvn_VerificationElectricityPriceSubsidy_wz.Nodes.Add("除尘月明细表", "OverLimits_Dust", null, "~/OverLimits_Dust_Month_wz.aspx");

                
                //yq
                tvn.Nodes.Add(tvn_VerificationElectricityPriceSubsidy);
                tvn.Nodes.Add(tvn_SewageStatisticsVerification);
                //wz
                tvn_wz.Nodes.Add(tvn_VerificationElectricityPriceSubsidy_wz);

                //yq
                tvn2.Nodes.Add(tvn);
                //wz
                tvn2.Nodes.Add(tvn_wz);

                ASPxTreeView1.Nodes.Add(tvn2);
                
                string currentnodename = (string)Session["CurrentNode"];
                if (currentnodename == null)
                {
                    currentnodename = "EnvStatisticYQ";
                }
                NodesExpand(currentnodename, ASPxTreeView1.Nodes);
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        protected void ASPxTreeView1_NodeClick(object s, TreeViewNodeEventArgs e)
        {
            Session["CurrentNode"] = e.Node.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodename"></param>
        /// <param name="nc"></param>
        protected bool NodesExpand(string nodename, TreeViewNodeCollection nc)
        {
            bool isexist = false;
            foreach (TreeViewNode tvn in nc)
            {
                if (tvn.Name != nodename)
                {
                    if (tvn.Nodes.Count > 0)
                    {
                        isexist = NodesExpand(nodename, tvn.Nodes);
                        if(isexist)
                        {
                            //tvn.Parent.Expanded = true;
                            return true;
                        }
                    }
                }
                else
                {
                    tvn.TreeView.ExpandToNode(tvn);
                    return true;
                }
            }
            return isexist;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void CheckAuthority()
        {
            if (Session["authority"] == null)
            {
                Response.Redirect("/logon.aspx");
            }
        }

        protected void ASPxTreeView1_PreRender(object sender, EventArgs e)
        {
            ASPxTreeView1.ExpandToNode(ASPxTreeView1.SelectedNode);
        }
    }
}