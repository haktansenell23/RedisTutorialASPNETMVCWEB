using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NuGet.Protocol;
using RedisTutorial.DistributedCache.Models;
using System.Drawing;
using System.Text;

namespace RedisTutorial.DistributedCache.Controllers
{
    public class ProductController : Controller
    {
        private IDistributedCache _distributedCache;

        public ProductController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        


        public IActionResult Index()
        {
            DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions(); 

            cacheOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(10);    

            _distributedCache.SetString("date", DateTime.Now.ToString());

            ViewBag.distrubedCacheValue = _distributedCache.GetString("date");


            return View();
        }



        public IActionResult RemMemory()
        {
              
            _distributedCache.Remove("date");

            return View();
        }




        public IActionResult ClassSerialize()
        {
            //Sadece JSON ile get ve set
            Product product = new Product { Id = 1, Name = "pen", Description = "fddssf" };
            string jsonProduct = JsonConvert.SerializeObject(product);
            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions(); 
            distributedCacheEntryOptions.AbsoluteExpiration= DateTime.Now.AddMinutes(10);   
            _distributedCache.SetString("product:1", jsonProduct,distributedCacheEntryOptions);
            string productJson = _distributedCache.GetString("product:1");
            Product deserializeProduct = JsonConvert.DeserializeObject<Product>(productJson);
            ViewBag.Product = deserializeProduct;

            //Byte ile set ve get 
            Product product1 = new Product { Id = 2, Name = "FASDDAS", Description = "ASDSDA" };
            string jsonProduct1 = JsonConvert.SerializeObject(product1);
            Byte[]byteProduct = Encoding.UTF8.GetBytes(jsonProduct1);
            _distributedCache.Set("product:2", byteProduct, distributedCacheEntryOptions);
            Byte[]Product = _distributedCache.Get("product:2");
            string Product1String = Encoding.UTF8.GetString(Product);
           Product viewBagProduct1 = JsonConvert.DeserializeObject<Product >(Product1String);
            ViewBag.Product1 = viewBagProduct1;

                



            return View();
        }
        public IActionResult FileSerialize()
        {

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ouroborus.jpg");

            byte[] imageByte;
           imageByte = System.IO.File.ReadAllBytes(filePath);

            _distributedCache.Set("image:1", imageByte);




            return View();
        }

        public IActionResult GetFile()
        {
            byte[] imagesByte=_distributedCache.Get("image:1");
           
            
            return File(imagesByte,"image/jpg");  
        }


    }
}
