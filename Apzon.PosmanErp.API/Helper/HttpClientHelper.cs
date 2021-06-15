using Apzon.PosmanErp.Commons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace PosmanErp.Helper
{
    public class HttpClientHelper<TEntity> : IDisposable where TEntity : class
    {
        /// <summary>
        ///     HttpClient
        /// </summary>
        private HttpClient HttpClient { get; set; }

        /// <summary>
        /// API address
        /// </summary>
        public string ApiServer { get; set; }

        /// <summary>
        /// Controller Handling
        /// </summary>
        public string Controller { get; set; }

        public HttpClientHelper(string apiServer, string controler, string Token = null, int requestTimeout = 30)
        {
            if (string.IsNullOrEmpty(apiServer))
            {
                throw new ArgumentNullException(apiServer);
            }
            if (string.IsNullOrEmpty(controler))
            {
                throw new ArgumentNullException(controler);
            }
            ApiServer = apiServer;
            Controller = controler;
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Token", Token);
            HttpClient.Timeout = new TimeSpan(0, requestTimeout, 0);
        }

        /// <summary>
        /// dispose resource
        /// </summary>
        public void Dispose()
        {
            HttpClient.Dispose();
        }

        /// <summary>
        /// Hàm async Gọi API theo phương phức GET
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public async Task<TEntity> GetAsync(string action, object pr)
        {
            using (var response = await HttpClient.GetAsync(GetApiUri(ApiServer, Controller, action, pr)).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var resultJsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject(resultJsonString, typeof(TEntity));
                    return (TEntity)result;
                }
                throw new Exception(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// hàm async gọi api theo phương thức POST
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pr"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<TEntity> PostAsync(string action, object pr, string sessionId = "")
        {
            var uri = GetApiUri(ApiServer, Controller, action);
            using (var response = await HttpClient.PostAsJsonAsync(uri, pr))
            {
                if (response.IsSuccessStatusCode)
                {
                    var resultJsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TEntity>(resultJsonString);
                    return result;
                }
                throw new Exception(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Gọi API theo phương phức GET
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public TEntity Get(string action, object pr)
        {
            using (var response = HttpClient.GetAsync(GetApiUri(ApiServer, Controller, action, pr)).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var resultJsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<TEntity>(resultJsonString);
                    return result;
                }
                throw new Exception(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Gọi API theo phương thức POST
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public TEntity Post(string action, object pr)
        {
            var uri = GetApiUri(ApiServer, Controller, action);
            using (var response = HttpClient.PostAsJsonAsync(uri, pr).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var resultJsonString = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<TEntity>(resultJsonString);
                    return result;
                }
                throw new Exception(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// hàm lấy ủi của request dựa vào các thành phần của 1 controller
        /// </summary>
        /// <param name="apiServer"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="pr">tham số trên uri</param>
        /// <returns></returns>
        private string GetApiUri(string apiServer, string controller, string action, object pr = null)
        {
            if (string.IsNullOrEmpty(apiServer))
            {
                throw new Exception("Invalid api server");
            }
            if (string.IsNullOrEmpty(controller))
            {
                throw new Exception("Invalid api controller");
            }
            if (string.IsNullOrEmpty(action))
            {
                throw new Exception("Invalid api action");
            }
            if (pr == null)
                return string.Join("/", apiServer, controller, action);
            PropertyInfo[] property = pr.GetType().GetProperties();
            if (property != null && property.Length > 0)
            {
                string uri = string.Join("/", apiServer, controller, action) + "?";
                var lstPr = new List<string>();
                Type type = pr.GetType();
                foreach (PropertyInfo propertyInfo in property)
                {
                    string prValue = Function.ToString(type.GetProperty(propertyInfo.Name).GetValue(pr, null));
                    lstPr.Add(propertyInfo.Name + "=" + WebUtility.UrlEncode(prValue));
                }
                uri += string.Join("&", lstPr.ToArray());
                return uri;
            }
            return string.Join("/", apiServer, controller, action);
        }
    }
}
