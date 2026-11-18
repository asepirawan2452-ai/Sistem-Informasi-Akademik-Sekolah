using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Controllers;

public class SiswaController : Controller
{
    private readonly AppDbContext _context;

    public SiswaController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.Siswa
            .AsNoTracking()
            .Include(s => s.Kelas)
            .OrderBy(s => s.NamaSiswa)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var siswa = await _context.Siswa
            .AsNoTracking()
            .Include(s => s.Kelas)
            .FirstOrDefaultAsync(m => m.Id == id);
        return siswa == null ? NotFound() : View(siswa);
    }

    public async Task<IActionResult> Create()
    {
        await LoadKelasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nis,Nisn,NamaSiswa,JenisKelamin,TanggalLahir,Alamat,IdKelas,Status")] Siswa siswa)
    {
        if (ModelState.IsValid)
        {
            _context.Add(siswa);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data siswa berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        await LoadKelasAsync(siswa.IdKelas);
        return View(siswa);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var siswa = await _context.Siswa.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (siswa == null)
        {
            return NotFound();
        }

        await LoadKelasAsync(siswa.IdKelas);
        return View(siswa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Nis,Nisn,NamaSiswa,JenisKelamin,TanggalLahir,Alamat,IdKelas,Status")] Siswa siswa)
    {
        if (SiswaExists(id) == false)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            siswa.Id = id;
            try
            {
                _context.Update(siswa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SiswaExists(siswa.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Success"] = "Data siswa berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        await LoadKelasAsync(siswa.IdKelas);
        return View(siswa);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var siswa = await _context.Siswa
            .AsNoTracking()
            .Include(s => s.Kelas)
            .FirstOrDefaultAsync(m => m.Id == id);
        return siswa == null ? NotFound() : View(siswa);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var siswa = await _context.Siswa.FindAsync(id);
        if (siswa == null)
        {
            return NotFound();
        }

        siswa.Status = "Nonaktif";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Data siswa dinonaktifkan.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadKelasAsync(int? selected = null)
    {
        var items = await _context.Kelas
            .AsNoTracking()
            .Where(k => k.Status == "Aktif")
            .OrderBy(k => k.NamaKelas)
            .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.NamaKelas })
            .ToListAsync();
        ViewBag.IdKelas = new SelectList(items, "Value", "Text", selected?.ToString());
    }

    private bool SiswaExists(int id)
    {
        return _context.Siswa.Any(e => e.Id == id);
    }
}
