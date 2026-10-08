using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QLTS.Controllers
{
    public class AssetDisposalController : Controller
    {
        private readonly AppDbContext _context;

        public AssetDisposalController(AppDbContext context)
        {
            _context = context;
        }

        private void LoadFormData(AssetDisposal? disposal = null)
        {
            var assets = _context.Assets.ToList();
            if (!assets.Any())
            {
                ViewBag.AssetId = new SelectList(new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Chọn tài sản --" } }, "Value", "Text");
            }
            else
            {
                var assetList = assets.Select(a => new { a.AssetId, DisplayText = a.AssetCode + " - " + a.AssetName }).ToList();
                ViewBag.AssetId = new SelectList(assetList, "AssetId", "DisplayText", disposal?.AssetId);
            }

            var users = _context.Users.ToList();
            if (!users.Any())
            {
                ViewBag.ApprovedBy = new SelectList(new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Chọn người phê duyệt --" } }, "Value", "Text");
            }
            else
            {
                var userList = users.Select(u => new { u.UserId, DisplayText = u.Username + " - " + u.FullName }).ToList();
                ViewBag.ApprovedBy = new SelectList(userList, "UserId", "DisplayText", disposal?.ApprovedBy);
            }

            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending",   Text = "Pending"   },
                new SelectListItem { Value = "Approved",  Text = "Approved"  },
                new SelectListItem { Value = "Rejected",  Text = "Rejected"  },
                new SelectListItem { Value = "Completed", Text = "Completed" }
            };
            ViewBag.Status = new SelectList(statuses, "Value", "Text", disposal?.Status ?? "Pending");

            var disposalMethods = new List<string> { "Bán", "Tiêu hủy", "Cho/Tặng", "Khác" };
            ViewBag.DisposalMethod = new SelectList(disposalMethods, disposal?.DisposalMethod);
        }

        // GET: AssetDisposal
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AssetDisposals.Include(a => a.Asset).Include(a => a.Approver);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AssetDisposal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetDisposal = await _context.AssetDisposals
                .Include(a => a.Asset)
                .Include(a => a.Approver)
                .FirstOrDefaultAsync(m => m.AssetDisposalId == id);
            if (assetDisposal == null)
            {
                return NotFound();
            }

            return View(assetDisposal);
        }

        // GET: AssetDisposal/Create
        public IActionResult Create()
        {
            LoadFormData();
            return View();
        }

        // POST: AssetDisposal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetDisposalId,AssetId,DisposalDate,Reason,DisposalMethod,DisposalValue,Status,ApprovedBy,Description")] AssetDisposal assetDisposal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assetDisposal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadFormData(assetDisposal);
            return View(assetDisposal);
        }

        // GET: AssetDisposal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetDisposal = await _context.AssetDisposals.FindAsync(id);
            if (assetDisposal == null)
            {
                return NotFound();
            }
            LoadFormData(assetDisposal);
            return View(assetDisposal);
        }

        // POST: AssetDisposal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetDisposalId,AssetId,DisposalDate,Reason,DisposalMethod,DisposalValue,Status,ApprovedBy,Description")] AssetDisposal assetDisposal)
        {
            if (id != assetDisposal.AssetDisposalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetDisposal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetDisposalExists(assetDisposal.AssetDisposalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            LoadFormData(assetDisposal);
            return View(assetDisposal);
        }

        // GET: AssetDisposal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetDisposal = await _context.AssetDisposals
                .Include(a => a.Asset)
                .Include(a => a.Approver)
                .FirstOrDefaultAsync(m => m.AssetDisposalId == id);
            if (assetDisposal == null)
            {
                return NotFound();
            }

            return View(assetDisposal);
        }

        // POST: AssetDisposal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetDisposal = await _context.AssetDisposals.FindAsync(id);
            if (assetDisposal != null)
            {
                _context.AssetDisposals.Remove(assetDisposal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool AssetDisposalExists(int id)
        {
            return _context.AssetDisposals.Any(e => e.AssetDisposalId == id);
        }
    }
}
