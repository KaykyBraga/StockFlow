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
    public class CaixaDaoTests
    {
        private CaixaDao _caixaDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<Caixa>> _mockCaixas;
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
            _mockCaixas = CreateDbSetMock(new List<Caixa>().AsQueryable());
            _mockMovimentacoes = CreateDbSetMock(new List<MovimentacaoCaixa>().AsQueryable());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);

            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(_mockMovimentacoes.Object);

            _caixaDao = new CaixaDao(_mockContext.Object);
        }

        [TestMethod]
        public void AbrirCaixa_QuandoNaoHaCaixaAberto_DeveAdicionarComSucesso()
        {
            // Arrange
            var novoCaixa = new Caixa { UsuarioAberturaId = 1, Status = "Aberto", ValorAbertura = 100 };

            // Act
            _caixaDao.AbrirCaixa(novoCaixa);

            // Assert
            Assert.AreEqual("", _caixaDao.mensagemErro);
            _mockCaixas.Verify(m => m.Add(It.IsAny<Caixa>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void AbrirCaixa_QuandoJaExisteCaixaAberto_DeveRetornarErro()
        {
            // Arrange
            var caixasIniciais = new List<Caixa> { new Caixa { UsuarioAberturaId = 1, Status = "Aberto" } }.AsQueryable();
            _mockCaixas = CreateDbSetMock(caixasIniciais);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _caixaDao = new CaixaDao(_mockContext.Object);

            var novoCaixa = new Caixa { UsuarioAberturaId = 1, Status = "Aberto" };

            // Act
            _caixaDao.AbrirCaixa(novoCaixa);

            // Assert
            Assert.AreEqual("Já existe um caixa aberto. Feche o caixa atual antes de abrir um novo.", _caixaDao.mensagemErro);
            _mockCaixas.Verify(m => m.Add(It.IsAny<Caixa>()), Times.Never());
        }

        [TestMethod]
        public void FecharCaixa_QuandoNaoHaCaixaAberto_DeveRetornarNuloEMensagemDeErro()
        {
            // Arrange
            // O Setup já começa com a lista de caixas vazia, perfeito para este teste.

            // Act
            var resultado = _caixaDao.FecharCaixa(1, 100m);

            // Assert
            Assert.IsNull(resultado);
            Assert.AreEqual("Nenhum caixa aberto encontrado para fechar.", _caixaDao.mensagemErro);
        }

        [TestMethod]
        public void FecharCaixa_ComDadosValidos_DeveAtualizarStatusECalcularValores()
        {
            // Arrange
            var caixaAberto = new Caixa { CaixaId = 1, UsuarioAberturaId = 1, Status = "Aberto" };
            var caixasIniciais = new List<Caixa> { caixaAberto }.AsQueryable();

            var movimentacoes = new List<MovimentacaoCaixa>
        {
            new MovimentacaoCaixa { CaixaId = 1, TipoMovimentacao = "Abertura", Valor = 100m, MetodoDePagamento = "Dinheiro" },
            new MovimentacaoCaixa { CaixaId = 1, TipoMovimentacao = "Venda", Valor = 50m, MetodoDePagamento = "Dinheiro" },
            new MovimentacaoCaixa { CaixaId = 1, TipoMovimentacao = "Venda", Valor = 30m, MetodoDePagamento = "Cartão" },
            new MovimentacaoCaixa { CaixaId = 1, TipoMovimentacao = "Sangria", Valor = 20m, MetodoDePagamento = "Dinheiro" }
        }.AsQueryable();

            _mockCaixas = CreateDbSetMock(caixasIniciais);
            _mockMovimentacoes = CreateDbSetMock(movimentacoes);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(_mockMovimentacoes.Object);
            _caixaDao = new CaixaDao(_mockContext.Object);

            // Act
            var caixaFechado = _caixaDao.FecharCaixa(1, 130m); // Informando 130

            // Assert
            Assert.IsNotNull(caixaFechado);
            Assert.AreEqual("Fechado", caixaFechado.Status);
            Assert.AreEqual(160m, caixaFechado.ValorFechamentoCalculado); // Total (100+50+30-20)
            Assert.AreEqual(130m, caixaFechado.ValorFechamentoCaixa);    // Dinheiro (100+50-20)
            Assert.AreEqual(0m, caixaFechado.Diferenca);                 // Diferença (130 calculado - 130 informado)
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void FazerSangria_ComSaldoSuficiente_DeveCriarMovimentacao()
        {
            // Arrange
            var caixaAberto = new Caixa { CaixaId = 1, UsuarioAberturaId = 1, Status = "Aberto" };
            var caixasIniciais = new List<Caixa> { caixaAberto }.AsQueryable();
            var movimentacoes = new List<MovimentacaoCaixa> { new MovimentacaoCaixa { CaixaId = 1, TipoMovimentacao = "Abertura", Valor = 200m } }.AsQueryable();

            _mockCaixas = CreateDbSetMock(caixasIniciais);
            _mockMovimentacoes = CreateDbSetMock(movimentacoes);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(_mockMovimentacoes.Object);
            _caixaDao = new CaixaDao(_mockContext.Object);

            // Act
            _caixaDao.FazerSangria(1, 50m, "Retirada");

            // Assert
            Assert.AreEqual("", _caixaDao.mensagemErro);
            _mockMovimentacoes.Verify(m => m.Add(It.Is<MovimentacaoCaixa>(mov => mov.TipoMovimentacao == "Sangria")), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void FazerSangria_ComSaldoInsuficiente_DeveRetornarErro()
        {
            // Arrange
            var caixaAberto = new Caixa { CaixaId = 1, UsuarioAberturaId = 1, Status = "Aberto" };
            var caixasIniciais = new List<Caixa> { caixaAberto }.AsQueryable();
            var movimentacoes = new List<MovimentacaoCaixa> { new MovimentacaoCaixa { CaixaId = 1, TipoMovimentacao = "Abertura", Valor = 100m } }.AsQueryable();

            _mockCaixas = CreateDbSetMock(caixasIniciais);
            _mockMovimentacoes = CreateDbSetMock(movimentacoes);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(_mockMovimentacoes.Object);
            _caixaDao = new CaixaDao(_mockContext.Object);

            // Act
            // Tenta tirar 150 de um caixa que só tem 100
            _caixaDao.FazerSangria(1, 150m, "Retirada excessiva");

            // Assert
            Assert.IsTrue(_caixaDao.mensagemErro.Contains("Não há saldo suficiente"));
            _mockMovimentacoes.Verify(m => m.Add(It.IsAny<MovimentacaoCaixa>()), Times.Never()); // Garante que nada foi salvo
        }

        [TestMethod]
        public void AdicionarReforco_QuandoHaCaixaAberto_DeveCriarMovimentacao()
        {
            // Arrange
            var caixaAberto = new Caixa { CaixaId = 1, UsuarioAberturaId = 1, Status = "Aberto" };
            var caixasIniciais = new List<Caixa> { caixaAberto }.AsQueryable();

            _mockCaixas = CreateDbSetMock(caixasIniciais);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _caixaDao = new CaixaDao(_mockContext.Object);

            // Act
            _caixaDao.AdicionarReforco(1, 75m, "Adição de troco");

            // Assert
            Assert.AreEqual("", _caixaDao.mensagemErro);
            _mockMovimentacoes.Verify(m => m.Add(It.Is<MovimentacaoCaixa>(mov => mov.TipoMovimentacao == "Reforco" && mov.Valor == 75m)), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public async Task ObterTodosOsCaixasAsync_QuandoExistemCaixas_DeveRetornarTodos()
        {
            // Arrange
            var caixasFalsos = new List<Caixa>
        {
            new Caixa { CaixaId = 1, UsuarioAberturaId = 1 },
            new Caixa { CaixaId = 2, UsuarioAberturaId = 2 },
        }.AsQueryable();

            var mockCaixasAsync = CreateDbSetMock(caixasFalsos);
            mockCaixasAsync.AsAsyncDbSet(caixasFalsos); // Usando a extensão para async

            _mockContext.Setup(c => c.Caixas).Returns(mockCaixasAsync.Object);
            _caixaDao = new CaixaDao(_mockContext.Object);

            // Act
            var resultado = await _caixaDao.ObterTodosOsCaixasAsync();

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }
    }
}
