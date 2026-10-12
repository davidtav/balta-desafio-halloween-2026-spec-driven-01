using AcervoPessoal.Api.Dominio;

namespace AcervoPessoal.Api.Aplicacao;

public sealed record LivroResposta(
    int Id,
    string Titulo,
    string Autor,
    int Ano,
    StatusLivro Situacao,
    EmprestimoAtualResposta? EmprestimoAtual)
{
    // Espera os empréstimos do livro carregados, para montar o empréstimo atual.
    public static LivroResposta De(Livro livro) => new(
        livro.Id,
        livro.Titulo,
        livro.Autor,
        livro.Ano,
        livro.Situacao,
        livro.EmprestimoAtual is { } emprestimo
            ? new EmprestimoAtualResposta(emprestimo.Pessoa, emprestimo.DataEmprestimo)
            : null);
}
