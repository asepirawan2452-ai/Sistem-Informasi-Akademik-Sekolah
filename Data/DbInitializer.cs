using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using SistemInformasiNilaiAkademik.Models;

namespace SistemInformasiNilaiAkademik.Data
{
    public static class DbInitializer
    {
        public static void SeedDevelopmentData(AppDbContext context)
        {
            if (context.Roles.Any())
            {
                return; // Database has been seeded
            }

            // 1. ROLES
            var roleAdmin = new Role { NamaRole = "Admin" };
            var roleGuru = new Role { NamaRole = "Guru" };
            var roleWali = new Role { NamaRole = "WaliKelas" };
            context.Roles.AddRange(roleAdmin, roleGuru, roleWali);
            context.SaveChanges();

            // 2. KOMPONEN NILAI (Default 30% Harian, 30% UTS, 40% UAS as per SRS)
            var kompHarian = new KomponenNilai { NamaKomponen = "Harian", JenisPenilaian = "Tugas, Kuis, & Praktik Harian", BobotDefault = 30m };
            var kompUTS = new KomponenNilai { NamaKomponen = "UTS", JenisPenilaian = "Ujian Tengah Semester", BobotDefault = 30m };
            var kompUAS = new KomponenNilai { NamaKomponen = "UAS", JenisPenilaian = "Ujian Akhir Semester", BobotDefault = 40m };
            context.KomponenNilai.AddRange(kompHarian, kompUTS, kompUAS);

            // 3. RENTANG NILAI HURUF (A: 85-100, B: 75-<85, C: 60-<75, D: 0-<60 as per SRS)
            var rentangA = new RentangNilaiHuruf { Huruf = "A", NilaiMin = 85.00m, NilaiMax = 100.00m, Keterangan = "Sangat Baik" };
            var rentangB = new RentangNilaiHuruf { Huruf = "B", NilaiMin = 75.00m, NilaiMax = 84.99m, Keterangan = "Baik" };
            var rentangC = new RentangNilaiHuruf { Huruf = "C", NilaiMin = 60.00m, NilaiMax = 74.99m, Keterangan = "Cukup" };
            var rentangD = new RentangNilaiHuruf { Huruf = "D", NilaiMin = 0.00m, NilaiMax = 59.99m, Keterangan = "Kurang" };
            context.RentangNilaiHuruf.AddRange(rentangA, rentangB, rentangC, rentangD);

            // 4. JURUSAN (5 Konsentrasi Keahlian SMK)
            var jurTkj = new Jurusan { Kode = "TKJ", Nama = "Teknik Komputer dan Jaringan" };
            var jurMplb = new Jurusan { Kode = "MPLB", Nama = "Manajemen Perkantoran dan Layanan Bisnis" };
            var jurAkl = new Jurusan { Kode = "AKL", Nama = "Akuntansi dan Keuangan Lembaga" };
            var jurTe = new Jurusan { Kode = "TE", Nama = "Teknik Elektronika" };
            var jurTbsm = new Jurusan { Kode = "TBSM", Nama = "Teknik dan Bisnis Sepeda Motor" };
            context.Jurusan.AddRange(jurTkj, jurMplb, jurAkl, jurTe, jurTbsm);

            // 5. PERIODE AKADEMIK
            var periodeAktif = new PeriodeAkademik
            {
                NamaPeriode = "Semester Ganjil 2025/2026",
                TahunAjaran = "2025/2026",
                Semester = "Ganjil",
                TanggalMulai = new DateTime(2025, 7, 14),
                TanggalSelesai = new DateTime(2025, 12, 23),
                Status = "Aktif"
            };
            context.PeriodeAkademik.Add(periodeAktif);
            context.SaveChanges();

            // 6. USERS & GURU
            string hashAdmin = BCrypt.Net.BCrypt.HashPassword("admin123");
            string hashGuru = BCrypt.Net.BCrypt.HashPassword("guru123");
            string hashWali = BCrypt.Net.BCrypt.HashPassword("wali123");

            // 6.1 Administrator (2 Akun)
            var admin1 = new Pengguna { IdRole = roleAdmin.IdRole, Nama = "Rahmat Hidayat", Username = "rahmat.hidayat", PasswordHash = hashAdmin, Status = "Aktif" };
            var admin2 = new Pengguna { IdRole = roleAdmin.IdRole, Nama = "Sri Wahyuni", Username = "sri.wahyuni", PasswordHash = hashAdmin, Status = "Aktif" };
            context.Pengguna.AddRange(admin1, admin2);

            // 6.2 Guru Muatan Umum (14 Akun)
            var guruUmumData = new List<(string nama, string username, string nip)>
            {
                ("Ahmad Fauzi", "ahmad.fauzi", "198001012005011001"),
                ("Wulan Sari", "wulan.sari", "198202022006022002"),
                ("Siti Aminah", "siti.aminah", "198303032007032003"),
                ("Rudi Hartono", "rudi.hartono", "198104042005011004"),
                ("Nanda Pratiwi", "nanda.pratiwi", "198505052009022005"),
                ("Yoga Pratama", "yoga.pratama", "198606062010011006"),
                ("Rina Marlina", "rina.marlina", "198707072011022007"),
                ("Dedi Kurniawan", "dedi.kurniawan", "198408082008011008"),
                ("Bayu Setiawan", "bayu.setiawan", "198909092012011009"),
                ("Lestari Wulandari", "lestari.wulandari", "199010102014022010"),
                ("Maya Anggraini", "maya.anggraini", "199111112015022011"),
                ("Fitri Handayani", "fitri.handayani", "199212122016022012"),
                ("Eko Prasetyo", "eko.prasetyo", "198801132013011013"),
                ("Hana Permata", "hana.permata", "199302142017022014")
            };

            var listGuruUmum = new List<Guru>();
            foreach (var item in guruUmumData)
            {
                var p = new Pengguna { IdRole = roleGuru.IdRole, Nama = item.nama, Username = item.username, PasswordHash = hashGuru, Status = "Aktif" };
                context.Pengguna.Add(p);
                context.SaveChanges();

                var g = new Guru { IdPengguna = p.IdPengguna, NipNrg = item.nip, Nama = item.nama, Status = "Aktif" };
                context.Guru.Add(g);
                listGuruUmum.Add(g);
            }
            context.SaveChanges();

            // 6.3 Guru Kejuruan Bukan Wali Kelas (5 Akun)
            var guruKejuruanData = new List<(string nama, string username, string nip)>
            {
                ("Dimas Ramadhan", "dimas.ramadhan", "198503152009011015"),
                ("Hendra Gunawan", "hendra.gunawan", "198604162010011016"),
                ("Lukman Hakim", "lukman.hakim", "198705172011011017"),
                ("Putra Wijaya", "putra.wijaya", "198806182012011018"),
                ("Usman Maulana", "usman.maulana", "198907192013011019")
            };

            var listGuruKejuruan = new List<Guru>();
            foreach (var item in guruKejuruanData)
            {
                var p = new Pengguna { IdRole = roleGuru.IdRole, Nama = item.nama, Username = item.username, PasswordHash = hashGuru, Status = "Aktif" };
                context.Pengguna.Add(p);
                context.SaveChanges();

                var g = new Guru { IdPengguna = p.IdPengguna, NipNrg = item.nip, Nama = item.nama, Status = "Aktif" };
                context.Guru.Add(g);
                listGuruKejuruan.Add(g);
            }
            context.SaveChanges();

            // 6.4 Wali Kelas (15 Akun)
            var waliKelasData = new List<(string nama, string username, string nip, string kelas, string tingkat, Jurusan jur)>
            {
                ("Andi Setiawan", "andi.setiawan", "198001202005011020", "X TKJ", "X", jurTkj),
                ("Budi Santoso", "budi.santoso", "198102212006011021", "XI TKJ", "XI", jurTkj),
                ("Citra Lestari", "citra.lestari", "198203222007022022", "XII TKJ", "XII", jurTkj),
                ("Eka Puspitasari", "eka.puspitasari", "198304232008022023", "X MPLB", "X", jurMplb),
                ("Fitriani Rahayu", "fitriani.rahayu", "198405242009022024", "XI MPLB", "XI", jurMplb),
                ("Gita Maharani", "gita.maharani", "198506252010022025", "XII MPLB", "XII", jurMplb),
                ("Indah Permatasari", "indah.permatasari", "198607262011022026", "X AKL", "X", jurAkl),
                ("Joko Susilo", "joko.susilo", "198708272012011027", "XI AKL", "XI", jurAkl),
                ("Kartika Dewi", "kartika.dewi", "198809282013022028", "XII AKL", "XII", jurAkl),
                ("Mulyadi", "mulyadi", "198910292014011029", "X TE", "X", jurTe),
                ("Nugroho Adi", "nugroho.adi", "199011302015011030", "XI TE", "XI", jurTe),
                ("Oki Firmansyah", "oki.firmansyah", "199112312016011031", "XII TE", "XII", jurTe),
                ("Rizal Anwar", "rizal.anwar", "199201012017011032", "X TBSM", "X", jurTbsm),
                ("Slamet Riyadi", "slamet.riyadi", "199302022018011033", "XI TBSM", "XI", jurTbsm),
                ("Taufik Hidayat", "taufik.hidayat", "199403032019011034", "XII TBSM", "XII", jurTbsm)
            };

            var listKelas = new List<Kelas>();
            var listGuruWali = new List<Guru>();

            foreach (var item in waliKelasData)
            {
                var p = new Pengguna { IdRole = roleWali.IdRole, Nama = item.nama, Username = item.username, PasswordHash = hashWali, Status = "Aktif" };
                context.Pengguna.Add(p);
                context.SaveChanges();

                var g = new Guru { IdPengguna = p.IdPengguna, NipNrg = item.nip, Nama = item.nama, Status = "Aktif" };
                context.Guru.Add(g);
                context.SaveChanges();
                listGuruWali.Add(g);

                var k = new Kelas
                {
                    NamaKelas = item.kelas,
                    Tingkat = item.tingkat,
                    IdJurusan = item.jur.IdJurusan,
                    IdWaliKelas = g.IdGuru,
                    Status = "Aktif"
                };
                context.Kelas.Add(k);
                listKelas.Add(k);
            }
            context.SaveChanges();

            // 7. MATA PELAJARAN (Muatan Umum + Kejuruan)
            var mapelList = new List<Mapel>
            {
                // Umum
                new Mapel { KodeMapel = "UM-01", NamaMapel = "Pendidikan Agama dan Budi Pekerti", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-02", NamaMapel = "Pendidikan Pancasila", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-03", NamaMapel = "Bahasa Indonesia", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-04", NamaMapel = "PJOK", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-05", NamaMapel = "Sejarah", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-06", NamaMapel = "Seni Budaya", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-07", NamaMapel = "Bahasa Inggris", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-08", NamaMapel = "Matematika", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-09", NamaMapel = "Informatika", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-10", NamaMapel = "Projek IPAS", Kelompok = "Muatan Umum", Status = "Aktif" },
                new Mapel { KodeMapel = "UM-11", NamaMapel = "Projek Kreatif dan Kewirausahaan", Kelompok = "Muatan Umum", Status = "Aktif" },

                // TKJ
                new Mapel { KodeMapel = "TKJ-01", NamaMapel = "Dasar-dasar TJKT", Kelompok = "Kejuruan", IdJurusan = jurTkj.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TKJ-02", NamaMapel = "Administrasi Infrastruktur Jaringan (AIJ)", Kelompok = "Kejuruan", IdJurusan = jurTkj.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TKJ-03", NamaMapel = "Administrasi Sistem Jaringan (ASJ)", Kelompok = "Kejuruan", IdJurusan = jurTkj.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TKJ-04", NamaMapel = "Teknologi Layanan Jaringan", Kelompok = "Kejuruan", IdJurusan = jurTkj.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TKJ-05", NamaMapel = "Teknologi Jaringan Berbasis Luas (WAN)", Kelompok = "Kejuruan", IdJurusan = jurTkj.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TKJ-06", NamaMapel = "Keamanan Jaringan", Kelompok = "Kejuruan", IdJurusan = jurTkj.IdJurusan, Status = "Aktif" },

                // MPLB
                new Mapel { KodeMapel = "MPL-01", NamaMapel = "Dasar-dasar MPLB", Kelompok = "Kejuruan", IdJurusan = jurMplb.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "MPL-02", NamaMapel = "Kearsipan Digital", Kelompok = "Kejuruan", IdJurusan = jurMplb.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "MPL-03", NamaMapel = "Administrasi Umum & Korespondensi", Kelompok = "Kejuruan", IdJurusan = jurMplb.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "MPL-04", NamaMapel = "Teknologi Perkantoran", Kelompok = "Kejuruan", IdJurusan = jurMplb.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "MPL-05", NamaMapel = "Administrasi Keuangan Humas", Kelompok = "Kejuruan", IdJurusan = jurMplb.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "MPL-06", NamaMapel = "Layanan Bisnis & Keprotokolan", Kelompok = "Kejuruan", IdJurusan = jurMplb.IdJurusan, Status = "Aktif" },

                // AKL
                new Mapel { KodeMapel = "AKL-01", NamaMapel = "Dasar-dasar Akuntansi Keuangan", Kelompok = "Kejuruan", IdJurusan = jurAkl.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "AKL-02", NamaMapel = "Akuntansi Keuangan Menengah", Kelompok = "Kejuruan", IdJurusan = jurAkl.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "AKL-03", NamaMapel = "Komputerisasi Akuntansi (MYOB/Accurate)", Kelompok = "Kejuruan", IdJurusan = jurAkl.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "AKL-04", NamaMapel = "Administrasi Pajak", Kelompok = "Kejuruan", IdJurusan = jurAkl.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "AKL-05", NamaMapel = "Praktikum Akuntansi Perusahaan Jasa & Dagang", Kelompok = "Kejuruan", IdJurusan = jurAkl.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "AKL-06", NamaMapel = "Akuntansi Perbankan & Lembaga Mikro", Kelompok = "Kejuruan", IdJurusan = jurAkl.IdJurusan, Status = "Aktif" },

                // TE
                new Mapel { KodeMapel = "TE-01", NamaMapel = "Dasar-dasar Teknik Elektronika", Kelompok = "Kejuruan", IdJurusan = jurTe.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TE-02", NamaMapel = "Teknik Elektronika Analog", Kelompok = "Kejuruan", IdJurusan = jurTe.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TE-03", NamaMapel = "Teknik Elektronika Digital", Kelompok = "Kejuruan", IdJurusan = jurTe.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TE-04", NamaMapel = "Mikrokontroler & Interface", Kelompok = "Kejuruan", IdJurusan = jurTe.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TE-05", NamaMapel = "Sistem Kontrol Elektropneumatik", Kelompok = "Kejuruan", IdJurusan = jurTe.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TE-06", NamaMapel = "Perawatan & Perbaikan Peralatan Elektronika", Kelompok = "Kejuruan", IdJurusan = jurTe.IdJurusan, Status = "Aktif" },

                // TBSM
                new Mapel { KodeMapel = "TB-01", NamaMapel = "Dasar-dasar Otomotif & TBSM", Kelompok = "Kejuruan", IdJurusan = jurTbsm.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TB-02", NamaMapel = "Pemeliharaan Mesin Sepeda Motor", Kelompok = "Kejuruan", IdJurusan = jurTbsm.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TB-03", NamaMapel = "Pemeliharaan Kelistrikan Sepeda Motor", Kelompok = "Kejuruan", IdJurusan = jurTbsm.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TB-04", NamaMapel = "Pemeliharaan Sasis & Pemindah Tenaga", Kelompok = "Kejuruan", IdJurusan = jurTbsm.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TB-05", NamaMapel = "Sistem Electronic Fuel Injection (EFI)", Kelompok = "Kejuruan", IdJurusan = jurTbsm.IdJurusan, Status = "Aktif" },
                new Mapel { KodeMapel = "TB-06", NamaMapel = "Pengelolaan Bengkel Sepeda Motor", Kelompok = "Kejuruan", IdJurusan = jurTbsm.IdJurusan, Status = "Aktif" }
            };

            context.Mapel.AddRange(mapelList);
            context.SaveChanges();

            // 8. BOBOT NILAI (Each Mapel gets Harian=30, UTS=30, UAS=40)
            foreach (var m in mapelList)
            {
                context.BobotNilai.AddRange(
                    new BobotNilai { IdMapel = m.IdMapel, IdKomponen = kompHarian.IdKomponen, Persentase = 30m },
                    new BobotNilai { IdMapel = m.IdMapel, IdKomponen = kompUTS.IdKomponen, Persentase = 30m },
                    new BobotNilai { IdMapel = m.IdMapel, IdKomponen = kompUAS.IdKomponen, Persentase = 40m }
                );
            }
            context.SaveChanges();

            // 9. PENUGASAN MENGAJAR
            // Assign Guru to their respective Mapel and Classes
            var mapelDict = mapelList.ToDictionary(m => m.KodeMapel, m => m);

            // 9.1 Wali Kelas Penugasan Kejuruan
            // Andi Setiawan -> TKJ-01 (X TKJ)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[0].IdGuru, IdMapel = mapelDict["TKJ-01"].IdMapel, IdKelas = listKelas[0].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Budi Santoso -> TKJ-02 & TKJ-03 (XI TKJ)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[1].IdGuru, IdMapel = mapelDict["TKJ-02"].IdMapel, IdKelas = listKelas[1].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[1].IdGuru, IdMapel = mapelDict["TKJ-03"].IdMapel, IdKelas = listKelas[1].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Citra Lestari -> TKJ-05 & TKJ-06 (XII TKJ)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[2].IdGuru, IdMapel = mapelDict["TKJ-05"].IdMapel, IdKelas = listKelas[2].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[2].IdGuru, IdMapel = mapelDict["TKJ-06"].IdMapel, IdKelas = listKelas[2].IdKelas, IdPeriode = periodeAktif.IdPeriode });

            // Eka Puspitasari -> MPL-01 (X MPLB)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[3].IdGuru, IdMapel = mapelDict["MPL-01"].IdMapel, IdKelas = listKelas[3].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Fitriani Rahayu -> MPL-02 & MPL-03 (XI MPLB)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[4].IdGuru, IdMapel = mapelDict["MPL-02"].IdMapel, IdKelas = listKelas[4].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[4].IdGuru, IdMapel = mapelDict["MPL-03"].IdMapel, IdKelas = listKelas[4].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Gita Maharani -> MPL-05 & MPL-06 (XII MPLB)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[5].IdGuru, IdMapel = mapelDict["MPL-05"].IdMapel, IdKelas = listKelas[5].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[5].IdGuru, IdMapel = mapelDict["MPL-06"].IdMapel, IdKelas = listKelas[5].IdKelas, IdPeriode = periodeAktif.IdPeriode });

            // Indah Permatasari -> AKL-01 (X AKL)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[6].IdGuru, IdMapel = mapelDict["AKL-01"].IdMapel, IdKelas = listKelas[6].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Joko Susilo -> AKL-02 & AKL-04 (XI AKL)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[7].IdGuru, IdMapel = mapelDict["AKL-02"].IdMapel, IdKelas = listKelas[7].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[7].IdGuru, IdMapel = mapelDict["AKL-04"].IdMapel, IdKelas = listKelas[7].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Kartika Dewi -> AKL-05 & AKL-06 (XII AKL)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[8].IdGuru, IdMapel = mapelDict["AKL-05"].IdMapel, IdKelas = listKelas[8].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[8].IdGuru, IdMapel = mapelDict["AKL-06"].IdMapel, IdKelas = listKelas[8].IdKelas, IdPeriode = periodeAktif.IdPeriode });

            // Mulyadi -> TE-01 (X TE)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[9].IdGuru, IdMapel = mapelDict["TE-01"].IdMapel, IdKelas = listKelas[9].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Nugroho Adi -> TE-02 & TE-03 (XI TE)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[10].IdGuru, IdMapel = mapelDict["TE-02"].IdMapel, IdKelas = listKelas[10].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[10].IdGuru, IdMapel = mapelDict["TE-03"].IdMapel, IdKelas = listKelas[10].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Oki Firmansyah -> TE-05 & TE-06 (XII TE)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[11].IdGuru, IdMapel = mapelDict["TE-05"].IdMapel, IdKelas = listKelas[11].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[11].IdGuru, IdMapel = mapelDict["TE-06"].IdMapel, IdKelas = listKelas[11].IdKelas, IdPeriode = periodeAktif.IdPeriode });

            // Rizal Anwar -> TB-01 (X TBSM)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[12].IdGuru, IdMapel = mapelDict["TB-01"].IdMapel, IdKelas = listKelas[12].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Slamet Riyadi -> TB-02 & TB-03 (XI TBSM)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[13].IdGuru, IdMapel = mapelDict["TB-02"].IdMapel, IdKelas = listKelas[13].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[13].IdGuru, IdMapel = mapelDict["TB-03"].IdMapel, IdKelas = listKelas[13].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Taufik Hidayat -> TB-05 & TB-06 (XII TBSM)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[14].IdGuru, IdMapel = mapelDict["TB-05"].IdMapel, IdKelas = listKelas[14].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruWali[14].IdGuru, IdMapel = mapelDict["TB-06"].IdMapel, IdKelas = listKelas[14].IdKelas, IdPeriode = periodeAktif.IdPeriode });

            // 9.2 Guru Kejuruan Bukan Wali Kelas
            // Dimas Ramadhan -> TKJ-04 (XI TKJ)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[0].IdGuru, IdMapel = mapelDict["TKJ-04"].IdMapel, IdKelas = listKelas[1].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Hendra Gunawan -> MPL-04 (XI MPLB)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[1].IdGuru, IdMapel = mapelDict["MPL-04"].IdMapel, IdKelas = listKelas[4].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Lukman Hakim -> AKL-03 (XI AKL & XII AKL)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[2].IdGuru, IdMapel = mapelDict["AKL-03"].IdMapel, IdKelas = listKelas[7].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[2].IdGuru, IdMapel = mapelDict["AKL-03"].IdMapel, IdKelas = listKelas[8].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Putra Wijaya -> TE-04 (XI TE & XII TE)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[3].IdGuru, IdMapel = mapelDict["TE-04"].IdMapel, IdKelas = listKelas[10].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[3].IdGuru, IdMapel = mapelDict["TE-04"].IdMapel, IdKelas = listKelas[11].IdKelas, IdPeriode = periodeAktif.IdPeriode });
            // Usman Maulana -> TB-04 (XI TBSM)
            context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruKejuruan[4].IdGuru, IdMapel = mapelDict["TB-04"].IdMapel, IdKelas = listKelas[13].IdKelas, IdPeriode = periodeAktif.IdPeriode });

            // 9.3 Guru Umum Penugasan
            // Ahmad Fauzi -> UM-01 (Semua Kelas X, XI, XII)
            foreach (var k in listKelas)
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[0].IdGuru, IdMapel = mapelDict["UM-01"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Wulan Sari -> UM-02 (Semua Kelas X, XI, XII)
            foreach (var k in listKelas)
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[1].IdGuru, IdMapel = mapelDict["UM-02"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Siti Aminah -> UM-03 (Semua Kelas X dan XI)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X" || x.Tingkat == "XI"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[2].IdGuru, IdMapel = mapelDict["UM-03"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Rudi Hartono -> UM-03 (Semua Kelas XII)
            foreach (var k in listKelas.Where(x => x.Tingkat == "XII"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[3].IdGuru, IdMapel = mapelDict["UM-03"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Nanda Pratiwi -> UM-07 (Semua Kelas X dan XI)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X" || x.Tingkat == "XI"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[4].IdGuru, IdMapel = mapelDict["UM-07"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Yoga Pratama -> UM-07 (Semua Kelas XII)
            foreach (var k in listKelas.Where(x => x.Tingkat == "XII"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[5].IdGuru, IdMapel = mapelDict["UM-07"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Rina Marlina -> UM-08 (Semua Kelas X)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[6].IdGuru, IdMapel = mapelDict["UM-08"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Dedi Kurniawan -> UM-08 (Semua Kelas XI dan XII)
            foreach (var k in listKelas.Where(x => x.Tingkat == "XI" || x.Tingkat == "XII"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[7].IdGuru, IdMapel = mapelDict["UM-08"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Bayu Setiawan -> UM-04 (Semua Kelas X, XI, XII)
            foreach (var k in listKelas)
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[8].IdGuru, IdMapel = mapelDict["UM-04"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Lestari Wulandari -> UM-05 (Semua Kelas X dan XI)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X" || x.Tingkat == "XI"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[9].IdGuru, IdMapel = mapelDict["UM-05"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Maya Anggraini -> UM-06 (Semua Kelas X)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[10].IdGuru, IdMapel = mapelDict["UM-06"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Fitri Handayani -> UM-09 (Semua Kelas X)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[11].IdGuru, IdMapel = mapelDict["UM-09"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Eko Prasetyo -> UM-10 (Semua Kelas X)
            foreach (var k in listKelas.Where(x => x.Tingkat == "X"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[12].IdGuru, IdMapel = mapelDict["UM-10"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            // Hana Permata -> UM-11 (Semua Kelas XI dan XII)
            foreach (var k in listKelas.Where(x => x.Tingkat == "XI" || x.Tingkat == "XII"))
            {
                context.PenugasanMengajar.Add(new PenugasanMengajar { IdGuru = listGuruUmum[13].IdGuru, IdMapel = mapelDict["UM-11"].IdMapel, IdKelas = k.IdKelas, IdPeriode = periodeAktif.IdPeriode });
            }
            context.SaveChanges();

            // 10. SISWA SAMPLE (3 siswa per rombel = 45 siswa terdaftar)
            var siswaList = new List<Siswa>();
            int studentIndex = 1;
            var sampleNames = new[]
            {
                ("Aditya Pratama", "L"), ("Bella Safira", "P"), ("Candra Wijaya", "L"),
                ("Dina Mariana", "P"), ("Fajar Hidayat", "L"), ("Gita Nurul", "P"),
                ("Hendra Saputra", "L"), ("Intan Permata", "P"), ("Kevin Sanjaya", "L"),
                ("Laila Fitria", "P"), ("Muhammad Rizki", "L"), ("Nurul Aini", "P"),
                ("Oscar Ramadhan", "L"), ("Putri Ayu", "P"), ("Rian Anggara", "L")
            };

            foreach (var k in listKelas)
            {
                for (int i = 0; i < 3; i++)
                {
                    var nameTuple = sampleNames[(studentIndex - 1) % sampleNames.Length];
                    string nis = (2025000 + studentIndex).ToString();
                    string nisn = "008" + (1000000 + studentIndex).ToString();

                    var s = new Siswa
                    {
                        Nis = nis,
                        Nisn = nisn,
                        NamaSiswa = $"{nameTuple.Item1} ({k.NamaKelas})",
                        JenisKelamin = nameTuple.Item2,
                        TanggalLahir = new DateTime(2008, 1 + (studentIndex % 12), 1 + (studentIndex % 28)),
                        Alamat = $"Jl. Pendidikan No. {studentIndex}, Jakarta",
                        IdKelas = k.IdKelas,
                        Status = "Aktif"
                    };
                    siswaList.Add(s);
                    studentIndex++;
                }
            }
            context.Siswa.AddRange(siswaList);
            context.SaveChanges();

            // 11. INITIAL GRADES & REKAP (Sample grades for X TKJ, XI TKJ, XII TKJ to make dashboards instantly live)
            var sampleClass = listKelas[0]; // X TKJ
            var studentsInClass = siswaList.Where(s => s.IdKelas == sampleClass.IdKelas).ToList();
            var subjectsInClass = new[] { mapelDict["UM-01"], mapelDict["UM-02"], mapelDict["UM-03"], mapelDict["TKJ-01"] };

            foreach (var s in studentsInClass)
            {
                foreach (var m in subjectsInClass)
                {
                    decimal t1 = 80m + (s.IdSiswa * 3 % 16);
                    decimal t2 = 84m + (s.IdSiswa * 2 % 14);
                    decimal harianAvg = Math.Round((t1 + t2) / 2m, 2);
                    decimal uts = 78m + (s.IdSiswa * 2 % 20);
                    decimal uas = 82m + (s.IdSiswa * 4 % 16);

                    // Add Nilai Harian: Tugas 1
                    context.Nilai.Add(new Nilai
                    {
                        IdSiswa = s.IdSiswa,
                        IdMapel = m.IdMapel,
                        IdPeriode = periodeAktif.IdPeriode,
                        IdKomponen = kompHarian.IdKomponen,
                        NamaTugas = "Tugas 1",
                        NilaiAngka = t1,
                        IdGuruPenginput = listGuruWali[0].IdGuru
                    });

                    // Add Nilai Harian: Tugas 2
                    context.Nilai.Add(new Nilai
                    {
                        IdSiswa = s.IdSiswa,
                        IdMapel = m.IdMapel,
                        IdPeriode = periodeAktif.IdPeriode,
                        IdKomponen = kompHarian.IdKomponen,
                        NamaTugas = "Tugas 2",
                        NilaiAngka = t2,
                        IdGuruPenginput = listGuruWali[0].IdGuru
                    });

                    // Add Nilai UTS
                    context.Nilai.Add(new Nilai
                    {
                        IdSiswa = s.IdSiswa,
                        IdMapel = m.IdMapel,
                        IdPeriode = periodeAktif.IdPeriode,
                        IdKomponen = kompUTS.IdKomponen,
                        NamaTugas = "UTS",
                        NilaiAngka = uts,
                        IdGuruPenginput = listGuruWali[0].IdGuru
                    });

                    // Add Nilai UAS
                    context.Nilai.Add(new Nilai
                    {
                        IdSiswa = s.IdSiswa,
                        IdMapel = m.IdMapel,
                        IdPeriode = periodeAktif.IdPeriode,
                        IdKomponen = kompUAS.IdKomponen,
                        NamaTugas = "UAS",
                        NilaiAngka = uas,
                        IdGuruPenginput = listGuruWali[0].IdGuru
                    });

                    // Calculate Nilai Akhir: (HarianAvg * 0.3) + (UTS * 0.3) + (UAS * 0.4)
                    decimal akhir = Math.Round((harianAvg * 0.30m) + (uts * 0.30m) + (uas * 0.40m), 2);
                    string huruf = akhir >= 85 ? "A" : akhir >= 75 ? "B" : akhir >= 60 ? "C" : "D";

                    context.RekapNilai.Add(new RekapNilai
                    {
                        IdSiswa = s.IdSiswa,
                        IdMapel = m.IdMapel,
                        IdPeriode = periodeAktif.IdPeriode,
                        NilaiAkhir = akhir,
                        NilaiHuruf = huruf,
                        TanggalPerhitungan = DateTime.UtcNow,
                        Status = "Final"
                    });
                }
            }
            context.SaveChanges();
        }

        public static void EnsureProductionAdmin(AppDbContext context, IConfiguration configuration)
        {
            if (context.Pengguna.Any())
            {
                return;
            }

            var username = configuration["BootstrapAdmin:Username"]?.Trim();
            var password = configuration["BootstrapAdmin:Password"];
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Database produksi belum memiliki pengguna. Atur BootstrapAdmin__Username dan BootstrapAdmin__Password sekali untuk membuat akun admin awal.");
            }

            var roleNames = new[] { "Admin", "Guru", "WaliKelas" };
            var existingRoles = context.Roles.ToList();
            foreach (var roleName in roleNames.Where(name => existingRoles.All(role => role.NamaRole != name)))
            {
                context.Roles.Add(new Role { NamaRole = roleName });
            }
            context.SaveChanges();

            var adminRole = context.Roles.Single(role => role.NamaRole == "Admin");
            context.Pengguna.Add(new Pengguna
            {
                IdRole = adminRole.IdRole,
                Nama = configuration["BootstrapAdmin:Name"]?.Trim() ?? "Administrator",
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Status = "Aktif"
            });
            context.SaveChanges();
        }
    }
}
