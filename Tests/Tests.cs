﻿using System;
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
        public void writeToRedisComics()
        {
            var offset = 24600;

            for (var i = 0; i <= 400; i++)
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

                var st = DateTime.Now;
                Console.WriteLine(st);
                var resp = url.GetJsonFromUrl().FromJson<ComicDataWrapper>();
                Console.WriteLine(offset);
                Console.WriteLine(DateTime.Now - st);
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
        public void writeToRedisCharacter()
        {
            var offset = 0;

            for (var i = 0; i <= 20; i++)
            {
                var ts = Guid.NewGuid();
                var hash = GetMd5Hash(MD5.Create(),
                                      ts + ConfigurationManager.AppSettings["marvelPrivateKey"] +
                                      ConfigurationManager.AppSettings["marvelPublicKey"]);
                var url = "http://gateway.marvel.com/v1/public/characters"
                    .AddQueryParam("ts", ts)
                    .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                    .AddQueryParam("hash", hash)
                    .AddQueryParam("limit", 100)
                    .AddQueryParam("offset", offset)
                    .AddQueryParam("orderBy", "name");

                var st = DateTime.Now;
                Console.WriteLine(st);
                var resp = url.GetJsonFromUrl().FromJson<CharacterDataWrapper>();
                Console.WriteLine(offset);
                Console.WriteLine(DateTime.Now - st);
                offset += 100;

                using (var redisClient = new RedisClient())
                {
                    foreach (var c in resp.data.results)
                    {
                        redisClient.Set("urn:Characters:" + c.id, c);
                    }
                }
            }
        }

        [Test]
        public void GetAllWolverineComics()
        {
            //currently character summaries are limited to 20...get ALL
            using (var redisClient = new RedisClient())
            {
                var wolverine = redisClient.Get<Character>("urn:Characters:1009718");

                var allSummaries = getComicSummaries(wolverine.comics.collectionURI, wolverine.comics.available);

                wolverine.comics.items = allSummaries.ToList();

                redisClient.Set<Character>("urn:Characters:1009718", wolverine);
            }
        }

        private IList<ComicSummary> getComicSummaries(string collectionUrl, int total)
        {
            int loopTimes = (total/100) + 1;
            var offset = 0;
            var ret = new List<ComicSummary>();
            for (var i = 0; i < loopTimes; i++)
            {
                var ts = Guid.NewGuid();
                var hash = GetMd5Hash(MD5.Create(),
                                      ts + ConfigurationManager.AppSettings["marvelPrivateKey"] +
                                      ConfigurationManager.AppSettings["marvelPublicKey"]);
                var url = collectionUrl
                    .AddQueryParam("ts", ts)
                    .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                    .AddQueryParam("hash", hash)
                    .AddQueryParam("limit", 100)
                    .AddQueryParam("offset", offset);

                var st = DateTime.Now;
                Console.WriteLine(st);
                try
                {
                    var resp = url.GetJsonFromUrl().FromJson<ComicDataWrapper>();
                    Console.WriteLine(offset);
                    Console.WriteLine(DateTime.Now - st);
                    offset += 100;
                    ret.AddRange(resp.data.results.Select(x => new ComicSummary() {name = x.title, resourceURI = "http://gateway.marvel.com/v1/public/comics/" + x.id}));
                }
                catch (Exception ex)
                {
                    //Swallow Exception
                    Console.WriteLine(ex); 
                }
            }

            return ret;
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

        [Test]
        public void readCharacterFromRedis()
        {
            using (var redisClient = new RedisClient())
            {
                var wolverine = redisClient.Get<Character>("urn:Characters:1009718");
                var wolverineComicIds = wolverine.comics.items.Select(x => x.resourceURI.Replace("http://gateway.marvel.com/v1/public/comics/", "urn:Comics:"));
                var wolverineComics = redisClient.GetAll<Comic>(wolverineComicIds).Where(x => x.Value != null).Select(x => x.Value);
                var o = wolverineComics.Where(x=> x.onSaleDate != null).OrderBy(x => x.onSaleDate).ToList();
                Assert.IsNotNull(wolverine);
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
