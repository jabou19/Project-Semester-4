using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ST4_ImplementationExamples
{
    public class REST
    {
        private static string url = "http://localhost:8082/v1/status";

        public REST()
        {
        }

        //init REST
        private RestClient client = new RestClient("http://localhost:8082");
        private RestRequest Request = new RestRequest("v1/status/");

        /*
        //runner
        public async Task RunExample()
        {
            
            GetStatus();
        }
        */

        //test PUT request
        public async void ChooseOperation(int i)
        {
            var httpRequest = (HttpWebRequest) WebRequest.Create(url);
            httpRequest.Method = "PUT";

            httpRequest.ContentType = "application/json";

            string operation = "";

            switch (i)
            {
                case 1:
                {
                    operation = Operations.MoveToStorageOperation.ToString();
                    break;
                }
                case 2:
                {
                    operation = Operations.PickWarehouseOperation.ToString();
                    break;
                }
                case 3:
                {
                    operation = Operations.PutWarehouseOperation.ToString();
                    break;
                }
                case 4:
                {
                    operation = Operations.MoveToAssemblyOperation.ToString();
                    break;
                }
                case 5:
                {
                    operation = Operations.PutAssemblyOperation.ToString();
                    break;
                }
                case 6:
                {
                    operation = Operations.PickAssemblyOperation.ToString();
                    break;
                }
                case 7:
                {
                    operation = Operations.MoveToChargerOperation.ToString();
                    break;
                }
            }

            var msg = $@"{{
                ""Program name"": ""{operation}"",
                ""State"": 1
            }}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(msg);
            }

            //DONT FUCKING DELETE OR IT WILL BREAK!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //ps. don't know why *wondering*
            //if it's stupid but it works it ain't stupid
            var httpResponse = (HttpWebResponse) httpRequest.GetResponse();
        }

        public void goToWarehouse()
        {
            var msg = new OperationMessage();
            msg.Programname = "Go to warehouse";
            msg.State = 1;
            RestRequest request = new RestRequest("v1/status/");
            String json = JsonConvert.SerializeObject(msg);
            request.AddJsonBody(json, DataFormat.Json.ToString());
            client.ExecutePutAsync(request).Wait();
        }

        //test status method
        public async void GetStatus()
        {
            //GET request
            RestResponse response = await client.GetAsync(Request);
            Console.WriteLine("GET request response: " + response.Content);

            //dynamic msg = JsonConvert.DeserializeObject(response);
        }

        public async void execute()
        {
            var httpRequest = (HttpWebRequest) WebRequest.Create(url);
            httpRequest.Method = "PUT";

            httpRequest.ContentType = "application/json";

            var msg = $@"{{
                ""State"": 2
            }}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(msg);
            }

            //DONT FUCKING DELETE OR IT WILL BREAK!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //ps. don't know why *wondering*
            //if it's stupid but it works it ain't stupid
            var httpResponse = (HttpWebResponse) httpRequest.GetResponse();
        }

        public async void CheckBattery()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                    
                HttpResponseMessage responseMessage = client.GetAsync("").Result;

                if (responseMessage.IsSuccessStatusCode)
                {
                    var contents = await responseMessage.Content.ReadAsStringAsync();

                    string s = contents;
                    string[] value = s.Split(',');
                    var battery = Convert.ToInt32(value[0].Trim('"', ':').Remove(0, 11));
                    Console.WriteLine("Battery: " + battery);

                    if (battery < 20)
                    {
                        ChooseOperation(7);
                        execute();
                        Thread.Sleep(15000);
                    }
                }
            }
        }
    }

    //class to serialize json objects
    public class OperationMessage
    {
        //tag forces the name of the json attribute on serialization to the specified PropertyName
        [JsonProperty(PropertyName = "Program name")]
        public string Programname { get; set; }

        public int State { get; set; }
    }
}