using ChatBot_Moderator.Bot;
using ChatBot_Moderator.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TelegramBot
{
    class Program
    {
        static JackBot? botJack;

        static async Task Main()
        {
            try
            {
                // t.me/LookerJack_bot - ссылка для работы с ботом.
                Console.WriteLine("Запуск");

                // Получаем API ключ:
                var apiKey = System.IO.File.ReadAllText(@"C:\\C#\\Telegram\\BotAPI.txt");

                #region Database
                // Работа с БД: 
                var builder = new ConfigurationBuilder();
                // Устанавливаем путь к текущему каталогу:
                builder.SetBasePath(Directory.GetCurrentDirectory());
                // Получаем конфигурацию из файла appsettings.json:
                builder.AddJsonFile("appsettings.json");
                // создаем конфигурацию:
                var config = builder.Build();
                // Получаем строку подключения:
                string? connectionString = config.GetConnectionString("DefaultConnection");
                // Передаем данные подключения и создаем БД:
                var optionsBuilder = new DbContextOptionsBuilder<ContextData>();
                var options = optionsBuilder.UseSqlite(connectionString).Options;
                var db = new ContextData(options);
                #endregion

                // Cоздаём клиента(бота), передаем строку подключения, начинаем принимать обновления:
                botJack = new JackBot(db);
                await botJack.StartReceiving(apiKey);

                Console.Read();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }
    }
}
