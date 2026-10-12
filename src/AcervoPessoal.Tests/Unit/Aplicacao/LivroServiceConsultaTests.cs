using AcervoPessoal.Api.Aplicacao;
using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Tests.Unit.Aplicacao;

public sealed class LivroServiceConsultaTests : IDisposable
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 12, 14, 30, 0, TimeSpan.Zero);

    private readonly BancoEmMemoria _banco = new();

    public void Dispose() => _banco.Dispose();

    private async Task<LivroResposta> CadastrarAsync(string titulo, string autor = "Autor", int ano = 2000)
    {
        using var contexto = _banco.CriarContexto();
        return await new LivroService(contexto).CadastrarAsync(new LivroRequisicao(titulo, autor, ano));
    }

    private void AlterarNoBanco(int id, Action<Livro> alterar)
    {
        using var contexto = _banco.CriarContexto();
        var livro = contexto.Livros.Single(l => l.Id == id);
        alterar(livro);
        contexto.SaveChanges();
    }

    [Fact]
    public async Task RF01_CadastroValido_GravaLivroDisponivelEDevolveIdentificador()
    {
        var resposta = await CadastrarAsync("O Hobbit", "J. R. R. Tolkien", 2019);

        Assert.True(resposta.Id > 0);
        Assert.Equal(StatusLivro.Disponivel, resposta.Situacao);
        Assert.Null(resposta.EmprestimoAtual);
        using var contexto = _banco.CriarContexto();
        Assert.Equal("O Hobbit", contexto.Livros.Single(l => l.Id == resposta.Id).Titulo);
    }

    [Fact]
    public async Task RN01_CadastroInvalido_LancaValidacaoENaoGravaNada()
    {
        using var contexto = _banco.CriarContexto();
        var servico = new LivroService(contexto);

        var excecao = await Assert.ThrowsAsync<ValidacaoException>(
            () => servico.CadastrarAsync(new LivroRequisicao("  ", "Autor", 2000)));

        Assert.Equal("titulo", excecao.Campo);
        Assert.Empty(contexto.Livros);
    }

    [Fact]
    public async Task RN13_ExemplarRepetido_EhCadastradoComNovoIdentificador()
    {
        var primeiro = await CadastrarAsync("O Hobbit", "J. R. R. Tolkien", 2019);
        var segundo = await CadastrarAsync("O Hobbit", "J. R. R. Tolkien", 2019);

        Assert.NotEqual(primeiro.Id, segundo.Id);
    }

    [Fact]
    public async Task RF02_ObterLivroExistente_DevolveDadosESituacao()
    {
        var cadastrado = await CadastrarAsync("O Hobbit", "J. R. R. Tolkien", 2019);
        using var contexto = _banco.CriarContexto();

        var resposta = await new LivroService(contexto).ObterPorIdAsync(cadastrado.Id);

        Assert.Equal(cadastrado.Id, resposta.Id);
        Assert.Equal("O Hobbit", resposta.Titulo);
        Assert.Equal("J. R. R. Tolkien", resposta.Autor);
        Assert.Equal(2019, resposta.Ano);
        Assert.Equal(StatusLivro.Disponivel, resposta.Situacao);
    }

    [Fact]
    public async Task RF02_ObterLivroEmprestado_DevolveOEmprestimoAtual()
    {
        var cadastrado = await CadastrarAsync("O Hobbit");
        AlterarNoBanco(cadastrado.Id, livro => livro.Emprestar("Maria", Agora));
        using var contexto = _banco.CriarContexto();

        var resposta = await new LivroService(contexto).ObterPorIdAsync(cadastrado.Id);

        Assert.Equal(StatusLivro.Emprestado, resposta.Situacao);
        Assert.Equal(new EmprestimoAtualResposta("Maria", Agora), resposta.EmprestimoAtual);
    }

    [Fact]
    public async Task RN07_ObterIdInexistente_LancaNaoEncontrado()
    {
        using var contexto = _banco.CriarContexto();

        await Assert.ThrowsAsync<NaoEncontradoException>(() => new LivroService(contexto).ObterPorIdAsync(999));
    }

    [Fact]
    public async Task RN07_ObterLivroRemovido_LancaNaoEncontrado()
    {
        var cadastrado = await CadastrarAsync("O Hobbit");
        AlterarNoBanco(cadastrado.Id, livro => livro.Remover(Agora));
        using var contexto = _banco.CriarContexto();

        await Assert.ThrowsAsync<NaoEncontradoException>(
            () => new LivroService(contexto).ObterPorIdAsync(cadastrado.Id));
    }

    [Fact]
    public async Task RF03_Listar_DevolveOrdenadoPorTituloSemDiferenciarMaiusculas()
    {
        await CadastrarAsync("cereja");
        await CadastrarAsync("Abacate");
        await CadastrarAsync("banana");
        using var contexto = _banco.CriarContexto();

        var livros = await new LivroService(contexto).ListarAsync();

        Assert.Equal(["Abacate", "banana", "cereja"], livros.Select(l => l.Titulo));
    }

    [Fact]
    public async Task RN07_Listar_DeixaRemovidosDeFora()
    {
        var removido = await CadastrarAsync("Removido");
        await CadastrarAsync("Mantido");
        AlterarNoBanco(removido.Id, livro => livro.Remover(Agora));
        using var contexto = _banco.CriarContexto();

        var livros = await new LivroService(contexto).ListarAsync();

        Assert.Equal("Mantido", Assert.Single(livros).Titulo);
    }

    [Fact]
    public async Task RF03_ListarAcervoVazio_DevolveListaVazia()
    {
        using var contexto = _banco.CriarContexto();

        var livros = await new LivroService(contexto).ListarAsync();

        Assert.Empty(livros);
    }
}
