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
    public class PromocaoDaoTests
    {
        private PromocaoDao _promocaoDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<Promocao>> _mockPromocoes;

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
            _mockPromocoes = CreateDbSetMock(new List<Promocao>().AsQueryable());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockContext.Setup(c => c.Promocoes).Returns(_mockPromocoes.Object);

            _promocaoDao = new PromocaoDao(_mockContext.Object);
        }

        [TestMethod]
        public void CadastrarPromocao_ComDadosValidos_DeveAdicionarComSucesso()
        {
            // Arrange
            var novaPromocao = new Promocao { DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), ValorDesconto = 10 };
            // Act
            _promocaoDao.CadastrarPromocao(novaPromocao);
            // Assert
            Assert.AreEqual("", _promocaoDao.mensagemErro);
            _mockPromocoes.Verify(m => m.Add(novaPromocao), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void CadastrarPromocao_ComDataFimInvalida_DeveRetornarErro()
        {
            // Arrange
            var novaPromocao = new Promocao { DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(-1), ValorDesconto = 10 };
            // Act
            _promocaoDao.CadastrarPromocao(novaPromocao);
            // Assert
            Assert.AreEqual("A data de fim deve ser posterior à data de início.", _promocaoDao.mensagemErro);
            _mockPromocoes.Verify(m => m.Add(It.IsAny<Promocao>()), Times.Never());
        }

        [TestMethod]
        public void CadastrarPromocao_ComValorDescontoInvalido_DeveRetornarErro()
        {
            // Arrange
            var novaPromocao = new Promocao { DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), ValorDesconto = 0 };
            // Act
            _promocaoDao.CadastrarPromocao(novaPromocao);
            // Assert
            Assert.AreEqual("O valor do desconto deve ser maior que zero.", _promocaoDao.mensagemErro);
            _mockPromocoes.Verify(m => m.Add(It.IsAny<Promocao>()), Times.Never());
        }

        [TestMethod]
        public void DesativarPromocoesExpiradas_DeveDesativarApenasAsExpiradas()
        {
            // Arrange
            var promocaoExpiradaAtiva = new Promocao { NomePromocao = "Expirada", Ativo = true, DataFim = DateTime.Now.AddMinutes(-5) };
            var promocaoVigente = new Promocao { NomePromocao = "Vigente", Ativo = true, DataFim = DateTime.Now.AddDays(1) };
            var promocaoExpiradaJaInativa = new Promocao { NomePromocao = "Já Inativa", Ativo = false, DataFim = DateTime.Now.AddDays(-10) };

            var promocoes = new List<Promocao> { promocaoExpiradaAtiva, promocaoVigente, promocaoExpiradaJaInativa }.AsQueryable();
            _mockPromocoes = CreateDbSetMock(promocoes);
            _mockContext.Setup(c => c.Promocoes).Returns(_mockPromocoes.Object);
            _promocaoDao = new PromocaoDao(_mockContext.Object);

            // Act
            _promocaoDao.DesativarPromocoesExpiradas();

            // Assert
            Assert.IsFalse(promocaoExpiradaAtiva.Ativo);
            Assert.IsTrue(promocaoVigente.Ativo);
            Assert.IsFalse(promocaoExpiradaJaInativa.Ativo);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void DesativarPromocoesExpiradas_QuandoNaoHaExpiradas_NaoDeveChamarSaveChanges()
        {
            // Arrange
            var promocaoVigente = new Promocao { NomePromocao = "Vigente", Ativo = true, DataFim = DateTime.Now.AddDays(1) };
            var promocaoJaInativa = new Promocao { NomePromocao = "Já Inativa", Ativo = false, DataFim = DateTime.Now.AddDays(-10) };

            var promocoes = new List<Promocao> { promocaoVigente, promocaoJaInativa }.AsQueryable();
            _mockPromocoes = CreateDbSetMock(promocoes);
            _mockContext.Setup(c => c.Promocoes).Returns(_mockPromocoes.Object);
            _promocaoDao = new PromocaoDao(_mockContext.Object);

            // Act
            _promocaoDao.DesativarPromocoesExpiradas();

            // Assert
            Assert.IsTrue(promocaoVigente.Ativo); // Garante que nada mudou
            _mockContext.Verify(m => m.SaveChanges(), Times.Never()); // A verificação mais importante
        }

        [TestMethod]
        public async Task ObterTodosAsPromocoesAsync_DeveRetornarTodasAsPromocoes()
        {
            // Arrange
            var promocoes = new List<Promocao>
        {
            new Promocao { Ativo = true },
            new Promocao { Ativo = false },
            new Promocao { Ativo = true }
        }.AsQueryable();
            var mockPromocoesAsync = CreateDbSetMock(promocoes);
            mockPromocoesAsync.AsAsyncDbSet(promocoes);
            _mockContext.Setup(c => c.Promocoes).Returns(mockPromocoesAsync.Object);
            _promocaoDao = new PromocaoDao(_mockContext.Object);

            // Act
            var resultado = await _promocaoDao.ObterTodosAsPromocoesAsync();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(3, resultado.Count); // Deve retornar todas, independente do status 'Ativo'
        }

        [TestMethod]
        public async Task ObterPromocoesAtivasAsync_DeveRetornarApenasAtivas()
        {
            // Arrange
            var promocoes = new List<Promocao>
        {
            new Promocao { Ativo = true },
            new Promocao { Ativo = false },
            new Promocao { Ativo = true }
        }.AsQueryable();
            var mockPromocoesAsync = CreateDbSetMock(promocoes);
            mockPromocoesAsync.AsAsyncDbSet(promocoes);
            _mockContext.Setup(c => c.Promocoes).Returns(mockPromocoesAsync.Object);
            _promocaoDao = new PromocaoDao(_mockContext.Object);

            // Act
            var resultado = await _promocaoDao.ObterPromocoesAtivasAsync();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }
    }
}
