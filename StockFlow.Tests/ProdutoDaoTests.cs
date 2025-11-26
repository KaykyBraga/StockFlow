using Microsoft.EntityFrameworkCore;
using Moq;
using StockFlow.DAL;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Tests
{
    [TestClass]
    public class ProdutoDaoTests
    {
        private ProdutoDao _produtoDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<Produto>> _mockProdutos;

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(items.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => items.GetEnumerator());

            // Simular o Find para que BuscarPorId funcione
            // Esta implementação é específica para a entidade Produto.
            dbSetMock.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => items.FirstOrDefault(d => ((Produto)(object)d).ProdutoId == (int)ids[0]));

            return dbSetMock;
        }

        [TestInitialize]
        public void Setup()
        {
            _mockProdutos = CreateDbSetMock(new List<Produto>().AsQueryable());
            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
        }

        [TestMethod]
        public void CadastrarProduto_QuandoNaoExiste_DeveAdicionarEChamarSaveChanges()
        {
            // Arrange
            var novoProduto = new Produto { ProdutoId = 1, Ean = "12345", NomeCompleto = "Produto Teste" };
            // Act
            var resultado = _produtoDao.CadastrarProduto(novoProduto);
            // Assert
            Assert.IsTrue(resultado);
            // MUDANÇA: Seja específico! Verifique se o objeto correto foi adicionado.
            _mockProdutos.Verify(m => m.Add(novoProduto), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void CadastrarProduto_QuandoEanJaExiste_DeveRetornarFalse()
        {
            // Arrange
            var produtosIniciais = new List<Produto> { new Produto { ProdutoId = 1, Ean = "12345" } }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            var produtoDuplicado = new Produto { Ean = "12345" };

            // Act
            var resultado = _produtoDao.CadastrarProduto(produtoDuplicado);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual("Produto já cadastrado!", _produtoDao.mensagem);
            // MUDANÇA: Verifica se Add NUNCA foi chamado, o que é uma asserção forte.
            _mockProdutos.Verify(m => m.Add(It.IsAny<Produto>()), Times.Never());
        }

        [TestMethod]
        public void BuscarProdutoPorId_ComIdExistente_DeveRetornarProdutoComTodosOsDados()
        {
            // Arrange
            var produtoEsperado = new Produto { ProdutoId = 5, NomeCompleto = "Produto Encontrado", EstoqueAtual = 10 };
            var produtosIniciais = new List<Produto> { produtoEsperado }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);

            // Act
            var resultado = _produtoDao.BuscarProdutoPorId(5);

            // Assert
            Assert.IsNotNull(resultado);
            // MUDANÇA: Verifique mais de uma propriedade para garantir que o objeto correto foi retornado.
            Assert.AreEqual(produtoEsperado.ProdutoId, resultado.ProdutoId);
            Assert.AreEqual(produtoEsperado.NomeCompleto, resultado.NomeCompleto);
            Assert.AreEqual(produtoEsperado.EstoqueAtual, resultado.EstoqueAtual);
        }

        // Nenhuma mudança necessária aqui, este teste já é bom.
        [TestMethod]
        public void BuscarProdutoPorId_ComIdInexistente_DeveRetornarNulo()
        {
            var resultado = _produtoDao.BuscarProdutoPorId(99);
            Assert.IsNull(resultado);
            Assert.AreEqual("Produto não encontrado.", _produtoDao.mensagem);
        }

        [TestMethod]
        public void BuscarProdutoPorNome_ComTermoExistente_DeveRetornarProdutosCorretos()
        {
            // Arrange
            var produtosIniciais = new List<Produto>
        {
            new Produto { NomeCompleto = "Coca-Cola Lata" },
            new Produto { NomeCompleto = "Pepsi Black" },
            // MUDANÇA: Adiciona um caso de teste para a lógica de case-insensitive (maiúsculas/minúsculas)
            new Produto { NomeCompleto = "Guaraná ANTARCTICA" }
        }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);

            // Act
            var resultado = _produtoDao.BuscarProdutoPorNome("coca");

            // Assert
            Assert.IsNotNull(resultado);
            // MUDANÇA: Verifique o conteúdo da lista, não apenas a contagem.
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Coca-Cola Lata", resultado[0].NomeCompleto);
        }

        // Nenhuma mudança necessária aqui, este teste já é forte porque verifica o estado final.
        [TestMethod]
        public void AdicionarEstoque_QuandoProdutoExiste_DeveAtualizarEstoque()
        {
            var produto = new Produto { ProdutoId = 10, EstoqueAtual = 50 };
            var produtosIniciais = new List<Produto> { produto }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);

            _produtoDao.AdicionarEstoque(10, 25);

            Assert.AreEqual(75, produto.EstoqueAtual);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void DesativarProduto_QuandoProdutoExiste_DeveMarcarComoInativoESalvar()
        {
            // Arrange
            var produto = new Produto { ProdutoId = 20, Ativo = true };
            var produtosIniciais = new List<Produto> { produto }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);

            // Act
            _produtoDao.DesativarProduto(20);

            // Assert
            Assert.IsFalse(produto.Ativo);
            // MUDANÇA: Verifique se o objeto CORRETO foi atualizado.
            _mockProdutos.Verify(m => m.Update(produto), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        // ... (restante dos testes async)

        [TestMethod]
        public async Task ObterProdutosAtivosAsync_DeveRetornarApenasProdutosAtivos()
        {
            // Arrange
            var produtos = new List<Produto>
        {
            new Produto { NomeCompleto = "Ativo 1", Ativo = true },
            new Produto { NomeCompleto = "Inativo 1", Ativo = false },
            new Produto { NomeCompleto = "Ativo 2", Ativo = true },
        }.AsQueryable();
            var mockProdutosAsync = CreateDbSetMock(produtos);
            // IMPORTANTE: Adicionar a extensão para mockar async
            mockProdutosAsync.AsAsyncDbSet(produtos);
            _mockContext.Setup(c => c.Produtos).Returns(mockProdutosAsync.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);

            // Act
            var resultado = await _produtoDao.ObterProdutosAtivosAsync();

            // Assert
            Assert.AreEqual(2, resultado.Count);
            // Esta asserção já é forte, ela verifica a propriedade de TODOS os itens. Ótimo!
            Assert.IsTrue(resultado.All(p => p.Ativo));
        }

        [TestMethod]
        public async Task ObterProdutosComEstoqueBaixoAsync_DeveRetornarApenasCorretos()
        {
            // Arrange
            // MUDANÇA: Adicionado o caso de borda onde EstoqueAtual == EstoqueMinimo
            var produtos = new List<Produto>
        {
            new Produto { EstoqueAtual = 5, EstoqueMinimo = 10 },  // Deve retornar
            new Produto { EstoqueAtual = 10, EstoqueMinimo = 10 }, // Não deve retornar (caso de borda)
            new Produto { EstoqueAtual = 15, EstoqueMinimo = 10 }, // Não deve retornar
            new Produto { EstoqueAtual = 1, EstoqueMinimo = 5 }    // Deve retornar
        }.AsQueryable();
            var mockProdutosAsync = CreateDbSetMock(produtos);
            mockProdutosAsync.AsAsyncDbSet(produtos);
            _mockContext.Setup(c => c.Produtos).Returns(mockProdutosAsync.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);

            // Act
            var resultado = await _produtoDao.ObterProdutosComEstoqueBaixoAsync();

            // Assert
            Assert.AreEqual(2, resultado.Count);
            // MUDANÇA: Verifica se a condição é verdadeira para todos os itens retornados.
            Assert.IsTrue(resultado.All(p => p.EstoqueAtual < p.EstoqueMinimo));
        }
    }
}
