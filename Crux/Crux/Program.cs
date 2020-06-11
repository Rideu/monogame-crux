using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace Crux
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        { 
            using (var game = new CoreTests())
                game.Run();
        }
    }
#endif
}
