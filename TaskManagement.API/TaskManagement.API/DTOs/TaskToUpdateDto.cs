using TaskManagement.API.Model;

namespace TaskManagement.API.DTOs
{
    public class TaskToUpdateDto
    {
        public string title { get; set; }
        public Status status { get; set; }
        public Priority priority { get; set; }
    }

    public class TaskStatusToUpdateDto
    {
        public int userId { get; set; }
        public Status status { get; set; }
    
    }
}
