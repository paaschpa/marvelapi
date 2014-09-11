using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MarvelApi;
using MarvelApi.Models;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Redis;

namespace Tests
{
    public class ComicLoadFunctions
    {
        //Call api several hundred times to pull down comics and insert into Redis
        public void LoadComicsFromApi()
        {
            //32699 total comics 8.20.2014
            var offset = 0;
            for (var i = 0; i <= 330; i++)
            {
                var ts = Guid.NewGuid();
                var hashString = ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"];
                var hash = hashString.GetMd5Hash();

                var url = "http://gateway.marvel.com/v1/public/comics"
                    .AddQueryParam("ts", ts)
                    .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                    .AddQueryParam("hash", hash)
                    .AddQueryParam("limit", 100)
                    .AddQueryParam("offset", offset);

                try
                {
                    var resp = url.GetJsonFromUrl().FromJson<ComicDataWrapper>();
                    offset += 100;

                    using (var redisClient = new RedisClient())
                    {
                        foreach (var c in resp.data.results)
                        {
                            redisClient.Set("urn:Comics:" + c.id, c);
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

        //~150 comics, max characters 92
        public void FillInCharacterArraysForComicsWithOver20Characters()
        {
            using (var redisClient = new RedisClient())
            {
                var allComics = redisClient.GetAll<Comic>(redisClient.SearchKeys("urn:Comics:*"));
                var comicsOver20 = allComics.Values.Where(x => x.characters.available > 20);

                foreach (var comic in comicsOver20)
                {
                    var ts = Guid.NewGuid();
                    var hashString = ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"];
                    var hash = hashString.GetMd5Hash();

                    var url = "http://gateway.marvel.com/v1/public/comics/" + comic.id + "/characters"
                    .AddQueryParam("ts", ts)
                        .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                        .AddQueryParam("hash", hash)
                        .AddQueryParam("limit", 100);

                    try
                    {
                        var resp = url.GetJsonFromUrl().FromJson<CharacterDataWrapper>();
                        comic.characters.items = resp.data.results.Select(x => new CharacterSummary()
                        {
                            resourceURI = x.resourceURI,
                            name = x.name
                        }).ToList();

                        redisClient.Set("urn:Comics:" + comic.id, comic);
                    }
                    catch (Exception ex)
                    {
                        //swallow exception 
                        Console.WriteLine(ex);
                    }
                }
            }
        }

    }
}
