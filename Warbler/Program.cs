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
            case 2:
                warbler.RunExpressionsGenerator(args[0], args[1]);
                break;
            default:
                Console.WriteLine(Common.Usage);
                break;
        }
    }
}