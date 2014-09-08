using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarvelApi.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public IDictionary<string, Guid> Neighbors { get; set; }
    }
}