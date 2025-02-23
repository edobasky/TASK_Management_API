﻿using TaskManagement.API.Model;

namespace TaskManagement.API.DTOs
{
    public class TaskCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public Status Status { get; set; }

        public Priority  Priority { get; set; }
        public int userId { get; set; }
    }
}
