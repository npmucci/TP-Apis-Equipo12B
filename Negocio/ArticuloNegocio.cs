using AccesoDatos;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Negocio
{
    public class ArticuloNegocio
    {

        public List<Articulo> ListarArticulos()
        {
            List<Articulo> lista = new List<Articulo>();
            ImagenNegocio imagen = new ImagenNegocio();

            using (Datos datos = new Datos())
            {
                try
                {
                    //Agrupo por nombre asi no se repiten articulos
                    datos.SetearConsulta("SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, M.Id AS IdMarca, M.Descripcion AS MarcaDescripcion, C.Id AS IdCategoria, C.Descripcion AS CategoriaDescripcion FROM Articulos A INNER JOIN Marcas M ON A.IdMarca = M.Id INNER JOIN Categorias C ON A.IdCategoria = C.Id ORDER BY A.Nombre");


                    datos.EjecutarLectura();

                    while (datos.Lector.Read())
                    {
                        Articulo art = new Articulo
                        {
                            Id = (int)datos.Lector["Id"],
                            Codigo = (string)datos.Lector["Codigo"],
                            Nombre = (string)datos.Lector["Nombre"],
                            Descripcion = (string)datos.Lector["Descripcion"],
                            Precio = Convert.ToDecimal(datos.Lector["Precio"]),

                            Marca = new Marca
                            {
                                Id = (int)datos.Lector["IdMarca"],
                                Descripcion = (string)datos.Lector["MarcaDescripcion"]
                            },
                            Categoria = new Categoria
                            {
                                Id = (int)datos.Lector["IdCategoria"],
                                Descripcion = (string)datos.Lector["CategoriaDescripcion"]
                            }
                        };


                        art.Imagenes = new List<Imagen>();
                        art.Imagenes = imagen.ListarImagenes(art.Id);

                        lista.Add(art);
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        public int AgregarArticulo(Articulo nuevo)
        {
            using (Datos datos = new Datos())
            {

                try
                {
                    string query = @"
                    INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio)
                    OUTPUT INSERTED.Id
                    VALUES (@codigo, @nombre, @descripcion, @idMarca, @idCategoria, @precio)";

                    datos.SetearConsulta(query);
                    datos.SetearParametro("@codigo", nuevo.Codigo);
                    datos.SetearParametro("@nombre", nuevo.Nombre);
                    datos.SetearParametro("@descripcion", nuevo.Descripcion);
                    datos.SetearParametro("@idMarca", nuevo.Marca.Id);
                    datos.SetearParametro("@idCategoria", nuevo.Categoria.Id);
                    datos.SetearParametro("@precio", nuevo.Precio);

                    return datos.EjecutarAccionEscalar(); // devuelve el Id generado
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    datos.CerrarConexion();
                }
            }

        }
        public void Eliminar(int id)
        {
            using (Datos datos = new Datos())
            {
                try
                {
                    datos.SetearConsulta("DELETE FROM ARTICULOS WHERE Id = @id");
                    datos.SetearParametro("@id", id);
                    datos.EjecutarAccion();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Modificar(Articulo articulo)
        {
            using (Datos datos = new Datos())
            {

                try
                {
                    string query = "UPDATE ARTICULOS SET Codigo = @codigo,Nombre = @nombre,Descripcion = @descripcion,IdMarca = @idMarca,IdCategoria = @idCategoria,Precio = @precio WHERE Id = @id";
                    datos.SetearConsulta(query);
                    datos.SetearParametro("@codigo", articulo.Codigo);
                    datos.SetearParametro("@nombre", articulo.Nombre);
                    datos.SetearParametro("@descripcion", articulo.Descripcion);
                    datos.SetearParametro("@idMarca", articulo.Marca.Id);
                    datos.SetearParametro("@idCategoria", articulo.Categoria.Id);
                    datos.SetearParametro("@precio", articulo.Precio);
                    datos.SetearParametro("@id", articulo.Id);
                    datos.EjecutarAccion();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        public bool Existe(int id)
        {
           
            using (AccesoDatos.Datos datos = new AccesoDatos.Datos())
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM ARTICULOS WHERE Id = @id";
                    datos.SetearConsulta(query);
                    datos.SetearParametro("@id", id);
                    int count = datos.EjecutarAccionEscalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }
            }
        }

        public bool ExisteCodigo(string codigo)
        {

            using (AccesoDatos.Datos datos = new AccesoDatos.Datos())
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM ARTICULOS WHERE CODIGO = @codigo";
                    datos.SetearConsulta(query);
                    datos.SetearParametro("@codigo", codigo);
                    int count = datos.EjecutarAccionEscalar();
                    return count > 0;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public string ValidarCampos(Articulo articulo)
        {

            if (articulo == null)
            {
                return "El objeto artículo no puede ser nulo.";
            }


            if (string.IsNullOrWhiteSpace(articulo.Codigo))
            {
                return "El campo 'Codigo' es obligatorio.";
            }
            if (articulo.Codigo.Length > 50)
            {
                return "El campo 'Codigo' no puede superar los 50 caracteres.";
            }
            if (ExisteCodigo(articulo.Codigo))
            {
                return $"El código de artículo '{articulo.Codigo}' ya está en uso.";
            }


            if (string.IsNullOrWhiteSpace(articulo.Nombre))
            {
                return "El campo 'Nombre' es obligatorio.";
            }
            if (articulo.Nombre.Length > 50)
            {
                return "El campo 'Nombre' no puede superar los 50 caracteres.";
            }


            if (articulo.Descripcion != null && articulo.Descripcion.Length > 150)
            {
                return "El campo 'Descripcion' no puede superar los 150 caracteres.";
            }


            if (articulo.Precio < 0)
            {
                return "El campo 'Precio' debe ser mayor o igual a cero.";
            }


            return null;
        }


    }
}



