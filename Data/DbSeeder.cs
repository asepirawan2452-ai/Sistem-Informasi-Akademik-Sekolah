using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Data;

public static class DbSeeder
{
    private static readonly Dictionary<string, string> NamaJurusan = new()
    {
        ["TKJ"] = "Teknik Komputer dan Jaringan",
        ["MPLB"] = "Manajemen Perkantoran dan Layanan Bisnis",
        ["AKL"] = "Akuntansi dan Keuangan Lembaga",
        ["TE"] = "Teknik Elektronika",
        ["TBSM"] = "Teknik dan Bisnis Sepeda Motor"
    };

    private static readonly (string Nama, string Jenis)[] Komponen =
    {
        ("Harian", "HARIAN"),
        ("UTS", "UTS"),
        ("UAS", "UAS")
    };

    private static readonly (string Huruf, decimal NilaiMin)[] Rentang =
    {
        ("A", 85m),
        ("B", 75m),
        ("C", 60m),
        ("D", 0m)
    };

    public static void Seed(AppDbContext context, string contentRoot)
    {
        var hasher = new PasswordHasher<Pengguna>();

        SeedRoles(context);
        SeedJurusan(context);
        SeedKomponen(context);
        SeedRentang(context);
        var periode = SeedPeriode(context);
        SeedMapel(context, contentRoot);
        SeedPenggunaDanGuru(context, contentRoot, hasher);
        SeedKelas(context, contentRoot);
        SeedPenugasan(context, contentRoot, periode);
        SeedBobot(context);
    }

    public static Dictionary<string, int> Hitung(AppDbContext context) => new()
    {
        ["Role"] = context.Roles.Count(),
        ["Jurusan"] = context.Jurusan.Count(),
        ["KomponenNilai"] = context.KomponenNilai.Count(),
        ["RentangNilaiHuruf"] = context.RentangNilaiHuruf.Count(),
        ["PeriodeAkademik"] = context.PeriodeAkademik.Count(),
        ["Pengguna"] = context.Pengguna.Count(),
        ["Guru"] = context.Guru.Count(),
        ["Kelas"] = context.Kelas.Count(),
        ["Mapel"] = context.Mapel.Count(),
        ["PenugasanMengajar"] = context.PenugasanMengajar.Count(),
        ["BobotNilai"] = context.BobotNilai.Count()
    };

    private static List<string[]> BacaBaris(string contentRoot, string namaFile)
    {
        var path = Path.Combine(contentRoot, "seed", namaFile);
        return File.ReadAllLines(path)
            .Skip(1)
            .Where(baris => !string.IsNullOrWhiteSpace(baris))
            .Select(baris => baris.Split(','))
            .ToList();
    }

    private static void SeedRoles(AppDbContext context)
    {
        foreach (var nama in new[] { "ADMIN", "GURU", "WALI_KELAS" })
        {
            if (!context.Roles.Any(r => r.NamaRole == nama))
            {
                context.Roles.Add(new Role { NamaRole = nama });
            }
        }
        context.SaveChanges();
    }

    private static void SeedJurusan(AppDbContext context)
    {
        foreach (var (kode, nama) in NamaJurusan)
        {
            if (!context.Jurusan.Any(j => j.Kode == kode))
            {
                context.Jurusan.Add(new Jurusan { Kode = kode, Nama = nama });
            }
        }
        context.SaveChanges();
    }

    private static void SeedKomponen(AppDbContext context)
    {
        foreach (var (nama, jenis) in Komponen)
        {
            if (!context.KomponenNilai.Any(k => k.NamaKomponen == nama))
            {
                context.KomponenNilai.Add(new KomponenNilai { NamaKomponen = nama, JenisPenilaian = jenis });
            }
        }
        context.SaveChanges();
    }

    private static void SeedRentang(AppDbContext context)
    {
        foreach (var (huruf, nilaiMin) in Rentang)
        {
            if (!context.RentangNilaiHuruf.Any(r => r.Huruf == huruf))
            {
                context.RentangNilaiHuruf.Add(new RentangNilaiHuruf { Huruf = huruf, NilaiMin = nilaiMin });
            }
        }
        context.SaveChanges();
    }

    private static PeriodeAkademik SeedPeriode(AppDbContext context)
    {
        var periode = context.PeriodeAkademik.FirstOrDefault(p => p.NamaPeriode == "2026/2027 Ganjil");
        if (periode is null)
        {
            periode = new PeriodeAkademik
            {
                NamaPeriode = "2026/2027 Ganjil",
                TahunAjaran = "2026/2027",
                Semester = "Ganjil",
                TanggalMulai = new DateTime(2026, 7, 1),
                TanggalSelesai = new DateTime(2026, 12, 31),
                Status = "Aktif"
            };
            context.PeriodeAkademik.Add(periode);
        }
        else
        {
            periode.Status = "Aktif";
        }

        foreach (var lain in context.PeriodeAkademik.Where(p => p.Id != periode.Id && p.Status == "Aktif"))
        {
            lain.Status = "Nonaktif";
        }
        context.SaveChanges();
        return periode;
    }

    private static string? KodeJurusanDariMapel(string kodeMapel)
    {
        if (kodeMapel.StartsWith("TKJ-")) return "TKJ";
        if (kodeMapel.StartsWith("MPL-")) return "MPLB";
        if (kodeMapel.StartsWith("AKL-")) return "AKL";
        if (kodeMapel.StartsWith("TE-")) return "TE";
        if (kodeMapel.StartsWith("TB-")) return "TBSM";
        return null;
    }

    private static void SeedMapel(AppDbContext context, string contentRoot)
    {
        var jurusan = context.Jurusan.ToDictionary(j => j.Kode, j => j.Id);
        foreach (var baris in BacaBaris(contentRoot, "mapel.csv"))
        {
            var kode = baris[0];
            if (context.Mapel.Any(m => m.KodeMapel == kode))
            {
                continue;
            }

            var kelompok = baris[2];
            int? idJurusan = null;
            if (kelompok == "Kejuruan")
            {
                var kodeJurusan = KodeJurusanDariMapel(kode);
                if (kodeJurusan is not null && jurusan.TryGetValue(kodeJurusan, out var id))
                {
                    idJurusan = id;
                }
            }

            context.Mapel.Add(new Mapel
            {
                KodeMapel = kode,
                NamaMapel = baris[1],
                Kelompok = kelompok,
                IdJurusan = idJurusan,
                Status = "Aktif"
            });
        }
        context.SaveChanges();
    }

    private static void SeedPenggunaDanGuru(AppDbContext context, string contentRoot, PasswordHasher<Pengguna> hasher)
    {
        var role = context.Roles.ToDictionary(r => r.NamaRole, r => r.Id);
        foreach (var baris in BacaBaris(contentRoot, "users.csv"))
        {
            var namaRole = baris[0];
            var nama = baris[1];
            var username = baris[2];
            var passwordPlain = baris[3];

            var pengguna = context.Pengguna.FirstOrDefault(u => u.Username == username);
            if (pengguna is null)
            {
                pengguna = new Pengguna
                {
                    IdRole = role[namaRole],
                    Nama = nama,
                    Username = username,
                    PasswordHash = hasher.HashPassword(new Pengguna(), passwordPlain),
                    Status = "Aktif",
                    DibuatPada = DateTime.Now
                };
                context.Pengguna.Add(pengguna);
                context.SaveChanges();
            }

            if (namaRole != "ADMIN" && !context.Guru.Any(g => g.IdPengguna == pengguna.Id))
            {
                context.Guru.Add(new Guru
                {
                    IdPengguna = pengguna.Id,
                    Nama = nama,
                    NipNrg = null,
                    Status = "Aktif"
                });
            }
        }
        context.SaveChanges();
    }

    private static void SeedKelas(AppDbContext context, string contentRoot)
    {
        var jurusan = context.Jurusan.ToDictionary(j => j.Kode, j => j.Id);
        var guru = context.Guru
            .Include(g => g.Pengguna)
            .ToDictionary(g => g.Pengguna.Username, g => g.Id);

        foreach (var baris in BacaBaris(contentRoot, "kelas.csv"))
        {
            var namaKelas = baris[0];
            if (context.Kelas.Any(k => k.NamaKelas == namaKelas))
            {
                continue;
            }

            context.Kelas.Add(new Kelas
            {
                NamaKelas = namaKelas,
                Tingkat = baris[1],
                IdJurusan = jurusan[baris[2]],
                IdWaliKelas = guru[baris[3]],
                Status = "Aktif"
            });
        }
        context.SaveChanges();
    }

    private static void SeedPenugasan(AppDbContext context, string contentRoot, PeriodeAkademik periode)
    {
        var guru = context.Guru
            .Include(g => g.Pengguna)
            .ToDictionary(g => g.Pengguna.Username, g => g.Id);
        var mapel = context.Mapel.ToDictionary(m => m.KodeMapel, m => m.Id);
        var kelas = context.Kelas.ToDictionary(k => k.NamaKelas, k => k.Id);

        foreach (var baris in BacaBaris(contentRoot, "penugasan_mengajar.csv"))
        {
            var idGuru = guru[baris[0]];
            var idMapel = mapel[baris[1]];
            var idKelas = kelas[baris[2]];

            var ada = context.PenugasanMengajar.Any(p =>
                p.IdGuru == idGuru && p.IdMapel == idMapel &&
                p.IdKelas == idKelas && p.IdPeriode == periode.Id);
            if (ada)
            {
                continue;
            }

            context.PenugasanMengajar.Add(new PenugasanMengajar
            {
                IdGuru = idGuru,
                IdMapel = idMapel,
                IdKelas = idKelas,
                IdPeriode = periode.Id
            });
        }
        context.SaveChanges();
    }

    private static void SeedBobot(AppDbContext context)
    {
        var komponen = context.KomponenNilai.ToDictionary(k => k.NamaKomponen, k => k.Id);
        var persentase = new Dictionary<string, decimal>
        {
            ["Harian"] = 30m,
            ["UTS"] = 30m,
            ["UAS"] = 40m
        };

        foreach (var mapel in context.Mapel.ToList())
        {
            foreach (var (nama, nilai) in persentase)
            {
                var idKomponen = komponen[nama];
                if (!context.BobotNilai.Any(b => b.IdMapel == mapel.Id && b.IdKomponen == idKomponen))
                {
                    context.BobotNilai.Add(new BobotNilai
                    {
                        IdMapel = mapel.Id,
                        IdKomponen = idKomponen,
                        Persentase = nilai
                    });
                }
            }
        }
        context.SaveChanges();
    }
}
