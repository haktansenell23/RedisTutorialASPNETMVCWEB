using Microsoft.AspNetCore.Mvc;
using Microsoft.NET.StringTools;
using RedisTutorial.StackExchange.Services;
using StackExchange.Redis;

namespace RedisTutorial.StackExchange.Controllers
{
    public class ProductController : Controller
    {
        private readonly RedisService redisService;
        private string listKey = "hashName";
        public ProductController(RedisService redisService)
        {
            this.redisService = redisService;



        }

        public IActionResult Index()
        {
            IDatabase db = this.redisService.GetDb(0);

            db.StringSet("fener", 100);


            return View();
        }

        public IActionResult StringType()
        {
            //0 . Dbye girdik 
            IDatabase db = this.redisService.GetDb(0);
            // String type gitirdik 
            ViewBag.fener = db.StringGet("fener").ToString();
            // Valueyi arttırma
            db.StringIncrement("fener", 1);
            // Valueyi azaltma
            db.StringDecrement("fener", 3);
            // Range aralığındaki valueleri alma
            var range = db.StringGetRangeAsync("fener", 0, 2).Result;

            return View();
        }
        [HttpPost]
        public IActionResult RedisList(string name)
        {
            // C# taki karşılığı linked list 'tir.
            IDatabase db = this.redisService.GetDb(0);
          
            List<String>strings = new List<String> { "sdffdsfsdfds","sdffdssfdasdD  QWE","  QW  R"};

            // En sona ekleme
            foreach (var item in strings)
            {
                db.ListRightPush(listKey,item);
            }
            // Data çeker ve siler
            ViewBag.Data=db.ListRightPop(listKey);


            return View();
        }
        [HttpGet]
        public IActionResult RedisList()
        {
          

            return View();


        }
        [HttpGet]
        public IActionResult RedisSetList()
        {
            IDatabase database = this.redisService.GetDb(0);

            

           var list1 = database.SetMembers(listKey).ToList();
            return View();
        }
        [HttpPost]
        public IActionResult RedisSetList(string name)
        {
            IDatabase database = this.redisService.GetDb(0);
            database.KeyExpire(listKey, DateTime.Now.AddMinutes(5));
            database.SetAdd(listKey, name);


            return View();
        }
        public IActionResult SortedSet()
        {
            IDatabase database = this.redisService.GetDb(0);

            HashSet<string> keys = new HashSet<string>();
            HashSet<string> sortedSetByRank = new HashSet<string>();

            string listSortedKey = "sortedset";

            //Redisteki sıralama
            database.SortedSetScan(listSortedKey).ToList().ForEach(x =>
            {
                keys.Add(x.ToString());
                database.SortedSetRemove(listSortedKey,x.ToString());
            });

            //Score 'a göre küçükten büyüğüe sıralama
            database.SortedSetRangeByRank(listSortedKey,order:Order.Descending).ToList().ForEach(x =>
            {
                sortedSetByRank.Add(x.ToString());
                database.SortedSetRemove(listSortedKey, x.ToString());
            });


            return View();
        }
        [HttpPost]
        public IActionResult SortedSet(string name,double score)
        {

            IDatabase database = this.redisService.GetDb(0);

             string listSortedKey = "sortedset";
            database.SortedSetAdd(listSortedKey, name, score);
            return View();
        }
        [HttpGet]
        public IActionResult HashType()
        {

            IDatabase database = this.redisService.GetDb(0);

            string hashTypeKey = "hashType";
            Dictionary<string,string>list = new Dictionary<string,string>();

            database.HashGetAll(hashTypeKey).ToList().ForEach(x =>
            {
                list.Add(x.Name,x.Value);
            });

            return View(list);
        }

        [HttpPost]
        public IActionResult HashType(string key, string val)
        {
            IDatabase database = this.redisService.GetDb(0);
            string hashTypeKey = "hashType";

            database.HashSet(hashTypeKey, key,val);
            Dictionary<string, string> list = new Dictionary<string, string>();

            database.HashGetAll(hashTypeKey).ToList().ForEach(x =>
            {
                list.Add(x.Name, x.Value);
            });
            if(list!=null)
            {
                return View(list);

            }

            return View();
        }

    }
}
