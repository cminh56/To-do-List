namespace NoteBook_API.DTO.TaskDTO
{
    public class TaskDTO
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
