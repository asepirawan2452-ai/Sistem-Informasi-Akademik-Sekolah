using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemNilaiAkademik.Data;
using SistemNilaiAkademik.Models;

namespace SistemNilaiAkademik.Controllers;

public class PeriodeController : Controller
{
    private readonly AppDbContext _context;

    public PeriodeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.PeriodeAkademik
            .AsNoTracking()
            .OrderByDescending(p => p.TanggalMulai)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var periode = await _context.PeriodeAkademik.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return periode == null ? NotFound() : View(periode);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NamaPeriode,TahunAjaran,Semester,TanggalMulai,TanggalSelesai,Status")] PeriodeAkademik periode)
    {
        if (ModelState.IsValid)
        {
            _context.Add(periode);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Periode akademik berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        return View(periode);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var periode = await _context.PeriodeAkademik.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return periode == null ? NotFound() : View(periode);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("NamaPeriode,TahunAjaran,Semester,TanggalMulai,TanggalSelesai,Status")] PeriodeAkademik periode)
    {
        if (!PeriodeExists(id))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            periode.Id = id;
            try
            {
                _context.Update(periode);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeriodeExists(periode.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Success"] = "Periode akademik berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        return View(periode);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var periode = await _context.PeriodeAkademik.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return periode == null ? NotFound() : View(periode);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var periode = await _context.PeriodeAkademik.FindAsync(id);
        if (periode == null)
        {
            return NotFound();
        }

        periode.Status = "Nonaktif";
        await _context.SaveChangesAsync();
        TempData["Success"] = "Periode akademik dinonaktifkan.";
        return RedirectToAction(nameof(Index));
    }

    private bool PeriodeExists(int id)
    {
        return _context.PeriodeAkademik.Any(e => e.Id == id);
    }
}
