using AcervoPessoal.Api.Aplicacao;
using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Aplicacao;

public sealed class LivroServiceBuscaTests : IDisposable
{
    private readonly BancoEmMemoria _banco = new();

    public LivroServiceBuscaTests()
    {
        using var contexto = _banco.CriarContexto();
        contexto.Livros.AddRange(
            new Livro("O Hobbit", "J. R. R. Tolkien", 2019),
            new Livro("Dom Casmurro", "Machado de Assis", 1997));
        contexto.SaveChanges();
    }

    public void Dispose() => _banco.Dispose();

    private async Task<IReadOnlyList<LivroResposta>> BuscarAsync(string? termo)
    {
        using var contexto = _banco.CriarContexto();
        return await new LivroService(contexto).BuscarAsync(termo);
    }

    [Fact]
    public async Task RF04_BuscaPorAutorSemDiferenciarMaiusculas_EncontraOLivro()
    {
        var livros = await BuscarAsync("tolkien");

        Assert.Equal("J. R. R. Tolkien", Assert.Single(livros).Autor);
    }

    [Fact]
    public async Task RF04_BuscaPorParteDoTitulo_EncontraOLivro()
    {
        var livros = await BuscarAsync("casmurro");

        Assert.Equal("Dom Casmurro", Assert.Single(livros).Titulo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task RF04_TermoVazio_LancaValidacao(string? termo)
    {
        var excecao = await Assert.ThrowsAsync<ValidacaoException>(() => BuscarAsync(termo));

        Assert.Equal("termo", excecao.Campo);
    }

    [Fact]
    public async Task RF04_BuscaSemResultado_DevolveListaVazia()
    {
        var livros = await BuscarAsync("inexistente");

        Assert.Empty(livros);
    }
}
