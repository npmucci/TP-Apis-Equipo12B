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
        [HttpGet]
        [Route("")]
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
                ArticuloNegocio ArticuloNegocio = new ArticuloNegocio();

                if (articulo == null)
                {
                    return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
                }


                Articulo nuevoArticulo = new Articulo
                {
                    Codigo = articulo.Codigo,
                    Nombre = articulo.Nombre,
                    Descripcion = articulo.Descripcion,
                    Marca = new Marca { Id = articulo.IdMarca },
                    Categoria = new Categoria { Id = articulo.IdCategoria },
                    Precio = articulo.Precio
                };


                string error = ArticuloNegocio.ValidarCampos(nuevoArticulo);
                if (error != null)
                {
                    return BadRequest(error);
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


                int nuevoId = ArticuloNegocio.AgregarArticulo(nuevoArticulo);


                return CreatedAtRoute("GetArticuloById", new { id = nuevoId }, nuevoArticulo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/Imagen
        [HttpPost]
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

        // PUT: api/Articulo/5
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody] ArticuloDto articuloDto)
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                if (!negocio.Existe(id))
                {
                    return Content(HttpStatusCode.NotFound, $"El artículo con ID {id} no fue encontrado.");
                }

                if (articuloDto == null)
                {
                    return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
                }

                Articulo articuloModificado = new Articulo
                {
                    Id = id,
                    Codigo = articuloDto.Codigo,
                    Nombre = articuloDto.Nombre,
                    Descripcion = articuloDto.Descripcion,
                    Marca = new Marca { Id = articuloDto.IdMarca },
                    Categoria = new Categoria { Id = articuloDto.IdCategoria },
                    Precio = articuloDto.Precio
                };


                string error = negocio.ValidarCampos(articuloModificado);
                if (error != null)
                {
                    return BadRequest(error);
                }


                if (!new MarcaNegocio().Existe(articuloDto.IdMarca))
                {
                    return BadRequest($"La marca con ID {articuloDto.IdMarca} no existe.");
                }
                if (!new CategoriaNegocio().Existe(articuloDto.IdCategoria))
                {
                    return BadRequest($"La categoría con ID {articuloDto.IdCategoria} no existe.");
                }


                negocio.Modificar(articuloModificado);


                return Ok("Artículo modificado correctamente.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/Producto/5
        public void Delete(int id)
        {
        }
    }
}
