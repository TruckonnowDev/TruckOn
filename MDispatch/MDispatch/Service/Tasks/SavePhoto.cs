using MDispatch.Service.Inspection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using RestSharp;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.Tasks
{
    public class SavePhoto : ITask
    {
        private readonly IInspectionService _inspection;

        public SavePhoto()
        {
            _inspection = DependencyService.Get<IInspectionService>(); 
        }

        public void StartTask(params object[] task)
        {
            if ((int)task[0] == 1)
            {
                string token = (string)task[1];
                string vehiclwInformationId = (string)task[2];
                Models.PhotoInspection photoInspection = (Models.PhotoInspection)task[3];
                if (photoInspection.Damages != null)
                {
                    photoInspection.Damages.ForEach((dm) =>
                    {
                        dm.Image = null;
                        dm.ImageSource = null;
                    });
                }
                StartTask1(token, vehiclwInformationId, photoInspection);
            }
            else if((int)task[0] == 2)
            {
                string token = (string)task[1];
                string taskId = (string)task[2];
                LoadTask1(token, taskId);
            }
            else if ((int)task[0] == 3)
            {
                string token = (string)task[1];
                string taskId = (string)task[2];
                EndTask1(token, taskId);
            }
        }

        private void StartTask1(string token, string id, Models.PhotoInspection photoInspection)
        {
            string res = null;
            try
            {
                IRestResponse response = null;
                string content = null;
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("api.Task/StartTask", Method.POST);
                client.Timeout = 5000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("nameMethod", "SavePhoto");
                request.AddParameter("optionalParameter", $"{id}");
                response = client.Execute(request);
                content = response.Content;
                res = GetData(content);
                if (res != null && res != "" && App.isNetwork)
                {
                    string photoInspectionJson = JsonConvert.SerializeObject(photoInspection);
                    byte[] photoInspectionArray = Encoding.Default.GetBytes(photoInspectionJson);
                    string photoInspectionBase64 = Convert.ToBase64String(photoInspectionArray);
                    CrossSettings.Current.AddOrUpdateValue(res, photoInspectionBase64);
                    CrossSettings.Current.AddOrUpdateValue(res + "Param", $"{id}");
                    CrossSettings.Current.AddOrUpdateValue(res + "method", $"SavePhoto");
                    string allTaskLoad = CrossSettings.Current.GetValueOrDefault("allTaskLoad", "");
                    string workLoad = CrossSettings.Current.GetValueOrDefault("workLoad", "");
                    CrossSettings.Current.AddOrUpdateValue("allTaskLoad", allTaskLoad + res + ",");
                    CrossSettings.Current.AddOrUpdateValue("workLoad", workLoad + res + ",");
                    LoadTask1(token, res);
                }
                else
                {
                    string noStartkLoad = CrossSettings.Current.GetValueOrDefault("noStartkLoad", "");
                    int noStartkLoads = noStartkLoad.Split(',').Where(s => s.Contains(id)).ToList().Count;
                    TaskManager.isWorkTask = false;
                    string photoInspectionJson = JsonConvert.SerializeObject(photoInspection);
                    byte[] photoInspectionArray = Encoding.Default.GetBytes(photoInspectionJson);
                    string photoInspectionBase64 = Convert.ToBase64String(photoInspectionArray);
                    CrossSettings.Current.AddOrUpdateValue($"{id}_{noStartkLoads}", photoInspectionBase64);
                    CrossSettings.Current.AddOrUpdateValue($"{id}_{noStartkLoads}" + "Param", $"{id}");
                    CrossSettings.Current.AddOrUpdateValue($"{id}_{noStartkLoads}" + "method", $"SavePhoto");
                    CrossSettings.Current.AddOrUpdateValue("noStartkLoad", noStartkLoad + $"{id}_{noStartkLoads}" + ",");
                }
            }
            catch
            {
                string noStartkLoad = CrossSettings.Current.GetValueOrDefault("noStartkLoad", "");
                int noStartkLoads = noStartkLoad.Split(',').Where(s => s.Contains(id)).ToList().Count;
                string photoInspectionJson = JsonConvert.SerializeObject(photoInspection);
                byte[] photoInspectionArray = Encoding.Default.GetBytes(photoInspectionJson);
                string photoInspectionBase64 = Convert.ToBase64String(photoInspectionArray);
                CrossSettings.Current.AddOrUpdateValue($"{id}_{noStartkLoads}", photoInspectionBase64);
                CrossSettings.Current.AddOrUpdateValue($"{id}_{noStartkLoads}" + "Param", $"{id}");
                CrossSettings.Current.AddOrUpdateValue($"{id}_{noStartkLoads}" + "method", $"SavePhoto");
                CrossSettings.Current.AddOrUpdateValue("noStartkLoad", noStartkLoad + $"{id}_{noStartkLoads}" + ",");
            }
        }

        private async void LoadTask1(string token, string idTask)
        {
            try
            {
                IRestResponse response = null;
                string content = null;
                string photoInspectionBase64 = CrossSettings.Current.GetValueOrDefault(idTask, null);
                if (photoInspectionBase64 != null)
                {
                    RestClient client = new RestClient(Config.BaseReqvesteUrl);
                    int countReqvest = photoInspectionBase64.Length / 5120;
                    for (int i = 0; countReqvest != 0 && i < countReqvest + 1; i++)
                    {
                        photoInspectionBase64 = CrossSettings.Current.GetValueOrDefault(idTask, null);
                        string photoInspectionTmp = GetRangeArray(photoInspectionBase64, 5120);
                        RestRequest request = new RestRequest("api.Task/LoadTask", Method.POST);
                        client.Timeout = 10000;
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("token", token);
                        request.AddParameter("idTask", idTask);
                        request.AddParameter("byteBase64", photoInspectionTmp);
                        response = client.Execute(request);
                        content = response.Content;
                        string res = GetData(content);
                        if (res == null && res == "" && !App.isNetwork)
                        {
                            TaskManager.isWorkTask = false;
                            break;
                        }
                        else if (res != "No")
                        {
                            CrossSettings.Current.AddOrUpdateValue(idTask, photoInspectionBase64.Remove(0, photoInspectionTmp.Length));
                        }
                        else
                        {
                            break;
                            //save Continue
                            //Wait or Remove, Roma needs to think about it more
                        }
                        await Task.Delay(200);
                    }
                    EndTask1(token, idTask);
                }
                else
                {
                    //RemoveTask
                }
            }
            catch
            {

            }
        }

        private void EndTask1(string token, string idTask)
        {
            string res = null;
            try
            {
                IRestResponse response = null;
                string content = null;
                string method = CrossSettings.Current.GetValueOrDefault(idTask + "method", "");
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("api.Task/EndTask", Method.POST);
                client.Timeout = 10000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idTask", idTask);
                request.AddParameter("nameMethod", method);
                response = client.Execute(request);
                content = response.Content;
                res = GetData(content);
                if (res == "3")
                {
                    CrossSettings.Current.Remove(idTask);
                    CrossSettings.Current.Remove(idTask + "Param");
                    CrossSettings.Current.Remove(idTask + "method");
                    string allTaskLoad = CrossSettings.Current.GetValueOrDefault("allTaskLoad", "");
                    string workLoad = CrossSettings.Current.GetValueOrDefault("workLoad", "");
                    CrossSettings.Current.AddOrUpdateValue("allTaskLoad", allTaskLoad.Replace(res + ",", ""));
                    CrossSettings.Current.AddOrUpdateValue("workLoad", workLoad.Replace(res + ",", ""));
                }
                else if (res != null && res != "" && App.isNetwork)
                {
                    //TaskManager.isWorkTask = false;
                    //string noEndkLoad = CrossSettings.Current.GetValueOrDefault("noEndkLoad", "");
                    //CrossSettings.Current.AddOrUpdateValue("noEndkLoad", noEndkLoad + idTask + ",");
                }
            }
            catch
            {
                //string noEndkLoad = CrossSettings.Current.GetValueOrDefault("noEndkLoad", "");
                //CrossSettings.Current.AddOrUpdateValue("noEndkLoad", noEndkLoad + idTask + ",");
            }
        }


        private string GetData(string respJsonStr)
        {
            string res = null;
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                res =  responseAppS.
                        SelectToken("ResponseStr").ToString();
            }
            return res;
        }

        private string GetRangeArray(string base64, int count)
        {
            string tmp = null;
            if (base64.Length != 0)
            {
                if(base64.Length >= count)
                {
                    for(int i = 0; i < count; i++)
                    {
                        tmp += base64[i];
                    }
                    return tmp;
                }
                else
                {
                    return base64;
                }
            }
            else
            {
                return null;
            }
        }

        //private byte[] ObjectToByteArray(Models.PhotoInspection obj)
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    using (var ms = new MemoryStream())
        //    {
        //        bf.Serialize(ms, obj);
        //        return ms.ToArray();
        //    }
        //}
    }
}