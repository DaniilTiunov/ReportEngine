using ReportEngine.App.Dds;
using ReportEngine.App.Dds.Remote;

namespace ReportEngine.AnatoliyTest;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Тест");



        List<string> _keys = new List<string>
        {
            "var sraka",
            "var zhopa",
            "var huy"
        };

        var ddsMailService = new DdsMailService();

        var message = "Тестовое сообщение";
        var keyString = "var sraka";

        var encryptedMessage = DdsСryptoService.EncryptAndCombine(message, keyString);

        Console.WriteLine($"Зашифрованное сообщение: {encryptedMessage}");

        var decryptedMessage = DdsСryptoService.SplitAndDecrypt(encryptedMessage, keyString);

        Console.WriteLine($"Расшифрованное сообщение: {decryptedMessage}");

        await ddsMailService.ViewAllFolders();
    }
}