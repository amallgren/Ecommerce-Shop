using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Product;
using Shop.Models;
using System.Diagnostics;
using static Product.Information;
using static Sales.Data;
using System.Text.Json;
using Grpc.Net.Client.Web;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var channel = GrpcChannel.ForAddress(_configuration.GetSection("Services:Product").Value, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
            var client = new InformationClient(channel);
            var reply = client.GetSetAsync(new Product.InformationOffering() { SetId = 1, CampaignId = 1 });
            InformationSet set = reply.ResponseAsync.Result;
            return View(set);
        }
        [HttpPost]
        public IActionResult SubmitOrder()
        {
            StreamReader reader = new StreamReader(Request.Body);
            OrderDetails details = JsonSerializer.DeserializeAsync<OrderDetails>(Request.Body).Result;
            Sales.Receipt receipt = new Sales.Receipt();
            receipt.OrderDetails = details.description;
            receipt.TransactionDetails = details.transactionDetails;
            var channel = GrpcChannel.ForAddress(_configuration.GetSection("Services:Sales").Value, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
            var client = new DataClient(channel);
            var reply = client.AddAsync(receipt);
            return Json(new { status = "Success" });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class OrderDetails
    {
        public string? description { get; set; }
        public string? transactionDetails { get; set; }
    }
}