using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Dominio;

public class LivroCriacaoTests
{
    [Fact]
    public void RF01_LivroValido_NasceDisponivelComOsDadosInformados()
    {
        var livro = new Livro("O Hobbit", "J. R. R. Tolkien", 2019);

        Assert.Equal("O Hobbit", livro.Titulo);
        Assert.Equal("J. R. R. Tolkien", livro.Autor);
        Assert.Equal(2019, livro.Ano);
        Assert.Equal(StatusLivro.Disponivel, livro.Situacao);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RN01_TituloVazioOuSoComEspacos_LancaValidacao(string? titulo)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Livro(titulo, "Autor", 2000));

        Assert.Equal("titulo", excecao.Campo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RN02_AutorVazioOuSoComEspacos_LancaValidacao(string? autor)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Livro("Título", autor, 2000));

        Assert.Equal("autor", excecao.Campo);
    }

    [Fact]
    public void RN03_AnoNaoInformado_LancaValidacao()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Livro("Título", "Autor", null));

        Assert.Equal("ano", excecao.Campo);
        Assert.Equal("Ano é obrigatório.", excecao.Message);
    }

    [Theory]
    [InlineData(1960)]
    [InlineData(2025)]
    public void RN03_AnoNosLimitesDaFaixa_EhAceito(int ano)
    {
        var livro = new Livro("Título", "Autor", ano);

        Assert.Equal(ano, livro.Ano);
    }

    [Theory]
    [InlineData(1959)]
    [InlineData(2026)]
    public void RN03_AnoForaDaFaixa_LancaValidacao(int ano)
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Livro("Título", "Autor", ano));

        Assert.Equal("ano", excecao.Campo);
    }

    [Fact]
    public void RN10_EspacosDasPontas_SaoDescartados()
    {
        var livro = new Livro("  O Hobbit  ", "  Tolkien ", 2019);

        Assert.Equal("O Hobbit", livro.Titulo);
        Assert.Equal("Tolkien", livro.Autor);
    }

    [Fact]
    public void RN12_TituloEAutorCom200Caracteres_SaoAceitos()
    {
        var texto = new string('a', 200);

        var livro = new Livro(texto, texto, 2000);

        Assert.Equal(200, livro.Titulo.Length);
        Assert.Equal(200, livro.Autor.Length);
    }

    [Fact]
    public void RN12_TituloCom201Caracteres_LancaValidacao()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Livro(new string('a', 201), "Autor", 2000));

        Assert.Equal("titulo", excecao.Campo);
    }

    [Fact]
    public void RN12_AutorCom201Caracteres_LancaValidacao()
    {
        var excecao = Assert.Throws<ValidacaoException>(() => new Livro("Título", new string('a', 201), 2000));

        Assert.Equal("autor", excecao.Campo);
    }

    [Fact]
    public void RN12_LimiteContadoDepoisDeDescartarEspacos()
    {
        var livro = new Livro($"  {new string('a', 200)}  ", "Autor", 2000);

        Assert.Equal(200, livro.Titulo.Length);
    }
}
