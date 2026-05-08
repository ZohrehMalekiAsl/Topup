using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Topup.Domain.Entities;

namespace Topup.Infrastructure.Configs
{
    public class ChargeRequestConfig : IEntityTypeConfiguration<ChargeRequest>
    {
        public void Configure(EntityTypeBuilder<ChargeRequest> builder)
        {
            builder.Property(x => x.PhoneNumber).HasMaxLength(11).IsRequired();
            builder.Property(x => x.SystemTrace).HasMaxLength(500).IsRequired().IsUnicode();
            builder.Property(x => x.TerminalId).HasMaxLength(2).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
        }
    }
}
