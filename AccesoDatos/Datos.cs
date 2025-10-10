using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    public class Datos : IDisposable  // --> Interfaz IDisposable para usar using() --> (obliga a implementar Dispose())
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        private string connectionString;

        public Datos()
        {
            // Toma la cadena desde Web.config
            connectionString = ConfigurationManager.ConnectionStrings["CATALOGO_P3_DB"].ConnectionString;

            conexion = new SqlConnection(connectionString);
        }

        // Lector de solo lectura (para SELECT)
        public SqlDataReader Lector
        {
            get { return lector; }
        }

        // Setea la consulta SQL
        public void SetearConsulta(string query)
        {
            comando = new SqlCommand(query, conexion);
        }

        // Agregar parametros
        public void SetearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        // Ejecutar SELECT
        public void EjecutarLectura()
        {
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar lectura: " + ex.Message, ex);
            }
        }

        // Ejecutar INSERT/UPDATE/DELETE
        public void EjecutarAccion()
        {
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar acción: " + ex.Message, ex);
            }
            finally
            {
                conexion.Close();
            }
        }

        // Ejecutar consulta que devuelve un solo valor
        public int EjecutarAccionEscalar()
        {
            try
            {
                conexion.Open();
                object resultado = comando.ExecuteScalar();
                return resultado != null ? Convert.ToInt32(resultado) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar acción escalar: " + ex.Message, ex);
            }
            finally
            {
                conexion.Close();
            }
        }

        // Cierra la conexion y limpia recursos
        public void CerrarConexion()
        {
            if (lector != null && !lector.IsClosed)
                lector.Close();

            if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
                conexion.Close();
        }

        // Implementación de IDisposable para usar con using()
        public void Dispose()
        {
            CerrarConexion();
            if (comando != null)
                comando.Dispose();
            if (conexion != null)
                conexion.Dispose();
        }
    }
}
