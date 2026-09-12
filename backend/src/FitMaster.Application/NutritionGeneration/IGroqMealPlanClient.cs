namespace FitMaster.Application.NutritionGeneration;

/// <summary>
/// Thin contract over the Groq chat-completions API - implemented in Infrastructure.
/// Returns the raw completion text; parsing/validation is <see cref="IMealPlanResponseParser"/>'s
/// job so that logic stays testable without a network call.
/// </summary>
public interface IGroqMealPlanClient
{
    Task<string> GetChatCompletionAsync(string prompt, CancellationToken cancellationToken);
}
