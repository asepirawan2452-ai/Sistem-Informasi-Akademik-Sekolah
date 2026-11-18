using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemInformasiNilaiAkademik.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jurusan",
                columns: table => new
                {
                    id_jurusan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nama = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jurusan", x => x.id_jurusan);
                });

            migrationBuilder.CreateTable(
                name: "komponen_nilai",
                columns: table => new
                {
                    id_komponen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nama_komponen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    jenis_penilaian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    bobot_default = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_komponen_nilai", x => x.id_komponen);
                });

            migrationBuilder.CreateTable(
                name: "periode_akademik",
                columns: table => new
                {
                    id_periode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nama_periode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tahun_ajaran = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    semester = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    tanggal_mulai = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tanggal_selesai = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periode_akademik", x => x.id_periode);
                });

            migrationBuilder.CreateTable(
                name: "rentang_nilai_huruf",
                columns: table => new
                {
                    id_rentang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    huruf = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    nilai_min = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    nilai_max = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    keterangan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentang_nilai_huruf", x => x.id_rentang);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nama_role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_role);
                });

            migrationBuilder.CreateTable(
                name: "mapel",
                columns: table => new
                {
                    id_mapel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kode_mapel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nama_mapel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    kelompok = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    id_jurusan = table.Column<int>(type: "int", nullable: true),
                    deskripsi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mapel", x => x.id_mapel);
                    table.ForeignKey(
                        name: "FK_mapel_jurusan_id_jurusan",
                        column: x => x.id_jurusan,
                        principalTable: "jurusan",
                        principalColumn: "id_jurusan");
                });

            migrationBuilder.CreateTable(
                name: "pengguna",
                columns: table => new
                {
                    id_pengguna = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_role = table.Column<int>(type: "int", nullable: false),
                    nama = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    dibuat_pada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pengguna", x => x.id_pengguna);
                    table.ForeignKey(
                        name: "FK_pengguna_roles_id_role",
                        column: x => x.id_role,
                        principalTable: "roles",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bobot_nilai",
                columns: table => new
                {
                    id_bobot = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_mapel = table.Column<int>(type: "int", nullable: false),
                    id_komponen = table.Column<int>(type: "int", nullable: false),
                    persentase = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bobot_nilai", x => x.id_bobot);
                    table.ForeignKey(
                        name: "FK_bobot_nilai_komponen_nilai_id_komponen",
                        column: x => x.id_komponen,
                        principalTable: "komponen_nilai",
                        principalColumn: "id_komponen",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bobot_nilai_mapel_id_mapel",
                        column: x => x.id_mapel,
                        principalTable: "mapel",
                        principalColumn: "id_mapel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guru",
                columns: table => new
                {
                    id_guru = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pengguna = table.Column<int>(type: "int", nullable: false),
                    nip_nrg = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nama = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guru", x => x.id_guru);
                    table.ForeignKey(
                        name: "FK_guru_pengguna_id_pengguna",
                        column: x => x.id_pengguna,
                        principalTable: "pengguna",
                        principalColumn: "id_pengguna",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "kelas",
                columns: table => new
                {
                    id_kelas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nama_kelas = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tingkat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    id_jurusan = table.Column<int>(type: "int", nullable: false),
                    id_wali_kelas = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kelas", x => x.id_kelas);
                    table.ForeignKey(
                        name: "FK_kelas_guru_id_wali_kelas",
                        column: x => x.id_wali_kelas,
                        principalTable: "guru",
                        principalColumn: "id_guru",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_kelas_jurusan_id_jurusan",
                        column: x => x.id_jurusan,
                        principalTable: "jurusan",
                        principalColumn: "id_jurusan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "penugasan_mengajar",
                columns: table => new
                {
                    id_penugasan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_guru = table.Column<int>(type: "int", nullable: false),
                    id_mapel = table.Column<int>(type: "int", nullable: false),
                    id_kelas = table.Column<int>(type: "int", nullable: false),
                    id_periode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_penugasan_mengajar", x => x.id_penugasan);
                    table.ForeignKey(
                        name: "FK_penugasan_mengajar_guru_id_guru",
                        column: x => x.id_guru,
                        principalTable: "guru",
                        principalColumn: "id_guru",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_penugasan_mengajar_kelas_id_kelas",
                        column: x => x.id_kelas,
                        principalTable: "kelas",
                        principalColumn: "id_kelas",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_penugasan_mengajar_mapel_id_mapel",
                        column: x => x.id_mapel,
                        principalTable: "mapel",
                        principalColumn: "id_mapel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_penugasan_mengajar_periode_akademik_id_periode",
                        column: x => x.id_periode,
                        principalTable: "periode_akademik",
                        principalColumn: "id_periode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "siswa",
                columns: table => new
                {
                    id_siswa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nis = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nisn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nama_siswa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    jenis_kelamin = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    tanggal_lahir = table.Column<DateTime>(type: "datetime2", nullable: false),
                    alamat = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    id_kelas = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_siswa", x => x.id_siswa);
                    table.ForeignKey(
                        name: "FK_siswa_kelas_id_kelas",
                        column: x => x.id_kelas,
                        principalTable: "kelas",
                        principalColumn: "id_kelas",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nilai",
                columns: table => new
                {
                    id_nilai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_siswa = table.Column<int>(type: "int", nullable: false),
                    id_mapel = table.Column<int>(type: "int", nullable: false),
                    id_periode = table.Column<int>(type: "int", nullable: false),
                    id_komponen = table.Column<int>(type: "int", nullable: false),
                    nama_tugas = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nilai = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    tanggal_penilaian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_guru_penginput = table.Column<int>(type: "int", nullable: false),
                    diubah_pada = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nilai", x => x.id_nilai);
                    table.ForeignKey(
                        name: "FK_nilai_guru_id_guru_penginput",
                        column: x => x.id_guru_penginput,
                        principalTable: "guru",
                        principalColumn: "id_guru",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nilai_komponen_nilai_id_komponen",
                        column: x => x.id_komponen,
                        principalTable: "komponen_nilai",
                        principalColumn: "id_komponen",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nilai_mapel_id_mapel",
                        column: x => x.id_mapel,
                        principalTable: "mapel",
                        principalColumn: "id_mapel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nilai_periode_akademik_id_periode",
                        column: x => x.id_periode,
                        principalTable: "periode_akademik",
                        principalColumn: "id_periode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_nilai_siswa_id_siswa",
                        column: x => x.id_siswa,
                        principalTable: "siswa",
                        principalColumn: "id_siswa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rekap_nilai",
                columns: table => new
                {
                    id_rekap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_siswa = table.Column<int>(type: "int", nullable: false),
                    id_mapel = table.Column<int>(type: "int", nullable: false),
                    id_periode = table.Column<int>(type: "int", nullable: false),
                    nilai_akhir = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    nilai_huruf = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    tanggal_perhitungan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rekap_nilai", x => x.id_rekap);
                    table.ForeignKey(
                        name: "FK_rekap_nilai_mapel_id_mapel",
                        column: x => x.id_mapel,
                        principalTable: "mapel",
                        principalColumn: "id_mapel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rekap_nilai_periode_akademik_id_periode",
                        column: x => x.id_periode,
                        principalTable: "periode_akademik",
                        principalColumn: "id_periode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rekap_nilai_siswa_id_siswa",
                        column: x => x.id_siswa,
                        principalTable: "siswa",
                        principalColumn: "id_siswa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "log_perubahan_nilai",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_nilai = table.Column<int>(type: "int", nullable: false),
                    nilai_lama = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    nilai_baru = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    id_pengguna = table.Column<int>(type: "int", nullable: false),
                    waktu_perubahan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    alasan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_perubahan_nilai", x => x.id_log);
                    table.ForeignKey(
                        name: "FK_log_perubahan_nilai_nilai_id_nilai",
                        column: x => x.id_nilai,
                        principalTable: "nilai",
                        principalColumn: "id_nilai",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_log_perubahan_nilai_pengguna_id_pengguna",
                        column: x => x.id_pengguna,
                        principalTable: "pengguna",
                        principalColumn: "id_pengguna",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bobot_nilai_id_komponen",
                table: "bobot_nilai",
                column: "id_komponen");

            migrationBuilder.CreateIndex(
                name: "IX_bobot_nilai_id_mapel_id_komponen",
                table: "bobot_nilai",
                columns: new[] { "id_mapel", "id_komponen" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guru_id_pengguna",
                table: "guru",
                column: "id_pengguna",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guru_nip_nrg",
                table: "guru",
                column: "nip_nrg",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_jurusan_kode",
                table: "jurusan",
                column: "kode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kelas_id_jurusan",
                table: "kelas",
                column: "id_jurusan");

            migrationBuilder.CreateIndex(
                name: "IX_kelas_id_wali_kelas",
                table: "kelas",
                column: "id_wali_kelas",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kelas_nama_kelas",
                table: "kelas",
                column: "nama_kelas",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_komponen_nilai_nama_komponen",
                table: "komponen_nilai",
                column: "nama_komponen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_log_perubahan_nilai_id_nilai",
                table: "log_perubahan_nilai",
                column: "id_nilai");

            migrationBuilder.CreateIndex(
                name: "IX_log_perubahan_nilai_id_pengguna",
                table: "log_perubahan_nilai",
                column: "id_pengguna");

            migrationBuilder.CreateIndex(
                name: "IX_mapel_id_jurusan",
                table: "mapel",
                column: "id_jurusan");

            migrationBuilder.CreateIndex(
                name: "IX_mapel_kode_mapel",
                table: "mapel",
                column: "kode_mapel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nilai_id_guru_penginput",
                table: "nilai",
                column: "id_guru_penginput");

            migrationBuilder.CreateIndex(
                name: "IX_nilai_id_komponen",
                table: "nilai",
                column: "id_komponen");

            migrationBuilder.CreateIndex(
                name: "IX_nilai_id_mapel",
                table: "nilai",
                column: "id_mapel");

            migrationBuilder.CreateIndex(
                name: "IX_nilai_id_periode",
                table: "nilai",
                column: "id_periode");

            migrationBuilder.CreateIndex(
                name: "IX_nilai_id_siswa_id_mapel_id_periode_id_komponen_nama_tugas",
                table: "nilai",
                columns: new[] { "id_siswa", "id_mapel", "id_periode", "id_komponen", "nama_tugas" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pengguna_id_role",
                table: "pengguna",
                column: "id_role");

            migrationBuilder.CreateIndex(
                name: "IX_pengguna_username",
                table: "pengguna",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_penugasan_mengajar_id_guru_id_mapel_id_kelas_id_periode",
                table: "penugasan_mengajar",
                columns: new[] { "id_guru", "id_mapel", "id_kelas", "id_periode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_penugasan_mengajar_id_kelas",
                table: "penugasan_mengajar",
                column: "id_kelas");

            migrationBuilder.CreateIndex(
                name: "IX_penugasan_mengajar_id_mapel",
                table: "penugasan_mengajar",
                column: "id_mapel");

            migrationBuilder.CreateIndex(
                name: "IX_penugasan_mengajar_id_periode",
                table: "penugasan_mengajar",
                column: "id_periode");

            migrationBuilder.CreateIndex(
                name: "IX_rekap_nilai_id_mapel",
                table: "rekap_nilai",
                column: "id_mapel");

            migrationBuilder.CreateIndex(
                name: "IX_rekap_nilai_id_periode",
                table: "rekap_nilai",
                column: "id_periode");

            migrationBuilder.CreateIndex(
                name: "IX_rekap_nilai_id_siswa_id_mapel_id_periode",
                table: "rekap_nilai",
                columns: new[] { "id_siswa", "id_mapel", "id_periode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nama_role",
                table: "roles",
                column: "nama_role",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_siswa_id_kelas",
                table: "siswa",
                column: "id_kelas");

            migrationBuilder.CreateIndex(
                name: "IX_siswa_nis",
                table: "siswa",
                column: "nis",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_siswa_nisn",
                table: "siswa",
                column: "nisn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bobot_nilai");

            migrationBuilder.DropTable(
                name: "log_perubahan_nilai");

            migrationBuilder.DropTable(
                name: "penugasan_mengajar");

            migrationBuilder.DropTable(
                name: "rekap_nilai");

            migrationBuilder.DropTable(
                name: "rentang_nilai_huruf");

            migrationBuilder.DropTable(
                name: "nilai");

            migrationBuilder.DropTable(
                name: "komponen_nilai");

            migrationBuilder.DropTable(
                name: "mapel");

            migrationBuilder.DropTable(
                name: "periode_akademik");

            migrationBuilder.DropTable(
                name: "siswa");

            migrationBuilder.DropTable(
                name: "kelas");

            migrationBuilder.DropTable(
                name: "guru");

            migrationBuilder.DropTable(
                name: "jurusan");

            migrationBuilder.DropTable(
                name: "pengguna");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
