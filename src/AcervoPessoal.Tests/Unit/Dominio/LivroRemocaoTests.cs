using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Dominio;

public class LivroRemocaoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 12, 14, 30, 0, TimeSpan.Zero);

    private static Livro CriarLivro() => new("O Hobbit", "J. R. R. Tolkien", 2019);

    [Fact]
    public void RF06_RemoverLivroDisponivel_MarcaComoRemovidoMantendoOsDados()
    {
        var livro = CriarLivro();

        livro.Remover(Agora);

        Assert.True(livro.Removido);
        Assert.Equal(Agora, livro.RemovidoEm);
        Assert.Equal("O Hobbit", livro.Titulo);
    }

    [Fact]
    public void RF06_LivroDevolvido_PodeSerRemovidoMantendoOsEmprestimos()
    {
        var livro = CriarLivro();
        livro.Emprestar("Maria", Agora);
        livro.Devolver(Agora);

        livro.Remover(Agora);

        Assert.True(livro.Removido);
        Assert.Single(livro.Emprestimos);
    }

    [Fact]
    public void RN11_RemoverLivroEmprestado_LancaRegraNegocioSemRemover()
    {
        var livro = CriarLivro();
        livro.Emprestar("Maria", Agora);

        Assert.Throws<RegraNegocioException>(() => livro.Remover(Agora));

        Assert.False(livro.Removido);
        Assert.Null(livro.RemovidoEm);
        Assert.Equal(StatusLivro.Emprestado, livro.Situacao);
    }

    [Fact]
    public void RF01_LivroNovo_NaoNasceRemovido()
    {
        var livro = CriarLivro();

        Assert.False(livro.Removido);
        Assert.Null(livro.RemovidoEm);
    }
}
