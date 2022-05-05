using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge.Models
{

    public class Product
    {
        public string code { get; set; }
        public string sku { get; set; }
        public double stock { get; set; }
        public string currency { get; set; }
        public double price { get; set; }
        public double iva { get; set; }
        public double ii { get; set; }
    }
    public class ProductListResponse
    {
        public int total { get; set; }
        public int offset { get; set; }
        public List<Product> products { get; set; }
    }



}
