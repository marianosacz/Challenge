using Challenge.Data;
using Challenge.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {   
            while (!stoppingToken.IsCancellationRequested)
            {
                // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                GetProduct();
                await Task.Delay(600000, stoppingToken); // 10 minutos = 600000 milisegundos
            }
        }

        private async void GetProduct()
        {   

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-apikey", "081ae9a0-bb54-4621-8475-9e49f18d2413");
                var response = await client.GetAsync("https://apilab.distecna.com/Product");
                if (response.IsSuccessStatusCode)
                {
                    //_logger.LogInformation(await response.Content.ReadAsStringAsync());

                    ProductListResponse products = JsonConvert.DeserializeObject<ProductListResponse>(await response.Content.ReadAsStringAsync());
                    foreach (Product product in products.products)
                    {
                        _logger.LogInformation($"Code: {product.code}, Sku: {product.sku}, Stock: {product.stock}, Currency: {product.currency}, Price: {product.price}, Iva: {product.iva}, Ii: {product.ii}");

                        ProductData.Registrar(product);//Guarda los datos
                        ProductData.Actualizar(product);
                    }

                    

                }
                
                else
                {
                    _logger.LogInformation($"Error al consumir Api {response.StatusCode}");
                }
            }
        }


    }
}
