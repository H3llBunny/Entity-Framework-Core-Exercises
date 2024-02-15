using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShopXML.Models
{
    public class Product
    {
        public Product()
        {
            this.CategoryProducts = new List<CategoryProduct>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int SellerId { get; set; }
        public User Seller { get; set; }

        public int? BuyerId { get; set; }
        public User Buyer { get; set; }

        public ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}
