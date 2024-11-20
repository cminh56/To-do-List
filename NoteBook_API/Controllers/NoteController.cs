using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBook_API.DTO.NoteDTO;
using NoteBook_API.Models;

namespace NoteBook_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {

        private readonly PRN231_NoteContext _context;

        public NoteController(PRN231_NoteContext context)
        {
            _context = context;
        }

        // GET: api/Note
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetAllNotes()
        {
            var notes = await _context.Notes
                .Select(n => new NoteDTO
                {
                    NoteId = n.NoteId,
                    UserId = n.UserId,
                    Title = n.Title,
                    Status = n.Status,
                    Content = n.Content,
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotesByUserId(int userId)
        {
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .Select(n => new NoteDTO
                {
                    NoteId = n.NoteId,
                    UserId = n.UserId,
                    Status = n.Status,
                    Title = n.Title,
                    Content = n.Content,
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync();


            return Ok(notes);
        }

        [HttpGet("{noteId}")]
        public async Task<ActionResult<NoteDTO>> GetNoteById(int noteId)
        {
            var note = await _context.Notes
                .Where(n => n.NoteId == noteId)
                .Select(n => new NoteDTO
                {
                    NoteId = n.NoteId,
                    UserId = n.UserId,
                    Title = n.Title,
                    Content = n.Content,
                    UpdatedAt = n.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (note == null)
            {
                return NotFound(new { Message = "Note not found." });
            }

            return Ok(note);
        }
        // POST: api/Note
        [HttpPost]
        public async Task<ActionResult<NoteDTO>> AddNote(NoteDTO noteDTO)
        {
            // Create a new Note entity from the DTO
            var note = new Note
            {
                UserId = noteDTO.UserId,
                Title = noteDTO.Title,
                Content = noteDTO.Content,
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            };

            // Add the new note to the database context
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Set the NoteId in the DTO to the generated ID
            noteDTO.NoteId = note.NoteId;

            return CreatedAtAction(nameof(GetNoteById), new { noteId = noteDTO.NoteId }, noteDTO);
        }


        // PUT: api/Note/{noteId}
        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, NoteDTO noteDTO)
        {
          
            // Retrieve the existing note
            var existingNote = await _context.Notes.FindAsync(noteId);
            if (existingNote == null)
            {
                return NotFound();
            }

            // Update the note properties
            existingNote.Title = noteDTO.Title;
            existingNote.Content = noteDTO.Content;
         

            // Save changes to the database
            _context.Entry(existingNote).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/Note/{noteId
    
    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteNote(int noteId)
    {
        // Retrieve the note by its ID
        var note = await _context.Notes.FindAsync(noteId);
        if (note == null)
        {
            return NotFound();
        }

        // Remove the note from the database
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }

        [HttpPut("{noteId}/status")]
        public async Task<IActionResult> UpdateNoteStatus(int noteId)
        {
            // Find the note by ID
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                return NotFound($"Note with ID {noteId} not found.");
            }

            // Update the status to "Inactive" if it's currently "Active"
            
                note.Status = "Inactive";

                // Save changes to the database
                await _context.SaveChangesAsync();
     

            return Ok($"Status of note {noteId} changed to Inactive.");
        }

    }

}
