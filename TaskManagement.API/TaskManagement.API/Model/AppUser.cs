﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Model
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem>?  TaskItems { get; set; }
    }
}
