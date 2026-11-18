using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Models;

namespace SistemInformasiNilaiAkademik.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Pengguna> Pengguna => Set<Pengguna>();
        public DbSet<Jurusan> Jurusan => Set<Jurusan>();
        public DbSet<Guru> Guru => Set<Guru>();
        public DbSet<Kelas> Kelas => Set<Kelas>();
        public DbSet<Siswa> Siswa => Set<Siswa>();
        public DbSet<PeriodeAkademik> PeriodeAkademik => Set<PeriodeAkademik>();
        public DbSet<Mapel> Mapel => Set<Mapel>();
        public DbSet<KomponenNilai> KomponenNilai => Set<KomponenNilai>();
        public DbSet<BobotNilai> BobotNilai => Set<BobotNilai>();
        public DbSet<PenugasanMengajar> PenugasanMengajar => Set<PenugasanMengajar>();
        public DbSet<Nilai> Nilai => Set<Nilai>();
        public DbSet<LogPerubahanNilai> LogPerubahanNilai => Set<LogPerubahanNilai>();
        public DbSet<RekapNilai> RekapNilai => Set<RekapNilai>();
        public DbSet<RentangNilaiHuruf> RentangNilaiHuruf => Set<RentangNilaiHuruf>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes & Unique Constraints
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.NamaRole)
                .IsUnique();

            modelBuilder.Entity<Pengguna>()
                .HasIndex(p => p.Username)
                .IsUnique();

            modelBuilder.Entity<Jurusan>()
                .HasIndex(j => j.Kode)
                .IsUnique();

            modelBuilder.Entity<Guru>()
                .HasIndex(g => g.NipNrg)
                .IsUnique();

            modelBuilder.Entity<Guru>()
                .HasIndex(g => g.IdPengguna)
                .IsUnique();

            modelBuilder.Entity<Kelas>()
                .HasIndex(k => k.NamaKelas)
                .IsUnique();

            modelBuilder.Entity<Kelas>()
                .HasIndex(k => k.IdWaliKelas)
                .IsUnique()
                .HasFilter(null); // allow nulls

            modelBuilder.Entity<Siswa>()
                .HasIndex(s => s.Nis)
                .IsUnique();

            modelBuilder.Entity<Siswa>()
                .HasIndex(s => s.Nisn)
                .IsUnique();

            modelBuilder.Entity<Mapel>()
                .HasIndex(m => m.KodeMapel)
                .IsUnique();

            modelBuilder.Entity<KomponenNilai>()
                .HasIndex(k => k.NamaKomponen)
                .IsUnique();

            // Nilai unique composite (mendukung banyak tugas harian: Tugas 1, Tugas 2, Tugas 3, dst.)
            modelBuilder.Entity<Nilai>()
                .HasIndex(n => new { n.IdSiswa, n.IdMapel, n.IdPeriode, n.IdKomponen, n.NamaTugas })
                .IsUnique();

            // Bobot Nilai unique composite
            modelBuilder.Entity<BobotNilai>()
                .HasIndex(b => new { b.IdMapel, b.IdKomponen })
                .IsUnique();

            // Rekap Nilai unique composite
            modelBuilder.Entity<RekapNilai>()
                .HasIndex(r => new { r.IdSiswa, r.IdMapel, r.IdPeriode })
                .IsUnique();

            // Penugasan Mengajar unique composite
            modelBuilder.Entity<PenugasanMengajar>()
                .HasIndex(p => new { p.IdGuru, p.IdMapel, p.IdKelas, p.IdPeriode })
                .IsUnique();

            // Delete Behaviors (Protect data integrity as per SRS)
            modelBuilder.Entity<Siswa>()
                .HasOne(s => s.Kelas)
                .WithMany(k => k.SiswaList)
                .HasForeignKey(s => s.IdKelas)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nilai>()
                .HasOne(n => n.Siswa)
                .WithMany(s => s.NilaiList)
                .HasForeignKey(n => n.IdSiswa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nilai>()
                .HasOne(n => n.Mapel)
                .WithMany(m => m.NilaiList)
                .HasForeignKey(n => n.IdMapel)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nilai>()
                .HasOne(n => n.GuruPenginput)
                .WithMany(g => g.NilaiDiinputList)
                .HasForeignKey(n => n.IdGuruPenginput)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RekapNilai>()
                .HasOne(r => r.Siswa)
                .WithMany(s => s.RekapNilaiList)
                .HasForeignKey(r => r.IdSiswa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kelas>()
                .HasOne(k => k.WaliKelas)
                .WithOne(g => g.KelasWali)
                .HasForeignKey<Kelas>(k => k.IdWaliKelas)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PenugasanMengajar>()
                .HasOne(p => p.Guru)
                .WithMany(g => g.PenugasanList)
                .HasForeignKey(p => p.IdGuru)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PenugasanMengajar>()
                .HasOne(p => p.Kelas)
                .WithMany(k => k.PenugasanList)
                .HasForeignKey(p => p.IdKelas)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PenugasanMengajar>()
                .HasOne(p => p.Mapel)
                .WithMany(m => m.PenugasanList)
                .HasForeignKey(p => p.IdMapel)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LogPerubahanNilai>()
                .HasOne(l => l.Nilai)
                .WithMany(n => n.LogPerubahanList)
                .HasForeignKey(l => l.IdNilai)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
