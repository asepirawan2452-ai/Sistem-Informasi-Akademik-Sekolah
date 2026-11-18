using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Controllers;

public class GuruController : Controller
{
    private readonly AppDbContext _context;

    public GuruController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.Guru
            .AsNoTracking()
            .Include(g => g.Pengguna)
            .OrderBy(g => g.Nama)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var guru = await _context.Guru
            .AsNoTracking()
            .Include(g => g.Pengguna)
            .FirstOrDefaultAsync(m => m.Id == id);
        return guru == null ? NotFound() : View(guru);
    }

    public async Task<IActionResult> Create()
    {
        await LoadPenggunaAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPengguna,NipNrg,Nama,Status")] Guru guru)
    {
        if (ModelState.IsValid)
        {
            _context.Add(guru);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data guru berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        await LoadPenggunaAsync(guru.IdPengguna);
        return View(guru);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var guru = await _context.Guru.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (guru == null)
        {
            return NotFound();
        }

        await LoadPenggunaAsync(guru.IdPengguna, guru.Id);
        return View(guru);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdPengguna,NipNrg,Nama,Status")] Guru guru)
    {
        if (!GuruExists(id))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            guru.Id = id;
            try
            {
                _context.Update(guru);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuruExists(guru.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Success"] = "Data guru berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        await LoadPenggunaAsync(guru.IdPengguna, id);
        return View(guru);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var guru = await _context.Guru
            .AsNoTracking()
            .Include(g => g.Pengguna)
            .FirstOrDefaultAsync(m => m.Id == id);
        return guru == null ? NotFound() : View(guru);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var guru = await _context.Guru.FindAsync(id);
        if (guru == null)
        {
            return NotFound();
        }

        guru.Status = "Nonaktif";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Data guru dinonaktifkan.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadPenggunaAsync(int? selected = null, int? currentGuruId = null)
    {
        var query = _context.Pengguna
            .AsNoTracking()
            .Where(p => p.Status == "Aktif" && p.Role.NamaRole == "GURU");

        if (currentGuruId.HasValue)
        {
            query = query.Where(p => p.Guru == null || p.Guru.Id == currentGuruId.Value);
        }

        var items = await query
            .OrderBy(p => p.Nama)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nama + " (" + p.Username + ")" })
            .ToListAsync();
        ViewBag.IdPengguna = new SelectList(items, "Value", "Text", selected?.ToString());
    }

    private bool GuruExists(int id)
    {
        return _context.Guru.Any(e => e.Id == id);
    }
}
