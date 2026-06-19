using System;

namespace HomeworkManager.Models
{
    public class Homework
    {
        public string Id { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public string StatusText => IsCompleted ? "已完成" : "未完成";

        public string ReminderText
        {
            get
            {
                if (IsCompleted) return "✅ 已完成";
                var today = DateTime.Today;
                var diff = (DueDate.Date - today).Days;
                if (diff < 0) return "⛔ 已逾期";
                if (diff == 0) return "🔴 今天到期";
                if (diff <= 3) return $"🟡 還有 {diff} 天";
                return $"🟢 還有 {diff} 天";
            }
        }

        public Homework()
        {
            Id = Guid.NewGuid().ToString();
            DueDate = DateTime.Today;
            IsCompleted = false;
        }
    }
}