using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemNilaiAkademik.Migrations
{
    /// <inheritdoc />
    public partial class InisialisasiAwal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Jurusan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Kode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nama = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jurusan", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KomponenNilai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NamaKomponen = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JenisPenilaian = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KomponenNilai", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PeriodeAkademik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NamaPeriode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TahunAjaran = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Semester = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TanggalMulai = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TanggalSelesai = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodeAkademik", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RentangNilaiHuruf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Huruf = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NilaiMin = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentangNilaiHuruf", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NamaRole = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Mapel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KodeMapel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NamaMapel = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kelompok = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdJurusan = table.Column<int>(type: "int", nullable: true),
                    Deskripsi = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mapel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mapel_Jurusan_IdJurusan",
                        column: x => x.IdJurusan,
                        principalTable: "Jurusan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pengguna",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    Nama = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DibuatPada = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pengguna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pengguna_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BobotNilai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdMapel = table.Column<int>(type: "int", nullable: false),
                    IdKomponen = table.Column<int>(type: "int", nullable: false),
                    Persentase = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BobotNilai", x => x.Id);
                    table.CheckConstraint("CK_BobotNilai_Persentase", "`Persentase` >= 0 AND `Persentase` <= 100");
                    table.ForeignKey(
                        name: "FK_BobotNilai_KomponenNilai_IdKomponen",
                        column: x => x.IdKomponen,
                        principalTable: "KomponenNilai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BobotNilai_Mapel_IdMapel",
                        column: x => x.IdMapel,
                        principalTable: "Mapel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Guru",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPengguna = table.Column<int>(type: "int", nullable: false),
                    NipNrg = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nama = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guru", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guru_Pengguna_IdPengguna",
                        column: x => x.IdPengguna,
                        principalTable: "Pengguna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kelas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NamaKelas = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tingkat = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdJurusan = table.Column<int>(type: "int", nullable: false),
                    IdWaliKelas = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kelas_Guru_IdWaliKelas",
                        column: x => x.IdWaliKelas,
                        principalTable: "Guru",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kelas_Jurusan_IdJurusan",
                        column: x => x.IdJurusan,
                        principalTable: "Jurusan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PenugasanMengajar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdGuru = table.Column<int>(type: "int", nullable: false),
                    IdMapel = table.Column<int>(type: "int", nullable: false),
                    IdKelas = table.Column<int>(type: "int", nullable: false),
                    IdPeriode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenugasanMengajar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PenugasanMengajar_Guru_IdGuru",
                        column: x => x.IdGuru,
                        principalTable: "Guru",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PenugasanMengajar_Kelas_IdKelas",
                        column: x => x.IdKelas,
                        principalTable: "Kelas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PenugasanMengajar_Mapel_IdMapel",
                        column: x => x.IdMapel,
                        principalTable: "Mapel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PenugasanMengajar_PeriodeAkademik_IdPeriode",
                        column: x => x.IdPeriode,
                        principalTable: "PeriodeAkademik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Siswa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nis = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nisn = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NamaSiswa = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JenisKelamin = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TanggalLahir = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Alamat = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdKelas = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siswa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siswa_Kelas_IdKelas",
                        column: x => x.IdKelas,
                        principalTable: "Kelas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Nilai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdSiswa = table.Column<int>(type: "int", nullable: false),
                    IdMapel = table.Column<int>(type: "int", nullable: false),
                    IdPeriode = table.Column<int>(type: "int", nullable: false),
                    IdKomponen = table.Column<int>(type: "int", nullable: false),
                    NilaiAngka = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TanggalPenilaian = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdGuruPenginput = table.Column<int>(type: "int", nullable: false),
                    DiubahPada = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nilai", x => x.Id);
                    table.CheckConstraint("CK_Nilai_Rentang", "`NilaiAngka` >= 0 AND `NilaiAngka` <= 100");
                    table.ForeignKey(
                        name: "FK_Nilai_Guru_IdGuruPenginput",
                        column: x => x.IdGuruPenginput,
                        principalTable: "Guru",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nilai_KomponenNilai_IdKomponen",
                        column: x => x.IdKomponen,
                        principalTable: "KomponenNilai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nilai_Mapel_IdMapel",
                        column: x => x.IdMapel,
                        principalTable: "Mapel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nilai_PeriodeAkademik_IdPeriode",
                        column: x => x.IdPeriode,
                        principalTable: "PeriodeAkademik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nilai_Siswa_IdSiswa",
                        column: x => x.IdSiswa,
                        principalTable: "Siswa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RekapNilai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdSiswa = table.Column<int>(type: "int", nullable: false),
                    IdMapel = table.Column<int>(type: "int", nullable: false),
                    IdPeriode = table.Column<int>(type: "int", nullable: false),
                    NilaiAkhir = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NilaiHuruf = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TanggalPerhitungan = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RekapNilai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RekapNilai_Mapel_IdMapel",
                        column: x => x.IdMapel,
                        principalTable: "Mapel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RekapNilai_PeriodeAkademik_IdPeriode",
                        column: x => x.IdPeriode,
                        principalTable: "PeriodeAkademik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RekapNilai_Siswa_IdSiswa",
                        column: x => x.IdSiswa,
                        principalTable: "Siswa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LogPerubahanNilai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdNilai = table.Column<int>(type: "int", nullable: false),
                    NilaiLama = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NilaiBaru = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IdPengguna = table.Column<int>(type: "int", nullable: false),
                    WaktuPerubahan = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Alasan = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogPerubahanNilai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogPerubahanNilai_Nilai_IdNilai",
                        column: x => x.IdNilai,
                        principalTable: "Nilai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogPerubahanNilai_Pengguna_IdPengguna",
                        column: x => x.IdPengguna,
                        principalTable: "Pengguna",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BobotNilai_IdKomponen",
                table: "BobotNilai",
                column: "IdKomponen");

            migrationBuilder.CreateIndex(
                name: "IX_BobotNilai_IdMapel_IdKomponen",
                table: "BobotNilai",
                columns: new[] { "IdMapel", "IdKomponen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guru_IdPengguna",
                table: "Guru",
                column: "IdPengguna",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guru_NipNrg",
                table: "Guru",
                column: "NipNrg",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jurusan_Kode",
                table: "Jurusan",
                column: "Kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kelas_IdJurusan",
                table: "Kelas",
                column: "IdJurusan");

            migrationBuilder.CreateIndex(
                name: "IX_Kelas_IdWaliKelas",
                table: "Kelas",
                column: "IdWaliKelas",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kelas_NamaKelas",
                table: "Kelas",
                column: "NamaKelas",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KomponenNilai_NamaKomponen",
                table: "KomponenNilai",
                column: "NamaKomponen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogPerubahanNilai_IdNilai",
                table: "LogPerubahanNilai",
                column: "IdNilai");

            migrationBuilder.CreateIndex(
                name: "IX_LogPerubahanNilai_IdPengguna",
                table: "LogPerubahanNilai",
                column: "IdPengguna");

            migrationBuilder.CreateIndex(
                name: "IX_Mapel_IdJurusan",
                table: "Mapel",
                column: "IdJurusan");

            migrationBuilder.CreateIndex(
                name: "IX_Mapel_KodeMapel",
                table: "Mapel",
                column: "KodeMapel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nilai_IdGuruPenginput",
                table: "Nilai",
                column: "IdGuruPenginput");

            migrationBuilder.CreateIndex(
                name: "IX_Nilai_IdKomponen",
                table: "Nilai",
                column: "IdKomponen");

            migrationBuilder.CreateIndex(
                name: "IX_Nilai_IdMapel",
                table: "Nilai",
                column: "IdMapel");

            migrationBuilder.CreateIndex(
                name: "IX_Nilai_IdPeriode",
                table: "Nilai",
                column: "IdPeriode");

            migrationBuilder.CreateIndex(
                name: "IX_Nilai_IdSiswa_IdMapel_IdPeriode_IdKomponen",
                table: "Nilai",
                columns: new[] { "IdSiswa", "IdMapel", "IdPeriode", "IdKomponen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pengguna_IdRole",
                table: "Pengguna",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Pengguna_Username",
                table: "Pengguna",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PenugasanMengajar_IdGuru_IdMapel_IdKelas_IdPeriode",
                table: "PenugasanMengajar",
                columns: new[] { "IdGuru", "IdMapel", "IdKelas", "IdPeriode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PenugasanMengajar_IdKelas",
                table: "PenugasanMengajar",
                column: "IdKelas");

            migrationBuilder.CreateIndex(
                name: "IX_PenugasanMengajar_IdMapel",
                table: "PenugasanMengajar",
                column: "IdMapel");

            migrationBuilder.CreateIndex(
                name: "IX_PenugasanMengajar_IdPeriode",
                table: "PenugasanMengajar",
                column: "IdPeriode");

            migrationBuilder.CreateIndex(
                name: "IX_RekapNilai_IdMapel",
                table: "RekapNilai",
                column: "IdMapel");

            migrationBuilder.CreateIndex(
                name: "IX_RekapNilai_IdPeriode",
                table: "RekapNilai",
                column: "IdPeriode");

            migrationBuilder.CreateIndex(
                name: "IX_RekapNilai_IdSiswa_IdMapel_IdPeriode",
                table: "RekapNilai",
                columns: new[] { "IdSiswa", "IdMapel", "IdPeriode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentangNilaiHuruf_Huruf",
                table: "RentangNilaiHuruf",
                column: "Huruf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NamaRole",
                table: "Roles",
                column: "NamaRole",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Siswa_IdKelas",
                table: "Siswa",
                column: "IdKelas");

            migrationBuilder.CreateIndex(
                name: "IX_Siswa_Nis",
                table: "Siswa",
                column: "Nis",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Siswa_Nisn",
                table: "Siswa",
                column: "Nisn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BobotNilai");

            migrationBuilder.DropTable(
                name: "LogPerubahanNilai");

            migrationBuilder.DropTable(
                name: "PenugasanMengajar");

            migrationBuilder.DropTable(
                name: "RekapNilai");

            migrationBuilder.DropTable(
                name: "RentangNilaiHuruf");

            migrationBuilder.DropTable(
                name: "Nilai");

            migrationBuilder.DropTable(
                name: "KomponenNilai");

            migrationBuilder.DropTable(
                name: "Mapel");

            migrationBuilder.DropTable(
                name: "PeriodeAkademik");

            migrationBuilder.DropTable(
                name: "Siswa");

            migrationBuilder.DropTable(
                name: "Kelas");

            migrationBuilder.DropTable(
                name: "Guru");

            migrationBuilder.DropTable(
                name: "Jurusan");

            migrationBuilder.DropTable(
                name: "Pengguna");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
