using System;
using System.Collections.Generic;

namespace NoteBook_API.Models
{
    public partial class Task
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
