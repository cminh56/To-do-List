namespace Web_Notebook.Models
{
    public class Notification
    {
        public string Message { get; set; }
        public string DueDate { get; set; }  // Thay đổi từ DateTime thành string
        public bool IsRead { get; set; }
    }

}
