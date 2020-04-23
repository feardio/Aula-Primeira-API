using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstAPI.Data;
using FirstAPI.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FirstAPI.Hateoas;

namespace FirstAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        private Hateoas.Hateoas Hateoas;

        public ProdutosController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new Hateoas.Hateoas("localhost:44318/api/v1/produtos/");
            Hateoas.AddAction("GET_INFO","GET");
            Hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            Hateoas.AddAction("EDIT_PRODUCT", "PATCH");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var produtos = database.Produtos.ToList();
            List<ProdutoContainer> produtosHateoas = new List<ProdutoContainer>();
            foreach (var prod in produtos)
            {
                ProdutoContainer produtoHateoas = new ProdutoContainer();
                produtoHateoas.produto = prod;
                produtoHateoas.links = Hateoas.GetActions(prod.Id.ToString());
                produtosHateoas.Add(produtoHateoas);
            }
            //return NotFound(); //status code 404
            return Ok(produtosHateoas);//status code = 200 && dados
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Produto produto = database.Produtos.First(p => p.Id == id);
                ProdutoContainer produtoHateoas = new ProdutoContainer();
                produtoHateoas.produto = produto;
                produtoHateoas.links = Hateoas.GetActions(produto.Id.ToString());
                return Ok(produtoHateoas);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
                //return BadRequest(new {msg = "Id Invalido!"});
            }


        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp)
        {
            //Validação

            if (pTemp.Preco <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O preço do prouduto não pode ser menor ou igual 0" });
            }

            if (pTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "O nome do produto precisa ter mais de um caracter." });

            }


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
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");

            }

        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto)
        {
            if (produto.Id > 0)
            {
                try
                {
                    var p = database.Produtos.First(pTemp => pTemp.Id == produto.Id);

                    if (p != null)
                    {
                        //Editar
                        // condicao ?(verdadeiro) faz algo :(falso) faz outra coisa
                        p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                        p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;

                        database.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Produto não encontrado!" });
                    }

                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Produto não encontrado!" });

                }

            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Id do produto é inválido!" });

            }
        }


        public class ProdutoTemp
        {
            public string Nome { get; set; }
            public float Preco { get; set; }
        }

        public class ProdutoContainer
            {
            public Produto produto;
            public Link[] links;
            }

    }
}