using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.General
{
    public class BackendAPI
    {
        public static bool GetAsync(string url, string usuario = null, string token = null)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(token))
                {
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                var status = response.IsSuccessStatusCode;

                if (!status) throw new Exception("Error en la respuesta: " + result);

                return status;
            }
        }

        public static bool PostAsync<T>(string url, T data, string usuario = null, string token = null)
        {
            using (var client = new HttpClient()) {
                if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(token))
                {
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = client.PostAsync(url, content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                var status = response.IsSuccessStatusCode;

                if (!status) throw new Exception("Error en la respuesta: " + result);

                return status;
            }
        }

    }
}
