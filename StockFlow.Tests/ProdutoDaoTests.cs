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

        // Função auxiliar para configurar um DbSet mockado corretamente
        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(items.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => items.GetEnumerator());
            // Simular o método Find para que BuscarPorId funcione
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
            _mockProdutos.Verify(m => m.Add(It.IsAny<Produto>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void CadastrarProduto_QuandoEanJaExiste_DeveRetornarFalseEMensagem()
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
            _mockProdutos.Verify(m => m.Add(It.IsAny<Produto>()), Times.Never());
        }

        [TestMethod]
        public void BuscarProdutoPorId_ComIdExistente_DeveRetornarProdutoCorreto()
        {
            // Arrange
            var produtosIniciais = new List<Produto> { new Produto { ProdutoId = 5, NomeCompleto = "Produto Encontrado" } }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            // Act
            var resultado = _produtoDao.BuscarProdutoPorId(5);
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(5, resultado.ProdutoId);
        }

        [TestMethod]
        public void BuscarProdutoPorId_ComIdInexistente_DeveRetornarNulo()
        {
            // Arrange
            // Nenhuma preparação necessária, o setup já inicia com a lista vazia
            // Act
            var resultado = _produtoDao.BuscarProdutoPorId(99);
            // Assert
            Assert.IsNull(resultado);
            Assert.AreEqual("Produto não encontrado.", _produtoDao.mensagem);
        }

        [TestMethod]
        public void BuscarProdutoPorNome_ComTermoExistente_DeveRetornarProdutosCorrespondentes()
        {
            // Arrange
            var produtosIniciais = new List<Produto>
        {
            new Produto { NomeCompleto = "Coca-Cola Lata" },
            new Produto { NomeCompleto = "Pepsi Black" },
            new Produto { NomeCompleto = "COCA-COLA ZERO" }
        }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            // Act
            var resultado = _produtoDao.BuscarProdutoPorNome("coca");
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }

        [TestMethod]
        public void AdicionarEstoque_QuandoProdutoExiste_DeveAtualizarEstoque()
        {
            // Arrange
            var produto = new Produto { ProdutoId = 10, EstoqueAtual = 50 };
            var produtosIniciais = new List<Produto> { produto }.AsQueryable();
            _mockProdutos = CreateDbSetMock(produtosIniciais);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            // Act
            _produtoDao.AdicionarEstoque(10, 25);
            // Assert
            Assert.AreEqual(75, produto.EstoqueAtual);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void DesativarProduto_QuandoProdutoExiste_DeveMarcarComoInativo()
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
            _mockProdutos.Verify(m => m.Update(It.IsAny<Produto>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void EditarProduto_DeveChamarUpdateESaveChanges()
        {
            // Arrange
            var produtoParaEditar = new Produto { ProdutoId = 1, NomeCompleto = "Nome Editado" };
            // Act
            _produtoDao.EditarProduto(produtoParaEditar);
            // Assert
            _mockProdutos.Verify(m => m.Update(produtoParaEditar), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public async Task ObterTodosOsProdutosAsync_DeveRetornarTodos()
        {
            // Arrange
            var produtos = new List<Produto> { new Produto(), new Produto(), new Produto() }.AsQueryable();
            var mockProdutosAsync = CreateDbSetMock(produtos);
            mockProdutosAsync.AsAsyncDbSet(produtos);
            _mockContext.Setup(c => c.Produtos).Returns(mockProdutosAsync.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            // Act
            var resultado = await _produtoDao.ObterTodosOsProdutosAsync();
            // Assert
            Assert.AreEqual(3, resultado.Count);
        }

        [TestMethod]
        public async Task ObterProdutosAtivosAsync_DeveRetornarApenasProdutosAtivos()
        {
            // Arrange
            var produtos = new List<Produto>
        {
            new Produto { Ativo = true }, new Produto { Ativo = false }, new Produto { Ativo = true },
        }.AsQueryable();
            var mockProdutosAsync = CreateDbSetMock(produtos);
            mockProdutosAsync.AsAsyncDbSet(produtos);
            _mockContext.Setup(c => c.Produtos).Returns(mockProdutosAsync.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            // Act
            var resultado = await _produtoDao.ObterProdutosAtivosAsync();
            // Assert
            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.All(p => p.Ativo));
        }

        [TestMethod]
        public async Task ObterProdutosComEstoqueBaixoAsync_DeveRetornarApenasCorretos()
        {
            // Arrange
            var produtos = new List<Produto>
        {
            new Produto { EstoqueAtual = 5, EstoqueMinimo = 10 },  // Estoque baixo
            new Produto { EstoqueAtual = 10, EstoqueMinimo = 10 }, // Estoque não está baixo
            new Produto { EstoqueAtual = 15, EstoqueMinimo = 10 }, // Estoque não está baixo
            new Produto { EstoqueAtual = 1, EstoqueMinimo = 5 }    // Estoque baixo
        }.AsQueryable();
            var mockProdutosAsync = CreateDbSetMock(produtos);
            mockProdutosAsync.AsAsyncDbSet(produtos);
            _mockContext.Setup(c => c.Produtos).Returns(mockProdutosAsync.Object);
            _produtoDao = new ProdutoDao(_mockContext.Object);
            // Act
            var resultado = await _produtoDao.ObterProdutosComEstoqueBaixoAsync();
            // Assert
            Assert.AreEqual(2, resultado.Count);
        }
    }
}
