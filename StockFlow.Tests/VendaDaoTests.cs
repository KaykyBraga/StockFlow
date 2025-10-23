using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockFlow.DAL; // Namespace do seu VendaDao
using StockFlow.Modelo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Classe auxiliar para ajudar a "mockar" operações async do EF Core

namespace StockFlow.Tests
{
    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> AsAsyncDbSet<T>(this Mock<DbSet<T>> dbSetMock, IQueryable<T> data) where T : class
        {
            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return dbSetMock;
        }
    }


    [TestClass]
    public class VendaDaoTests
    {
        private VendaDao _vendaDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<Venda>> _mockVendas;
        private Mock<DbSet<Produto>> _mockProdutos;
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
            _mockVendas = CreateDbSetMock(new List<Venda>().AsQueryable());
            _mockProdutos = CreateDbSetMock(new List<Produto>().AsQueryable());
            _mockCaixas = CreateDbSetMock(new List<Caixa>().AsQueryable());
            _mockMovimentacoes = CreateDbSetMock(new List<MovimentacaoCaixa>().AsQueryable());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);

            _mockContext.Setup(c => c.Vendas).Returns(_mockVendas.Object);
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
            _mockContext.Setup(c => c.MovimentacaoCaixas).Returns(_mockMovimentacoes.Object);

            _vendaDao = new VendaDao(_mockContext.Object);
        }

        private void SetupCaixaAberto(int usuarioId = 1)
        {
            var caixas = new List<Caixa> { new Caixa { CaixaId = 1, Status = "Aberto", UsuarioAberturaId = usuarioId } }.AsQueryable();
            _mockCaixas = CreateDbSetMock(caixas);
            _mockContext.Setup(c => c.Caixas).Returns(_mockCaixas.Object);
        }

        private void SetupProdutos(List<Produto> produtos)
        {
            _mockProdutos = CreateDbSetMock(produtos.AsQueryable());
            _mockContext.Setup(c => c.Produtos).Returns(_mockProdutos.Object);
        }

        [TestMethod]
        public void RegistrarVenda_ComDadosValidos_DeveAdicionarVendaEMovimentacao()
        {
            // Arrange
            SetupCaixaAberto();
            var produto = new Produto { ProdutoId = 1, EstoqueAtual = 10, PrecoVenda = 5.0m, PromocaoProdutos = new List<PromocaoProduto>() };
            SetupProdutos(new List<Produto> { produto });
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 1, Quantidade = 2 } };

            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");

            // Assert
            Assert.AreEqual("", _vendaDao.mensagemErro);
            _mockVendas.Verify(m => m.Add(It.Is<Venda>(v => v.ValorTotal == 10m)), Times.Once());
            _mockMovimentacoes.Verify(m => m.Add(It.IsAny<MovimentacaoCaixa>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual(8, produto.EstoqueAtual);
        }

        [TestMethod]
        public void RegistrarVenda_ComCaixaFechado_DeveRetornarMensagemDeErro()
        {
            // Arrange (Setup já inicia sem caixa aberto)
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 1, Quantidade = 1 } };
            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");
            // Assert
            Assert.AreEqual("Não é possível registrar a venda. Nenhum caixa está aberto.", _vendaDao.mensagemErro);
            _mockVendas.Verify(m => m.Add(It.IsAny<Venda>()), Times.Never());
        }

        [TestMethod]
        public void RegistrarVenda_ComEstoqueInsuficiente_DeveRetornarMensagemDeErro()
        {
            // Arrange
            SetupCaixaAberto();
            SetupProdutos(new List<Produto> { new Produto { ProdutoId = 1, EstoqueAtual = 1, PrecoVenda = 5.0m, PromocaoProdutos = new List<PromocaoProduto>() } });
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 1, Quantidade = 2 } };
            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");
            // Assert
            Assert.IsTrue(_vendaDao.mensagemErro.Contains("estoque insuficiente"));
            _mockVendas.Verify(m => m.Add(It.IsAny<Venda>()), Times.Never());
        }

        [TestMethod]
        public void RegistrarVenda_ComProdutoInexistente_DeveRetornarMensagemDeErro()
        {
            // Arrange
            SetupCaixaAberto();
            SetupProdutos(new List<Produto>()); // Nenhum produto no "banco"
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 99, Quantidade = 1 } };
            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");
            // Assert
            Assert.IsTrue(_vendaDao.mensagemErro.Contains("indisponível"));
            _mockVendas.Verify(m => m.Add(It.IsAny<Venda>()), Times.Never());
        }

        [TestMethod]
        public void RegistrarVenda_ComPromocaoPorcentagem_DeveAplicarDescontoCorreto()
        {
            // Arrange
            SetupCaixaAberto();
            var promocao = new Promocao { Ativo = true, DataInicio = DateTime.Now.AddDays(-1), DataFim = DateTime.Now.AddDays(1), TipoDesconto = "Porcentagem", ValorDesconto = 10 }; // 10%
            var produto = new Produto { ProdutoId = 1, EstoqueAtual = 10, PrecoVenda = 100m, PromocaoProdutos = new List<PromocaoProduto> { new PromocaoProduto { Promocao = promocao } } };
            SetupProdutos(new List<Produto> { produto });
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 1, Quantidade = 2 } };

            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");

            // Assert
            // Preço com desconto: 100 - (10% de 100) = 90.  Valor total: 90 * 2 = 180. Desconto total: 10 * 2 = 20.
            _mockVendas.Verify(m => m.Add(It.Is<Venda>(v => v.ValorTotal == 180m && v.DescontoTotal == 20m)), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void RegistrarVenda_ComPromocaoValorFixo_DeveAplicarDescontoCorreto()
        {
            // Arrange
            SetupCaixaAberto();
            var promocao = new Promocao { Ativo = true, DataInicio = DateTime.Now.AddDays(-1), DataFim = DateTime.Now.AddDays(1), TipoDesconto = "ValorFixo", ValorDesconto = 5m }; // R$5,00
            var produto = new Produto { ProdutoId = 1, EstoqueAtual = 10, PrecoVenda = 50m, PromocaoProdutos = new List<PromocaoProduto> { new PromocaoProduto { Promocao = promocao } } };
            SetupProdutos(new List<Produto> { produto });
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 1, Quantidade = 3 } };

            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");

            // Assert
            // Preço com desconto: 50 - 5 = 45. Valor total: 45 * 3 = 135. Desconto total: 5 * 3 = 15.
            _mockVendas.Verify(m => m.Add(It.Is<Venda>(v => v.ValorTotal == 135m && v.DescontoTotal == 15m)), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void RegistrarVenda_ComPromocaoInativa_NaoDeveAplicarDesconto()
        {
            // Arrange
            SetupCaixaAberto();
            var promocao = new Promocao { Ativo = false, TipoDesconto = "Porcentagem", ValorDesconto = 50 }; // Promoção INATIVA
            var produto = new Produto { ProdutoId = 1, EstoqueAtual = 10, PrecoVenda = 100m, PromocaoProdutos = new List<PromocaoProduto> { new PromocaoProduto { Promocao = promocao } } };
            SetupProdutos(new List<Produto> { produto });
            var itens = new List<VendaItem> { new VendaItem { ProdutoId = 1, Quantidade = 2 } };

            // Act
            _vendaDao.RegistrarVenda(1, itens, "Dinheiro");

            // Assert
            // Preço normal: 100 * 2 = 200. Desconto: 0.
            _mockVendas.Verify(m => m.Add(It.Is<Venda>(v => v.ValorTotal == 200m && v.DescontoTotal == 0m)), Times.Once());
        }

        [TestMethod]
        public async Task ObterTodaAsVendasAsync_QuandoExistemVendas_DeveRetornarTodas()
        {
            // Arrange
            var vendasFalsas = new List<Venda> { new Venda(), new Venda() }.AsQueryable();
            var mockVendasAsync = CreateDbSetMock(vendasFalsas);
            mockVendasAsync.AsAsyncDbSet(vendasFalsas);
            _mockContext.Setup(c => c.Vendas).Returns(mockVendasAsync.Object);
            _vendaDao = new VendaDao(_mockContext.Object);

            // Act
            var resultado = await _vendaDao.ObterTodaAsVendasAsync();
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }

        [TestMethod]
        public async Task ObterTodaAsVendasAsync_QuandoNaoExistemVendas_DeveRetornarListaVazia()
        {
            // Arrange
            var vendasFalsas = new List<Venda>().AsQueryable(); // Lista VAZIA
            var mockVendasAsync = CreateDbSetMock(vendasFalsas);
            mockVendasAsync.AsAsyncDbSet(vendasFalsas);
            _mockContext.Setup(c => c.Vendas).Returns(mockVendasAsync.Object);
            _vendaDao = new VendaDao(_mockContext.Object);

            // Act
            var resultado = await _vendaDao.ObterTodaAsVendasAsync();
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }
    }
}