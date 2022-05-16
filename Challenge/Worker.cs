using Challenge.Data;
using Challenge.Exceptions;
using Challenge.Interface;
using Challenge.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        private readonly ILogger<Worker> _logger;
        private readonly IProductData _serviceProduct;

        public Worker(ILogger<Worker> logger, IProductData serviceProduct, IConfiguration configuration)
        {
            _config = configuration;
            _logger = logger;
            _serviceProduct = serviceProduct;
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
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(_config.GetValue<string>("Api:Auth"), _config.GetValue<string>("Api:Headers"));
                    var response = await client.GetAsync(_config.GetValue<string>("Api:URL"));

                    if (response.IsSuccessStatusCode)
                    {
                        //_logger.LogInformation(await response.Content.ReadAsStringAsync());

                        ProductListResponse products = JsonConvert.DeserializeObject<ProductListResponse>(await response.Content.ReadAsStringAsync());
                        foreach (Product product in products.products)
                        {
                            _logger.LogInformation($"Code: {product.code}, Sku: {product.sku}, Stock: {product.stock}, Currency: {product.currency}, Price: {product.price}, Iva: {product.iva}, Ii: {product.ii}");

                            _serviceProduct.Registrar(product);//Guarda los datos
                            _serviceProduct.Actualizar(product);
                        }
                    }

                    else
                    {
                        _logger.LogInformation($"Error al consumir Api {response.StatusCode}");
                    }
                }

            }

            catch (CustomException e)
            {
                _logger.LogInformation(e.Message);
            }
        }


    }
}
