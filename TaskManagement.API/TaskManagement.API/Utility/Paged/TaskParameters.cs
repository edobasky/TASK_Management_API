namespace TaskManagement.API.Utility.Paged
{
    public class TaskParameters : RequestParameters
    {
        public int Status { get; set; }
        public int Priority { get; set; }
    }
}
