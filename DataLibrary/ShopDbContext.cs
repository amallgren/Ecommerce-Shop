using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Product>? Products { get; set; }
        public DbSet<Product_Category>? Product_Categories { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Campaign>? Campaigns { get; set; }
        public DbSet<Product_Campaign>? Product_Campaigns { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public IConfiguration? _configuration { get; }
        public string _connectionString { get; set; }
        public ShopDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString is not null)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
    }

    public class Product_Category
    {
        public int Product_CategoryId { get; set; }
        public Product? Product { get; set; }
        public Category? Category { get; set; }
    }
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    public class Product_Campaign
    {
        public int Product_CampaignId { get; set; }
        public Product? Product { get; set; }
        public Campaign? Campaign { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal Price { get; set; }
    }
    public class Campaign
    {
        public int CampaignId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    public class Order
    { 
        public int OrderId { get; set; }
        public string? Description { get; set; }
        public string? TransactionDetails { get; set; }
    }

}