using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingSystem.Data.EntityConfigurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");
            builder.HasKey(x => x.BookingId);

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(10,2)");

            builder.Property(x => x.SpecialRequests)
                .HasMaxLength(500);
        }
    }
}