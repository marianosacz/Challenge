using System;
using System.Collections.Generic;
using System.Data;
using Challenge.Models;
using Microsoft.Data.SqlClient;

namespace Challenge.Data
{
    public class ProductData
    {
        private static object code;

        public static bool Registrar(Product product)
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

                try
                {
                    oConexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool Modificar(Product product)
        {
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_modificar", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@code", product.code);
                cmd.Parameters.AddWithValue("@sku", product.sku);
                cmd.Parameters.AddWithValue("@stock", product.stock);
                cmd.Parameters.AddWithValue("@currency", product.currency);
                cmd.Parameters.AddWithValue("@price", product.price);
                cmd.Parameters.AddWithValue("@iva", product.iva);
                cmd.Parameters.AddWithValue("@ii", product.ii);

                try
                {
                    oConexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static List<Product> Listar()
        {
            List<Product> oListaProduct = new List<Product>();
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_listar", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    oConexion.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader dr = cmd.ExecuteReader()) {
                        
                        while (dr.Read())
                        {
                            oListaProduct.Add(new Product() {
                            Code = dr["Code"].ToString(),
                            Sku = dr["Sku"].ToString(),
                            Stock = dr["Stock"].ToString(),
                            Currency = dr["Currency"].ToString,
                            Price = dr["Price"].ToString(),
                            Iva = dr["Iva"].ToString(),
                            Ii = dr["Ii"].ToString
                            });
                        }

                    }



                    return oListaProduct;
                }
                catch (Exception ex)
                {
                    return oListaProduct;
                }
            }
        }

        public static Product Obtener(int idProduct)
        {
            Product product = new Product();
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_obtener", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idProduct", idProduct);

                try
                {
                    oConexion.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            product = new Product()
                            {
                                Code = dr["Code"].ToString(),
                                Sku = dr["Sku"].ToString(),
                                Stock = dr["Stock"].ToString(),
                                Currency = dr["Currency"].ToString,
                                Price = dr["Price"].ToString(),
                                Iva = dr["Iva"].ToString(),
                                Ii = dr["Ii"].ToString
                            };
                        }

                    }



                    return product;
                }
                catch (Exception ex)
                {
                    return product;
                }
            }
        }

        public static bool Eliminar(int id)
        {
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_eliminar", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@code", code);

                try
                {
                    oConexion.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

    }
}