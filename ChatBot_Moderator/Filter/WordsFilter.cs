using System.IO;

namespace ChatBot_Moderator.Setting_Bad_Words
{
    public class WordsFilter
    {
        public async Task<bool> ReadBadWordAsync(string userMessage)
        {
            bool result = false;
            string[] words = File.ReadAllLines("C:\\Users\\NevRSS\\source\\repos\\ChatBot_Moderator\\ChatBot_Moderator\\Filter\\words.txt");
            foreach (string w in words)
            {
                if (userMessage.ToLower().Contains(w))
                    result = true;
            }
            return result;
        }
    }
}
