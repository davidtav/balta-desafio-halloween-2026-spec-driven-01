namespace AcervoPessoal.Api.Dominio;

internal static class Validacao
{
    // RN-10: espaços das pontas são descartados antes de validar (RN-12 conta o tamanho já sem eles).
    public static string TextoObrigatorio(string? valor, string campo, string nome, int tamanhoMaximo)
    {
        var texto = valor?.Trim();

        if (string.IsNullOrEmpty(texto))
            throw new ValidacaoException(campo, $"{nome} é obrigatório.");

        if (texto.Length > tamanhoMaximo)
            throw new ValidacaoException(campo, $"{nome} deve ter no máximo {tamanhoMaximo} caracteres.");

        return texto;
    }
}
