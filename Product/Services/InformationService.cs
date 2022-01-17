using Grpc.Core;
using Product;
using DataLibrary;

namespace Product.Services
{
    public class InformationService : Information.InformationBase
    {
        private string? _connectionString;
        private readonly ILogger<InformationService> _logger;
        private IConfiguration _configuration;
        public InformationService(ILogger<InformationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Shop");
        }
        
        public override Task<InformationReply> Add(InformationEntity entity, ServerCallContext context)
        {
            int productId = 0;
            if (_connectionString is not null)
            {
                var shopDbContext = new ShopDbContext(_configuration.GetConnectionString("Shop"));
                DataLibrary.Product product = new DataLibrary.Product() { Name = entity.Name, Description = entity.Description, Image = entity.Image.ToByteArray() };
                if (shopDbContext is not null && shopDbContext.Products is not null)
                {
                    shopDbContext.Products.Add(product);
                    shopDbContext.SaveChanges();
                    productId = product.ProductId;
                }
            } 
            return Task.FromResult(new InformationReply
            {
                ProductId = productId
            });
        }
        public override Task<InformationSet> GetSet(InformationOffering offering, ServerCallContext context)
        {
            InformationSet set = new InformationSet();
            if (_connectionString is not null)
            {
                var shopDbContext = new ShopDbContext(_configuration.GetConnectionString("Shop"));
                if (shopDbContext is not null && shopDbContext.Products is not null)
                {
                    var items = from p in shopDbContext.Products
                                join pcat in shopDbContext.Product_Categories
                                    on p.ProductId equals pcat.Product.ProductId
                                join pcam in shopDbContext.Product_Campaigns
                                    on p.ProductId equals pcam.Product.ProductId
                                where pcat.Category.CategoryId == offering.SetId
                                    && pcam.Campaign.CampaignId == offering.CampaignId
                                select new { ProductId = p.ProductId, Name = p.Name, Description = p.Description, Image = p.Image, Price = pcam.Price };

                    foreach (var item in items)
                    {
                        InformationEntity entity = new InformationEntity();
                        entity.Id = item.ProductId;
                        entity.Name = item.Name;
                        entity.Description = item.Description;
                        entity.Image = Google.Protobuf.ByteString.CopyFrom(item.Image);
                        entity.Price = (double)item.Price;
                        set.Items.Add(entity);
                    }
                }
            }
            return Task.FromResult(set);
        }
    }
}