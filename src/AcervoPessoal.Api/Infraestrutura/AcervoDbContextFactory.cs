using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AcervoPessoal.Api.Infraestrutura;

// Usado só pela ferramenta dotnet-ef para gerar migrações (S-04).
public sealed class AcervoDbContextFactory : IDesignTimeDbContextFactory<AcervoDbContext>
{
    public AcervoDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AcervoDbContext>()
            .UseSqlite("Data Source=acervo.db")
            .Options;

        return new AcervoDbContext(options);
    }
}
