using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.EventType)
                .ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        public IActionResult Create()
        {
            ViewBag.EventTypeId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.EventTypes, "EventTypeId", "Name");
            return View();
        }

      

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            ViewBag.EventTypeId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.EventTypes, "EventTypeId", "Name", eventItem.EventTypeId);
            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventItem, IFormFile? imageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Bookings");
            ModelState.Remove("EventType");

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        eventItem.ImageUrl = await UploadImageAsync(imageFile);
                    }
                    catch
                    {
                        eventItem.ImageUrl = null;
                    }
                }
                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EventTypeId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.EventTypes, "EventTypeId", "Name", eventItem.EventTypeId);
            return View(eventItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if event has active bookings
            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var connectionString = "UseDevelopmentStorage=true";
            var containerClient = new Azure.Storage.Blobs.BlobContainerClient(
                connectionString, "venue-images");
            await containerClient.CreateIfNotExistsAsync(
                Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var blobName = Guid.NewGuid().ToString() +
                Path.GetExtension(imageFile.FileName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = imageFile.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}
