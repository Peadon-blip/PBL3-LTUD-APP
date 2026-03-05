public class ReminderConfig {
    public long IdConfig { get; set; }
    public long IdAcc { get; set; }
    public int MinsBefore { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? QuietHours { get; set; }
    public string Channel { get; set; } = "App";
}