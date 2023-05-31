namespace HL7Sender
{
    public interface IBootStrapper
    {
        bool Init(string[] args);
        Task StartSendingAsync();
    }
}