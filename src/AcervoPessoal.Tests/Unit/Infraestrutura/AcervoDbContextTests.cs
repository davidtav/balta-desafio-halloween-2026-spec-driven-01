using AcervoPessoal.Api.Dominio;
using Microsoft.EntityFrameworkCore;

namespace AcervoPessoal.Tests.Unit.Infraestrutura;

public sealed class AcervoDbContextTests : IDisposable
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 12, 14, 30, 0, TimeSpan.Zero);

    private readonly BancoEmMemoria _banco = new();

    public void Dispose() => _banco.Dispose();

    private int GravarLivro(Action<Livro>? preparar = null)
    {
        using var contexto = _banco.CriarContexto();
        var livro = new Livro("O Hobbit", "J. R. R. Tolkien", 2019);
        preparar?.Invoke(livro);
        contexto.Livros.Add(livro);
        contexto.SaveChanges();
        return livro.Id;
    }

    [Fact]
    public void S04_MigracaoInicial_EhAplicadaSemPendencias()
    {
        using var contexto = _banco.CriarContexto();

        Assert.Single(contexto.Database.GetAppliedMigrations());
        Assert.Empty(contexto.Database.GetPendingMigrations());
    }

    [Fact]
    public void RF01_LivroGravado_EhLidoDeVoltaComOsMesmosDados()
    {
        var id = GravarLivro();

        using var contexto = _banco.CriarContexto();
        var livro = contexto.Livros.Single(l => l.Id == id);

        Assert.Equal("O Hobbit", livro.Titulo);
        Assert.Equal("J. R. R. Tolkien", livro.Autor);
        Assert.Equal(2019, livro.Ano);
        Assert.Equal(StatusLivro.Disponivel, livro.Situacao);
    }

    [Fact]
    public void RN07_LivroRemovido_NaoVoltaEmConsultaMasContinuaGuardado()
    {
        var id = GravarLivro(livro => livro.Remover(Agora));

        using var contexto = _banco.CriarContexto();

        Assert.Null(contexto.Livros.SingleOrDefault(l => l.Id == id));
        var guardado = contexto.Livros.IgnoreQueryFilters().Single(l => l.Id == id);
        Assert.True(guardado.Removido);
        Assert.Equal(Agora, guardado.RemovidoEm);
    }

    [Fact]
    public void RF07_EmprestimoGravado_EhLidoDeVoltaComoEmprestimoAtual()
    {
        var id = GravarLivro(livro => livro.Emprestar("Maria", Agora));

        using var contexto = _banco.CriarContexto();
        var livro = contexto.Livros.Include(l => l.Emprestimos).Single(l => l.Id == id);

        Assert.Equal(StatusLivro.Emprestado, livro.Situacao);
        Assert.Equal("Maria", livro.EmprestimoAtual!.Pessoa);
        Assert.Equal(Agora, livro.EmprestimoAtual.DataEmprestimo);
    }

    [Fact]
    public void RN09_DoisEmprestimosEmAbertoParaOMesmoLivro_SaoRejeitadosPeloBanco()
    {
        var id = GravarLivro();
        using var primeiro = _banco.CriarContexto();
        using var segundo = _banco.CriarContexto();
        var livroNoPrimeiro = primeiro.Livros.Include(l => l.Emprestimos).Single(l => l.Id == id);
        var livroNoSegundo = segundo.Livros.Include(l => l.Emprestimos).Single(l => l.Id == id);

        livroNoPrimeiro.Emprestar("Maria", Agora);
        livroNoSegundo.Emprestar("João", Agora);
        primeiro.SaveChanges();

        Assert.Throws<DbUpdateException>(() => segundo.SaveChanges());
    }
}
