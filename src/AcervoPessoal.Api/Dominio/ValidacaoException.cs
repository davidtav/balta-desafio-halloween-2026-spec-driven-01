namespace AcervoPessoal.Api.Dominio;

public sealed class ValidacaoException(string campo, string mensagem) : Exception(mensagem)
{
    public string Campo { get; } = campo;
}
