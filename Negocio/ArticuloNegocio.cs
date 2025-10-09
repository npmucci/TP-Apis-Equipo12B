using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class ArticuloNegocio
    {
        Datos datos = new Datos();
        public List<Articulo> Listar(string orden = "")
        {
            List<Articulo> lista = new List<Articulo>();
           
            ImagenNegocio imagen = new ImagenNegocio();

            try
            {
                string query = (@"SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio,
                M.Id AS IdMarca, M.Descripcion AS Marca,
                C.Id AS IdCategoria, C.Descripcion AS Categoria
                FROM dbo.ARTICULOS A
                JOIN dbo.MARCAS M ON A.IdMarca = M.Id
                JOIN dbo.CATEGORIAS C ON A.IdCategoria = C.Id");
                if (orden == "asc")
                    query += " ORDER BY A.Precio ASC";
                else if (orden == "desc")
                    query += " ORDER BY A.Precio DESC";

                datos.SetearConsulta(query);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo art = new Articulo();
                    {
                        art.Id = (int)datos.Lector["Id"];
                        art.Codigo = datos.Lector["Codigo"].ToString();
                        art.Nombre = datos.Lector["Nombre"].ToString();
                        art.Descripcion = datos.Lector["Descripcion"].ToString();
                        art.Precio = (decimal)datos.Lector["Precio"];
                        art.Marca = new Marca();

                        art.Marca.Id = (int)datos.Lector["IdMarca"];
                        art.Marca.Descripcion = datos.Lector["Marca"].ToString();


                        art.Categoria = new Categoria();

                        art.Categoria.Id = (int)datos.Lector["IdCategoria"];
                        art.Categoria.Descripcion = datos.Lector["Categoria"].ToString();

                    }
                    art.Imagenes = new List<Imagen>();
                    art.Imagenes = imagen.ListarImagenes(art.Id);
                    lista.Add(art);
                }

                return lista;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public int AgregarArticulo(Articulo nuevo)
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


        public void Eliminar(int id)
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
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void Modificar(Articulo articulo)
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
            finally
            {
                datos.CerrarConexion();
            }
        }


    }
}
