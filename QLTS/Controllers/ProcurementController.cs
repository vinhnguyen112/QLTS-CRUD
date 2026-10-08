using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;

public class ProcurementController : Controller
{
    private readonly AppDbContext _context;

    public ProcurementController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Procurement
    public async Task<IActionResult> Index()
    {
        var procurements = await _context.Procurements
            .Include(p => p.Department)
            .ToListAsync();

        return View(procurements);
    }

    // GET: Procurement/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procurement = await _context.Procurements
            .Include(p => p.Department)
            .FirstOrDefaultAsync(m => m.ProcurementId == id);

        if (procurement == null)
        {
            return NotFound();
        }

        return View(procurement);
    }

    // GET: Procurement/Create
    public async Task<IActionResult> Create()
    {
        await LoadDepartments();
        return View();
    }

    // POST: Procurement/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ProcurementId,ProcurementCode,RequesterName,DepartmentId,RequestDate,Reason,Status,PlannedCost,Description,CreatedAt")]
        Procurement procurement)
    {
        // Kiểm tra trùng mã phiếu nhập
        if (await _context.Procurements.AnyAsync(p => p.ProcurementCode == procurement.ProcurementCode))
        {
            ModelState.AddModelError("ProcurementCode", "Mã phiếu nhập đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(procurement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await LoadDepartments(procurement.DepartmentId);
        return View(procurement);
    }

    // GET: Procurement/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procurement = await _context.Procurements.FindAsync(id);

        if (procurement == null)
        {
            return NotFound();
        }

        await LoadDepartments(procurement.DepartmentId);
        return View(procurement);
    }

    // POST: Procurement/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("ProcurementId,ProcurementCode,RequesterName,DepartmentId,RequestDate,Reason,Status,PlannedCost,Description,CreatedAt")]
        Procurement procurement)
    {
        if (id != procurement.ProcurementId)
        {
            return NotFound();
        }

        // Kiểm tra trùng mã phiếu nhập với bản ghi khác
        if (await _context.Procurements.AnyAsync(
            p => p.ProcurementCode == procurement.ProcurementCode
            && p.ProcurementId != procurement.ProcurementId))
        {
            ModelState.AddModelError("ProcurementCode", "Mã phiếu nhập đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(procurement);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcurementExists(procurement.ProcurementId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await LoadDepartments(procurement.DepartmentId);
        return View(procurement);
    }

    // GET: Procurement/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procurement = await _context.Procurements
            .Include(p => p.Department)
            .FirstOrDefaultAsync(m => m.ProcurementId == id);

        if (procurement == null)
        {
            return NotFound();
        }

        return View(procurement);
    }

    // POST: Procurement/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var procurement = await _context.Procurements.FindAsync(id);

        if (procurement != null)
        {
            _context.Procurements.Remove(procurement);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Hàm nạp danh sách phòng ban cho dropdown
    private async Task LoadDepartments(int? selectedId = null)
    {
        ViewBag.DepartmentId = new SelectList(
            await _context.Departments.ToListAsync(),
            "DepartmentId",
            "DepartmentName",
            selectedId
        );
    }

    private bool ProcurementExists(int id)
    {
        return _context.Procurements.Any(e => e.ProcurementId == id);
    }
}
