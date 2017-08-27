using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepositoryScaffolding.Data;

namespace RepositoryScaffolding.Controllers
{
    public interface ITestsRepository {
        Task<IEnumerable<Test>> ReadAsync();
        Task<Test> ReadAsync(int id);
        Task<Test> CreateAsync(Test test);
        Task UpdateAsync(Test test);
        Task DeleteAsync(int id);
    }
    public class TestsRepository: ITestsRepository {
        private readonly ApplicationDbContext _context;

        public TestsRepository(ApplicationDbContext context)
        {
            _context = context;    
        }


        public async Task<IEnumerable<Test>> ReadAsync()
        {
            return await _context.Test.ToListAsync();
        }
    
        public async Task<Test> ReadAsync(int id) {
            return await _context.Test
                   .SingleOrDefaultAsync(m => m.Id == id);
        }
    
        public async Task<Test> CreateAsync(Test test) {
            _context.Add(test);
            await _context.SaveChangesAsync();
            return test;
        }

        public async Task UpdateAsync(Test test) {
            _context.Update(test);
            await _context.SaveChangesAsync();
        }
    
        public async Task DeleteAsync(int id) {
            var test = await ReadAsync(id);
            if (test != null) {
                _context.Test.Remove(test);
            }
            await _context.SaveChangesAsync();
        }

    }

    public class TestsController : Controller
    {
        private readonly ITestsRepository _repository;
    
        public TestsController(ITestsRepository repository)
        {
            _repository = repository;    
        }


        // GET: Tests

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = await _repository.ReadAsync();
            return View(applicationDbContext);
        }

        // GET: Tests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _repository.ReadAsync(id.GetValueOrDefault());
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // GET: Tests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Test1,Test2")] Test test)
        {
            if (ModelState.IsValid)
            {
                await _repository.CreateAsync(test);  //TODO: What if create is unsuccesful
                return RedirectToAction("Index");
            }
            return View(test);
        }

        // GET: Tests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _repository.ReadAsync(id.GetValueOrDefault());
            if (test == null)
            {
                return NotFound();
            }
            return View(test);
        }

        // POST: Tests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Test1,Test2")] Test test)
        {
            if (id != test.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(test);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TestExists(test.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(test);
        }

        // GET: Tests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _repository.ReadAsync(id.GetValueOrDefault());
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        private async Task<bool> TestExists(int id)
        {
            return await _repository.ReadAsync(id) != null;
        }
    }
}
