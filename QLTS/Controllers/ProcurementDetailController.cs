using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;

public class ProcurementDetailController : Controller
{
    private readonly AppDbContext _context;

    public ProcurementDetailController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ProcurementDetail
    public async Task<IActionResult> Index()
    {
        var details = await _context.ProcurementDetails
            .Include(pd => pd.Procurement)
            .Include(pd => pd.Asset)
            .ToListAsync();

        return View(details);
    }

    // GET: ProcurementDetail/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procurementDetail = await _context.ProcurementDetails
            .Include(pd => pd.Procurement)
            .Include(pd => pd.Asset)
            .FirstOrDefaultAsync(m => m.ProcurementDetailId == id);

        if (procurementDetail == null)
        {
            return NotFound();
        }

        return View(procurementDetail);
    }

    // GET: ProcurementDetail/Create
    public async Task<IActionResult> Create()
    {
        await LoadFormData();
        return View();
    }

    // POST: ProcurementDetail/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ProcurementDetailId,ProcurementId,AssetId,UnitCost")]
        ProcurementDetail procurementDetail)
    {
        // Kiểm tra không cho cùng một Asset xuất hiện 2 lần trong cùng một Procurement
        if (await _context.ProcurementDetails.AnyAsync(
            pd => pd.ProcurementId == procurementDetail.ProcurementId
            && pd.AssetId == procurementDetail.AssetId))
        {
            ModelState.AddModelError("AssetId", "Tài sản này đã tồn tại trong phiếu nhập đã chọn.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(procurementDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await LoadFormData(procurementDetail.ProcurementId, procurementDetail.AssetId);
        return View(procurementDetail);
    }

    // GET: ProcurementDetail/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procurementDetail = await _context.ProcurementDetails.FindAsync(id);

        if (procurementDetail == null)
        {
            return NotFound();
        }

        await LoadFormData(procurementDetail.ProcurementId, procurementDetail.AssetId);
        return View(procurementDetail);
    }

    // POST: ProcurementDetail/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("ProcurementDetailId,ProcurementId,AssetId,UnitCost")]
        ProcurementDetail procurementDetail)
    {
        if (id != procurementDetail.ProcurementDetailId)
        {
            return NotFound();
        }

        // Kiểm tra không cho trùng Asset trong cùng Procurement (loại trừ chính bản ghi đang sửa)
        if (await _context.ProcurementDetails.AnyAsync(
            pd => pd.ProcurementId == procurementDetail.ProcurementId
            && pd.AssetId == procurementDetail.AssetId
            && pd.ProcurementDetailId != procurementDetail.ProcurementDetailId))
        {
            ModelState.AddModelError("AssetId", "Tài sản này đã tồn tại trong phiếu nhập đã chọn.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(procurementDetail);
                await _context.SaveChangesAsync();

                // Cập nhật lại OriginalCost của tài sản
                var asset = await _context.Assets.FindAsync(procurementDetail.AssetId);
                if (asset != null)
                {
                    asset.OriginalCost = procurementDetail.UnitCost;
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcurementDetailExists(procurementDetail.ProcurementDetailId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await LoadFormData(procurementDetail.ProcurementId, procurementDetail.AssetId);
        return View(procurementDetail);
    }

    // GET: ProcurementDetail/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procurementDetail = await _context.ProcurementDetails
            .Include(pd => pd.Procurement)
            .Include(pd => pd.Asset)
            .FirstOrDefaultAsync(m => m.ProcurementDetailId == id);

        if (procurementDetail == null)
        {
            return NotFound();
        }

        return View(procurementDetail);
    }

    // POST: ProcurementDetail/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var procurementDetail = await _context.ProcurementDetails.FindAsync(id);

        if (procurementDetail != null)
        {
            _context.ProcurementDetails.Remove(procurementDetail);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Nạp dữ liệu dropdown cho Phiếu nhập và Tài sản
    private async Task LoadFormData(int? procurementId = null, int? assetId = null)
    {
        ViewBag.ProcurementId = new SelectList(
            await _context.Procurements
                .Select(p => new
                {
                    p.ProcurementId,
                    DisplayText = p.ProcurementCode + " - " + p.RequesterName
                })
                .ToListAsync(),
            "ProcurementId",
            "DisplayText",
            procurementId
        );

        ViewBag.AssetId = new SelectList(
            await _context.Assets
                .Select(a => new
                {
                    a.AssetId,
                    DisplayText = a.AssetCode + " - " + a.AssetName
                })
                .ToListAsync(),
            "AssetId",
            "DisplayText",
            assetId
        );
    }

    private bool ProcurementDetailExists(int id)
    {
        return _context.ProcurementDetails.Any(e => e.ProcurementDetailId == id);
    }
}
