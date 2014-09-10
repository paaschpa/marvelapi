using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MarvelApi.Models;
using ServiceStack;
using ServiceStack.Redis;

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
            using (var redisClient = new RedisClient())
            {
                var room = redisClient.Get<Room>("urn:Rooms:" + request.FindId.ToString());
                return room;
            }
        }
       
    }
}