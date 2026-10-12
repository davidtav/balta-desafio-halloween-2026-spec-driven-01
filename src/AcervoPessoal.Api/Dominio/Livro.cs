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

    public Livro(string? titulo, string? autor, int? ano)
    {
        (Titulo, Autor, Ano) = ValidarDados(titulo, autor, ano);
        Situacao = StatusLivro.Disponivel;
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
