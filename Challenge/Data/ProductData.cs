using System;
using System.Collections.Generic;
using System.Data;
using Challenge.Exceptions;
using Challenge.Interface;
using Challenge.Models;
using Microsoft.Data.SqlClient;

namespace Challenge.Data
{

    public class ProductData : IProductData
    {

        public void Registrar(Product product)
        {
            try
            {
                using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
                {
                    SqlCommand cmd = new SqlCommand("usp_registrar", oConexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@code", product.code);
                    cmd.Parameters.AddWithValue("@sku", product.sku);
                    cmd.Parameters.AddWithValue("@stock", product.stock);
                    cmd.Parameters.AddWithValue("@currency", product.currency);
                    cmd.Parameters.AddWithValue("@price", product.price);
                    cmd.Parameters.AddWithValue("@iva", product.iva);
                    cmd.Parameters.AddWithValue("@ii", product.ii);
                    oConexion.Open();
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                throw new CustomException(e);
            }



        }

        public void Actualizar(Product product)
        {
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_actualizar", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@code", product.code);
                cmd.Parameters.AddWithValue("@sku", product.sku);
                cmd.Parameters.AddWithValue("@stock", product.stock);
                cmd.Parameters.AddWithValue("@currency", product.currency);
                cmd.Parameters.AddWithValue("@price", product.price);
                cmd.Parameters.AddWithValue("@iva", product.iva);
                cmd.Parameters.AddWithValue("@ii", product.ii);
                oConexion.Open();
                cmd.ExecuteNonQuery();
            }

        }
    }
}