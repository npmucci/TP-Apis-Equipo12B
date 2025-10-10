using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dominio;
using Negocio; 
using Apis_Productos.Models;

namespace Apis_Productos.Controllers
{
    [RoutePrefix("api/Articulo")]
    public class ArticuloController : ApiController
    {
        // GET: api/Producto
        public IEnumerable<Articulo> Get()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            return negocio.ListarArticulos();
        }

        // GET: api/Producto/5
        [HttpGet]
        [Route("{id}", Name = "GetArticuloById")] 
        public IHttpActionResult Get(int id)
        {
           return Ok(new ArticuloNegocio().ListarArticulos().FirstOrDefault(a => a.Id == id));
        }

        // POST: api/Producto
        [HttpPost, Route("")]
        public IHttpActionResult Post([FromBody] ArticuloDto articulo)
        {
            try
            {
                // Validaciones:

                if (articulo == null)
                {
                    return BadRequest("El objeto artículo no puede ser nulo.");
                }

                

                if (string.IsNullOrWhiteSpace(articulo.Codigo))
                {
                    return BadRequest("El campo 'Codigo' es obligatorio.");
                }
                if (articulo.Codigo.Length > 50)
                {
                    return BadRequest("El campo 'Codigo' no puede superar los 50 caracteres.");
                }

                if (string.IsNullOrWhiteSpace(articulo.Nombre))
                {
                    return BadRequest("El campo 'Nombre' es obligatorio.");
                }
                if (articulo.Nombre.Length > 50)
                {
                    return BadRequest("El campo 'Nombre' no puede superar los 50 caracteres.");
                }

                if (articulo.Descripcion != null && articulo.Descripcion.Length > 150)
                {
                    return BadRequest("El campo 'Descripcion' no puede superar los 150 caracteres.");
                }


                if (articulo.Precio < 0)
                {
                    return BadRequest("El campo 'Precio' debe ser un valor positivo.");
                }

                
                MarcaNegocio marcaNegocio = new MarcaNegocio();
                if (!marcaNegocio.Existe(articulo.IdMarca))
                {
                    return BadRequest($"La marca con ID {articulo.IdMarca} no existe.");
                }

                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
                if (!categoriaNegocio.Existe(articulo.IdCategoria))
                {
                    return BadRequest($"La categoría con ID {articulo.IdCategoria} no existe.");
                }

                ArticuloNegocio ArticuloNegocio = new ArticuloNegocio();

                
                if (ArticuloNegocio.ExisteCodigo(articulo.Codigo))
                {
                    return BadRequest($"El código de artículo '{articulo.Codigo}' ya está en uso.");
                }

                // Creacion y agregado:

                Articulo nuevoArticulo = new Articulo
                {
                    Codigo = articulo.Codigo,
                    Nombre = articulo.Nombre,
                    Descripcion = articulo.Descripcion,
                    Marca = new Marca { Id = articulo.IdMarca },
                    Categoria = new Categoria { Id = articulo.IdCategoria },
                    Precio = articulo.Precio
                };

                
                int nuevoId = ArticuloNegocio.AgregarArticulo(nuevoArticulo);


                return CreatedAtRoute("GetArticuloById", new { id = nuevoId }, nuevoArticulo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/Imagen
        [Route("{id}/imagenes")]
        public IHttpActionResult Post(int id, [FromBody] List<ImagenDto> imagenes)
        {
            try
            {
                //  VALIDACIÓN

                ArticuloNegocio articuloNegocio = new ArticuloNegocio();
                if (!articuloNegocio.Existe(id))
                {
                    // 404 Not Found si el ID no existe...
                    var respuesta = new { Message = "El artículo con el ID " + id + " no fue encontrado." };
                    return Content(HttpStatusCode.NotFound, respuesta);
                }

                
                if (imagenes == null || !imagenes.Any())
                {
                    // 400 Bad Request si no se enviaron imagenes...
                    return BadRequest("La lista de imágenes no puede estar vacía.");
                }

                // AGREGAR

                ImagenNegocio imagenNegocio = new ImagenNegocio();
                List<Imagen> listaImagenes = imagenes.Select(i => new Imagen { Url = i.Url }).ToList();

                imagenNegocio.AgregarImagenes(listaImagenes, id);

                
                return Ok("Imágenes agregadas correctamente.");
                // Retorna un 200 OK.
            }
            catch (Exception ex)
            {
                
                return InternalServerError(ex);
            }
        }

        // PUT: api/Producto/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Producto/5
        public void Delete(int id)
        {
        }
    }
}
