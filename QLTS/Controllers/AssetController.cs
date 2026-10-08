using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;

public class AssetController : Controller
{
    private readonly AppDbContext _context;

    public AssetController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Asset
    public async Task<IActionResult> Index()
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Department)
            .Include(a => a.Employee)
            .ToListAsync();

        return View(assets);
    }

    // GET: Asset/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asset = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Department)
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.AssetId == id);

        if (asset == null)
        {
            return NotFound();
        }

        return View(asset);
    }

    // GET: Asset/Create
    public async Task<IActionResult> Create()
    {
        await LoadFormData();

        return View();
    }

    // POST: Asset/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("AssetId,AssetCode,AssetName,CategoryId,DepartmentId,EmployeeId,SerialNumber,OriginalCost,PurchaseDate,Location,Status,Description,CreatedAt")]
        Asset asset)
    {
        // Kiểm tra mã tài sản trùng
        if (await _context.Assets.AnyAsync(
            a => a.AssetCode == asset.AssetCode))
        {
            ModelState.AddModelError(
                "AssetCode",
                "Mã tài sản đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(asset);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        await LoadFormData(asset);

        return View(asset);
    }

    // GET: Asset/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asset = await _context.Assets.FindAsync(id);

        if (asset == null)
        {
            return NotFound();
        }

        await LoadFormData(asset);

        return View(asset);
    }

    // POST: Asset/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("AssetId,AssetCode,AssetName,CategoryId,DepartmentId,EmployeeId,SerialNumber,OriginalCost,PurchaseDate,Location,Status,Description,CreatedAt")]
        Asset asset)
    {
        if (id != asset.AssetId)
        {
            return NotFound();
        }

        // Kiểm tra mã tài sản trùng
        if (await _context.Assets.AnyAsync(
            a => a.AssetCode == asset.AssetCode
            && a.AssetId != asset.AssetId))
        {
            ModelState.AddModelError(
                "AssetCode",
                "Mã tài sản đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(asset);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(asset.AssetId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await LoadFormData(asset);

        return View(asset);
    }

    // GET: Asset/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asset = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.Department)
            .Include(a => a.Employee)
            .Include(a => a.AssetAssignments)
            .Include(a => a.MaintenanceRequests)
            .Include(a => a.AssetDisposals)
            .Include(a => a.ProcurementDetails)
            .FirstOrDefaultAsync(a => a.AssetId == id);

        if (asset == null)
        {
            return NotFound();
        }

        bool hasHistory = asset.AssetAssignments.Any()
                       || asset.MaintenanceRequests.Any()
                       || asset.AssetDisposals.Any()
                       || asset.ProcurementDetails.Any();

        ViewBag.HasHistory = hasHistory;
        ViewBag.AssignmentCount = asset.AssetAssignments.Count;
        ViewBag.MaintenanceCount = asset.MaintenanceRequests.Count;
        ViewBag.DisposalCount = asset.AssetDisposals.Count;
        ViewBag.ProcurementCount = asset.ProcurementDetails.Count;

        return View(asset);
    }

    // POST: Asset/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var asset = await _context.Assets
            .Include(a => a.AssetAssignments)
            .Include(a => a.MaintenanceRequests)
            .Include(a => a.AssetDisposals)
            .Include(a => a.ProcurementDetails)
            .FirstOrDefaultAsync(a => a.AssetId == id);

        if (asset == null)
        {
            return NotFound();
        }

        // Kiểm tra lịch sử – không cho xóa nếu đã có dữ liệu liên quan
        bool hasHistory = asset.AssetAssignments.Any()
                       || asset.MaintenanceRequests.Any()
                       || asset.AssetDisposals.Any()
                       || asset.ProcurementDetails.Any();

        if (hasHistory)
        {
            TempData["ErrorMessage"] = "Không thể xóa tài sản này vì đã có lịch sử cấp phát, bảo trì, thanh lý hoặc nhập kho.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Dữ liệu dùng cho form Create/Edit
    private async Task LoadFormData(Asset? asset = null)
    {
        ViewBag.CategoryId = new SelectList(
            await _context.Categories.ToListAsync(),
            "CategoryId",
            "CategoryName",
            asset?.CategoryId
        );

        ViewBag.DepartmentId = new SelectList(
            await _context.Departments.ToListAsync(),
            "DepartmentId",
            "DepartmentName",
            asset?.DepartmentId
        );

        ViewBag.EmployeeId = new SelectList(
            await _context.Employees.ToListAsync(),
            "EmployeeId",
            "FullName",
            asset?.EmployeeId
        );
    }

    private bool AssetExists(int? id)
    {
        return _context.Assets.Any(e => e.AssetId == id);
    }
}