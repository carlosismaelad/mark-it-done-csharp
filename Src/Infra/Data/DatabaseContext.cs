using MarkItDoneApi.V1.Session.Entity;
using MarkItDoneApi.V1.User.Entity;
using Microsoft.EntityFrameworkCore;

namespace MarkItDoneApi.Infra.Data
{
  public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
  {
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Global configuration to use snake_case names in table names
      modelBuilder.HasDefaultSchema("public");
      
      // User Migration
      modelBuilder.Entity<UserEntity>(entity =>
      {
        entity.ToTable("users");
        
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
          .HasColumnName("id")
          .HasColumnType("uuid")
          .HasDefaultValueSql("gen_random_uuid()")
          .ValueGeneratedOnAdd();
        
        entity.Property(e => e.Username)
          .HasColumnName("username")
          .HasColumnType("varchar(255)")
          .IsRequired();
        entity.HasIndex(e => e.Username)
          .IsUnique()
          .HasDatabaseName("index_users_on_username");
        
        entity.Property(e => e.Email)
          .HasColumnName("email")
          .HasColumnType("varchar(255)")
          .IsRequired();
        entity.HasIndex(e => e.Email)
          .IsUnique()
          .HasDatabaseName("index_users_on_email");
        
        entity.Property(e => e.Password)
          .HasColumnName("password_digest")
          .HasColumnType("varchar(255)")
          .IsRequired();
        
        entity.Property(e => e.CreatedAt)
          .HasColumnName("created_at")
          .HasColumnType("timestamp with time zone")
          .HasDefaultValueSql("timezone('utc', now())")
          .ValueGeneratedOnAdd();
        
        entity.Property(e => e.UpdatedAt)
          .HasColumnName("updated_at")
          .HasColumnType("timestamp with time zone")
          .HasDefaultValueSql("timezone('utc', now())")
          .ValueGeneratedOnAddOrUpdate();
      });
      
      // Session Migration
      modelBuilder.Entity<SessionEntity>(entity =>
      {
        entity.ToTable("sessions");
        
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
          .HasColumnName("id")
          .HasColumnType("uuid")
          .HasDefaultValueSql("gen_random_uuid()")
          .ValueGeneratedOnAdd();
        
        entity.Property(e => e.UserId)
          .HasColumnName("user_id")
          .HasColumnType("uuid")
          .IsRequired();
        entity.HasIndex(e => e.UserId)
          .HasDatabaseName("index_sessions_on_user_id");
        
        entity.Property(e => e.Token)
          .HasColumnName("token")
          .HasColumnType("varchar(500)")
          .IsRequired();
        entity.HasIndex(e => e.Token)
          .IsUnique()
          .HasDatabaseName("index_sessions_on_token");
        
        entity.Property(e => e.ExpiresAt)
          .HasColumnName("expires_at")
          .HasColumnType("timestamp with time zone")
          .IsRequired();
                
        // 2FA Code
        entity.Property(e => e.Code)
          .HasColumnName("code")
          .HasColumnType("varchar(6)")
          .IsRequired(false);
        entity.HasIndex(e => e.Code)
          .HasDatabaseName("index_sessions_on_code");
        
        entity.Property(e => e.CodeExpiresAt)
          .HasColumnName("code_expires_at")
          .HasColumnType("timestamp with time zone")
          .IsRequired(false);
        
        entity.Property(e => e.CodeAttempts)
          .HasColumnName("code_attempts")
          .HasColumnType("integer")
          .HasDefaultValue(0)
          .IsRequired();
        
        // Status - string with default value 'pending_verification'
        entity.Property(e => e.Status)
          .HasColumnName("status")
          .HasColumnType("varchar(50)")
          .HasDefaultValue("pending_verification")
          .IsRequired();
        entity.HasIndex(e => e.Status)
          .HasDatabaseName("index_sessions_on_status");
        
        entity.Property(e => e.CreatedAt)
          .HasColumnName("created_at")
          .HasColumnType("timestamp with time zone")
          .HasDefaultValueSql("timezone('utc', now())")
          .ValueGeneratedOnAdd();
        
        entity.Property(e => e.UpdatedAt)
          .HasColumnName("updated_at")
          .HasColumnType("timestamp with time zone")
          .HasDefaultValueSql("timezone('utc', now())")
          .ValueGeneratedOnAddOrUpdate();
      });
    }
    
  }
}