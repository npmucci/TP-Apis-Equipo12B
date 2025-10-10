using Dominio;
using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class ImagenNegocio

    {
      
        public List<Imagen> ListarImagenes(int idArticulo)
        {
            List<Imagen> lista = new List<Imagen>();

            using (Datos datos = new Datos())
            {
                try
                {
                    //imagenes.setearConsulta("select ImagenUrl from Imagenes where IdArticulo = " + idArticulo + ";");
                    datos.SetearConsulta("select Id,ImagenUrl from Imagenes where IdArticulo = @IdArticulo;");
                    datos.SetearParametro("IdArticulo", idArticulo);
                    datos.EjecutarLectura();
                    while (datos.Lector.Read())
                    {
                        Imagen aux = new Imagen();
                        aux.Id = (int)datos.Lector["Id"];
                        aux.Url = (string)datos.Lector["ImagenUrl"];
                        aux.IdArticulo = idArticulo;
                        lista.Add(aux);
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al listar las imágenes del artículo con Id " + idArticulo, ex);
                }
            }
         
        }
        public void AgregarImagenes(List<Imagen> urls, int idArticulo)
        {
            using (Datos datos = new Datos())
            {
                try
                {
                    foreach (var img in urls)
                    {
                        string query = "INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@idArticulo, @url)";
                        datos.SetearConsulta(query);
                        datos.SetearParametro("@idArticulo", idArticulo);
                        datos.SetearParametro("@url", img.Url);
                        datos.EjecutarAccion();
                    }
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
        //elimino todas las imagenes de un articulo para despues agregar las nuevas
        public void EliminarImagen(int idArticulo)
        {
            using (Datos datos = new Datos())
            {
                try
                {
                    string query = "DELETE FROM IMAGENES WHERE IdArticulo = @idArticulo";
                    datos.SetearConsulta(query);
                    datos.SetearParametro("@idArticulo", idArticulo);
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
        private void EliminarImagenes(List<Imagen> eliminadas, int idArticulo)
        {
            using (Datos datos = new Datos())
            {
                try
                {
                    foreach (Imagen img in eliminadas)
                    {
                        string query = "DELETE FROM IMAGENES WHERE IdArticulo = @idArticulo AND ImagenUrl = @url";
                        datos.SetearConsulta(query);
                        datos.SetearParametro("@idArticulo", idArticulo);
                        datos.SetearParametro("@url", img.Url);
                        datos.EjecutarAccion();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }


        private void InsertarImagenes(List<Imagen> nuevas, int idArticulo)
        {
            using (Datos datos = new Datos())
            {


                try
                {
                    foreach (Imagen img in nuevas)
                    {
                        string query = "INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@idArticulo, @url)";
                        datos.SetearConsulta(query);
                        datos.SetearParametro("@idArticulo", idArticulo);
                        datos.SetearParametro("@url", img.Url);
                        datos.EjecutarAccion();
                    }
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


        public void ModificarImagenes(List<Imagen> imagenesActuales, int idArticulo, List<Imagen> imagenesOriginales)
        {
            List<Imagen> nuevas = ObtenerNuevasImagenes(imagenesActuales, imagenesOriginales);
            List<Imagen> eliminadas = ObtenerImagenesEliminadas(imagenesActuales, imagenesOriginales);
            using (Datos datos = new Datos())
            {

                try
                {
                    EliminarImagenes(eliminadas, idArticulo); //borro de la bbdd las imagnes que se eliminaron en el modificar articulo
                    InsertarImagenes(nuevas, idArticulo); //agrego a la bbdd las nuevas imagenes que se agregaron en el modificar articulo
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private List<Imagen> ObtenerNuevasImagenes(List<Imagen> actuales, List<Imagen> originales)
        {
            List<Imagen> nuevas = new List<Imagen>();
            // si no existe en originales, es nueva
            foreach (var img in actuales)
            {
                bool existeEnOriginales = originales.Exists(original => original.Url == img.Url);
                if (!existeEnOriginales)
                    nuevas.Add(img);
            }

            return nuevas;
        }

        private List<Imagen> ObtenerImagenesEliminadas(List<Imagen> actuales, List<Imagen> originales)
        {
            List<Imagen> eliminadas = new List<Imagen>();
            // si no existe en actuales, la imagen se elimino
            foreach (var img in originales)
            {
                bool existeEnActuales = actuales.Exists(actual => actual.Url == img.Url);
                if (!existeEnActuales)
                    eliminadas.Add(img);
            }
            return eliminadas;
        }


    }
}
