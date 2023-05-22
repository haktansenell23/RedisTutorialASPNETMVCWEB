using StackExchange.Redis;
using System.Runtime.CompilerServices;

namespace RedisTutorial.StackExchange.Services
{
    public class RedisService
    {
        private readonly string _connectionsString;

        private ConnectionMultiplexer _redis;

        public RedisService( string connectionsString)
        {
           
            _connectionsString = connectionsString;
            _redis = ConnectionMultiplexer.Connect(_connectionsString);


        }

        public IDatabase db { get; set; }
        public IDatabase GetDb(int db)
        {
            return _redis.GetDatabase(db);
        }

    }
}
