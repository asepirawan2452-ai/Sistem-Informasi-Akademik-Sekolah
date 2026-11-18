using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Controllers;

public class JurusanController : Controller
{
    private readonly AppDbContext _context;

    public JurusanController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.Jurusan
            .AsNoTracking()
            .OrderBy(j => j.Kode)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var jurusan = await _context.Jurusan.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return jurusan == null ? NotFound() : View(jurusan);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Kode,Nama")] Jurusan jurusan)
    {
        if (ModelState.IsValid)
        {
            _context.Add(jurusan);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Jurusan berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        return View(jurusan);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var jurusan = await _context.Jurusan.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return jurusan == null ? NotFound() : View(jurusan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Kode,Nama")] Jurusan jurusan)
    {
        if (!JurusanExists(id))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            jurusan.Id = id;
            try
            {
                _context.Update(jurusan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JurusanExists(jurusan.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Success"] = "Jurusan berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        return View(jurusan);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var jurusan = await _context.Jurusan
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        return jurusan == null ? NotFound() : View(jurusan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var jurusan = await _context.Jurusan.FindAsync(id);
        if (jurusan == null)
        {
            return NotFound();
        }

        var dipakai = await _context.Kelas.AnyAsync(k => k.IdJurusan == id)
            || await _context.Mapel.AnyAsync(m => m.IdJurusan == id);
        if (dipakai)
        {
            TempData["Error"] = "Jurusan masih digunakan oleh data kelas atau mata pelajaran.";
            return RedirectToAction(nameof(Index));
        }

        _context.Jurusan.Remove(jurusan);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Jurusan berhasil dihapus.";
        return RedirectToAction(nameof(Index));
    }

    private bool JurusanExists(int id)
    {
        return _context.Jurusan.Any(e => e.Id == id);
    }
}
