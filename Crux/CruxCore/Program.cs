using System;

namespace CruxСore
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new CruxСoreTests())
                game.Run();
        }
    }
}
