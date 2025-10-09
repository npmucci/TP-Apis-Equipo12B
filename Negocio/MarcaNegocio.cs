using Dominio;
using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class MarcaNegocio
    {
        Datos datos = new Datos();
        public List<Marca> Listar()
        {           
            List<Marca> lista = new List<Marca>();
            try
            {
                datos.SetearConsulta("SELECT Id, Descripcion FROM MARCAS");
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                {
                    Marca marca = new Marca();
                    marca.Id = (int)datos.Lector["Id"];
                    marca.Descripcion = datos.Lector["Descripcion"].ToString();
                    lista.Add(marca);
                }
                return lista;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void AgregarMarca(Marca nueva)
        {
            try
            {
                string query = @"
                    INSERT INTO MARCAS (Descripcion)
                    VALUES (@descripcion)";

                datos.SetearConsulta(query);
                datos.SetearParametro("@descripcion", nueva.Descripcion);

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

        public void EliminarMarca(int id)
        {    
            try
            {
                string query = "DELETE FROM MARCAS WHERE Id = @id";
                datos.SetearConsulta(query);
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
    }
}
