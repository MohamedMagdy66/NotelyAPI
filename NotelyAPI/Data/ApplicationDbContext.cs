using Microsoft.EntityFrameworkCore;
using NotelyAPI.Models.Entities;

namespace NotelyAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Note> Notes {get;set;}
    }
}
