namespace AcervoPessoal.Api.Dominio;

public sealed class Livro
{
    public const int AnoMinimo = 1960;
    public const int AnoMaximo = 2025;
    public const int TamanhoMaximoTexto = 200;

    public int Id { get; private set; }
    public string Titulo { get; private set; }
    public string Autor { get; private set; }
    public int Ano { get; private set; }
    public StatusLivro Situacao { get; private set; }
    public bool Removido { get; private set; }
    public DateTimeOffset? RemovidoEm { get; private set; }

    private readonly List<Emprestimo> _emprestimos = [];
    public IReadOnlyCollection<Emprestimo> Emprestimos => _emprestimos.AsReadOnly();

    // RN-09: no máximo um empréstimo em aberto por vez.
    public Emprestimo? EmprestimoAtual => _emprestimos.SingleOrDefault(e => e.EmAberto);

    // Usado pelo EF Core ao materializar do banco.
    private Livro()
    {
        Titulo = string.Empty;
        Autor = string.Empty;
    }

    public Livro(string? titulo, string? autor, int? ano)
    {
        (Titulo, Autor, Ano) = ValidarDados(titulo, autor, ano);
        Situacao = StatusLivro.Disponivel;
    }

    // RN-08: alterar dados não muda a situação. Tudo é validado antes de atribuir,
    // então uma alteração inválida não deixa o livro pela metade.
    public void AlterarDados(string? titulo, string? autor, int? ano)
    {
        (Titulo, Autor, Ano) = ValidarDados(titulo, autor, ano);
    }

    // RN-04: só livro disponível pode ser emprestado. RN-06/RN-12: a pessoa é validada pelo Emprestimo.
    public void Emprestar(string? pessoa, DateTimeOffset agora)
    {
        if (Situacao != StatusLivro.Disponivel)
            throw new RegraNegocioException("O livro já está emprestado.");

        _emprestimos.Add(new Emprestimo(pessoa, agora));
        Situacao = StatusLivro.Emprestado;
    }

    // RN-05: só livro emprestado pode ser devolvido. RN-09: a devolução encerra o empréstimo em aberto.
    public void Devolver(DateTimeOffset agora)
    {
        if (Situacao != StatusLivro.Emprestado)
            throw new RegraNegocioException("O livro não está emprestado.");

        EmprestimoAtual!.Encerrar(agora);
        Situacao = StatusLivro.Disponivel;
    }

    // RF-06: remoção lógica, os dados e os empréstimos continuam guardados.
    // RN-11: livro emprestado precisa ser devolvido antes.
    public void Remover(DateTimeOffset agora)
    {
        if (Situacao == StatusLivro.Emprestado)
            throw new RegraNegocioException("O livro está emprestado e precisa ser devolvido antes de ser removido.");

        Removido = true;
        RemovidoEm = agora;
    }

    private static (string Titulo, string Autor, int Ano) ValidarDados(string? titulo, string? autor, int? ano)
    {
        var tituloValido = Validacao.TextoObrigatorio(titulo, "titulo", "Título", TamanhoMaximoTexto);
        var autorValido = Validacao.TextoObrigatorio(autor, "autor", "Autor", TamanhoMaximoTexto);

        if (ano is null)
            throw new ValidacaoException("ano", "Ano é obrigatório.");

        if (ano < AnoMinimo || ano > AnoMaximo)
            throw new ValidacaoException("ano", $"Ano deve estar entre {AnoMinimo} e {AnoMaximo}.");

        return (tituloValido, autorValido, ano.Value);
    }
}
