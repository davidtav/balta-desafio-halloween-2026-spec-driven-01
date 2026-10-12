namespace AcervoPessoal.Api.Aplicacao;

public interface ILivroService
{
    Task<LivroResposta> CadastrarAsync(LivroRequisicao requisicao, CancellationToken cancellationToken = default);
    Task<LivroResposta> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LivroResposta>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LivroResposta>> BuscarAsync(string? termo, CancellationToken cancellationToken = default);
}
