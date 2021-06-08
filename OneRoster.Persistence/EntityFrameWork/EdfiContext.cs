using Microsoft.EntityFrameworkCore;

namespace EdFi.OneRoster.Persistence.EntityFrameWork
{
    public class EdFiContext : DbContext
    {
        public EdFiContext(DbContextOptions<EdFiContext> options) : base(options) { }

        public virtual DbSet<academicsessions> academicsessions { get; set; }
        public virtual DbSet<classes> classes { get; set; }
        public virtual DbSet<courses> courses { get; set; }
        public virtual DbSet<demographics> demographics { get; set; }
        public virtual DbSet<enrollments> enrollments { get; set; }
        public virtual DbSet<orgs> orgs { get; set; }
        public virtual DbSet<users> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasPostgresExtension("pgcrypto");

            modelBuilder.Entity<academicsessions>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("academicsessions", "onerosterv11");
                //entity.Property(e => e.title).HasColumnType("character varying");
            });

            modelBuilder.Entity<classes>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("classes", "onerosterv11");

                //entity.Property(e => e.dateLastModified).HasColumnType("timestamp with time zone");
                //entity.Property(e => e.location).HasMaxLength(60);
            });

            modelBuilder.Entity<courses>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("courses", "onerosterv11");

                //entity.Property(e => e.courseCode).HasMaxLength(60);
                //entity.Property(e => e.dateLastModified).HasColumnType("timestamp with time zone");
                //entity.Property(e => e.title).HasMaxLength(60);
            });

            modelBuilder.Entity<demographics>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("demographics", "onerosterv11");

                //entity.Property(e => e.americanIndianOrAlaskaNative).HasColumnType("character varying");
                //entity.Property(e => e.asian).HasColumnType("character varying");
                //entity.Property(e => e.birthDate).HasColumnType("date");
                //entity.Property(e => e.blackOrAfricanAmerican).HasColumnType("character varying");
                //entity.Property(e => e.dateLastModified).HasColumnType("timestamp with time zone");
                //entity.Property(e => e.demographicRaceTwoOrMoreRaces).HasColumnType("character varying");
                //entity.Property(e => e.hispanicOrLatinoEthnicity).HasColumnType("character varying");
                //entity.Property(e => e.nativeHawaiianOrOtherPacificIslander).HasColumnType("character varying");
                //entity.Property(e => e.white).HasColumnType("character varying");
            });

            modelBuilder.Entity<enrollments>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("enrollments", "onerosterv11");
                entity.Property(e => e._class).HasColumnName("class");

                //entity.Property(e => e.beginDate).HasColumnType("timestamp with time zone");
                //entity.Property(e => e.dateLastModified).HasColumnType("timestamp with time zone");
                //entity.Property(e => e.endDate).HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<orgs>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("orgs", "onerosterv11");
            });

            modelBuilder.Entity<users>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("users", "onerosterv11");
                //entity.Property(e => e.familyName).HasMaxLength(75);
                //entity.Property(e => e.givenName).HasMaxLength(75);
                //entity.Property(e => e.identifier).HasMaxLength(32);
                //entity.Property(e => e.dateLastModified).HasColumnType("timestamp with time zone");
                //entity.Property(e => e.email).HasColumnType("character varying");
                //entity.Property(e => e.middleName).HasColumnType("character varying");
                //entity.Property(e => e.username).HasColumnType("character varying");
            });
        }
    }
}
