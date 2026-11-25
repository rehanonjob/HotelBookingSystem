using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Data.EntityConfigurations
{
    public class BookingStatusConfiguration : IEntityTypeConfiguration<BookingStatus>
    {
        public void Configure(EntityTypeBuilder<BookingStatus> builder)
        {
            builder.ToTable("BookingStatuses");
            builder.HasKey(x => x.BookingStatusId);

            builder.Property(x => x.StatusName)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}