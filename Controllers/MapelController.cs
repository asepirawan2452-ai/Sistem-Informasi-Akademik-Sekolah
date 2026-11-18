using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Controllers;

public class MapelController : Controller
{
    private readonly AppDbContext _context;

    public MapelController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.Mapel
            .AsNoTracking()
            .Include(m => m.Jurusan)
            .OrderBy(m => m.KodeMapel)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mapel = await _context.Mapel
            .AsNoTracking()
            .Include(m => m.Jurusan)
            .FirstOrDefaultAsync(m => m.Id == id);
        return mapel == null ? NotFound() : View(mapel);
    }

    public async Task<IActionResult> Create()
    {
        await LoadJurusanAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("KodeMapel,NamaMapel,Kelompok,IdJurusan,Deskripsi,Status")] Mapel mapel)
    {
        if (ModelState.IsValid)
        {
            _context.Add(mapel);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data mata pelajaran berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        await LoadJurusanAsync(mapel.IdJurusan);
        return View(mapel);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mapel = await _context.Mapel.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (mapel == null)
        {
            return NotFound();
        }

        await LoadJurusanAsync(mapel.IdJurusan);
        return View(mapel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("KodeMapel,NamaMapel,Kelompok,IdJurusan,Deskripsi,Status")] Mapel mapel)
    {
        if (!MapelExists(id))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            mapel.Id = id;
            try
            {
                _context.Update(mapel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MapelExists(mapel.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Success"] = "Data mata pelajaran berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        await LoadJurusanAsync(mapel.IdJurusan);
        return View(mapel);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mapel = await _context.Mapel
            .AsNoTracking()
            .Include(m => m.Jurusan)
            .FirstOrDefaultAsync(m => m.Id == id);
        return mapel == null ? NotFound() : View(mapel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mapel = await _context.Mapel.FindAsync(id);
        if (mapel == null)
        {
            return NotFound();
        }

        mapel.Status = "Nonaktif";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Data mata pelajaran dinonaktifkan.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadJurusanAsync(int? selected = null)
    {
        var items = new List<SelectListItem>
        {
            new SelectListItem { Value = string.Empty, Text = "-- Umum / Semua Jurusan --" }
        };
        items.AddRange(await _context.Jurusan
            .AsNoTracking()
            .OrderBy(j => j.Nama)
            .Select(j => new SelectListItem { Value = j.Id.ToString(), Text = j.Nama })
            .ToListAsync());
        ViewBag.IdJurusan = new SelectList(items, "Value", "Text", selected?.ToString() ?? string.Empty);
    }

    private bool MapelExists(int id)
    {
        return _context.Mapel.Any(e => e.Id == id);
    }
}
