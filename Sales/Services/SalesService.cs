using Grpc.Core;
using DataLibrary;

namespace Sales.Services
{
    public class SalesService : Data.DataBase
    {
        private string? _connectionString;
        private readonly ILogger<SalesService> _logger;
        private IConfiguration _configuration;
        public SalesService(ILogger<SalesService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Shop");
        }
        public override Task<SalesReply> Add(Receipt data, ServerCallContext context)
        {
            int salesId = 0;
            if (_connectionString is not null)
            {
                var shopDbContext = new ShopDbContext(_connectionString);
                DataLibrary.Order order = new Order() { Description = data.OrderDetails, TransactionDetails = data.TransactionDetails };
                shopDbContext.Orders.Add(order);
                shopDbContext.SaveChanges();
                salesId = order.OrderId;
            }
            SalesReply reply = new SalesReply() { SalesId = salesId };
            return Task.FromResult(reply);
        }
    }
}