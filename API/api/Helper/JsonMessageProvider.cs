using System.Text.Json;

namespace api.Helper
{
    public class JsonMessageProvider : IMessageProvider
    {
        private readonly Dictionary<string, string> _messages;

        public JsonMessageProvider(string filePath)
        {
            var json = File.ReadAllText(filePath);
            _messages = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        public string GetMessage(string key)
        {
            return _messages.TryGetValue(key, out var message) ? message : "Message not found.";
        }
    }
}