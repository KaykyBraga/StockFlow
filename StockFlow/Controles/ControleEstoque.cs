using Microsoft.EntityFrameworkCore.Query.Internal;
using StockFlow.DAL;
using StockFlow.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Controles
{
    public class ControleEstoque
    {
        public string mensagem = "";
        public void AdicionarMarca(string nome)
        {
           this.mensagem = "";
            MarcaDao marcaDao = new MarcaDao();
            Marca marca = new Marca();
            marca.NomeMarca = nome;
            marca.Ativo = true;
            marcaDao.CadastrarMarca(marca);
            this.mensagem = marcaDao.mensagem;
        }

        public void AdicionarCategoria(string nome)
        {
            this.mensagem = "";
            CategoriaDao categoriaDao = new CategoriaDao();
            Categoria categoria = new Categoria();
            categoria.NomeCategoria = nome;
            categoria.Ativo = true;
            categoriaDao.CadastrarCategoria(categoria);
            mensagem = categoriaDao.mensagem;
        }

        public void AdicionarFornecedor(List<string> ListaDados)
        {

            this.mensagem = "";
            DataHoraCorreta dataHoraCorreta = new DataHoraCorreta();
            ValidacaoUsuario validacaoUsuario = new ValidacaoUsuario();
            FornecedorDao fornecedorDao = new FornecedorDao();
            Fornecedor fornecedor = new Fornecedor();
            fornecedor.NomeFantasia = ListaDados[0];
            fornecedor.RazaoSocial = ListaDados[1];
            fornecedor.Cnpj = ListaDados[2];
            fornecedor.EmailPrincipal = ListaDados[3];
            fornecedor.TelefonePrincipal = ListaDados[4];
            fornecedor.Ativo = true;
            

            dataHoraCorreta.ObterHoraCorretaComCallback(horaAtual =>
            {
                if (horaAtual.HasValue)
                {
                    fornecedor.DataCadastro = horaAtual.Value;
                }
                else
                {
                    this.mensagem = "Não foi possível obter a hora correta. Fornecedor não cadastrado.";
                    return;
                }

            });

            fornecedorDao.CadastrarFornecedor(fornecedor);
            this.mensagem = fornecedorDao.mensagem;
        }

        public void CadastrarProduto(List<string> listaDados)
        {
            this.mensagem = "";
            ProdutoDao produtoDao = new ProdutoDao();
            Produto produto = new Produto();
            MovimentacaoDao movimentacaoDao = new MovimentacaoDao();
            Movimentacao movimentacao = new Movimentacao();
            ValidacaoEstoque validacao = new ValidacaoEstoque();
            DataHoraCorreta dataHoraCorreta = new DataHoraCorreta();

            decimal precoVenda;
            validacao.TentarConverterParaDecimal(listaDados[3], out precoVenda);
            decimal precoCusto;
            validacao.TentarConverterParaDecimal(listaDados[4], out precoCusto);
            

            produto.Sku = listaDados[0];
            produto.Ean = listaDados[1];
            produto.NomeCompleto = listaDados[2];
            produto.PrecoVenda = precoVenda;
            produto.PrecoCusto = precoCusto;
            produto.Ativo = true;
            produto.EstoqueAtual = validacao.CoverterParaInt(listaDados[5]);
            produto.EstoqueMinimo = validacao.CoverterParaInt(listaDados[6]);
            produto.MarcaId = validacao.CoverterParaInt(listaDados[7]);
            produto.FornecedorId = validacao.CoverterParaInt(listaDados[8]);
            produto.CategoriaId = validacao.CoverterParaInt(listaDados[9]);

            DateTime? horaAtual;
            dataHoraCorreta.ObterHoraCorretaComCallback(horaAtual =>
            {
                if (horaAtual.HasValue)
                {
                    produto.DataCadastro = horaAtual.Value;
                    movimentacao.Data = horaAtual.Value;
                }
                else
                {
                    this.mensagem = "Não foi possível obter a hora correta. Produto não cadastrado.";
                    return;
                }
            });

            if (validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }


            movimentacao.TipoMovimentacao = "Entrada";
            movimentacao.Quantidade = produto.EstoqueAtual;
            movimentacao.UsuarioId = SessaoUsuario.UsuarioId;
            movimentacao.Observacao = "Cadastro de produto.";

            produto.Movimentacaos.Add(movimentacao);

            produtoDao.CadastrarProduto(produto);

            this.mensagem = produtoDao.mensagem;

        }

        public void AdicionarProduto(List<string> listaDados)
        {
            this.mensagem = "";
            ProdutoDao produtoDao = new ProdutoDao();           
            MovimentacaoDao movimentacaoDao = new MovimentacaoDao();
            Movimentacao movimentacao = new Movimentacao();
            DataHoraCorreta dataHoraCorreta = new DataHoraCorreta();
            ValidacaoEstoque validacao = new ValidacaoEstoque();

            int id = validacao.CoverterParaInt(listaDados[0]);
            int quantidadeAdicionar = validacao.CoverterParaInt(listaDados[1]);
            DateTime? horaAtual;

            dataHoraCorreta.ObterHoraCorretaComCallback(horaAtual =>
            {
                if (horaAtual.HasValue)
                {
                    movimentacao.Data = horaAtual.Value;
                }
                else
                {
                    this.mensagem = "Não foi possível obter a hora correta. Fornecedor não cadastrado.";
                    return;
                }

            });
            movimentacao.Quantidade = quantidadeAdicionar;
            movimentacao.TipoMovimentacao = "Entrada de Produto";
            movimentacao.UsuarioId = SessaoUsuario.UsuarioId;
            movimentacao.Observacao = "Adicionando mais produtos ao estoque";
            movimentacao.ProdutoId = id;
            
            produtoDao.AdicionarEstoque(id, quantidadeAdicionar);

            if (produtoDao.mensagem != "")
            {
                this.mensagem = produtoDao.mensagem;
                return;
            }

            movimentacaoDao.CadastrarMovimentacao(movimentacao);
            if (movimentacaoDao.mensagem != "")
            {
                this.mensagem = movimentacaoDao.mensagem;
                return;
            }
            
            this.mensagem = "Produtos adicionados ao estoque com sucesso.";

        }

        public void DestativarCategoria(Categoria categoria)
        {
            this.mensagem = "";
            CategoriaDao categoriaDao = new CategoriaDao();
            categoriaDao.DesativarCategoria(categoria);
            this.mensagem = categoriaDao.mensagem;
        }

        public void DesatiarMarca(Marca marca)
        {
            this.mensagem = "";
            MarcaDao marcaDao = new MarcaDao();
            marcaDao.DesativarMarca(marca);
            this.mensagem = marcaDao.mensagem;
        }

        // Remover Fornecedor
        public void DesativarFornecedor(string id)
        {
            this.mensagem = "";
            Fornecedor fornecedor = new Fornecedor();
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();
            FornecedorDao fornecedorDao = new FornecedorDao();
            fornecedor.FornecedorId = validacaoEstoque.CoverterParaInt(id);

            if (fornecedor.FornecedorId == 0)
            {
                this.mensagem = "ID de fornecedor inválido.";
                return;
            }
            if (validacaoEstoque.mensagem != "")
            {
                this.mensagem = validacaoEstoque.mensagem;
                return;
            }
            fornecedorDao.DesativarFornecedor(fornecedor);
            this.mensagem = fornecedorDao.mensagem;

        }

        //Remover Produto
        public void DesativarProduto(string id)
        {
            Produto produto = new Produto();
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();
            ProdutoDao produtoDao = new ProdutoDao();

            produto.ProdutoId = validacaoEstoque.CoverterParaInt(id);
            if (produto.ProdutoId == 0)
            {
                this.mensagem = "ID de produto inválido.";
                return;
            }
            if (validacaoEstoque.mensagem != "")
            {
                this.mensagem = validacaoEstoque.mensagem;
                return;
            }
            produtoDao.DesativarProduto(produto);
            this.mensagem = produtoDao.mensagem;
        }

        public void EditarFornecedor(List<string> listaDados)
        {
            this.mensagem = "";
            Fornecedor fornecedor = new Fornecedor();
            ValidacaoEstoque validacaoEstoque = new ValidacaoEstoque();
            FornecedorDao fornecedorDao = new FornecedorDao();
            fornecedor.FornecedorId = validacaoEstoque.CoverterParaInt(listaDados[0]);
            fornecedor.NomeFantasia = listaDados[1];
            fornecedor.RazaoSocial = listaDados[2];
            fornecedor.Cnpj = listaDados[3];
            fornecedor.EmailPrincipal = listaDados[4];
            fornecedor.TelefonePrincipal = listaDados[5];
            fornecedor.Ativo = true;
            fornecedor.DataCadastro = Convert.ToDateTime(listaDados[6]);
            if (validacaoEstoque.mensagem != "")
            {
                this.mensagem = validacaoEstoque.mensagem;
                return;
            }
            fornecedorDao.EditarFornecedor(fornecedor);
            this.mensagem = fornecedorDao.mensagem;
        }
        public void EditarProduto(List<string> listaProduto)
        {
            this.mensagem = "";
            Produto produto = new Produto();
            ValidacaoEstoque validacao = new ValidacaoEstoque();
            MovimentacaoDao movimentacaoDao = new MovimentacaoDao();
            Movimentacao movimentacao = new Movimentacao();
            ProdutoDao produtoDao = new ProdutoDao();
            DataHoraCorreta dataHoraCorreta = new DataHoraCorreta();




            produto.ProdutoId = validacao.CoverterParaInt(listaProduto[0]);
            produto.Sku = listaProduto[1];
            produto.Ean = listaProduto[2];
            produto.NomeCompleto = listaProduto[3];
            validacao.TentarConverterParaDecimal(listaProduto[4], out decimal precoVenda);
            produto.PrecoVenda = precoVenda;
            validacao.TentarConverterParaDecimal(listaProduto[5], out decimal precoCusto);
            produto.PrecoCusto = precoCusto;
            produto.Ativo = true;
            produto.EstoqueAtual = validacao.CoverterParaInt(listaProduto[6]);
            produto.EstoqueMinimo = validacao.CoverterParaInt(listaProduto[7]);
            produto.DataCadastro = Convert.ToDateTime(listaProduto[8]);
            produto.MarcaId = validacao.CoverterParaInt(listaProduto[9]);
            produto.CategoriaId = validacao.CoverterParaInt(listaProduto[10]);
            produto.FornecedorId = validacao.CoverterParaInt(listaProduto[11]);
            if (validacao.mensagem != "")
            {
                this.mensagem = validacao.mensagem;
                return;
            }


            dataHoraCorreta.ObterHoraCorretaComCallback(horaAtual =>
            {
                if (horaAtual.HasValue)
                {
                    movimentacao.Data = horaAtual.Value;
                }
                else
                {
                    this.mensagem = "Não foi possível obter a hora correta. Fornecedor não cadastrado.";
                    return;
                }

            });
            movimentacao.Quantidade = 0;
            movimentacao.TipoMovimentacao = "Edição de Produto";
            movimentacao.UsuarioId = SessaoUsuario.UsuarioId;
            movimentacao.Observacao = listaProduto[12];
            movimentacao.ProdutoId = produto.ProdutoId;

            produtoDao.EditarProduto(produto);

            if (produtoDao.mensagem != "")
            {
                this.mensagem = produtoDao.mensagem;
                return;
            }

            movimentacaoDao.CadastrarMovimentacao(movimentacao);
            if (movimentacaoDao.mensagem != "")
            {
                this.mensagem = movimentacaoDao.mensagem;
                return;
            }

            this.mensagem = "Produto editado com sucesso.";
            return;
        }

        public List<Produto> BuscarProdutoPorNome(string nome)
        {
            ProdutoDao produtoDao = new ProdutoDao();
            var produtos = produtoDao.BuscarProdutoPorNome(nome);
            this.mensagem = produtoDao.mensagem;
            return produtos;
        }

        public async Task<List<Produto>> ObterTodosOsProdutosAsync()
        {
            ProdutoDao produtoDao = new ProdutoDao();
            List<Produto> listaProdutos = new List<Produto>();
            listaProdutos = await produtoDao.ObterTodosOsProdutosAsync();
            return listaProdutos;
        }

        public async Task<List<Fornecedor>> ObterTodosOsFornecedoresAsync()
        {
            FornecedorDao fornecedorDao = new FornecedorDao();
            List<Fornecedor> listaFornecedores = new List<Fornecedor>();
            listaFornecedores = await fornecedorDao.ObterTodosOsFornecedoresAsync();
            return listaFornecedores;
        }

        public async Task<List<Marca>> ObterTodasAsMarcasAsync()
        {
            MarcaDao marcaDao = new MarcaDao();
            List<Marca> listaMarcas = new List<Marca>();
            listaMarcas = await marcaDao.ObterTodasAsMarcasAsync();
            return listaMarcas;
        }

        public async Task<List<Categoria>> ObterTodasAsCategoriasAsync()
        {
            CategoriaDao categoriaDao = new CategoriaDao();
            List<Categoria> listaCategorias = new List<Categoria>();
            listaCategorias = await categoriaDao.ObterTodasAsCategoriasAsync();
            return listaCategorias;
        }

        public async Task<List<Movimentacao>> ObterTodasAsMovimentacoesAsync()
        {
            MovimentacaoDao movimentacaoDao = new MovimentacaoDao();
            List<Movimentacao> listaMovimentacoes = new List<Movimentacao>();
            listaMovimentacoes = await movimentacaoDao.ObterTodasAsMovimentacoesAsync();
            return listaMovimentacoes;
        }

        public async Task<List<Produto>> ObterTodosOsProdutosAtivosAsync()
        {
            ProdutoDao produtoDao = new ProdutoDao();
            List<Produto> listaProdutos = new List<Produto>();
            listaProdutos = await produtoDao.ObterProdutosAtivosAsync();
            return listaProdutos;
        }

        public async Task<List<Marca>> ObterTodosAsMarcasAtivasAsync()
        {
            MarcaDao marcaDao = new MarcaDao();
            List<Marca> listaMarcas = new List<Marca>();
            listaMarcas = await marcaDao.ObterMarcasAtivasAsync();
            return listaMarcas;
        }

        public async Task<List<Fornecedor>> ObterTodosOsFornecedoresAtivosAsync()
        {
            FornecedorDao fornecedorDao = new FornecedorDao();
            List<Fornecedor> listaFornecedores = new List<Fornecedor>();
            listaFornecedores = await fornecedorDao.ObterFornecedoresAtivosAsync();
            return listaFornecedores;
        }

        public async Task<List<Categoria>> ObterTodasCategoriasAtivasAsync()
        {
            CategoriaDao categoriaDao = new CategoriaDao();
            List<Categoria> listaCategorias = new List<Categoria>();
            listaCategorias = await categoriaDao.ObterCategoriasAtivasAsync();
            return listaCategorias;
        }

        public async Task<List<Produto>> ObterTodosOsProdtuosComEstoqueBaixoAsync()
        {
            ProdutoDao produtoDao = new ProdutoDao();
            List<Produto> listaCategorias = new List<Produto>();
            listaCategorias = await produtoDao.ObterProdutosComEstoqueBaixoAsync();
            return listaCategorias;
        }
    }
}
