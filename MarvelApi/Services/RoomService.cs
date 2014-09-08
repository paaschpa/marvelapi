using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MarvelApi.Models;
using ServiceStack;

namespace MarvelApi.Services
{
    [Route("/Room")]
    public class RoomRequest
    {
        public Guid FindId { get; set; }
    }

    public class RoomService : Service
    {
        public Room Get(RoomRequest request)
        {
            if (request.FindId == Guid.Empty)
            {
                return new Room()
                {
                    Id = Guid.NewGuid(),
                    Description = "Go North young man!",
                    Neighbors = new Dictionary<string, Guid>
                        {
                            {"N", Guid.Empty},
                            {"S", Guid.Empty},
                            {"E", Guid.Empty},
                            {"W", Guid.Empty}
                        }
                };
            }
            return new Room()
                {
                    Id = Guid.NewGuid(),
                    Description = "So you like reading comics do ya?",
                    Neighbors = new Dictionary<string, Guid>
                        {
                            {"N", Guid.Empty},
                            {"S", Guid.Empty},
                            {"E", Guid.Empty},
                            {"W", Guid.Empty}
                        }
                };
        }
       
    }
}