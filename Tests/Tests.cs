using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MarvelApi.Models;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Redis;

namespace Tests
{
    public class Tests {
        [Test]
        public void writeToRedis()
        {
            var offset = 0;

            for (var i = 0; i <= 100; i++)
            {
                var ts = Guid.NewGuid();
                var hash = GetMd5Hash(MD5.Create(),
                                      ts + ConfigurationManager.AppSettings["marvelPrivateKey"] +
                                      ConfigurationManager.AppSettings["marvelPublicKey"]);
                var url = "http://gateway.marvel.com/v1/public/comics"
                    .AddQueryParam("ts", ts)
                    .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                    .AddQueryParam("hash", hash)
                    .AddQueryParam("limit", 100)
                    .AddQueryParam("offset", offset)
                    .AddQueryParam("orderBy", "issueNumber");

                var resp = url.GetJsonFromUrl().FromJson<ComicDataWrapper>();
                offset += 100;

                using (var redisClient = new RedisClient())
                {
                    foreach(var c in resp.data.results)
                    {
                        redisClient.Set("urn:Comics:" + c.id, c);
                    }
                }
            }
        }

        [Test]
        public void readComicsFromRedis()
        {
            using (var redisClient = new RedisClient())
            {
                var comics = redisClient.GetAll<Comic>(redisClient.SearchKeys("urn:Comics:*"));
                Assert.IsNotNull(comics);
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}
