using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarvelApi.Models
{
    public class ComicDataWrapper
    {
        public int Code { get; set; }
        public string status { get; set; }
        public string copyright { get; set; }
        public string attributionText { get; set; }
        public string attributionHTML { get; set; }
        public ComicDataContainer data { get; set; }
        public string etag { get; set; }
    }

    public class ComicDataContainer
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public List<Comic> results { get; set; }
    }

    public class Comic
    {
        public int id { get; set; }
        public int digitalid { get; set; }
        public string title { get; set; }
        public double issueNumber { get; set; }
        public string variantDescription { get; set; }
        public string description { get; set; }
        public string modified { get; set; }
        public string isbn { get; set; }
        public string upc { get; set; }
        public string diamondCode { get; set; }
        public string ean { get; set; }
        public string issn { get; set; }
        public string format { get; set; }
        public int pageCount { get; set; }
        public List<TextObject> textObjects { get; set; }
        public string resourceURI { get; set; }
        public List<Url> urls { get; set; }
        public SeriesSummary series { get; set; }
        public List<ComicSummary> variants { get; set; }
        public List<ComicSummary> collections { get; set; }
        public List<ComicSummary> collectedIssues { get; set; }
        public List<ComicDate> dates { get; set; }
        public List<ComicPrice> prices { get; set; }
        public Image thumbnail { get; set; }
        public List<Image> images { get; set; }
        public CreatorList creators { get; set; }
        public CharacterList characters { get; set; }
        public StoryList stories { get; set; }
        public EventList events { get; set; }
    }
}