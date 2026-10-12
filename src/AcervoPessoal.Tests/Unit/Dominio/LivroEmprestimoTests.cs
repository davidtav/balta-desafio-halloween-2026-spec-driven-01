using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Dominio;

public class LivroEmprestimoTests
{
    private static readonly DateTimeOffset DataEmprestimo = new(2026, 10, 12, 14, 30, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset DataDevolucao = new(2026, 10, 20, 9, 0, 0, TimeSpan.Zero);

    private static Livro CriarLivro() => new("O Hobbit", "J. R. R. Tolkien", 2019);

    [Fact]
    public void RF07_EmprestarLivroDisponivel_MudaParaEmprestadoERegistraPessoaEData()
    {
        var livro = CriarLivro();

        livro.Emprestar("  Maria ", DataEmprestimo);

        Assert.Equal(StatusLivro.Emprestado, livro.Situacao);
        var emprestimo = Assert.Single(livro.Emprestimos);
        Assert.Same(emprestimo, livro.EmprestimoAtual);
        Assert.Equal("Maria", emprestimo.Pessoa);
        Assert.Equal(DataEmprestimo, emprestimo.DataEmprestimo);
        Assert.Null(emprestimo.DataDevolucao);
    }

    [Fact]
    public void RN04_EmprestarLivroJaEmprestado_LancaRegraNegocioSemAlterarOPrimeiro()
    {
        var livro = CriarLivro();
        livro.Emprestar("Maria", DataEmprestimo);

        Assert.Throws<RegraNegocioException>(() => livro.Emprestar("João", DataDevolucao));

        var emprestimo = Assert.Single(livro.Emprestimos);
        Assert.Equal("Maria", emprestimo.Pessoa);
        Assert.Equal(DataEmprestimo, emprestimo.DataEmprestimo);
        Assert.Equal(StatusLivro.Emprestado, livro.Situacao);
    }

    [Fact]
    public void RF08_RN09_DevolverLivroEmprestado_RegistraDataEEncerraEmprestimo()
    {
        var livro = CriarLivro();
        livro.Emprestar("Maria", DataEmprestimo);

        livro.Devolver(DataDevolucao);

        Assert.Equal(StatusLivro.Disponivel, livro.Situacao);
        Assert.Null(livro.EmprestimoAtual);
        var emprestimo = Assert.Single(livro.Emprestimos);
        Assert.Equal(DataDevolucao, emprestimo.DataDevolucao);
    }

    [Fact]
    public void RN05_DevolverLivroDisponivel_LancaRegraNegocio()
    {
        var livro = CriarLivro();

        Assert.Throws<RegraNegocioException>(() => livro.Devolver(DataDevolucao));

        Assert.Equal(StatusLivro.Disponivel, livro.Situacao);
    }

    [Fact]
    public void RN09_LivroDevolvido_PodeSerEmprestadoDeNovoMantendoOHistorico()
    {
        var livro = CriarLivro();
        livro.Emprestar("Maria", DataEmprestimo);
        livro.Devolver(DataDevolucao);

        livro.Emprestar("João", DataDevolucao);

        Assert.Equal(2, livro.Emprestimos.Count);
        Assert.Equal("João", livro.EmprestimoAtual!.Pessoa);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RN06_PessoaVaziaOuSoComEspacos_LancaValidacaoELivroContinuaDisponivel(string? pessoa)
    {
        var livro = CriarLivro();

        var excecao = Assert.Throws<ValidacaoException>(() => livro.Emprestar(pessoa, DataEmprestimo));

        Assert.Equal("pessoa", excecao.Campo);
        Assert.Equal(StatusLivro.Disponivel, livro.Situacao);
        Assert.Empty(livro.Emprestimos);
    }

    [Fact]
    public void RN12_PessoaCom100Caracteres_EhAceita()
    {
        var livro = CriarLivro();

        livro.Emprestar(new string('a', 100), DataEmprestimo);

        Assert.Equal(100, livro.EmprestimoAtual!.Pessoa.Length);
    }

    [Fact]
    public void RN12_PessoaCom101Caracteres_LancaValidacao()
    {
        var livro = CriarLivro();

        var excecao = Assert.Throws<ValidacaoException>(
            () => livro.Emprestar(new string('a', 101), DataEmprestimo));

        Assert.Equal("pessoa", excecao.Campo);
    }

    [Fact]
    public void RN08_LivroEmprestado_ContinuaEmprestadoDepoisDeAlterado()
    {
        var livro = CriarLivro();
        livro.Emprestar("Maria", DataEmprestimo);

        livro.AlterarDados("Outro título", "Outro autor", 2000);

        Assert.Equal(StatusLivro.Emprestado, livro.Situacao);
        Assert.Equal("Maria", livro.EmprestimoAtual!.Pessoa);
    }
}
