

using OpenAI.Chat;
using System.ClientModel;
using System.Text.RegularExpressions;

namespace AnimeTaste.Core.Utils
{
    /// <summary>
    /// 本地Ai大模型服务
    /// </summary>
    public partial class Ai
    {
        private ChatClient _client;

        public Ai()
        {
            var option = new OpenAI.OpenAIClientOptions
            {
                Endpoint = new Uri("http://192.168.31.53:1234/v1"),
            };

            var key = new ApiKeyCredential("lm-studio");
            _client = new(model: "qwen3-30b-a3b", key, option);
        }

        public string TranslateToChinese(string text)
        {
            var content = $"请将下列单词或句子翻译为中文，勿换行，不解释，直接给结果\n{text}";
            var completion = _client.CompleteChat(content);
            var list = completion?.Value?.Content.Select(m => m.Text) ?? [];
            var result = string.Concat(list);
            return ThinkRegex().Replace(result, "").Trim();
        }

        [GeneratedRegex(@"<think>.*?</think>", RegexOptions.Singleline)]
        private static partial Regex ThinkRegex();
    }
}
