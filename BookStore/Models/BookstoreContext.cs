using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models;

public partial class BookstoreContext : DbContext
{
    public BookstoreContext()
    {
    }

    public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookInvoicesIn> BookInvoicesIns { get; set; }

    public virtual DbSet<BookInvoicesOut> BookInvoicesOuts { get; set; }

    public virtual DbSet<Bookshelf> Bookshelves { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<InvoicesIn> InvoicesIns { get; set; }

    public virtual DbSet<InvoicesOut> InvoicesOuts { get; set; }

    public virtual DbSet<KindOfBook> KindOfBooks { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<PublisherBook> PublisherBooks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersRole> UsersRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-0J3580Q\\HLONG; Initial Catalog=bookstore; Persist Security Info=True; User ID=spring; Password=123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__book__490D1AE1F9FED6B2");

            entity.ToTable("book");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.Authors)
                .HasMaxLength(255)
                .HasColumnName("authors");
            entity.Property(e => e.Content)
                .IsUnicode(false)
                .HasColumnName("content");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.Date)
                .HasPrecision(6)
                .HasColumnName("date");
            entity.Property(e => e.KindOfBookId).HasColumnName("kind_of_book_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Number).HasColumnName("number");

            entity.HasOne(d => d.KindOfBook).WithMany(p => p.Books)
                .HasForeignKey(d => d.KindOfBookId)
                .HasConstraintName("FK8vs1uih8ygwyd8lodf861q8ty");
        });

        modelBuilder.Entity<BookInvoicesIn>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("book_invoices_in");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.InvoicesInId).HasColumnName("invoices_in_id");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKnmkuom9980yl1tfg1o8o8gci8");

            entity.HasOne(d => d.InvoicesIn).WithMany()
                .HasForeignKey(d => d.InvoicesInId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKp0dye1teu92637jyhl0hef28p");
        });

        modelBuilder.Entity<BookInvoicesOut>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("book_invoices_out");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.InvoicesOutId).HasColumnName("invoices_out_id");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKfhfpd8sq9pgun3enrskpkv03");

            entity.HasOne(d => d.InvoicesOut).WithMany()
                .HasForeignKey(d => d.InvoicesOutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK1s9mfh8dgt9teassdpfgogiyj");
        });

        modelBuilder.Entity<Bookshelf>(entity =>
        {
            entity.HasKey(e => e.BookshelfId).HasName("PK__bookshel__79379078A6AACB68");

            entity.ToTable("bookshelf");

            entity.HasIndex(e => e.KindOfBookId, "UK_qcdhpcwemtbgved4ig8qu0240")
                .IsUnique()
                .HasFilter("([kind_of_book_id] IS NOT NULL)");

            entity.Property(e => e.BookshelfId).HasColumnName("bookshelf_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.KindOfBookId).HasColumnName("kind_of_book_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.KindOfBook).WithOne(p => p.Bookshelf)
                .HasForeignKey<Bookshelf>(d => d.KindOfBookId)
                .HasConstraintName("FK958drrexm3j7d13k523a0bwmq");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB8577A1D790");

            entity.ToTable("customer");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__employee__C52E0BA83B8E3671");

            entity.ToTable("employee");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BookshelfId).HasColumnName("bookshelf_id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Salary).HasColumnName("salary");

            entity.HasOne(d => d.Bookshelf).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BookshelfId)
                .HasConstraintName("FKn8mp1u249ctaud7qv3l373dm3");

            entity.HasOne(d => d.Manager).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FKfemnv0llvsjg4adl4xl1m0cxv");
        });

        modelBuilder.Entity<InvoicesIn>(entity =>
        {
            entity.HasKey(e => e.InvoicesInId).HasName("PK__invoices__7E83114252A6E89F");

            entity.ToTable("invoices_in");

            entity.Property(e => e.InvoicesInId).HasColumnName("invoices_in_id");
            entity.Property(e => e.Date)
                .HasPrecision(6)
                .HasColumnName("date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.InvoicesIns)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK8ke5jhx8qy5q8c2bo5mks5g04");

            entity.HasOne(d => d.Publisher).WithMany(p => p.InvoicesIns)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK943botksrcjy0dgqudap3i48v");
        });

        modelBuilder.Entity<InvoicesOut>(entity =>
        {
            entity.HasKey(e => e.InvoicesOutId).HasName("PK__invoices__F2CE5CFCDD6D40D2");

            entity.ToTable("invoices_out");

            entity.Property(e => e.InvoicesOutId).HasColumnName("invoices_out_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Date)
                .HasPrecision(6)
                .HasColumnName("date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.InvoicesOuts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FKlxqkgy42tc418tycnfqrmyh1f");

            entity.HasOne(d => d.Employee).WithMany(p => p.InvoicesOuts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FKb83i5410wquk0s4ef7nbhrs2v");
        });

        modelBuilder.Entity<KindOfBook>(entity =>
        {
            entity.HasKey(e => e.KindOfBookId).HasName("PK__kind_of___5D3E5FA7CE8E7406");

            entity.ToTable("kind_of_book");

            entity.Property(e => e.KindOfBookId).HasColumnName("kind_of_book_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK__manager__5A6073FCCD94D543");

            entity.ToTable("manager");

            entity.HasIndex(e => e.BookshelfId, "UK_pjcyp4sri5vs4wf1fox14u3df")
                .IsUnique()
                .HasFilter("([bookshelf_id] IS NOT NULL)");

            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BookshelfId).HasColumnName("bookshelf_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Salary).HasColumnName("salary");

            entity.HasOne(d => d.Bookshelf).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.BookshelfId)
                .HasConstraintName("FKc4b4j5pujmf14j8q959a0pwmc");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__publishe__3263F29DE52C8661");

            entity.ToTable("publisher");

            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PublisherBook>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("publisher_book");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKnro4ab7u1j42osd4sehbkptrr");

            entity.HasOne(d => d.Publisher).WithMany()
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK6buft2dj1d6ig7hekbx7c0ysp");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F91E70585");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FCC8B84A0");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Enabled).HasColumnName("enabled");
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(255)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UsersRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("users_roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKj6m8fwv7oqv74fcehir1a9ffy");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK2o0jvgh89lemvvo17cbqvdxaa");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
