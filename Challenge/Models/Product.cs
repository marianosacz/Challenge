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
        public string Code { get; internal set; }
        public string sku { get; set; }
        public string Sku { get; internal set; }
        public double stock { get; set; }
        public string Stock { get; internal set; }
        public string currency { get; set; }
        public Func<string> Currency { get; internal set; }
        public double price { get; set; }
        public string Price { get; internal set; }
        public double iva { get; set; }
        public string Iva { get; internal set; }
        public double ii { get; set; }
        public Func<string> Ii { get; internal set; }
    }
    public class ProductListResponse
    {
        public int total { get; set; }
        public int offset { get; set; }
        public List<Product> products { get; set; }
    }



}
