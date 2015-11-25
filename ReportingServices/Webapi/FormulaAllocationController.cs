using Model;
using Model.Pi;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace ReportingServices.Webapi
{
    public class FormulaAllocationController : ApiController
    {
        public static Microsoft.JScript.Vsa.VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
        PetaPoco.Database db = new PetaPoco.Database("dbconn");
        // POST api/<controller>
        [HttpPost]
        public FormulaSetting Post(FormulaSetting editformulaSetting)
        {
            Sql sql = new Sql();
            sql.Select("*").From("tn_FormulaSetting").Where("Id=@0", editformulaSetting.Id);
            FormulaSetting formulaSetting = db.Fetch<FormulaSetting>(sql).FirstOrDefault();
            if (editformulaSetting != null && formulaSetting != null)
            {
                formulaSetting.Formula = editformulaSetting.Formula;
                formulaSetting.Remarks = editformulaSetting.Remarks;
                db.Update("tn_FormulaSetting", "Id", formulaSetting);
            }
            return formulaSetting;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateStart"></param>
        /// <param name="updateEnd"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonState UpdateData(int id, DateTime updateStart, DateTime updateEnd)
        {
            JsonState jsonState = new JsonState();
            string formula = "";
            jsonState.state = 0;
            if (id > 0)
            {
                Sql sql = new Sql();
                sql.Select("*").From("tn_FormulaSetting").Where("Id=@0", id);
                FormulaSetting formulaSetting = db.Fetch<FormulaSetting>(sql).FirstOrDefault();
                if (formulaSetting != null)
                {
                    formula = formulaSetting.Formula;
                    List<string> pipoints = GetPiPoints(formulaSetting.Formula);
                    List<FormulaPoint> formulaPoints = new List<FormulaPoint>();
                    formulaPoints = db.Fetch<FormulaPoint>("select * from tn_FormulaPoint");
                    IDictionary<string, List<PIAvgRecords>> piAvgRecords = new Dictionary<string, List<PIAvgRecords>>();
                    foreach (string pipoint in pipoints)
                    {
                        string pointName = pipoint.Replace("_MONTHAVG", "").Replace("_MONTH", "");
                        FormulaPoint formulaPoint = formulaPoints.Where(n => n.PointName == pointName).FirstOrDefault();
                        if (formulaPoint != null)
                            sql = new Sql("select * from " + formulaPoint.TableName + " where pname=@0 and timestamps<=@1 and timestamps>=@2", pointName, updateEnd, updateStart);
                        else
                            sql = new Sql("select * from PIAvgRecords where pname=@0 and timestamps<=@1 and timestamps>=@2", pointName, updateEnd, updateStart);
                        piAvgRecords[pipoint] = db.Fetch<PIAvgRecords>(sql);
                    }
                    Expre(pipoints, piAvgRecords, formulaSetting, updateStart, updateEnd);
                    jsonState.state = 1;
                }
            }
            return jsonState;
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="pipoints">点名</param>
        /// <param name="piAvgRecords">小时数据</param>
        /// <param name="formula">公式</param>
        public void Expre(List<string> pipoints, IDictionary<string, List<PIAvgRecords>> piAvgRecords, FormulaSetting formulaSetting, DateTime updateStart, DateTime updateEnd)
        {
            string formula = formulaSetting.Formula;
            if (pipoints.Count > 0)
            {
                //第一个参数是否月累计
                bool isMonth = pipoints[0].Contains("_MONTH") ? true : false;
                //第一个参数是否月均值
                bool isMonthAvg = pipoints[0].Contains("_MONTHAVG") ? true : false;
                bool isCalculation = true;
                if (!isMonth)
                {
                    using (var scope = db.GetTransaction())
                    {
                        try
                        {
                            Sql deleteSql = new Sql(string.Format("delete {0} where pname='{1}' and timestamps<='{2}' and timestamps>='{3}'", formulaSetting.TableName, formulaSetting.PointName, updateEnd, updateStart));
                            db.Execute(deleteSql);
                            //小时
                            foreach (DateTime itemTime in piAvgRecords[pipoints[0]].Select(n => n.timestamps))
                            {
                                for (int i = 0; i < pipoints.Count; i++)
                                {
                                    if (isCalculation)
                                    {
                                        if (piAvgRecords[pipoints[i]].Where(n => n.timestamps == itemTime).Count() == 0)
                                        {
                                            isCalculation = false;
                                            continue;
                                        }
                                        else
                                            formula = formula.Replace("[pi:" + pipoints[i] + "]", piAvgRecords[pipoints[i]].Where(n => n.timestamps == itemTime).FirstOrDefault().pvalue.ToString());
                                    }
                                }
                                if (isCalculation)
                                {
                                    float value = Convert.ToInt32(EvalJScript(formula));
                                    formula = formulaSetting.Formula;
                                    db.Insert(formulaSetting.TableName, "id", true, new { pname = formulaSetting.PointName, pvalue = value, timestamps = itemTime, updatetime = DateTime.Now, plantid = 1 });
                                }
                                isCalculation = true;
                            }
                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            scope.Dispose();
                        }
                    }
                }
                else
                {
                    using (var scope = db.GetTransaction())
                    {
                        try
                        {
                            Sql deleteSql = new Sql(string.Format("delete {0} where pname='{1}' and timestamps<='{2}' and timestamps>='{3}'", formulaSetting.TableName, formulaSetting.PointName, updateEnd, updateStart));
                            db.Execute(deleteSql);
                            //月计算
                            foreach (var itemTime in piAvgRecords[pipoints[0]].GroupBy(n => n.timestamps.ToString("yyyy-MM")))
                            {
                                for (int i = 0; i < pipoints.Count; i++)
                                {
                                    //是否月累计
                                    isMonth = pipoints[i].Contains("_MONTH") ? true : false;
                                    //是否月均值
                                    isMonthAvg = pipoints[i].Contains("_MONTHAVG") ? true : false;
                                    if (isCalculation)
                                    {
                                        if (piAvgRecords[pipoints[i]].Where(n => n.timestamps.ToString("yyyy-MM") == itemTime.Key).Count() == 0)
                                        {
                                            isCalculation = false;
                                            continue;
                                        }
                                        else
                                        {
                                            //如果是月均值
                                            if (isMonthAvg)
                                                //月均值
                                                formula = formula.Replace("[pi:" + pipoints[i] + "]", piAvgRecords[pipoints[i]].Where(n => n.timestamps.ToString("yyyy-MM") == itemTime.Key).Average(n => n.pvalue).ToString());
                                            else
                                                //月累计
                                                formula = formula.Replace("[pi:" + pipoints[i] + "]", piAvgRecords[pipoints[i]].Where(n => n.timestamps.ToString("yyyy-MM") == itemTime.Key).Sum(n => n.pvalue).ToString());
                                        }
                                    }
                                }
                                if (isCalculation)
                                {
                                    DateTime insertTime = DateTime.Parse(itemTime.Key + "-01");
                                    decimal value = Convert.ToDecimal(EvalJScript(formula));
                                    formula = formulaSetting.Formula;
                                    db.Insert(formulaSetting.TableName, "id", true, new { pname = formulaSetting.PointName, pvalue = value, timestamps = insertTime, updatetime = DateTime.Now, plantid = 1 });
                                }
                                isCalculation = true;
                            }
                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            scope.Dispose();
                        }
                    }
                }
            }
        }

        public List<string> GetPiPoints(string formula)
        {
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty("formula"))
            {
                string pattern = "pi:(?<key>[^]]*)";
                MatchCollection matchs = Regex.Matches(formula, pattern);
                if (matchs != null)
                {
                    foreach (Match match in matchs)
                    {
                        if (match != null)
                            result.Add(match.Groups["key"].Value);
                    }
                }
            }
            return result;
        }

        public static object EvalJScript(string JScript)
        {
            object Result = null;
            try
            {
                Result = Microsoft.JScript.Eval.JScriptEvaluate(JScript, Engine);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return Result;
        }
    }
}