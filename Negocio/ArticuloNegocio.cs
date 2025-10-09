using AccesoDatos;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    datos.SetearConsulta(@"
                    SELECT 
                        MIN(A.Id) AS Id,
                        A.Nombre,
                        MIN(A.Descripcion) AS Descripcion,
                        MIN(M.Descripcion) AS MarcaDescripcion,
                        MIN(C.Descripcion) AS CategoriaDescripcion
                    FROM Articulos A
                    INNER JOIN Marcas M ON A.IdMarca = M.Id
                    INNER JOIN Categorias C ON A.IdCategoria = C.Id
                    GROUP BY A.Nombre
                    ORDER BY A.Nombre
                ");

                    datos.EjecutarLectura();

                    while (datos.Lector.Read())
                    {
                        Articulo art = new Articulo
                        {
                            Id = (int)datos.Lector["Id"],
                            Nombre = (string)datos.Lector["Nombre"],
                            Descripcion = (string)datos.Lector["Descripcion"],
                            Marca = new Marca { Descripcion = (string)datos.Lector["MarcaDescripcion"] },
                            Categoria = new Categoria { Descripcion = (string)datos.Lector["CategoriaDescripcion"] }
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
    }

}

