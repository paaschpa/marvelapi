using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarvelApi.Models
{
    public class CharacterDataWrapper
    {
        public int code { get; set; }
        public string status { get; set; }
        public string copyright { get; set; }
        public string attributionText { get; set; }
        public string attributionHTML { get; set; }
        public CharacterDataContainer data { get; set; }
        public string etag { get; set; }
    }

    public class CharacterDataContainer
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public List<Character> results { get; set; }
    }

    public class Character
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string modified { get; set; }
        public string resourceURI { get; set; }
        public List<Url> urls { get; set; }
        public Image thumbnail { get; set; }
        public ComicList comics { get; set; }
        public StoryList stories { get; set; }
        public EventList events { get; set; }
        public SeriesList series { get; set; }
    }
}