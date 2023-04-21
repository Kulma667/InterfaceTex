using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace testweb2
{
    class Program
    {
        static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(15);

        static async Task Main(string[] args)
        {
            while (true)
            {
                Dictionary<string, string> Params = new Dictionary<string, string>()
                {
                    { "AgentId","8"},
                    { "AgentPassword","AgentTest"},
                    { "AgentName","AgentTest"},
                    { "RequestType","GetSettingServices"},
                    //{ "From","2023-04-17 13:24:03"},
                    //{ "To","2023-04-17 16:24:03"},
                    //{ "TransactionId","110"},
                    //{ "Service","1"},
                    //{ "RequestDate","2023-04-17 13:24:03"},
                    //{"Amount","10" }
                };

                Console.WriteLine("Enter a condition to add a parameter to the request dictionary:");
                string condition = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(condition))
                {
                    Params.Add("NewParam", condition);
                }

                await semaphoreSlim.WaitAsync();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        httpClient.Timeout = TimeSpan.FromSeconds(60);
                        var responseTask = await httpClient.PostAsync("https://processing.hgg-pay.kz/home/index", new FormUrlEncodedContent(Params));
                        string json = await responseTask.Content.ReadAsStringAsync();
                        object a = JsonConvert.DeserializeObject(json);
                        Console.WriteLine(a);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Timeout occurred.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });
            }
        }
    }
}