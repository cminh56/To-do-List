namespace NoteBook_API.DTO.NoteDTO
{
    public class NoteDTO
    {
        public int NoteId { get; set; }
        public int? UserId { get; set; }

        public string? Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
