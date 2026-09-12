namespace FitMaster.Infrastructure.Ai;

public class GroqSettings
{
    public required string ApiKey { get; set; }

    public string ApiUrl { get; set; } = "https://api.groq.com/openai/v1/chat/completions";

    // Deliberately not a "gpt-oss-*" reasoning model: on the meal-plan prompt those
    // spent their entire completion-token budget on hidden reasoning and returned
    // empty content (finish_reason "length") every time, even at 8000 tokens.
    // qwen/qwen3.8-27b answers directly and reliably for this structured-JSON task.
    public string Model { get; set; } = "qwen/qwen3.8-27b";
}
