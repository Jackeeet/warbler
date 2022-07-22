using Warbler.Resources.Common;

namespace Warbler;

internal class Program
{
    private static void Main(string[] args)
    {
        // Resources.ErrorMessages.Culture = new CultureInfo("ru");
        var warbler = new Warbler();
        switch (args.Length)
        {
            case 0:
                warbler.RunInteractive();
                break;
            case 1:
                warbler.RunFile(args[0]);
                break;
            default:
                Console.WriteLine(Common.Usage);
                break;
        }
    }
}