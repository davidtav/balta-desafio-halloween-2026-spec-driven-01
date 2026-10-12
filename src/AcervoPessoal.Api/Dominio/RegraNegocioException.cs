namespace AcervoPessoal.Api.Dominio;

public sealed class RegraNegocioException(string mensagem) : Exception(mensagem);
