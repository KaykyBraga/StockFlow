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
    public class MovimentacaoCaixaDaoTests
    {
        private MovimentacaoCaixaDao _movimentacaoCaixaDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<MovimentacaoCaixa>> _mockMovimentacoes;

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
            _mockMovimentacoes = CreateDbSetMock(new List<MovimentacaoCaixa>().AsQueryable());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(_mockMovimentacoes.Object);

            _movimentacaoCaixaDao = new MovimentacaoCaixaDao(_mockContext.Object);
        }

        [TestMethod]
        public async Task ObterTodosAsMovimentacoesDoCaixaAsync_QuandoExistemMovimentacoes_DeveRetornarTodas()
        {
            // Arrange
            var movimentacoes = new List<MovimentacaoCaixa>
        {
            new MovimentacaoCaixa { MovimentacaoCaixaId = 1, Valor = 100 },
            new MovimentacaoCaixa { MovimentacaoCaixaId = 2, Valor = 50 },
            new MovimentacaoCaixa { MovimentacaoCaixaId = 3, Valor = -20 }
        }.AsQueryable();

            var mockMovimentacoesAsync = CreateDbSetMock(movimentacoes);
            mockMovimentacoesAsync.AsAsyncDbSet(movimentacoes);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(mockMovimentacoesAsync.Object);
            _movimentacaoCaixaDao = new MovimentacaoCaixaDao(_mockContext.Object);

            // Act
            var resultado = await _movimentacaoCaixaDao.ObterTodosAsMovimentacoesDoCaixaAsync();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(3, resultado.Count);
        }
    }
}
