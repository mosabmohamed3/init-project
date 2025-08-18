using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using REM.Domain.Entities;

namespace REM.Infrastructure.Configurations;

public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(a => a.Id);
        builder.ToTable(builder.Metadata.ClrType.Name + "s");
        builder.HasIndex(a => a.Is_Deleted);
    }
}
