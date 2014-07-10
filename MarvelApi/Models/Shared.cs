using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarvelApi.Models
{
    public class TextObject
    {
        public string type { get; set; }
        public string language { get; set; }
        public string text { get; set; }
    }

    public class Url
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class SeriesSummary
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
    }

    public class ComicSummary
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
    }

    public class ComicDate
    {
        public string type { get; set; }
        public string date { get; set; }

        public DateTime? GetDate()
        {
            DateTime dt;
            if(DateTime.TryParse(date, out dt))
                return dt;

            return null;
        }
    }

    public class ComicPrice
    {
        public string type { get; set; }
        public float price { get; set; }
    }

    public class Image
    {
        public string path { get; set; }
        public string extension { get; set; }
    }

    public class CreatorList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<CreatorSummary> items { get; set; }
    }

    public class CreatorSummary
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
        public string role { get; set; }
    }

    public class CharacterList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<CharacterSummary> items { get; set; }
    }

    public class CharacterSummary
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
        public string role { get; set; }
    }

    public class StoryList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<StorySummary> items { get; set; }
    }

    public class StorySummary
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class EventList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<EventSummary> items { get; set; }
    }

    public class EventSummary
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
    }

    public class ComicList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<ComicSummary> items { get; set; }
    }

    public class SeriesList
    {
        public int available { get; set; }
        public int returned { get; set; }
        public string collectionURI { get; set; }
        public List<SeriesSummary> items { get; set; }
    }
}