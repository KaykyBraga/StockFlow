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
    public class PromocaoProdutoDaoTests
    {
        private PromocaoProdutoDao _promocaoProdutoDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<PromocaoProduto>> _mockPromocaoProdutos;

        // Função auxiliar para configurar um DbSet mockado corretamente
        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(items.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => items.GetEnumerator());
            return dbSetMock;
        }

        [TestInitialize]
        public void Setup()
        {
            _mockPromocaoProdutos = CreateDbSetMock(new List<PromocaoProduto>().AsQueryable());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockContext.Setup(c => c.PromocaoProdutos).Returns(_mockPromocaoProdutos.Object);

            _promocaoProdutoDao = new PromocaoProdutoDao(_mockContext.Object);
        }

        [TestMethod]
        public void VincularPromocaoProduto_ComDadosValidos_DeveAdicionarComSucesso()
        {
            // Arrange
            var novoVinculo = new PromocaoProduto { PromocaoId = 1, ProdutoId = 1 };

            // Act
            _promocaoProdutoDao.VincularPromocaoProduto(novoVinculo);

            // Assert
            Assert.AreEqual("", _promocaoProdutoDao.mensagemErro);
            _mockPromocaoProdutos.Verify(m => m.Add(novoVinculo), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public async Task ObterTodosAsPromocaoProdutoAsync_QuandoExistemVinculos_DeveRetornarTodos()
        {
            // Arrange
            var vinculos = new List<PromocaoProduto>
        {
            new PromocaoProduto { PromocaoProdutoId = 1, PromocaoId = 1, ProdutoId = 10 },
            new PromocaoProduto { PromocaoProdutoId = 2, PromocaoId = 1, ProdutoId = 12 }
        }.AsQueryable();

            var mockPromocaoProdutosAsync = CreateDbSetMock(vinculos);
            mockPromocaoProdutosAsync.AsAsyncDbSet(vinculos);
            _mockContext.Setup(c => c.PromocaoProdutos).Returns(mockPromocaoProdutosAsync.Object);
            _promocaoProdutoDao = new PromocaoProdutoDao(_mockContext.Object);

            // Act
            var resultado = await _promocaoProdutoDao.ObterTodosAsPromocaoProdutoAsync();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }
    }
}
