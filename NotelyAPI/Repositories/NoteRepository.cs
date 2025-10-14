using Microsoft.EntityFrameworkCore;
using NotelyAPI.Data;
using NotelyAPI.Models.Entities;

namespace NotelyAPI.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly ApplicationDbContext _context;
        public NoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
           
            return await _context.Notes.OrderBy(n=>n.IsCompleted)
                .ThenByDescending(n=>n.CreatedDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Note>> SearchNotesAsync(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllNotesAsync();
            }
            var lowerCaseTerm = searchTerm.ToLower();
            var query = _context.Notes
                .Where(n =>
                        n.Title.ToLower().Contains(lowerCaseTerm) ||
                        n.Category.ToString().ToLower() == lowerCaseTerm);
            return await query
                .OrderBy(n => n.IsCompleted)
                .ThenByDescending(n => n.CreatedDate)
                .ToListAsync();
        }
        public async Task<Note> AddNoteAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note is null)
            {
                throw new Exception("Note not found");
            }
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }


        public async Task<Note> GetNoteByIdAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note is null)
            {
                throw new Exception("Note not found");
            }
            return note ;
        }

        public async Task<Note> UpdateNoteAsync(int id, Note note)
        {
            var existingNote =  await _context.Notes.FindAsync(id);
            if (existingNote is null)
            {
                throw new Exception("Note not found");
            }
            existingNote.Title = note.Title;
            existingNote.Description = note.Description;
            existingNote.Category = note.Category;
            existingNote.IsCompleted = note.IsCompleted;
            // مالوش لازمة لانه بيجبر ال ef تعدل كل الخصائص حتي لو متغيرتش
            //_context.Notes.Update(existingNote);
            await _context.SaveChangesAsync();
            return existingNote;
        }

  
    }
}
