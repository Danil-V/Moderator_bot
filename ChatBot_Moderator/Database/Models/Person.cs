using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot_Moderator.Database.Models
{
    public class Person
    {
        public long Id { get; set; } = 0;
        public long ChatId { get; set; } = 0;
        public long FromId { get; set; } = 0;
        public string? MessageText { get; set; } = string.Empty;
        public int MessageId { get; set; } = 0;
        public string? GroupName { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
