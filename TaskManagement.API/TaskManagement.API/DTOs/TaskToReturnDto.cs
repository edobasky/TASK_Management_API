namespace TaskManagement.API.DTOs
{
    public class TaskToReturnDto {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }

    }

   
}
