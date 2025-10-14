using NotelyAPI.Models.Entities;

namespace NotelyAPI.Repositories
{
    public interface INoteRepository
    {
        // IEnumerable --> علشان ببساطة بتنفذ الكويري و تبدأ تخزن في الميموري وقت اللزوم و ده بيوفر وقت بدل ما تروح تجيب كل الداتا مرة واحدة
        // كمان علشان مجبرش الكود انه يتعامل مع داتا من نوع معين
        // كمان Task علشان العملية دي بتاخد وقت و مش بتتم في نفس اللحظة
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<IEnumerable<Note>> SearchNotesAsync(string searchTerm);
        Task<Note> GetNoteByIdAsync(int id);
        Task<Note> AddNoteAsync(Note note);
        Task<Note> UpdateNoteAsync(int id, Note note);
        Task DeleteNoteAsync(int id);
    }
}
