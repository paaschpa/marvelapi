using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarvelApi;
using MarvelApi.Models;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Redis;

namespace Tests
{
    public class CharacterLoadFunctions
    {
        //as of 9.10 there are 1478 characters
        public void LoadCharactersFromApi()
        {
            var offset = 0;

            for (var i = 0; i <= 20; i++)
            {
                var ts = Guid.NewGuid();
                var hashString = ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"];
                var hash = hashString.GetMd5Hash();

                var url = "http://gateway.marvel.com/v1/public/characters"
                    .AddQueryParam("ts", ts)
                    .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                    .AddQueryParam("hash", hash)
                    .AddQueryParam("limit", 100)
                    .AddQueryParam("offset", offset)
                    .AddQueryParam("orderBy", "name");

                var resp = url.GetJsonFromUrl().FromJson<CharacterDataWrapper>();
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

        //Comic summaries for characters only contain 20 comics/items
        //This should fill in a character summary with any comic it finds in the comic.characters
        [Test]
        public void FillAllComicsArrayForCharacters()
        {
            using (var redisClient = new RedisClient())
            {
                var allCharacters = redisClient.GetAll<Character>(redisClient.SearchKeys("urn:Characters:*"));
                var charactersInOver20Comics = allCharacters.Values.Where(x => x.comics.available > 20);

                foreach (var character in charactersInOver20Comics)
                {
                    var allSummaries = getComicSummariesLocal(character.name);
                    character.comics.items = allSummaries.ToList();

                    redisClient.Set<Character>("urn:Characters:" + character.id, character);
                }
            }
        }

        public void FillInEventArraysForCharactersInMoreThan20Events()
        {
            using (var redisClient = new RedisClient())
            {
                var allCharacters = redisClient.GetAll<Character>(redisClient.SearchKeys("urn:Characters:*"));
                var over20events = allCharacters.Values.Where(x => x.events.available > 20).ToList();

                foreach (var character in over20events)
                {
                    var ts = Guid.NewGuid();
                    var hashString = ts + ConfigurationManager.AppSettings["marvelPrivateKey"] + ConfigurationManager.AppSettings["marvelPublicKey"];
                    var hash = hashString.GetMd5Hash();
                    var url = "http://gateway.marvel.com/v1/public/characters/" + character.id + "/events"
                    .AddQueryParam("ts", ts)
                        .AddQueryParam("apikey", "de057f1f51e36402aeeafea0fd5a5936")
                        .AddQueryParam("hash", hash)
                        .AddQueryParam("limit", 100);

                    try
                    {
                        var resp = url.GetJsonFromUrl().FromJson<EventDataWrapper>();
                        
                        character.events.items = resp.data.results.Select(x => new EventSummary()
                        {
                            resourceURI = x.resourceURI,
                            name = x.title
                        }).ToList();

                        redisClient.Set("urn:Characters:" + character.id, character);
                    }
                    catch (Exception ex)
                    {
                        //swallow exception 
                        Console.WriteLine(ex);
                    }
                }

            }
        }

        private IDictionary<string, Comic> _comicsCache;
        public IEnumerable<ComicSummary> getComicSummariesLocal(string name)
        {
            if (_comicsCache == null)
            {
                using (var redisClient = new RedisClient())
                {
                    _comicsCache = redisClient.GetAll<Comic>(redisClient.SearchKeys("urn:Comics:*"));
                }
            }

            var filtered = _comicsCache.Values.Where(x => x.characters.items.Any(y => y.name == name));

            return filtered.Select(x => new ComicSummary()
                {
                    name = x.title,
                    resourceURI = "http://gateway.marvel.com/v1/public/comics/" + x.id
                });
        }
    }
}
