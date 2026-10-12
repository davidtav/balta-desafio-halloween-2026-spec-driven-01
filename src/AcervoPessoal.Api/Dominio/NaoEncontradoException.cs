namespace AcervoPessoal.Api.Dominio;

public sealed class NaoEncontradoException(string mensagem) : Exception(mensagem);
