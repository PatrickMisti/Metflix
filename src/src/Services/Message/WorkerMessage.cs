namespace Metflix.Services.Message
{
    public class WorkerMessage(List<int> message)
    {
        public List<int> Message { get; set; } = message;
    }

    public class WorkerMessageRequest
    {
        public static WorkerMessageRequest Instance = new WorkerMessageRequest();
    }
}
