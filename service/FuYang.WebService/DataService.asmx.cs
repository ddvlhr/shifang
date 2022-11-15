using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Xml;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RTLib;

namespace FuYang.WebService
{
    /// <summary>
    /// DataService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DataService : System.Web.Services.WebService
    {
        private bool isDataExist(MySqlCommand cmd, XmlNode xInfo)
        {
            cmd.Parameters.Clear();
            cmd.CommandText =
                "select id from t_group_record where begin_time=@beginTime and end_time=@endTime and instance=@instance and turn_id=@turnId and machine_id=@machineId";
            cmd.Parameters.AddWithValue("beginTime", xInfo.Attributes["beginTime"].Value);
            cmd.Parameters.AddWithValue("endTime", xInfo.Attributes["endTime"].Value);
            cmd.Parameters.AddWithValue("instance", xInfo.Attributes["instance"].Value);
            cmd.Parameters.AddWithValue("turnId", xInfo.Attributes["turnId"].Value);
            cmd.Parameters.AddWithValue("machineId", xInfo.Attributes["macId"].Value);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count >= 1)
            {
                return true;
            }

            return false;
        }

        private object getXmlAttr(XmlElement element, string attrName)
        {
            if (element == null)
                return null;
            if (element.HasAttribute(attrName))
            {
                var value = element.Attributes[attrName].Value;
                return string.IsNullOrEmpty(value) ? null : value;
            }
            else
            {
                return null;
            }
        }

        [WebMethod]
        public string listSpecification(out string failReason)
        {
            failReason = "";
            try
            {
                MariaDBHelper.setConnStr(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
                MariaDBHelper db = new MariaDBHelper();
                var systemSettingDt =
                    db.excuteToTable("select weight, circle, oval, length, resistance, hardness, filter_type_id from t_system_setting");
                var weightId = Convert.ToInt32(systemSettingDt.Rows[0]["weight"].ToString());
                var circleId = Convert.ToInt32(systemSettingDt.Rows[0]["circle"].ToString());
                var ovalId = Convert.ToInt32(systemSettingDt.Rows[0]["oval"].ToString());
                var lengthId = Convert.ToInt32(systemSettingDt.Rows[0]["length"].ToString());
                var resistanceId = Convert.ToInt32(systemSettingDt.Rows[0]["resistance"].ToString());
                var hardnessId = Convert.ToInt32(systemSettingDt.Rows[0]["hardness"].ToString());
                var filterTypeId = Convert.ToInt32(systemSettingDt.Rows[0]["filter_type_id"].ToString());
                DataTable dtSpecification = db.excuteToTable($"select s.*, st.`name` as specificationTypeName from t_specification s, t_specification_type st where s.status = 0 and s.specification_type_id = {filterTypeId} and s.specification_type_id = st.id");

                var xDoc = new XmlDocument();
                var desc = xDoc.CreateXmlDeclaration("1.0", "GB2312", null);
                xDoc.AppendChild(desc);
                var rootElement = xDoc.CreateElement("info");
                xDoc.AppendChild(rootElement);
                

                foreach (DataRow dr in dtSpecification.Rows)
                {
                    XmlElement element = xDoc.CreateElement("specification");
                    element.SetAttribute("id", dr["id"].ToString());
                    element.SetAttribute("fullName", dr["name"].ToString());
                    element.SetAttribute("motherName", "");
                    element.SetAttribute("type", dr["specificationTypeName"].ToString());
                    rootElement.AppendChild(element);

                    var dtRules = dr["single_rules"].ToString();
                    var rules = JsonConvert.DeserializeObject<List<Rule>>(dtRules);
                    var weight = rules.FirstOrDefault(c => c.Id == weightId);
                    if (weight != null)
                    {
                        var elt = xDoc.CreateElement("weight");
                        elt.SetAttribute("mid", weight.Standard);
                        elt.SetAttribute("high", weight.Upper);
                        elt.SetAttribute("low", weight.Lower);
                        elt.SetAttribute("highPre", "");
                        elt.SetAttribute("lowPre", "");
                        element.AppendChild(elt);
                    }
                    var circle = rules.FirstOrDefault(c => c.Id == circleId);
                    if (circle != null)
                    {
                        var elt = xDoc.CreateElement("circle");
                        elt.SetAttribute("mid", circle.Standard);
                        elt.SetAttribute("high", circle.Upper);
                        elt.SetAttribute("low", circle.Lower);
                        elt.SetAttribute("highPre", "");
                        elt.SetAttribute("lowPre", "");
                        element.AppendChild(elt);
                    }

                    var oval = rules.FirstOrDefault(c => c.Id == ovalId);
                    if (oval != null)
                    {
                        var elt = xDoc.CreateElement("oval");
                        elt.SetAttribute("mid", oval.Standard);
                        elt.SetAttribute("high", oval.Upper);
                        elt.SetAttribute("low", oval.Lower);
                        elt.SetAttribute("highPre", "");
                        elt.SetAttribute("lowPre", "");
                        element.AppendChild(elt);
                    }

                    var length = rules.FirstOrDefault(c => c.Id == lengthId);
                    if (length != null)
                    {
                        var elt = xDoc.CreateElement("length");
                        elt.SetAttribute("mid", length.Standard);
                        elt.SetAttribute("high", length.Upper);
                        elt.SetAttribute("low", length.Lower);
                        elt.SetAttribute("highPre", "");
                        elt.SetAttribute("lowPre", "");
                        element.AppendChild(elt);
                    }

                    var resistance = rules.FirstOrDefault(c => c.Id == resistanceId);
                    if (resistance != null)
                    {
                        var elt = xDoc.CreateElement("resistance");
                        elt.SetAttribute("mid", resistance.Standard);
                        elt.SetAttribute("high", resistance.Upper);
                        elt.SetAttribute("low", resistance.Lower);
                        elt.SetAttribute("highPre", "");
                        elt.SetAttribute("lowPre", "");
                        element.AppendChild(elt);
                    }

                    var hardness = rules.FirstOrDefault(c => c.Id == hardnessId);
                    if (hardness != null)
                    {
                        var elt = xDoc.CreateElement("hardness");
                        elt.SetAttribute("mid", hardness.Standard);
                        elt.SetAttribute("high", hardness.Upper);
                        elt.SetAttribute("low", hardness.Lower);
                        elt.SetAttribute("highPre", "");
                        elt.SetAttribute("lowPre", "");
                        element.AppendChild(elt);
                    }
                }

                #region 机台信息

                var machines = db.excuteToTable("select * from t_machine where status = 0");
                foreach (DataRow dr in machines.Rows)
                {
                    var element = xDoc.CreateElement("machine");
                    element.SetAttribute("id", dr["id"].ToString());
                    element.SetAttribute("name", dr["name"].ToString());
                    rootElement.AppendChild(element);
                }

                #endregion

                #region 班次信息

                var turns = db.excuteToTable("select * from t_turn where status = 0");
                foreach (DataRow dr in turns.Rows)
                {
                    var element = xDoc.CreateElement("turn");
                    element.SetAttribute("id", dr["id"].ToString());
                    element.SetAttribute("name", dr["name"].ToString());
                    rootElement.AppendChild(element);
                }

                #endregion

                #region 测量类型

                var measureTypes = db.excuteToTable("select * from t_measure_type where status = 0");
                foreach (DataRow dr in measureTypes.Rows)
                {
                    var element = xDoc.CreateElement("testType");
                    element.SetAttribute("id", dr["id"].ToString());
                    element.SetAttribute("name", dr["name"].ToString());
                    rootElement.AppendChild(element);
                }

                #endregion

                #region 牌号类型

                var specificationTypes = db.excuteToTable("select * from t_specification_type where status = 0");
                foreach (DataRow dr in specificationTypes.Rows)
                {
                    var element = xDoc.CreateElement("specifcationType");
                    element.SetAttribute("id", dr["id"].ToString());
                    element.SetAttribute("name", dr["name"].ToString());
                    rootElement.AppendChild(element);
                }

                #endregion

                return xDoc.OuterXml;
            }
            catch (Exception ex)
            {
                UploadLog.save(ex.ToString(), false);
                failReason = ex.ToString();
                return null;
            }
        }

        [WebMethod]
        public bool upload(string xmlData, out string failReason)
        {
            failReason = "";
            MySqlTransaction transaction = null;
            MySqlConnection conn = null;
            try
            {
                MariaDBHelper.setConnStr(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
                var xDoc = new XmlDocument();
                xDoc.LoadXml(xmlData);
                var xData = xDoc.SelectSingleNode("upload");
                if (xData == null)
                {
                    failReason = "找不到 upload 节点";
                    return false;
                }

                var xInfo = (XmlElement) xData.SelectSingleNode("info");
                if (xInfo == null)
                {
                    failReason = "找不到 info 节点";
                    return false;
                }

                var dtSpecification = new DataTable();
                var db = new MariaDBHelper();
                conn = db.getConnection();
                conn.Open();
                var specificationId = xInfo.Attributes["specificationId"].Value;
                using var da = new MySqlDataAdapter($"select * from t_specification where id={specificationId}", conn);
                da.Fill(dtSpecification);
                if (dtSpecification.Rows.Count == 0)
                {
                    failReason = "牌号ID不存在";
                    return false;
                }

                var typeId = xInfo.Attributes["type"].Value;

                var cmd = new MySqlCommand()
                {
                    Connection = conn
                };
                if (isDataExist(cmd, xInfo))
                    return true;

                var xDetail = xData.SelectNodes("data");
                if (xDetail == null)
                {
                    failReason = "没有找到 data 节点";
                    return false;
                }

                var count = xDetail.Count;
                transaction = conn.BeginTransaction();
                cmd.Parameters.Clear();
                cmd.Transaction = transaction;
                cmd.CommandText =
                    "insert into t_group_record (created_at_utc, modified_at_utc, begin_time, end_time, production_time, deliver_time, instance, specification_id, turn_id, machine_id, machine_model_id, measure_type_id, pickup_way, count, user_id, user_name) values(@createdTime, @modifiedTime, @beginTime, @endTime, @productionTime, @deliverTime, @instance, @specificationId, @turnId, @machineId, @machineModelId, @measureTypeId, @pickupway, @count, @userId, @userName)";
                cmd.Parameters.AddWithValue("createdTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("modifiedTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("beginTime", xInfo.Attributes["beginTime"].Value);
                cmd.Parameters.AddWithValue("endTime", xInfo.Attributes["endTime"].Value);
                cmd.Parameters.AddWithValue("productionTime", null);
                cmd.Parameters.AddWithValue("deliverTime", null);
                cmd.Parameters.AddWithValue("instance", xInfo.Attributes["instance"].Value);
                cmd.Parameters.AddWithValue("specificationId", xInfo.Attributes["specificationId"].Value);
                cmd.Parameters.AddWithValue("turnId", xInfo.Attributes["turnId"].Value);
                cmd.Parameters.AddWithValue("machineId", xInfo.Attributes["macId"].Value);
                cmd.Parameters.AddWithValue("machineModelId", "0");
                cmd.Parameters.AddWithValue("measureTypeId", xInfo.Attributes["type"].Value);
                cmd.Parameters.AddWithValue("pickupWay", "2");
                cmd.Parameters.AddWithValue("count", count);
                cmd.Parameters.AddWithValue("userId", "0");
                cmd.Parameters.AddWithValue("userName", "");
                cmd.ExecuteNonQuery();

                string lastGroupId = "";
                using var daGroup = new MySqlDataAdapter("select LAST_INSERT_ID()", conn);
                var dtGroup = new DataTable();
                daGroup.Fill(dtGroup);
                lastGroupId = dtGroup.Rows[0][0].ToString();

                foreach (XmlElement d in xDetail)
                {
                    cmd.CommandText =
                        "insert into t_data_record (created_at_utc, modified_at_utc, group_id, test_time, weight, circle, oval, length, resistance, resistance_open, hardness, ventilation_filter, ventilation_cigarette, ventilation_total, total, count) values (@createdTime, @modifiedTime, @groupId, @testTime, @weight, @circle, @oval, @length, @resistance, @resistanceOpen, @hardness, @ventilationFilter, @ventilationCigarette, @ventilationTotal, @total, @count)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("createdTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("modifiedTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("groupId", lastGroupId);
                    cmd.Parameters.AddWithValue("testTime", d.Attributes["time"].Value);
                    cmd.Parameters.AddWithValue("weight", getXmlAttr(d, "weight"));
                    cmd.Parameters.AddWithValue("circle", getXmlAttr(d, "circle"));
                    cmd.Parameters.AddWithValue("oval", getXmlAttr(d, "circleRate"));
                    cmd.Parameters.AddWithValue("length", getXmlAttr(d, "length"));
                    cmd.Parameters.AddWithValue("resistance", getXmlAttr(d, "resistance"));
                    cmd.Parameters.AddWithValue("resistanceOpen", getXmlAttr(d, "resistanceOpen"));
                    cmd.Parameters.AddWithValue("hardness", getXmlAttr(d, "hardness"));
                    cmd.Parameters.AddWithValue("ventilationFilter", getXmlAttr(d, "ventilationFilter"));
                    cmd.Parameters.AddWithValue("ventilationCigarette", getXmlAttr(d, "ventilationCigarette"));
                    cmd.Parameters.AddWithValue("ventilationTotal", getXmlAttr(d, "ventilationTotal"));
                    cmd.Parameters.AddWithValue("total", "0");
                    cmd.Parameters.AddWithValue("count", "0");
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                UploadLog.save(ex.ToString(), false);
                UploadLog.save(xmlData, false);
                failReason = ex.ToString();
                return false;
            }
            finally
            {
                conn?.Close();
            }
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
