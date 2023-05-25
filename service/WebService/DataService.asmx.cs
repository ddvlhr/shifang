using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Services;
using System.Xml;
using Microsoft.AspNetCore.SignalR.Client;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RTLib;

namespace WebService
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
        private readonly HubConnection _connection;
        public DataService()
        {
            var signalRServerUrl = ConfigurationManager.AppSettings["signalRUrl"];
            var serviceUser = new Dictionary<string, object>()
            {
                {"userId", 10086},
                {"userName", "DataService"},
                {"machine", 0}
            };

            // _connection = new HubConnectionBuilder()
            //     .WithUrl(signalRServerUrl, options =>
            //     {
            //         options.Headers.Add("access_token", JsonConvert.SerializeObject(serviceUser));
            //     })
            //     .WithAutomaticReconnect()
            //     .Build();
            //
            // startSignalR();
        }

        private void startSignalR()
        {
            try
            {
                _connection.StartAsync().Wait();
                NetLog.save("SignalR连接成功", false);
            }
            catch (Exception ex)
            {
                NetLog.save("SignalR连接失败: " + ex.ToString());
            }
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="equipmentType">仪器类型：1 RT，2 MTS，3 雪茄吸阻</param>
        /// <param name="xInfo"></param>
        /// <returns></returns>
        private bool isDataExist(MySqlCommand cmd, int equipmentType, XmlNode xInfo)
        {
            cmd.Parameters.Clear();
            var sql = "select id from t_group where begin_time=@beginTime and specification_id=@specificationId and instance=@instance";
            if (equipmentType != 3)
            {
                sql += " and turn_id=@turnId and machine_id=@machineId and measure_type_id=@measureTypeId";
            }
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("beginTime", xInfo.Attributes["beginTime"].Value);
            cmd.Parameters.AddWithValue("specificationId", xInfo.Attributes["specificationId"].Value);
            cmd.Parameters.AddWithValue("instance", xInfo.Attributes["instance"].Value);

            if (equipmentType != 3)
            {
                cmd.Parameters.AddWithValue("turnId", xInfo.Attributes["turnId"].Value);
                cmd.Parameters.AddWithValue("machineId", xInfo.Attributes["machineId"].Value);
                cmd.Parameters.AddWithValue("measureTypeId", xInfo.Attributes["typeId"].Value);
            }

            using (var da = new MySqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    return true;
                }
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
        public bool signalRTest(int groupId)
        {
            _connection.InvokeAsync("PushMetricalData", groupId, 3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return true;
        }
        

        /// <summary>
        /// 获取基础数据
        /// </summary>
        /// <param name="equipmentType">设备类型: 0 不限, 1 RT, 2 MTS, 3 雪茄吸阻</param>
        /// <param name="lastSyncServerTime">最近同步时间, 存在该参数时将只会同步该时间后发生修改的数据</param>
        /// <param name="recentDays"></param>
        /// <param name="responseXml">返回基础数据xml字符串</param>
        /// <param name="failReason">返回错误信息</param>
        /// <returns>成功True, 失败False</returns>
        [WebMethod]
        public bool listBaseInformation(int equipmentType, string lastSyncServerTime, int recentDays,
            out string responseXml, out string failReason)
        {
            failReason = "";
            responseXml = "";
            try
            {
                MariaDBHelper.setConnStr(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
                var db = new MariaDBHelper();
                var dtIndicator = db.excuteToTable("select id, name, alias from t_indicator");
                var indicators = ConvertHelper<Indicator>.dataTableToList(dtIndicator);

                var xDoc = new XmlDocument();
                var dec = xDoc.CreateXmlDeclaration("1.0", "GB2312", null);
                xDoc.AppendChild(dec);
                var rootElement = xDoc.CreateElement("info");
                xDoc.AppendChild(rootElement);

                #region 牌号信息

                var specificationSql = "select * from t_specification where 1=1";
                if (equipmentType > 0)
                {
                    specificationSql += " and equipment_type=" + equipmentType;
                }

                if (!string.IsNullOrEmpty(lastSyncServerTime))
                {
                    specificationSql += " and modified_at_utc >=" + "'" + lastSyncServerTime + "'";
                }

                var dtSpecification = db.excuteToTable(specificationSql);
                foreach (DataRow dr in dtSpecification.Rows)
                {
                    var id = dr["id"].ToString();

                    var el = xDoc.CreateElement("specification");
                    el.SetAttribute("id", id);
                    el.SetAttribute("name", dr["name"].ToString());
                    el.SetAttribute("equipment", equipmentType.ToString());
                    el.SetAttribute("status", dr["status"].ToString() == "0" ? "1" : "0");

                    if (!string.IsNullOrEmpty(dr["single_rules"].ToString()))
                    {
                        var singleRules = JsonConvert.DeserializeObject<List<Rule>>(dr["single_rules"].ToString());
                        foreach (var rule in singleRules)
                        {
                            var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                            if (indicator == null)
                                continue;

                            var item = xDoc.CreateElement(indicator.Alias);
                            item.SetAttribute("mid", rule.Standard);
                            item.SetAttribute("high", rule.Upper);
                            item.SetAttribute("low", rule.Lower);
                            if (!string.IsNullOrEmpty(rule.Position))
                                item.SetAttribute("position", rule.Position);

                            el.AppendChild(item);
                        }
                    }

                    rootElement.AppendChild(el);
                }

                #endregion

                #region 班次信息

                var dtTurn = db.excuteToTable("select * from t_turn");
                foreach (DataRow dr in dtTurn.Rows)
                {
                    var el = xDoc.CreateElement("turn");
                    el.SetAttribute("id", dr["id"].ToString());
                    el.SetAttribute("name", dr["name"].ToString());
                    el.SetAttribute("status", dr["status"].ToString());
                    rootElement.AppendChild(el);
                }

                #endregion

                #region 机台信息

                var dtMachine = db.excuteToTable("select * from t_machine");
                foreach (DataRow dr in dtMachine.Rows)
                {
                    var el = xDoc.CreateElement("machine");
                    el.SetAttribute("id", dr["id"].ToString());
                    el.SetAttribute("name", dr["name"].ToString());
                    el.SetAttribute("status", dr["status"].ToString());
                    rootElement.AppendChild(el);
                }

                #endregion

                #region 测量类型信息

                var dtMeasureType = db.excuteToTable("select * from t_measure_type");
                foreach (DataRow dr in dtMeasureType.Rows)
                {
                    var el = xDoc.CreateElement("type");
                    el.SetAttribute("id", dr["id"].ToString());
                    el.SetAttribute("name", dr["name"].ToString());
                    el.SetAttribute("status", dr["status"].ToString());
                    rootElement.AppendChild(el);
                }

                #endregion

                #region 记录最后同步时间

                var syncEl = xDoc.CreateElement("lastSyncServerTime");
                syncEl.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                rootElement.AppendChild(syncEl);

                #endregion

                responseXml = xDoc.OuterXml;

                return true;
            }
            catch (Exception ex)
            {
                failReason = ex.ToString();
                RunLog.save(ex.ToString(), false);
                return false;
            }
        }

        /// <summary>
        /// 上传测量数据
        /// </summary>
        /// <param name="machineType">设备类型: 1 RT, 2 MTS, 3 雪茄吸阻</param>
        /// <param name="failReason">返回错误信息</param>
        /// <returns>成功True, 失败False</returns>
        [WebMethod]
        public bool uploadMeasureData(int equipmentType, string xmlData, out string failReason)
        {
            failReason = "";
            MySqlTransaction transction = null;
            MySqlConnection conn = null;
            try
            {
                MariaDBHelper.setConnStr(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
                var db = new MariaDBHelper();

                var xDoc = new XmlDocument();
                xDoc.LoadXml(xmlData);
                var xUpload = xDoc.SelectSingleNode("upload");
                if (xUpload == null)
                {
                    failReason = "没有找到 upload 节点, 请检查 xml 字符串是否正确";
                    return false;
                }

                var xInfo = xUpload.SelectSingleNode("info") as XmlElement;

                if (string.IsNullOrEmpty(xInfo.Attributes["specificationId"].Value))
                {
                    failReason = "牌号ID不能为空";
                    return false;
                }

                // 检查牌号ID
                conn = db.getConnection();
                conn.Open();

                var dtSpecification = db.excuteToTable("select id, single_rules from t_specification where id=" +
                                                       xInfo.Attributes["specificationId"].Value);
                if (dtSpecification.Rows.Count == 0)
                {
                    failReason = "牌号ID不存在";
                    return false;
                }

                // 判断数据是否存在
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                if (isDataExist(cmd, equipmentType, xInfo))
                    return true;

                // 记录分组信息 t_group
                var xDetails = xInfo.SelectNodes("data");
                if (xDetails.Count == 0)
                {
                    failReason = "没有找到 data 节点, 请检查上传文件";
                    return false;
                }
                var specificationId = int.Parse(xInfo.Attributes["specificationId"].Value);
                var turnId = xInfo.Attributes["turnId"].Value;
                var machineId = xInfo.Attributes["machineId"].Value;
                var measureTypeId = xInfo.Attributes["typeId"].Value;
                transction = conn.BeginTransaction();
                cmd.Parameters.Clear();
                cmd.Transaction = transction;
                cmd.CommandText =
                    "insert into t_group (begin_time, end_time, production_time, deliver_time, instance, specification_id, turn_id, machine_id," +
                    " measure_type_id, pickup_way, count, created_at_utc, modified_at_utc, user_data, equipment_type)" +
                    " values (@beginTime, @endTime, @productionTime, @deliverTime, @instance, @specificationId, @turnId," +
                    " @machineId, @measureTypeId, @pickupWay, @count, @createdTime, @modifiedTime, @userData, @equipmentType)";

                //cmd.Parameters.AddWithValue("equipmentType", equipmentType);
                cmd.Parameters.AddWithValue("beginTime", xInfo.Attributes["beginTime"].Value);
                cmd.Parameters.AddWithValue("endTime", xInfo.Attributes["endTime"].Value);
                var productionTime = xInfo.Attributes["productionTime"].Value;
                var deliverTime = xInfo.Attributes["deliverTime"].Value;
                cmd.Parameters.AddWithValue("productionTime", productionTime == "" ? null : productionTime);
                cmd.Parameters.AddWithValue("deliverTime", deliverTime == "" ? null : deliverTime);
                cmd.Parameters.AddWithValue("instance", xInfo.Attributes["instance"].Value);
                cmd.Parameters.AddWithValue("specificationId", specificationId);
                cmd.Parameters.AddWithValue("turnId", turnId == "" ? 0 : int.Parse(turnId));
                cmd.Parameters.AddWithValue("machineId", machineId == "" ? 0 : int.Parse(machineId));
                cmd.Parameters.AddWithValue("measureTypeId", equipmentType == 1 ? int.Parse(measureTypeId) : 25);
                cmd.Parameters.AddWithValue("pickupWay", int.Parse(xInfo.Attributes["pickupWay"].Value));
                cmd.Parameters.AddWithValue("count", xDetails.Count);

                cmd.Parameters.AddWithValue("createdTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("modifiedTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("userData", xInfo.Attributes["userData"].Value);
                cmd.Parameters.AddWithValue("equipmentType", equipmentType);
                cmd.ExecuteNonQuery();

                var strGid = "";
                using (var da = new MySqlDataAdapter("select LAST_INSERT_ID()", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    strGid = dt.Rows[0][0].ToString();
                }

                if (machineId != "")
                    _connection.InvokeAsync("PushMetricalData", int.Parse(strGid), int.Parse(machineId), xInfo.Attributes["beginTime"].Value);

                // 记录详细数据t_data
                var dtIndicator = db.excuteToTable("select id, alias from t_indicator");
                var indicators = ConvertHelper<Indicator>.dataTableToList(dtIndicator);
                foreach (XmlElement d in xDetails)
                {
                    cmd.CommandText = "insert into t_data (group_id, test_time, data, created_at_utc, modified_at_utc) values (@groupId, @testTime, @data, @createTime, @modifiedTime)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("groupId", strGid);
                    cmd.Parameters.AddWithValue("testTime", d.Attributes["time"].Value);
                    var data = new JObject();
                    foreach (XmlAttribute attr in d.Attributes)
                    {
                        if (attr.Name == "time")
                        {
                            data.Add(new JProperty("testTime", d.Attributes["time"].Value));
                        }

                        var indicator = indicators.FirstOrDefault(c => c.Alias == attr.Name);
                        if (indicator == null)
                            continue;

                        data.Add(new JProperty(indicator.Id.ToString(), attr.Value));
                    }

                    cmd.Parameters.AddWithValue("data", JsonConvert.SerializeObject(data));
                    cmd.Parameters.AddWithValue("createTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("modifiedTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
                transction?.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transction?.Rollback();
                failReason += ex.ToString();
                UploadLog.save(ex.ToString(), false);
                UploadLog.save(xmlData, false);
                return false;
            }
            finally
            {
                conn?.Close();
            }
        }


        /// <summary>
        /// 上传标定数据
        /// </summary>
        /// <param name="equipmentType">设备类型: 1 RT, 2 MTS, 3 雪茄吸阻</param>
        /// <param name="xmlData">标定验证数据</param>
        /// <param name="failReason">返回错误信息</param>
        /// <returns>返回上传成功的记录id</returns>
        [WebMethod]
        public List<int> uploadCalibrationData(int equipmentType, string xmlData, out string failReason)
        {
            failReason = "";
            var ret = new List<int>();
            MySqlConnection conn = null;
            try
            {
                MariaDBHelper.setConnStr(ConfigurationManager.ConnectionStrings["mysql"].ConnectionString);
                var db = new MariaDBHelper();
                conn = db.getConnection();
                conn.Open();
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "insert into t_calibration (instance, equipment_type, time, operation, unit, unit_type, result_code, description, temperature, humidity, created_at_utc, modified_at_utc) values (@instance, @equipmentType, @time, @operation, @unit, @unitType, @resultCode, @description, @temperature, @humidity, @createdTime, @modifiedTime)";
                var xDoc = new XmlDocument();
                xDoc.LoadXml(xmlData);
                var xUpload = xDoc.SelectSingleNode("upload");
                var xInfo = xUpload.SelectSingleNode("info");
                if (xInfo == null)
                {
                    failReason = "没有找到 info 节点, 请检查 xml 字符串是否正确";
                    return ret;
                }

                var xDetails = xInfo.SelectNodes("record");
                var name = xInfo.Attributes["instance"].Value;
                foreach (XmlElement d in xDetails)
                {
                    var time = Convert.ToDateTime(d.Attributes["time"].Value);
                    var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var temperature = d.Attributes["temperature"].Value;
                    var humidity = d.Attributes["humidity"].Value;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("instance", name);
                    cmd.Parameters.AddWithValue("equipmentType", equipmentType);
                    cmd.Parameters.AddWithValue("time", time);
                    cmd.Parameters.AddWithValue("operation", d.Attributes["operation"].Value);
                    cmd.Parameters.AddWithValue("unit", d.Attributes["unit"].Value);
                    cmd.Parameters.AddWithValue("unitType", d.Attributes["unitType"].Value);
                    cmd.Parameters.AddWithValue("resultCode", Convert.ToInt32(d.Attributes["resultCode"].Value));
                    cmd.Parameters.AddWithValue("description", d.Attributes["description"].Value);
                    cmd.Parameters.AddWithValue("temperature", temperature == "" ? 0 : Convert.ToDouble(temperature));
                    cmd.Parameters.AddWithValue("humidity", humidity == "" ? 0 : Convert.ToDouble(humidity));
                    cmd.Parameters.AddWithValue("createdTime", currentTime);
                    cmd.Parameters.AddWithValue("modifiedTime", currentTime);
                    cmd.ExecuteNonQuery();
                    ret.Add(int.Parse(d.Attributes["id"].Value));
                }
            }
            catch (Exception ex)
            {
                failReason = ex.ToString();
                UploadLog.save(ex.ToString(), false);
                UploadLog.save(xmlData, false);
            }
            finally
            {
                conn?.Close();
            }

            return ret;
        }
    }
}
