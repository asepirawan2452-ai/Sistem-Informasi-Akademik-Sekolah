using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Controllers;

public class KelasController : Controller
{
    private readonly AppDbContext _context;

    public KelasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.Kelas
            .AsNoTracking()
            .Include(k => k.Jurusan)
            .Include(k => k.WaliKelas)
            .OrderBy(k => k.Tingkat)
            .ThenBy(k => k.NamaKelas)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var kelas = await _context.Kelas
            .AsNoTracking()
            .Include(k => k.Jurusan)
            .Include(k => k.WaliKelas)
            .FirstOrDefaultAsync(m => m.Id == id);
        return kelas == null ? NotFound() : View(kelas);
    }

    public async Task<IActionResult> Create()
    {
        await LoadJurusanAsync();
        await LoadWaliKelasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NamaKelas,Tingkat,IdJurusan,IdWaliKelas,Status")] Kelas kelas)
    {
        if (ModelState.IsValid)
        {
            _context.Add(kelas);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data kelas berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        await LoadJurusanAsync(kelas.IdJurusan);
        await LoadWaliKelasAsync(kelas.IdWaliKelas);
        return View(kelas);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var kelas = await _context.Kelas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (kelas == null)
        {
            return NotFound();
        }

        await LoadJurusanAsync(kelas.IdJurusan);
        await LoadWaliKelasAsync(kelas.IdWaliKelas, kelas.Id);
        return View(kelas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("NamaKelas,Tingkat,IdJurusan,IdWaliKelas,Status")] Kelas kelas)
    {
        if (!KelasExists(id))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            kelas.Id = id;
            try
            {
                _context.Update(kelas);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KelasExists(kelas.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Success"] = "Data kelas berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        await LoadJurusanAsync(kelas.IdJurusan);
        await LoadWaliKelasAsync(kelas.IdWaliKelas, id);
        return View(kelas);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var kelas = await _context.Kelas
            .AsNoTracking()
            .Include(k => k.Jurusan)
            .Include(k => k.WaliKelas)
            .FirstOrDefaultAsync(m => m.Id == id);
        return kelas == null ? NotFound() : View(kelas);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var kelas = await _context.Kelas.FindAsync(id);
        if (kelas == null)
        {
            return NotFound();
        }

        kelas.Status = "Nonaktif";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Data kelas dinonaktifkan.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadJurusanAsync(int? selected = null)
    {
        var items = await _context.Jurusan
            .AsNoTracking()
            .OrderBy(j => j.Nama)
            .Select(j => new SelectListItem { Value = j.Id.ToString(), Text = j.Nama })
            .ToListAsync();
        ViewBag.IdJurusan = new SelectList(items, "Value", "Text", selected?.ToString());
    }

    private async Task LoadWaliKelasAsync(int? selected = null, int? currentKelasId = null)
    {
        var query = _context.Guru
            .AsNoTracking()
            .Where(g => g.Status == "Aktif" || g.Id == selected);

        if (currentKelasId.HasValue)
        {
            query = query.Where(g => !g.KelasDiwali.Any(k => k.Id != currentKelasId.Value));
        }
        else
        {
            query = query.Where(g => !g.KelasDiwali.Any());
        }

        var items = await query
            .OrderBy(g => g.Nama)
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nama })
            .ToListAsync();
        ViewBag.IdWaliKelas = new SelectList(items, "Value", "Text", selected?.ToString());
    }

    private bool KelasExists(int id)
    {
        return _context.Kelas.Any(e => e.Id == id);
    }
}
