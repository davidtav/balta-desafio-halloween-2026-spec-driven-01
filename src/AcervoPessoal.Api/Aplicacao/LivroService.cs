using AcervoPessoal.Api.Dominio;
using AcervoPessoal.Api.Infraestrutura;
using Microsoft.EntityFrameworkCore;

namespace AcervoPessoal.Api.Aplicacao;

public sealed class LivroService(AcervoDbContext contexto) : ILivroService
{
    // RF-01 / RN-13: exemplares repetidos são livros independentes, sem checagem de unicidade.
    public async Task<LivroResposta> CadastrarAsync(LivroRequisicao requisicao, CancellationToken cancellationToken = default)
    {
        var livro = new Livro(requisicao.Titulo, requisicao.Autor, requisicao.Ano);

        contexto.Livros.Add(livro);
        await contexto.SaveChangesAsync(cancellationToken);

        return LivroResposta.De(livro);
    }

    // RF-02
    public async Task<LivroResposta> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        LivroResposta.De(await CarregarLivroAsync(id, cancellationToken));

    // RF-03: o filtro global já deixa os removidos de fora (D-08).
    public async Task<IReadOnlyList<LivroResposta>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var livros = await contexto.Livros
            .Include(l => l.Emprestimos)
            .OrderBy(l => l.Titulo)
            .ThenBy(l => l.Id)
            .ToListAsync(cancellationToken);

        return livros.Select(LivroResposta.De).ToList();
    }

    // RF-04: título ou autor contendo o termo, sem diferenciar maiúsculas (LIKE do SQLite, D-12).
    public async Task<IReadOnlyList<LivroResposta>> BuscarAsync(string? termo, CancellationToken cancellationToken = default)
    {
        var termoLimpo = termo?.Trim();

        if (string.IsNullOrEmpty(termoLimpo))
            throw new ValidacaoException("termo", "Termo de busca é obrigatório.");

        var padrao = $"%{EscaparLike(termoLimpo)}%";

        var livros = await contexto.Livros
            .Include(l => l.Emprestimos)
            .Where(l => EF.Functions.Like(l.Titulo, padrao, CaractereEscape)
                     || EF.Functions.Like(l.Autor, padrao, CaractereEscape))
            .OrderBy(l => l.Titulo)
            .ThenBy(l => l.Id)
            .ToListAsync(cancellationToken);

        return livros.Select(LivroResposta.De).ToList();
    }

    private const string CaractereEscape = "\\";

    // D-12: % e _ do termo são texto, não curingas.
    private static string EscaparLike(string termo) =>
        termo.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    // RN-07: inexistente e removido são igualmente "não encontrado".
    private async Task<Livro> CarregarLivroAsync(int id, CancellationToken cancellationToken) =>
        await contexto.Livros
            .Include(l => l.Emprestimos)
            .SingleOrDefaultAsync(l => l.Id == id, cancellationToken)
        ?? throw new NaoEncontradoException($"Livro {id} não encontrado.");
}
