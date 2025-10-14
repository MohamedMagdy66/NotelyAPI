using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotelyAPI.Data;
using NotelyAPI.Models;
using NotelyAPI.Models.Entities;
using NotelyAPI.Repositories;

namespace NotelyAPI.Controllers
{
    [ApiController] // علشان يعمل Model Validation automatically
    [Route("api/[controller]")] // هنا بس بعرفه انو ال Route هيكون api/Notely
    public class NotelyController : Controller
    {
        private readonly INoteRepository _noteRepository;
        public NotelyController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            return Ok(await _noteRepository.GetAllNotesAsync());
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchNotes([FromQuery] string query)
        {
            var notes = await _noteRepository.SearchNotesAsync(query);
            return Ok(notes);
        }
        // GET: api/notely/categories
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = Enum.GetNames(typeof(NoteCategory));
            return Ok(categories);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetNoteById(int Id)
        {
            var note = await _noteRepository.GetNoteByIdAsync(Id);
            return Ok(note);
        }
        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] Note note)
        {
            if (note is null)
            {
                return BadRequest("Note is null");
            }
            var createdNote = await _noteRepository.AddNoteAsync(note);
            // استخدام CreatedAtAction لإرجاع Status 201
            return CreatedAtAction(nameof(GetNoteById), new { Id = createdNote.NoteId }, createdNote);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateNote(int Id, [FromBody]Note note)
        {
            if (note is null || Id != note.NoteId)
            {
                return BadRequest("Note is null or Id mismatch");
            }
            var UpdatedNote = await _noteRepository.UpdateNoteAsync(Id,note);
            if (UpdatedNote is null)
            {
                return NotFound("Note not found");
            }

            return Ok(UpdatedNote);
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteNote(int Id)
        {
             await _noteRepository.DeleteNoteAsync(Id);

            return Ok("Note deleted successfully");
        }
    }
}
