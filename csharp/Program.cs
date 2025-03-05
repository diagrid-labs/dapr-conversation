using Dapr.AI.Conversation;
using Dapr.AI.Conversation.Extensions;

class Program
{
  private const string ConversationComponentName = "secure-model";

  static async Task Main(string[] args)
  {
    const string prompt = "Can you extract the domain name from this email john.doe@example.com ? If you cannot, make up an email address and return that";

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDaprConversationClient();
    var app = builder.Build();

    //Instantiate Dapr Conversation Client
    var conversationClient = app.Services.GetRequiredService<DaprConversationClient>();

    try
    {
      // Create conversation options with PII scrubbing and temperature
      var options = new ConversationOptions
      {
        ConversationId = Guid.NewGuid().ToString(),
        ScrubPII = true,
        Temperature = 0.5
      };

      // Send a request to the echo mock LLM component
      var response = await conversationClient.ConverseAsync(
        ConversationComponentName,
//        [new(prompt, DaprConversationRole.User)]
        [new DaprConversationInput(Content: prompt, Role: DaprConversationRole.Generic, ScrubPII: true)],
        options);
      Console.WriteLine("Input sent: " + prompt);

      if (response != null)
      {
        Console.Write("Output response:");
        foreach (var resp in response.Outputs)
        {
          Console.WriteLine($" {resp.Result}");
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error: " + ex.Message);
    }
  }
}