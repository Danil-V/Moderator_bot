namespace ChatBot_Moderator.Database.Repository
{
    public interface IRepository<T> where T : class
    {
        Task CreateUserAsync(T data);              // Добавление нового пользователя
        Task<T> ReadUserAsync(T data);             // Получение всех данных пользователя
        Task UpdateUserDataAsync(T data);          // Обновление данных пользователя
        Task DeleteUserAsync(T data);              // Удаление пользователя по ChatId
    }
}
