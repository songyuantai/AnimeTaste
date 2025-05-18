

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
                Endpoint = new Uri("http://127.0.0.1:1234/v1"),

            };

            var key = new ApiKeyCredential("lm-studio");
            _client = new(model: "qwen3-30b-a3b", key, option);

        }

        private static readonly ChatTool GetCurrentDateTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentDate),
            functionDescription: "获取当前的日期"
        );

        private static readonly ChatTool GetCurrentTimeTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentTime),
            functionDescription: "获取当前的时间"
        );

        private string GetCurrentDate() => DateTime.Now.ToString("yyyy年MM月dd日");

        private string GetCurrentTime() => DateTime.Now.ToString("HH:mm:ss");

        public string TranslateToChinese(string text)
        {
            var content = $"请将下列单词或句子翻译为中文，勿换行，不解释，但是保留标点符号，直接给结果\n{text}/no_think";

            var options = new ChatCompletionOptions() { Tools = { GetCurrentDateTool, GetCurrentTimeTool } };

            //"今天几月几号？"，"现在几点了？"
            var completion = _client.CompleteChat([text], options);

            if (completion.Value.ToolCalls.Count > 0)
            {
                foreach (var item in completion.Value.ToolCalls)
                {
                    if (item.FunctionName == nameof(GetCurrentTime))
                    {
                        var message = "工具调用结果：" + GetCurrentTime();
                        completion = _client.CompleteChat([message], options);
                        break;
                    }

                    if (item.FunctionName == nameof(GetCurrentDate))
                    {
                        var message = "工具调用结果：" + GetCurrentDate();
                        completion = _client.CompleteChat([message], options);
                        break;
                    }
                }
            }

            var list = completion?.Value?.Content.Select(m => m.Text) ?? [];
            var result = string.Concat(list);
            return ThinkRegex().Replace(result, "").Trim();
        }

        [GeneratedRegex(@"<think>.*?</think>", RegexOptions.Singleline)]
        private static partial Regex ThinkRegex();
    }
}
