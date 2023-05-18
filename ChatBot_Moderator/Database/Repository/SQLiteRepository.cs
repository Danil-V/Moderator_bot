using ChatBot_Moderator.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatBot_Moderator.Database.Repository
{
    public class SQLiteRepository : IRepository<Person>
    {
        private readonly ContextData? _dbContext;
        public SQLiteRepository(ContextData? dbContext) => _dbContext = dbContext;

        public async Task CreateUserAsync(Person data)
        {
            try
            {
                await _dbContext!.Persons!.AddAsync(data);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        public async Task<Person> ReadUserAsync(Person data)
        {
            var person = new Person();
            var persons = await _dbContext!.Persons!.ToListAsync();

            foreach (Person? p in persons)
            {
                if (p.ChatId == data.ChatId && p.FromId == data.FromId)
                    person = data;
                else
                    person = null;
            }
            return person;
        }

        public async Task UpdateUserDataAsync(Person data)
        {
            try
            {
                _dbContext?.Persons?.Update(data);
                await _dbContext!.SaveChangesAsync();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        public async Task DeleteUserAsync(Person data)
        {
            try
            {
                _dbContext?.Persons?.Remove(data);
                await _dbContext!.SaveChangesAsync();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }
    }
}
