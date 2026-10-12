using AcervoPessoal.Api.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcervoPessoal.Api.Infraestrutura;

public sealed class EmprestimoConfiguracao : IEntityTypeConfiguration<Emprestimo>
{
    public void Configure(EntityTypeBuilder<Emprestimo> builder)
    {
        builder.ToTable("Emprestimos");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Pessoa).HasMaxLength(Emprestimo.TamanhoMaximoPessoa).IsRequired();
        builder.Property(e => e.DataEmprestimo).IsRequired();
        builder.Property(e => e.DataDevolucao);
        builder.Ignore(e => e.EmAberto);

        // D-10 / RN-09: no máximo um empréstimo em aberto por livro, garantido pelo banco.
        builder.HasIndex(e => e.LivroId)
            .IsUnique()
            .HasFilter("\"DataDevolucao\" IS NULL")
            .HasDatabaseName("IX_Emprestimos_LivroId_EmAberto");
    }
}
