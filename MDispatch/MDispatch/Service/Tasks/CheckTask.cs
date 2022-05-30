using MDispatch.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MDispatch.Service.Tasks
{
    public class CheckTask : ITask
    {
        public async void StartTask(params object[] task)
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            List<Models.TaskModel> idTasks = GetTaskLoads();
            foreach(Models.TaskModel idTask in idTasks)
            {
                string allTaskLoad = CrossSettings.Current.GetValueOrDefault("allTaskLoad", "");
                string workLoad = CrossSettings.Current.GetValueOrDefault("workLoad", "");
                if(allTaskLoad.Contains(idTask.IdTask.ToString()))
                {
                    if (!workLoad.Contains(idTask.IdTask.ToString()))
                    {
                        CrossSettings.Current.AddOrUpdateValue("workLoad", workLoad + idTask.IdTask.ToString() + ",");
                    }
                    await Task.Delay(200);
                    TaskManager.CommandToDo(idTask.Method, 2, token, idTask.IdTask.ToString());
                }
                else
                {
                    //remove Task
                    continue;
                }
            }
            string noStartkLoad = CrossSettings.Current.GetValueOrDefault("noStartkLoad", "");
            string[] noStartkLoads = noStartkLoad.Split(',');
            for(int i = 0; i < noStartkLoads.Length-1; i++)
            {
                await Task.Delay(200);
                noStartkLoad = CrossSettings.Current.GetValueOrDefault("noStartkLoad", "");
                string method = CrossSettings.Current.GetValueOrDefault(noStartkLoads[i] + "method", "");
                GoToCommand1(token, 1, method, noStartkLoads[i]);
                string s = noStartkLoad.Replace(noStartkLoads[i] + ",", "");
                bool isSave = CrossSettings.Current.AddOrUpdateValue("noStartkLoad", s);
                CrossSettings.Current.Remove(noStartkLoads[i] + "method");
                CrossSettings.Current.Remove(noStartkLoads[i] + "Param");
                CrossSettings.Current.Remove(noStartkLoads[i]);
            }
        }

        private void GoToCommand1(string token, int type, string method, string idTaskNo)
        {
            if(method == "SavePhoto")
            {
                string obj = CrossSettings.Current.GetValueOrDefault(idTaskNo, "");
                string vehiclwInformationId = CrossSettings.Current.GetValueOrDefault(idTaskNo + "Param", "");
                byte[] photoInspectionArray = Convert.FromBase64String(obj);
                string photoInspectionjson = Encoding.Default.GetString(photoInspectionArray);
                Models.PhotoInspection photoInspection = JsonConvert.DeserializeObject<Models.PhotoInspection>(photoInspectionjson);
                TaskManager.CommandToDo("SavePhoto", type, token, vehiclwInformationId, photoInspection);
            }
            else if (method == "SaveInspactionDriver")
            {
                string obj = CrossSettings.Current.GetValueOrDefault(idTaskNo, "");
                string[] paramss = CrossSettings.Current.GetValueOrDefault(idTaskNo + "Param", "").Split(',');
                string IdDriver = paramss[0];
                string IndexCurent = paramss[1];
                byte[] photoArray = Convert.FromBase64String(obj);
                string photojson = Encoding.Default.GetString(photoArray);
                Models.Photo photo = JsonConvert.DeserializeObject<Models.Photo>(photojson);
                TaskManager.CommandToDo("SaveInspactionDriver", type, token, IdDriver, photo, IndexCurent);
            }
            else if (method == "SaveRecount")
            {
                string obj = CrossSettings.Current.GetValueOrDefault(idTaskNo, "");
                string[] paramss = CrossSettings.Current.GetValueOrDefault(idTaskNo + "Param", "").Split(',');
                string idShip = paramss[0];
                string typeVideo = paramss[1];
                byte[] videoArray = Convert.FromBase64String(obj);
                string videojson = Encoding.Default.GetString(videoArray);
                Video video = JsonConvert.DeserializeObject<Video>(videojson);
                TaskManager.CommandToDo("SaveRecount", type, token, idShip, 1, video);
            }
        }

        private List<Models.TaskModel> GetTaskLoads()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            IRestResponse response = null;
            string content = null;
            RestClient client = new RestClient(Config.BaseReqvesteUrl);
            RestRequest request = new RestRequest("api.Task/CheckTask", Method.POST);
            client.Timeout = 10000;
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", token);
            response = client.Execute(request);
            content = response.Content;
            List<Models.TaskModel> res = GetData(content);
            if(res != null && res.Count != 0)
            {
                return res;
            }
            else
            {
                //Waite connect Network
                return new List<Models.TaskModel>();
            }
        }

        private List<Models.TaskModel> GetData(string respJsonStr)
        {
            List<Models.TaskModel> res = null;
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                res = JsonConvert.DeserializeObject<List<Models.TaskModel>>(responseAppS.
                        SelectToken("ResponseStr").ToString());
            }
            return res;
        }
    }
}