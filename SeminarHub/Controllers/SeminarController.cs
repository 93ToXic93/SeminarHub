using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;
using SeminarHub.Data.Models;
using static SeminarHub.Data.Constants.DataConstants;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext _context;

        public SeminarController(SeminarHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarAddViewModel()
            {
                Categories = await GetCategoriesAsync()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Add(SeminarAddViewModel model)
        {

            DateTime date = DateTime.Now;

            if (!(DateTime.TryParseExact(model.DateAndTime,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date)))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date format! Must be: {DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesAsync();

                return View(model);
            }

            var modelToAdd = new Seminar()
            {
                Topic = model.Topic,
                CategoryId = model.CategoryId,
                Lecturer = model.Lecturer,
                DateAndTime = date,
                Details = model.Details,
                Id = model.Id,
                OrganizerId = GetUserId(),
                Duration = model.Duration
            };

            await _context.Seminars.AddAsync(modelToAdd);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }


        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await _context.Seminars
                .AsNoTracking()
                .Select(x => new SeminarAllViewModel()
                {
                    Lecturer = x.Lecturer,
                    DateAndTime = x.DateAndTime.ToString(DateFormat),
                    Topic = x.Topic,
                    Id = x.Id,
                    Category = x.Category.Name,
                    Organizer = x.Organizer.UserName

                }).ToListAsync();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var model = await _context.Seminars
                .Where(x => x.Id == id)
                .Include(e => e.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return BadRequest();
            }

            string currUser = GetUserId();

            if (!(model.SeminarsParticipants.Any(x => x.ParticipantId == currUser)))
            {
                model.SeminarsParticipants.Add(new SeminarParticipant()
                {
                    SeminarId = model.Id,
                    ParticipantId = currUser
                });


                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Joined));
            }

            return RedirectToAction(nameof(All));

        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string currUser = GetUserId();

            var model = await _context.SeminarsParticipants
                .AsNoTracking()
                .Where(x => x.ParticipantId == currUser)
                .Select(x => new SeminarAllViewModel()
                {
                    Lecturer = x.Seminar.Lecturer,
                    DateAndTime = x.Seminar.DateAndTime.ToString(DateFormat),
                    Topic = x.Seminar.Topic,
                    Id = x.Seminar.Id,
                    Category = x.Seminar.Category.Name,
                    Organizer = x.Seminar.Organizer.UserName

                }).ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var model = await _context.SeminarsParticipants
                .Where(x => x.SeminarId == id)
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            string currUser = GetUserId();

            if (currUser != model.ParticipantId)
            {
                return Unauthorized();
            }

            _context.SeminarsParticipants.Remove(model);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Seminars
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SeminarDetailsViewModel()
                {
                    Id = x.Id,
                    DateAndTime = x.DateAndTime.ToString(DateFormat),
                    Category = x.Category.Name,
                    Details = x.Details,
                    Duration = x.Duration,
                    Lecturer = x.Lecturer,
                    Organizer = x.Organizer.UserName,
                    Topic = x.Topic
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var foundModel = await _context.Seminars.FindAsync(id);

            if (foundModel == null)
            {
                return BadRequest();
            }

            string currUser = GetUserId();

            if (currUser != foundModel.OrganizerId)
            {
                return Unauthorized();
            }

            var model = new SeminarEditViewModel
            {
                Id = foundModel.Id,
                CategoryId = foundModel.CategoryId,
                Details = foundModel.Details,
                Duration = foundModel.Duration,
                DateAndTime = foundModel.DateAndTime.ToString(DateFormat),
                Lecturer = foundModel.Lecturer,
                OrganizerId = foundModel.OrganizerId,
                Topic = foundModel.Topic,
                Categories = await GetCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SeminarEditViewModel model)
        {
            if (model.Id != id)
            {
                return NotFound();
            }

            DateTime date = DateTime.Now;

            if (!(DateTime.TryParseExact(model.DateAndTime,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date)))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date format! Must be: {DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategoriesAsync();

                return View(model);
            }

            var modelToEdit = await _context.Seminars.FindAsync(model.Id);

            if (modelToEdit == null)
            {
                return NotFound();
            }

            modelToEdit.DateAndTime = date;
            modelToEdit.CategoryId = model.CategoryId;
            modelToEdit.Details = model.Details;
            modelToEdit.Topic = model.Topic;
            modelToEdit.Lecturer = model.Lecturer;
            modelToEdit.Duration = model.Duration;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Seminars
                .Where(x => x.Id == id)
                .Select(x => new SeminarDeleteViewModel()
                {
                    Id = x.Id,
                    OrganizerId = x.OrganizerId,
                    DateAndTime = x.DateAndTime.ToString(DateFormat),
                    Topic = x.Topic

                }).FirstOrDefaultAsync();


            if (model == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            string currUser = GetUserId();

            if (currUser != model.OrganizerId)
            {
                return Unauthorized();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, SeminarDeleteViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var modelToDelete = await _context.Seminars.FindAsync(model.Id);

            string currUser = GetUserId();

            if (modelToDelete == null)
            {
                return NotFound();
            }
            if (modelToDelete.OrganizerId != currUser)
            {
                return Unauthorized();
            }

            _context.Seminars.Remove(modelToDelete);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<ICollection<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(x => new CategoryViewModel()
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToListAsync();

            return categories;
        }
    }
}
