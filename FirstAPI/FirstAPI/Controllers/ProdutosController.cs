using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstAPI.Data;
using FirstAPI.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public ProdutosController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var produtos = database.Produtos.ToList();
            //return NotFound(); //status code 404
            return Ok(produtos);//status code = 200 && dados
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try{
                Produto produto = database.Produtos.First(p => p.Id == id);
                return Ok(produto);
            }
            catch(Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
                //return BadRequest(new {msg = "Id Invalido!"});
            }
            
            
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp)
        {
            Produto p = new Produto();
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;
            database.Produtos.Add(p);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
            //return Ok(new {msg = "Produto criado com sucesso!" }); //Response.StatusCode = 200;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                database.Produtos.Remove(produto);
                database.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
                
            }

        }


      public class ProdutoTemp
        {
            public string Nome { get; set; }
            public float Preco { get; set; }
        }

    }
}