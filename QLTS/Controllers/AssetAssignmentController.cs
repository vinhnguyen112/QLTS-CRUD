using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;

namespace QLTS.Controllers
{
    public class AssetAssignmentController : Controller
    {
        private readonly AppDbContext _context;

        public AssetAssignmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AssetAssignment
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.FromDepartment)
                .Include(a => a.ToDepartment)
                .Include(a => a.FromEmployee)
                .Include(a => a.ToEmployee)
                .Include(a => a.Creator)
                .ToListAsync();

            return View(assignments);
        }

        // GET: AssetAssignment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetAssignment = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.FromDepartment)
                .Include(a => a.ToDepartment)
                .Include(a => a.FromEmployee)
                .Include(a => a.ToEmployee)
                .Include(a => a.Creator)
                .FirstOrDefaultAsync(m => m.AssignmentId == id);

            if (assetAssignment == null)
            {
                return NotFound();
            }

            return View(assetAssignment);
        }

        // GET: AssetAssignment/GetAssetInfo/5
        [HttpGet]
        public async Task<IActionResult> GetAssetInfo(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            return Json(new
            {
                departmentId = asset.DepartmentId,
                employeeId = asset.EmployeeId,
                status = asset.Status
            });
        }

        // GET: AssetAssignment/Create
        public async Task<IActionResult> Create(int? assetId = null)
        {
            AssetAssignment? initial = null;
            if (assetId.HasValue)
            {
                var asset = await _context.Assets.FindAsync(assetId.Value);
                if (asset != null)
                {
                    initial = new AssetAssignment
                    {
                        AssetId = asset.AssetId,
                        FromDepartmentId = asset.DepartmentId,
                        FromEmployeeId = asset.EmployeeId,
                        AssignmentDate = DateTime.Today
                    };
                }
            }

            await LoadFormData(initial);
            return View(initial);
        }

        // POST: AssetAssignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("AssignmentId,AssetId,FromDepartmentId,ToDepartmentId,FromEmployeeId,ToEmployeeId,AssignmentDate,Reason,Status,CreatedBy")]
            AssetAssignment assetAssignment)
        {
            assetAssignment.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(assetAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadFormData(assetAssignment);
            return View(assetAssignment);
        }

        // GET: AssetAssignment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetAssignment = await _context.AssetAssignments.FindAsync(id);
            if (assetAssignment == null)
            {
                return NotFound();
            }

            await LoadFormData(assetAssignment);
            return View(assetAssignment);
        }

        // POST: AssetAssignment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int? id,
            [Bind("AssignmentId,AssetId,FromDepartmentId,ToDepartmentId,FromEmployeeId,ToEmployeeId,AssignmentDate,Reason,Status,CreatedBy,CreatedAt")]
            AssetAssignment assetAssignment)
        {
            if (id != assetAssignment.AssignmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetAssignmentExists(assetAssignment.AssignmentId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadFormData(assetAssignment);
            return View(assetAssignment);
        }

        // GET: AssetAssignment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetAssignment = await _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.FromDepartment)
                .Include(a => a.ToDepartment)
                .Include(a => a.FromEmployee)
                .Include(a => a.ToEmployee)
                .Include(a => a.Creator)
                .FirstOrDefaultAsync(m => m.AssignmentId == id);

            if (assetAssignment == null)
            {
                return NotFound();
            }

            return View(assetAssignment);
        }

        // POST: AssetAssignment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var assetAssignment = await _context.AssetAssignments.FindAsync(id);
            if (assetAssignment != null)
            {
                _context.AssetAssignments.Remove(assetAssignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Nạp dữ liệu các dropdown cho Form Create / Edit
        private async Task LoadFormData(AssetAssignment? assignment = null)
        {
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
                assignment?.AssetId
            );

            ViewBag.FromDepartmentId = new SelectList(
                await _context.Departments.ToListAsync(),
                "DepartmentId",
                "DepartmentName",
                assignment?.FromDepartmentId
            );

            ViewBag.ToDepartmentId = new SelectList(
                await _context.Departments.ToListAsync(),
                "DepartmentId",
                "DepartmentName",
                assignment?.ToDepartmentId
            );

            ViewBag.FromEmployeeId = new SelectList(
                await _context.Employees.ToListAsync(),
                "EmployeeId",
                "FullName",
                assignment?.FromEmployeeId
            );

            ViewBag.ToEmployeeId = new SelectList(
                await _context.Employees.ToListAsync(),
                "EmployeeId",
                "FullName",
                assignment?.ToEmployeeId
            );

            ViewBag.CreatedBy = new SelectList(
                await _context.Users.ToListAsync(),
                "UserId",
                "FullName",
                assignment?.CreatedBy
            );
        }

        private bool AssetAssignmentExists(int id)
        {
            return _context.AssetAssignments.Any(e => e.AssignmentId == id);
        }
    }
}
