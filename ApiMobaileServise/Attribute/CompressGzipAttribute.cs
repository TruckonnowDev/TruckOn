using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApiMobaileServise.Attribute
{
    public class CompressGzipAttribute : ResultFilterAttribute, IActionFilter
    {
        public string ParamUnZip { get; set; }
        public bool IsCompresReqvest { get; set; }
        public bool IsCompresRespons { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //object paramReqvest = null;
            //if(IsCompresReqvest && ParamUnZip != null)
            //{
            //    context.HttpContext.Response.Headers.Add("CompresReqvest", "Yes");
            //    string[] paramsUnZip = ParamUnZip.Split(',');
            //    foreach(string paramUnZip in paramsUnZip)
            //    {
            //        paramReqvest = context.ActionArguments[paramUnZip] = UnCompress(context.ActionArguments[paramUnZip].ToString());
                   
            //    }
            //}
            //else
            //{
            //    context.HttpContext.Response.Headers.Add("CompresReqvest", "No");
            //}
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
            if (IsCompresRespons)
            {
                PropertyInfo propertyInfo = context.Result.GetType().GetProperty("Value");
                propertyInfo.SetValue(context.Result, Compress(Encoding.UTF8.GetBytes((string)propertyInfo.GetValue(context.Result, null)), CompressionLevel.Optimal));
                //context.HttpContext.Response.Headers.Add("Compres Respons", "gzip");
               // context.HttpContext.Response.Headers.Add("Level Compression Respons", "Optimal");
            }
        }


        private string Compress(byte[] data, CompressionLevel level)
        {
            string res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, level, leaveOpen: true))
                {
                    gz.Write(data, 0, data.Length);
                }
                res = Convert.ToBase64String(ms.ToArray());
            }
            return res;
        }

        private string UnCompress(string dataStr)
        {
            dataStr = dataStr.Replace("\"", "");
            byte[] data = Convert.FromBase64String(dataStr);
            string res = null;
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);

                res = Encoding.UTF8.GetString(resultStream.ToArray());
            }
            return res;
        }
    }
}