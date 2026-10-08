using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;

namespace QLTS.Controllers
{
    public class MaintenanceRequestController : Controller
    {
        private readonly AppDbContext _context;

        public MaintenanceRequestController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MaintenanceRequest
        public async Task<IActionResult> Index()
        {
            var requests = await _context.MaintenanceRequests
                .Include(m => m.Asset)
                .Include(m => m.Requester)
                .Include(m => m.Technician)
                .ToListAsync();

            return View(requests);
        }

        // GET: MaintenanceRequest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.Asset)
                .Include(m => m.Requester)
                .Include(m => m.Technician)
                .FirstOrDefaultAsync(m => m.MaintenanceRequestId == id);

            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequest/Create
        public async Task<IActionResult> Create()
        {
            await LoadFormData();
            return View();
        }

        // POST: MaintenanceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaintenanceRequestId,AssetId,RequesterId,TechnicianId,RequestDate,ProblemDescription,Solution,Status,CompletedDate,Cost")]
            MaintenanceRequest maintenanceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadFormData(maintenanceRequest);
            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            await LoadFormData(maintenanceRequest);
            return View(maintenanceRequest);
        }

        // POST: MaintenanceRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int? id,
            [Bind("MaintenanceRequestId,AssetId,RequesterId,TechnicianId,RequestDate,ProblemDescription,Solution,Status,CompletedDate,Cost")]
            MaintenanceRequest maintenanceRequest)
        {
            if (id != maintenanceRequest.MaintenanceRequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceRequestExists(maintenanceRequest.MaintenanceRequestId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadFormData(maintenanceRequest);
            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.Asset)
                .Include(m => m.Requester)
                .Include(m => m.Technician)
                .FirstOrDefaultAsync(m => m.MaintenanceRequestId == id);

            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // POST: MaintenanceRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest != null)
            {
                _context.MaintenanceRequests.Remove(maintenanceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // Nạp dữ liệu các dropdown cho Form Create / Edit
        private async Task LoadFormData(MaintenanceRequest? request = null)
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
                request?.AssetId
            );

            ViewBag.RequesterId = new SelectList(
                await _context.Users.ToListAsync(),
                "UserId",
                "FullName",
                request?.RequesterId
            );

            ViewBag.TechnicianId = new SelectList(
                await _context.Users.ToListAsync(),
                "UserId",
                "FullName",
                request?.TechnicianId
            );
        }

        private bool MaintenanceRequestExists(int id)
        {
            return _context.MaintenanceRequests.Any(e => e.MaintenanceRequestId == id);
        }
    }
}
