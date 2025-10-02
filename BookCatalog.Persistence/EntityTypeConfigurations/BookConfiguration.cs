using BookCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCatalog.Persistence.EntityTypeConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Description)
                   .IsRequired();

            builder.Property(b => b.PublicationYear)
                   .IsRequired();

            builder.Property(b => b.CoverImageUrl)
                   .IsRequired();

            // Связь с Author
            builder.HasOne(b => b.Author)
                   .WithMany(a => a.Books)
                   .HasForeignKey(b => b.AuthorId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Связь многие-ко-многим с Genre
            builder.HasMany(b => b.Genres)
                   .WithMany(g => g.Books)
                   .UsingEntity<Dictionary<string, object>>(
                       "BookGenres",
                       j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                       j => j.HasOne<Book>().WithMany().HasForeignKey("BookId"),
                       j => j.HasKey("BookId", "GenreId"));
        }
    }
}
