using AcervoPessoal.Api.Infraestrutura;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AcervoPessoal.Tests.Unit;

// SQLite em memória isolado por teste (Q-06): vive enquanto a conexão estiver aberta.
public sealed class BancoEmMemoria : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly DbContextOptions<AcervoDbContext> _opcoes;

    public BancoEmMemoria()
    {
        _conexao = new SqliteConnection("DataSource=:memory:");
        _conexao.Open();

        _opcoes = new DbContextOptionsBuilder<AcervoDbContext>()
            .UseSqlite(_conexao)
            .Options;

        using var contexto = CriarContexto();
        contexto.Database.Migrate();
    }

    public AcervoDbContext CriarContexto() => new(_opcoes);

    public void Dispose() => _conexao.Dispose();
}
