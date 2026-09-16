namespace KermAdhd.Database.DataModels
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
