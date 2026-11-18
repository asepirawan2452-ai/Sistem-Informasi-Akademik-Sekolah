using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Data;

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
    public DbSet<Mapel> Mapel => Set<Mapel>();
    public DbSet<PeriodeAkademik> PeriodeAkademik => Set<PeriodeAkademik>();
    public DbSet<PenugasanMengajar> PenugasanMengajar => Set<PenugasanMengajar>();
    public DbSet<KomponenNilai> KomponenNilai => Set<KomponenNilai>();
    public DbSet<BobotNilai> BobotNilai => Set<BobotNilai>();
    public DbSet<Nilai> Nilai => Set<Nilai>();
    public DbSet<LogPerubahanNilai> LogPerubahanNilai => Set<LogPerubahanNilai>();
    public DbSet<RentangNilaiHuruf> RentangNilaiHuruf => Set<RentangNilaiHuruf>();
    public DbSet<RekapNilai> RekapNilai => Set<RekapNilai>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => x.NamaRole).IsUnique();
            entity.Property(x => x.NamaRole).HasMaxLength(50);
        });

        modelBuilder.Entity<Pengguna>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Nama).HasMaxLength(100);
            entity.Property(x => x.Username).HasMaxLength(100);
            entity.Property(x => x.PasswordHash).HasMaxLength(255);
            entity.Property(x => x.Status).HasMaxLength(20);
            entity.HasOne(x => x.Role).WithMany(x => x.Pengguna).HasForeignKey(x => x.IdRole);
        });

        modelBuilder.Entity<Jurusan>(entity =>
        {
            entity.HasIndex(x => x.Kode).IsUnique();
            entity.Property(x => x.Kode).HasMaxLength(20);
            entity.Property(x => x.Nama).HasMaxLength(100);
        });

        modelBuilder.Entity<Guru>(entity =>
        {
            entity.HasIndex(x => x.NipNrg).IsUnique();
            entity.Property(x => x.NipNrg).HasMaxLength(50);
            entity.Property(x => x.Nama).HasMaxLength(100);
            entity.HasOne(x => x.Pengguna).WithOne(x => x.Guru).HasForeignKey<Guru>(x => x.IdPengguna);
        });

        modelBuilder.Entity<Kelas>(entity =>
        {
            entity.HasIndex(x => x.NamaKelas).IsUnique();
            entity.HasIndex(x => x.IdWaliKelas).IsUnique();
            entity.Property(x => x.NamaKelas).HasMaxLength(50);
            entity.Property(x => x.Tingkat).HasMaxLength(5);
            entity.HasOne(x => x.Jurusan).WithMany(x => x.Kelas).HasForeignKey(x => x.IdJurusan);
            entity.HasOne(x => x.WaliKelas).WithMany(x => x.KelasDiwali).HasForeignKey(x => x.IdWaliKelas);
        });

        modelBuilder.Entity<Siswa>(entity =>
        {
            entity.HasIndex(x => x.Nis).IsUnique();
            entity.HasIndex(x => x.Nisn).IsUnique();
            entity.Property(x => x.Nis).HasMaxLength(30);
            entity.Property(x => x.Nisn).HasMaxLength(30);
            entity.Property(x => x.NamaSiswa).HasMaxLength(100);
            entity.Property(x => x.JenisKelamin).HasMaxLength(20);
            entity.Property(x => x.Alamat).HasMaxLength(255);
            entity.HasOne(x => x.Kelas).WithMany(x => x.Siswa).HasForeignKey(x => x.IdKelas);
        });

        modelBuilder.Entity<Mapel>(entity =>
        {
            entity.HasIndex(x => x.KodeMapel).IsUnique();
            entity.Property(x => x.KodeMapel).HasMaxLength(20);
            entity.Property(x => x.NamaMapel).HasMaxLength(100);
            entity.Property(x => x.Kelompok).HasMaxLength(20);
            entity.HasOne(x => x.Jurusan).WithMany(x => x.Mapel).HasForeignKey(x => x.IdJurusan);
        });

        modelBuilder.Entity<PeriodeAkademik>(entity =>
        {
            entity.Property(x => x.NamaPeriode).HasMaxLength(100);
            entity.Property(x => x.TahunAjaran).HasMaxLength(20);
            entity.Property(x => x.Semester).HasMaxLength(20);
        });

        modelBuilder.Entity<PenugasanMengajar>(entity =>
        {
            entity.HasIndex(x => new { x.IdGuru, x.IdMapel, x.IdKelas, x.IdPeriode }).IsUnique();
            entity.HasOne(x => x.Guru).WithMany(x => x.PenugasanMengajar).HasForeignKey(x => x.IdGuru);
            entity.HasOne(x => x.Mapel).WithMany(x => x.PenugasanMengajar).HasForeignKey(x => x.IdMapel);
            entity.HasOne(x => x.Kelas).WithMany(x => x.PenugasanMengajar).HasForeignKey(x => x.IdKelas);
            entity.HasOne(x => x.PeriodeAkademik).WithMany(x => x.PenugasanMengajar).HasForeignKey(x => x.IdPeriode);
        });

        modelBuilder.Entity<KomponenNilai>(entity =>
        {
            entity.HasIndex(x => x.NamaKomponen).IsUnique();
            entity.Property(x => x.NamaKomponen).HasMaxLength(50);
            entity.Property(x => x.JenisPenilaian).HasMaxLength(20);
        });

        modelBuilder.Entity<BobotNilai>(entity =>
        {
            entity.HasIndex(x => new { x.IdMapel, x.IdKomponen }).IsUnique();
            entity.Property(x => x.Persentase).HasColumnType("decimal(5,2)");
            entity.ToTable(t => t.HasCheckConstraint("CK_BobotNilai_Persentase", "`Persentase` >= 0 AND `Persentase` <= 100"));
            entity.HasOne(x => x.Mapel).WithMany(x => x.BobotNilai).HasForeignKey(x => x.IdMapel);
            entity.HasOne(x => x.KomponenNilai).WithMany(x => x.BobotNilai).HasForeignKey(x => x.IdKomponen);
        });

        modelBuilder.Entity<Nilai>(entity =>
        {
            entity.HasIndex(x => new { x.IdSiswa, x.IdMapel, x.IdPeriode, x.IdKomponen }).IsUnique();
            entity.Property(x => x.NilaiAngka).HasColumnType("decimal(5,2)");
            entity.ToTable(t => t.HasCheckConstraint("CK_Nilai_Rentang", "`NilaiAngka` >= 0 AND `NilaiAngka` <= 100"));
            entity.HasOne(x => x.Siswa).WithMany(x => x.Nilai).HasForeignKey(x => x.IdSiswa);
            entity.HasOne(x => x.Mapel).WithMany(x => x.Nilai).HasForeignKey(x => x.IdMapel);
            entity.HasOne(x => x.PeriodeAkademik).WithMany(x => x.Nilai).HasForeignKey(x => x.IdPeriode);
            entity.HasOne(x => x.KomponenNilai).WithMany(x => x.Nilai).HasForeignKey(x => x.IdKomponen);
            entity.HasOne(x => x.GuruPenginput).WithMany(x => x.NilaiDicatat).HasForeignKey(x => x.IdGuruPenginput);
        });

        modelBuilder.Entity<LogPerubahanNilai>(entity =>
        {
            entity.Property(x => x.NilaiLama).HasColumnType("decimal(5,2)");
            entity.Property(x => x.NilaiBaru).HasColumnType("decimal(5,2)");
            entity.Property(x => x.Alasan).HasMaxLength(255);
            entity.HasOne(x => x.Nilai).WithMany(x => x.LogPerubahanNilai).HasForeignKey(x => x.IdNilai);
            entity.HasOne(x => x.Pengguna).WithMany(x => x.LogPerubahanNilai).HasForeignKey(x => x.IdPengguna);
        });

        modelBuilder.Entity<RentangNilaiHuruf>(entity =>
        {
            entity.HasIndex(x => x.Huruf).IsUnique();
            entity.Property(x => x.Huruf).HasMaxLength(5);
            entity.Property(x => x.NilaiMin).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<RekapNilai>(entity =>
        {
            entity.HasIndex(x => new { x.IdSiswa, x.IdMapel, x.IdPeriode }).IsUnique();
            entity.Property(x => x.NilaiAkhir).HasColumnType("decimal(5,2)");
            entity.Property(x => x.NilaiHuruf).HasMaxLength(5);
            entity.HasOne(x => x.Siswa).WithMany(x => x.RekapNilai).HasForeignKey(x => x.IdSiswa);
            entity.HasOne(x => x.Mapel).WithMany(x => x.RekapNilai).HasForeignKey(x => x.IdMapel);
            entity.HasOne(x => x.PeriodeAkademik).WithMany(x => x.RekapNilai).HasForeignKey(x => x.IdPeriode);
        });
    }
}
