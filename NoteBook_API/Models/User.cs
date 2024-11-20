using System;
using System.Collections.Generic;

namespace NoteBook_API.Models
{
    public partial class User
    {
        public User()
        {
            Notes = new HashSet<Note>();
            Tasks = new HashSet<Task>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
