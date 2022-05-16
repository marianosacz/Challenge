using Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge.Interface
{
    public interface IProductData
    {
        void Registrar(Product product);
        void Actualizar(Product product);

    }
}
