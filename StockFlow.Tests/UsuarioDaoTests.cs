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
    public class UsuarioDaoTests
    {
        private UsuarioDao _usuarioDao;
        private Mock<AppDbContext> _mockContext;
        private Mock<DbSet<Usuario>> _mockUsuarios;

        // Função auxiliar para configurar um DbSet mockado corretamente
        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> items) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(items.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => items.GetEnumerator());
            // Simular o método Find
            dbSetMock.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => items.FirstOrDefault(d => ((Usuario)(object)d).UsuarioId == (int)ids[0]));
            return dbSetMock;
        }

        [TestInitialize]
        public void Setup()
        {
            _mockUsuarios = CreateDbSetMock(new List<Usuario>().AsQueryable());

            var options = new DbContextOptionsBuilder<AppDbContext>().Options;
            _mockContext = new Mock<AppDbContext>(options);
            _mockContext.Setup(c => c.Usuarios).Returns(_mockUsuarios.Object);

            _usuarioDao = new UsuarioDao(_mockContext.Object);
        }

        [TestMethod]
        public void AdicionarUsuario_ComDadosValidos_DeveChamarAddESaveChanges()
        {
            // Arrange
            var novoUsuario = new Usuario { NomeCompleto = "Teste" };
            // Act
            _usuarioDao.AdcionarUsuario(novoUsuario);
            // Assert
            _mockUsuarios.Verify(m => m.Add(novoUsuario), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual("Usuário adicionado com sucesso!", _usuarioDao.mensagem);
        }

        [TestMethod]
        public void BuscarUsuarioPorId_ComIdExistente_DeveRetornarUsuario()
        {
            // Arrange
            var usuarios = new List<Usuario> { new Usuario { UsuarioId = 10, NomeCompleto = "Usuário Dez" } }.AsQueryable();
            _mockUsuarios = CreateDbSetMock(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(_mockUsuarios.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            var resultado = _usuarioDao.BuscarUsuarioPorId(10);
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(10, resultado.UsuarioId);
        }

        [TestMethod]
        public void BuscarUsuarioPorId_ComIdInexistente_DeveRetornarNulo()
        {
            // Arrange (Lista vazia do Setup é suficiente)
            // Act
            var resultado = _usuarioDao.BuscarUsuarioPorId(99);
            // Assert
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void BuscarUsuarioPorNome_ComTermoExistente_DeveRetornarLista()
        {
            // Arrange
            var usuarios = new List<Usuario>
        {
            new Usuario { NomeCompleto = "João da Silva" },
            new Usuario { NomeCompleto = "Maria Joana" },
            new Usuario { NomeCompleto = "Pedro Souza" }
        }.AsQueryable();
            _mockUsuarios = CreateDbSetMock(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(_mockUsuarios.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            var resultado = _usuarioDao.BuscarUsuarioPorNome("jo");
            // Assert
            Assert.AreEqual(2, resultado.Count); // Deve encontrar João e Joana
        }

        [TestMethod]
        public void BuscarUsuarioPorEmail_ComEmailExistente_DeveRetornarUsuario()
        {
            // Arrange
            var usuarios = new List<Usuario> { new Usuario { Email = "teste@email.com", NomeCompleto = "Usuario Teste" } }.AsQueryable();
            _mockUsuarios = CreateDbSetMock(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(_mockUsuarios.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            var resultado = _usuarioDao.BuscarUsuarioPorEmail("  teste@email.com  ");
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Usuario Teste", resultado.NomeCompleto);
        }

        [TestMethod]
        public void BuscarUsuarioPorIdentificador_ComIdentificadorExistente_DeveRetornarUsuario()
        {
            // Arrange
            var usuarios = new List<Usuario> { new Usuario { IdentificadorFuncionario = "FUNC001", NomeCompleto = "Funcionario 001" } }.AsQueryable();
            _mockUsuarios = CreateDbSetMock(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(_mockUsuarios.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            var resultado = _usuarioDao.BuscarUsuarioPorIdentificador("func001");
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Funcionario 001", resultado.NomeCompleto);
        }

        [TestMethod]
        public void EditarUsuario_DeveChamarUpdateESaveChanges()
        {
            // Arrange
            var usuario = new Usuario { UsuarioId = 1, NomeCompleto = "Nome Editado" };
            // Act
            _usuarioDao.EditarUsuario(usuario);
            // Assert
            _mockUsuarios.Verify(m => m.Update(usuario), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual("Usuário editado com sucesso!", _usuarioDao.mensagem);
        }

        [TestMethod]
        public void DesativarUsuario_ComIdExistente_DeveMarcarComoInativo()
        {
            // Arrange
            var usuarioAtivo = new Usuario { UsuarioId = 1, Ativo = true };
            var usuarios = new List<Usuario> { usuarioAtivo }.AsQueryable();
            _mockUsuarios = CreateDbSetMock(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(_mockUsuarios.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            _usuarioDao.DesativarUsuario(1);
            // Assert
            Assert.IsFalse(usuarioAtivo.Ativo);
            _mockUsuarios.Verify(m => m.Update(It.Is<Usuario>(u => !u.Ativo)), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual("Usuário desativado com sucesso!", _usuarioDao.mensagem);
        }

        [TestMethod]
        public void DesativarUsuario_ComIdInexistente_DeveRetornarMensagem()
        {
            // Arrange (Lista vazia do Setup é suficiente)
            // Act
            _usuarioDao.DesativarUsuario(99);
            // Assert
            Assert.AreEqual("Usuário não encontrado para desativar!", _usuarioDao.mensagem);
            _mockUsuarios.Verify(m => m.Update(It.IsAny<Usuario>()), Times.Never());
        }

        [TestMethod]
        public async Task ObterTodosOsUsuariosAsync_DeveRetornarTodos()
        {
            // Arrange
            var usuarios = new List<Usuario> { new Usuario(), new Usuario(), new Usuario() }.AsQueryable();
            var mockUsuariosAsync = CreateDbSetMock(usuarios);
            mockUsuariosAsync.AsAsyncDbSet(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(mockUsuariosAsync.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            var resultado = await _usuarioDao.ObterTodosOsUsuariosAsync();
            // Assert
            Assert.AreEqual(3, resultado.Count);
        }

        [TestMethod]
        public async Task ObterUsuariosAtivosAsync_DeveRetornarApenasAtivos()
        {
            // Arrange
            var usuarios = new List<Usuario> { new Usuario { Ativo = true }, new Usuario { Ativo = false }, new Usuario { Ativo = true } }.AsQueryable();
            var mockUsuariosAsync = CreateDbSetMock(usuarios);
            mockUsuariosAsync.AsAsyncDbSet(usuarios);
            _mockContext.Setup(c => c.Usuarios).Returns(mockUsuariosAsync.Object);
            _usuarioDao = new UsuarioDao(_mockContext.Object);
            // Act
            var resultado = await _usuarioDao.ObterUsuariosAtivosAsync();
            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }
    }
}
