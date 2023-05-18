using ChatBot_Moderator.Database;
using ChatBot_Moderator.Database.Models;
using ChatBot_Moderator.Database.Repository;
using ChatBot_Moderator.Setting_Bad_Words;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBot_Moderator.Bot
{
    public class JackBot
    {
        SQLiteRepository repositoryData;
        WordsFilter badWords = new();
        Person person = new();
        Random rand = new(); // генерация рандомного бит значения для ProcessAsync

        public JackBot(ContextData data)
        {
            repositoryData = new(data);
        }

        public async Task StartReceiving(string apiKey)
        {
            // Cоздаём клиента(бота):
            var bot = new TelegramBotClient(apiKey);

            // Выбираем типы обновлений, которые будем получать (только сообщения от пользователя):
            var allowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.EditedMessage, UpdateType.CallbackQuery };
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = allowedUpdates
            };

            // Начинаем принимать обновления:
            bot.StartReceiving(UpdateHandlerAsync, ErrorHandlerAsync, receiverOptions, CancellationToken.None);

            // Выводим в консоль данные успешного запуска.
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
        }

        public async Task ProcessAsync(ITelegramBotClient bot, Update update, Message message, CancellationToken token)
        {
            if (update.Message != null)
            {
                // Вывод сообщений в консоль:
                Console.WriteLine(message.Text);

                // Работа с запросами пользователей:
                switch (message.Text)
                {
                    case "/force":
                        await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing); // отправление сообщения - живой бот (печатает).
                        await Task.Delay(500);                                             // Видимость печати:
                        await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                             text: "Да прибудет с тобой сила!",
                                                             cancellationToken: token);
                        break;
                    case "/enchant":
                        int luckNumber = rand.Next(1, 3);
                        if (luckNumber == 1)
                        {
                            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing); // отправление сообщения - живой бот (печатает).
                            await Task.Delay(500);                                             // Видимость печати:
                            await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                 text: "Чувствую запах удачи, погнали!",
                                                                 cancellationToken: token);
                        }
                        if (luckNumber == 2)
                        {
                            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing); // отправление сообщения - живой бот (печатает).
                            await Task.Delay(500);                                             // Видимость печати:
                            await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                 text: "Пожалуй, лучше отложить это на другой день!",
                                                                 cancellationToken: token);
                        }
                        break;
                }
            }
        }

        // Получаем ошибки:
        public async Task ErrorHandlerAsync(ITelegramBotClient bot, Exception exc, CancellationToken token) => Console.WriteLine(exc.Message);

        // Получаем сообщения:
        public async Task UpdateHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message || update.Type == UpdateType.EditedMessage)
            {
                person.ChatId = update.Message is null ? update.EditedMessage!.Chat.Id : update.Message!.Chat.Id;
                person.FromId = update.Message is null ? update.EditedMessage!.From!.Id : update.Message!.From!.Id;
                person.MessageText = update.Message is null ? update.EditedMessage!.Text : update.Message!.Text;
                person.MessageId = update.Message is null ? update.EditedMessage!.MessageId : update.Message!.MessageId;
                person.UserName = update.Message is null ? update.EditedMessage!.From!.Username : update.Message!.From!.Username;
                await ProcessAsync(bot, update, update.Message!, token);

                bool badWord = await badWords.ReadBadWordAsync(person.MessageText);
                if (person.MessageText != null && badWord == true)
                {
                    person.Date = DateTime.Now.AddMinutes(5);
                    person.GroupName = update.Message is null ? update.EditedMessage!.Chat.Username : update.Message!.Chat.Username;

                    try
                    {
                        // Ищем нарушителя в БД:
                        var personData = await repositoryData.ReadUserAsync(person);
                        if (personData != null)
                            await repositoryData.UpdateUserDataAsync(personData);           // Если нарушитель уже есть в БД, обновляем данные пользователя
                        else
                            await repositoryData.CreateUserAsync(person);                   // Если нарушителя нет в БД, добавляем пользователя в базу данных

                        // Удаляем нужное сообщение (по фильтру слов):
                        await bot.DeleteMessageAsync(person.ChatId, person.MessageId, token);

                        // Блокируем нарушителю отправку сообщений:
                        await bot.RestrictChatMemberAsync(person.ChatId, person.FromId,
                            new ChatPermissions() { CanSendMessages = false }, DateTime.Now.AddMinutes(5), token);     // Задаем время блокировки

                        // Отправляем сообщение о блокировке в групповой чат:
                        await bot.SendTextMessageAsync(
                            chatId: person.ChatId,
                            text: $"Пользователь {person.UserName} заблокирован за нарушение правил сообщества до {DateTime.Now.AddMinutes(5)}. Мат в чате запрещен!",
                            cancellationToken: token);
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            else
                Console.WriteLine(update.CallbackQuery.Data);
        }
    }
}



