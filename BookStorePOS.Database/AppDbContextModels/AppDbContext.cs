using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAuthor> TblAuthors { get; set; }

    public virtual DbSet<TblBook> TblBooks { get; set; }

    public virtual DbSet<TblBookEdition> TblBookEditions { get; set; }

    public virtual DbSet<TblEdition> TblEditions { get; set; }

    public virtual DbSet<TblGenre> TblGenres { get; set; }

    public virtual DbSet<TblOrder> TblOrders { get; set; }

    public virtual DbSet<TblOrderItem> TblOrderItems { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=BookStore;User ID=sa;Password=sasa@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAuthor>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__TblAutho__70DAFC346D92882E");

            entity.ToTable("TblAuthor");

            entity.HasIndex(e => e.AuthorName, "UIX_AuthorName")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.AuthorName).HasMaxLength(150);
            entity.Property(e => e.CreateAt).HasDefaultValueSql("(getdate())", "DF_TblAuthor_CreatedAt");
        });

        modelBuilder.Entity<TblBook>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__tmp_ms_x__3DE0C207D6DEDD12");

            entity.ToTable("TblBook");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Author).WithMany(p => p.TblBooks)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("Fk_Author");

            entity.HasOne(d => d.Genre).WithMany(p => p.TblBooks)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("Fk_Genre");
        });

        modelBuilder.Entity<TblBookEdition>(entity =>
        {
            entity.HasKey(e => e.BookEditionId).HasName("PK__tmp_ms_x__54819873A1D9433B");

            entity.ToTable("TblBookEdition");

            entity.HasIndex(e => e.Isbn, "Idx_TblEdition_ISBN")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("ISBN");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PublishDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ReorderLevel).HasDefaultValue(5);

            entity.HasOne(d => d.Book).WithMany(p => p.TblBookEditions)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookEdition_Book");

            entity.HasOne(d => d.Edition).WithMany(p => p.TblBookEditions)
                .HasForeignKey(d => d.EditionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookEdition_Edition");
        });

        modelBuilder.Entity<TblEdition>(entity =>
        {
            entity.HasKey(e => e.EditionId).HasName("PK__TblEditi__C7622363CFD08036");

            entity.ToTable("TblEdition");

            entity.HasIndex(e => e.EditionName, "Idx_EditionName_Active")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EditionName).HasMaxLength(50);
        });

        modelBuilder.Entity<TblGenre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__TblGenre__0385057E26A78F5E");

            entity.ToTable("TblGenre");

            entity.HasIndex(e => e.GenreName, "uidx_genrename")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_TblGenre_CreatedAt");
            entity.Property(e => e.GenreName).HasMaxLength(50);
        });

        modelBuilder.Entity<TblOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFB21F22B1");

            entity.ToTable("TblOrder");

            entity.HasIndex(e => e.CustomerId, "Idx_TblOrder_CustomerId");

            entity.HasIndex(e => e.OrderStatus, "Idx_TblOrder_OrderStatus");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_TblOrder_CreatedAt");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderSource)
                .HasMaxLength(20)
                .HasDefaultValue("POS", "DF_TblOrder_OrderSource");
            entity.Property(e => e.OrderStatus).HasDefaultValue((byte)2, "DF_TblOrder_OrderStatus");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.TblOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_TblOrder_TblUser");
        });

        modelBuilder.Entity<TblOrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681C2F930B0");

            entity.ToTable("TblOrderItem");

            entity.HasIndex(e => e.BookEditionId, "IX_TblOrderItem_BookEditionId");

            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.BookEdition).WithMany(p => p.TblOrderItems)
                .HasForeignKey(d => d.BookEditionId)
                .HasConstraintName("FK_TblOrderItem_BookEditionId");

            entity.HasOne(d => d.Order).WithMany(p => p.TblOrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItems_Orders");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__TblUser__1788CC4C06ADC7F1");

            entity.ToTable("TblUser");

            entity.HasIndex(e => e.Email, "Idx_TblUser_Email")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Phone, "Idx_TblUser_Phone")
                .IsUnique()
                .HasFilter("([isDeleted]=(0))");

            entity.HasIndex(e => e.Username, "Idx_TblUser_Username")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_TblUser_CreatedAt");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
