using ChatBot_Moderator.Database.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace ChatBot_Moderator.Database
{
    public class ContextData : DbContext                        // Определяем контекст данных, используемый для взаимодействия с базой данных
    {
        public DbSet<Person>? Persons { get; set; } = null;             // Определяем набор объектов, которые будут храниться в базе данных
        public ContextData(DbContextOptions<ContextData> options) : base(options)
        {
            bool isCreated = Database.EnsureCreated();
            if (isCreated == true)
                Console.WriteLine("База данных успешно создана");
            else
                Console.WriteLine("База данных уже существует");
        }
    }
}
