namespace Web_Notebook.Models.Note
{
    public class Note
    {
        public int NoteId { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
