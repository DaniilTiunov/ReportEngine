namespace ReportEngine.Contracts.Commands;

public class KiparisCommands(string commandName)
{
    public string CommandName { get; set; } = commandName;
}