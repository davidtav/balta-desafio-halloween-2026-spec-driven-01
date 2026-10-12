using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Dominio;

public class LivroAlteracaoTests
{
    private static Livro CriarLivro() => new("O Hobbit", "J. R. R. Tolkien", 2019);

    [Fact]
    public void RF05_AlteracaoValida_AtualizaTituloAutorEAno()
    {
        var livro = CriarLivro();

        livro.AlterarDados("  O Senhor dos Anéis ", "Tolkien", 2001);

        Assert.Equal("O Senhor dos Anéis", livro.Titulo);
        Assert.Equal("Tolkien", livro.Autor);
        Assert.Equal(2001, livro.Ano);
    }

    [Theory]
    [InlineData("", "Autor", 2000, "titulo")]
    [InlineData("Título", "  ", 2000, "autor")]
    [InlineData("Título", "Autor", null, "ano")]
    [InlineData("Título", "Autor", 2026, "ano")]
    public void RN01_RN02_RN03_AlteracaoInvalida_LancaValidacaoSemMudarNenhumDado(
        string titulo, string autor, int? ano, string campoEsperado)
    {
        var livro = CriarLivro();

        var excecao = Assert.Throws<ValidacaoException>(() => livro.AlterarDados(titulo, autor, ano));

        Assert.Equal(campoEsperado, excecao.Campo);
        Assert.Equal("O Hobbit", livro.Titulo);
        Assert.Equal("J. R. R. Tolkien", livro.Autor);
        Assert.Equal(2019, livro.Ano);
    }

    [Fact]
    public void RN12_AlteracaoComTituloDe201Caracteres_LancaValidacao()
    {
        var livro = CriarLivro();

        var excecao = Assert.Throws<ValidacaoException>(
            () => livro.AlterarDados(new string('a', 201), "Autor", 2000));

        Assert.Equal("titulo", excecao.Campo);
    }

    [Fact]
    public void RN08_LivroDisponivel_ContinuaDisponivelDepoisDeAlterado()
    {
        var livro = CriarLivro();

        livro.AlterarDados("Outro título", "Outro autor", 2000);

        Assert.Equal(StatusLivro.Disponivel, livro.Situacao);
    }
}
