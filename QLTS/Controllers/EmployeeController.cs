using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EmployeeController : Controller
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: EMPLOYEES
    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .ToListAsync();

        return View(employees);
    }

    // GET: EMPLOYEES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
        .Include(e => e.Department)
        .FirstOrDefaultAsync(m => m.EmployeeId == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: EMPLOYEES/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.DepartmentId = new SelectList(
            await _context.Departments.ToListAsync(),
            "DepartmentId",
            "DepartmentName"
        );

        return View();
    }

    // POST: EMPLOYEES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("EmployeeId,EmployeeCode,FullName,DepartmentId,Position,Phone,Email,Description")]
        Employee employee)
    {
        // Kiểm tra mã nhân viên trùng
        if (await _context.Employees.AnyAsync(
            e => e.EmployeeCode == employee.EmployeeCode))
        {
            ModelState.AddModelError(
                "EmployeeCode",
                "Mã nhân viên đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(employee);
    }

    // GET: EMPLOYEES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        ViewBag.DepartmentId = new SelectList(
            await _context.Departments.ToListAsync(),
            "DepartmentId",
            "DepartmentName",
            employee.DepartmentId
        );

        return View(employee);
    }

    // POST: EMPLOYEES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("EmployeeId,EmployeeCode,FullName,DepartmentId,Position,Phone,Email,Description")]
        Employee employee)
    {
        if (id != employee.EmployeeId)
        {
            return NotFound();
        }

        // Kiểm tra mã nhân viên trùng với nhân viên khác
        if (await _context.Employees.AnyAsync(
            e => e.EmployeeCode == employee.EmployeeCode
            && e.EmployeeId != employee.EmployeeId))
        {
            ModelState.AddModelError(
                "EmployeeCode",
                "Mã nhân viên đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.EmployeeId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.DepartmentId = new SelectList(
        await _context.Departments.ToListAsync(),
        "DepartmentId",
        "DepartmentName",
        employee.DepartmentId
        );

        return View(employee);
    }

    // GET: EMPLOYEES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(m => m.EmployeeId == id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: EMPLOYEES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee != null)
        {
            _context.Employees.Remove(employee);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.EmployeeId == id);
    }
}