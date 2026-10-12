namespace AcervoPessoal.Api.Aplicacao;

// D-13: Ano anulável para distinguir "não informado" de "fora da faixa".
public sealed record LivroRequisicao(string? Titulo, string? Autor, int? Ano);
