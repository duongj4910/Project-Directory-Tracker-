using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;

namespace PTracking.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalesEntities
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalesData.ToListAsync());
        }

        public IActionResult ShowSalesData()
        {
            return View();
        }
        [HttpPost]
        public List<object> GetSalesData()
        {

            List<object> data = new List<object>();
            //Tickets = Table Name
            List<string> labels = _context.SalesData.Select(p => p.Monthname).ToList();

            data.Add(labels);
            List<int> TicketStatusByMonth = _context.SalesData.Select(p => p.TotalSales).ToList();
            data.Add(TicketStatusByMonth);

            return data;
        }


        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SalesData == null)
            {
                return NotFound();
            }

            var salesEntity = await _context.SalesData
                .FirstOrDefaultAsync(m => m.id == id);
            if (salesEntity == null)
            {
                return NotFound();
            }

            return View(salesEntity);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,TotalSales,Monthname")] SalesEntity salesEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesEntity);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SalesData == null)
            {
                return NotFound();
            }

            var salesEntity = await _context.SalesData.FindAsync(id);
            if (salesEntity == null)
            {
                return NotFound();
            }
            return View(salesEntity);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,TotalSales,Monthname")] SalesEntity salesEntity)
        {
            if (id != salesEntity.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesEntityExists(salesEntity.id))
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
            return View(salesEntity);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SalesData == null)
            {
                return NotFound();
            }

            var salesEntity = await _context.SalesData
                .FirstOrDefaultAsync(m => m.id == id);
            if (salesEntity == null)
            {
                return NotFound();
            }

            return View(salesEntity);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SalesData == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalesData'  is null.");
            }
            var salesEntity = await _context.SalesData.FindAsync(id);
            if (salesEntity != null)
            {
                _context.SalesData.Remove(salesEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesEntityExists(int id)
        {
          return (_context.SalesData?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
