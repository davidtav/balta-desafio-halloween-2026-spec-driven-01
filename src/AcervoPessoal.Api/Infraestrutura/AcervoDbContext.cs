using AcervoPessoal.Api.Dominio;
using Microsoft.EntityFrameworkCore;

namespace AcervoPessoal.Api.Infraestrutura;

public sealed class AcervoDbContext(DbContextOptions<AcervoDbContext> options) : DbContext(options)
{
    public DbSet<Livro> Livros => Set<Livro>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcervoDbContext).Assembly);
}
