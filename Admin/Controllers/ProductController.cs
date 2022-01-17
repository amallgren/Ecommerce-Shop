using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using static Product.Information;

namespace Admin.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        [ActivatorUtilitiesConstructorAttribute]
        public ProductController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Admin.Models.Product product)
        {
            Product.InformationEntity request = new Product.InformationEntity();
            request.Description = product.Description;
            request.Name = product.Name;
            if (Request.Form.Files["ImageData"] is not null)
            { 
                using (BinaryReader br = new BinaryReader(Request.Form.Files["ImageData"].OpenReadStream()))
                {
                    request.Image = Google.Protobuf.ByteString.CopyFrom(br.ReadBytes((int)Request.Form.Files["ImageData"].Length));
                }
            }
            var channel = GrpcChannel.ForAddress(_configuration.GetSection("Services:Product").Value);
            var client = new InformationClient(channel);
            var reply = client.AddAsync(request);
            return View();
        }
    }
}
