namespace AcervoPessoal.Api.Dominio;

public sealed class Emprestimo
{
    public const int TamanhoMaximoPessoa = 100;

    public int Id { get; private set; }
    public int LivroId { get; private set; }
    public string Pessoa { get; private set; }
    public DateTimeOffset DataEmprestimo { get; private set; }
    public DateTimeOffset? DataDevolucao { get; private set; }

    public bool EmAberto => DataDevolucao is null;

    // Só o Livro cria e encerra empréstimos (D-09).
    internal Emprestimo(string? pessoa, DateTimeOffset dataEmprestimo)
    {
        Pessoa = Validacao.TextoObrigatorio(pessoa, "pessoa", "Pessoa", TamanhoMaximoPessoa);
        DataEmprestimo = dataEmprestimo;
    }

    internal void Encerrar(DateTimeOffset dataDevolucao) => DataDevolucao = dataDevolucao;
}
