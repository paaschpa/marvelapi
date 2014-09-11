using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarvelApi;
using MarvelApi.Models;
using ServiceStack;
using ServiceStack.Redis;

namespace Tests
{
    public class EventLoadFunctions
    {
        //There are only 64 Events as of 9.10.2014
        public void LoadEventsFromApi()
        {
            var ts = Guid.NewGuid();
            var hashString = ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"];
            var hash = hashString.GetMd5Hash();

            var url = "http://gateway.marvel.com/v1/public/events";
            url = url.AddQueryParam("ts", ts)
                     .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                     .AddQueryParam("hash", hash)
                     .AddQueryParam("limit", 100);

            try
            {
                var resp = url.GetJsonFromUrl().FromJson<EventDataWrapper>();

                using (var redisClient = new RedisClient())
                {
                    foreach (var c in resp.data.results)
                    {
                        redisClient.Set("urn:Events:" + c.id, c);
                    }
                }
            }
            catch (Exception ex)
            {
                //swallow exception 
                Console.WriteLine(ex);
            }
        }
    }
}
