using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarvelApi.Models;
using NUnit.Framework;
using ServiceStack.Redis;

namespace Tests
{
    [TestFixture]
    public class RoomTests
    {
        [Test]
        public void AddThreeRooms()
        {
            var room1Id = Guid.NewGuid();
            var roomNId = Guid.NewGuid();
            var roomEId = Guid.NewGuid();

            var room = new Room()
                {
                    Id = room1Id,
                    Description = "The Main Room",
                    Neighbors = new Dictionary<string, Guid>
                        {
                            {"N", roomNId},
                            {"E", roomEId}
                        },
                    ComicIds = new List<int> {28691, 17122, 20047, 49686 }
                };

            var roomN = new Room()
                {
                    Id = roomNId,
                    Description = "The North Room",
                    Neighbors = new Dictionary<string, Guid>()
                        {
                            {"S", room1Id}
                        },
                    ComicIds = new List<int>() {39959, 12169, 22619, 49476}
                };

            var roomE = new Room()
                {
                    Id = roomEId,
                    Description = "The East Room",
                    Neighbors = new Dictionary<string, Guid>()
                        {
                            {"W", room1Id}
                        },
                    ComicIds = new List<int>() {23773, 8520, 22089, 18055, 50891}
                };

            using (var redisClient = new RedisClient())
            {
                redisClient.Set("urn:Rooms:" + room.Id, room);
                redisClient.Set("urn:Rooms:" + roomN.Id, roomN);
                redisClient.Set("urn:Rooms:" + roomE.Id, roomE);
            }
        }

    }
}