using AcervoPessoal.Api.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AcervoPessoal.Api.Infraestrutura;

public sealed class LivroConfiguracao : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.ToTable("Livros");
        builder.HasKey(l => l.Id);

        // D-12: ordenação e comparação de título sem diferenciar maiúsculas.
        builder.Property(l => l.Titulo).HasMaxLength(Livro.TamanhoMaximoTexto).UseCollation("NOCASE").IsRequired();
        builder.Property(l => l.Autor).HasMaxLength(Livro.TamanhoMaximoTexto).IsRequired();
        builder.Property(l => l.Ano).IsRequired();
        builder.Property(l => l.Situacao).IsRequired();
        builder.Property(l => l.Removido).HasDefaultValue(false).IsRequired();
        builder.Property(l => l.RemovidoEm);

        builder.HasMany(l => l.Emprestimos).WithOne().HasForeignKey(e => e.LivroId).IsRequired();
        builder.Navigation(l => l.Emprestimos).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(l => l.EmprestimoAtual);

        // D-08 / RN-07: livro removido não aparece em nenhuma consulta.
        builder.HasQueryFilter(l => !l.Removido);
    }
}
