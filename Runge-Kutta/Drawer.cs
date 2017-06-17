using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;


namespace Runge_Kutta
{
    class Drawer
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);

        static IntPtr hWnd = IntPtr.Zero;
        static IntPtr hDC = IntPtr.Zero;

        public static void Draw(int xStart, int yStart, Complex[,] grid1, Complex[,] grid2, double[] mist)
        {
                    var b = new Bitmap(1500, 2000);
                    using (Graphics consoleGraphics = Graphics.FromImage(b))
                    {

                        Pen whitePen = new Pen(Color.Black);
                        Pen redPen = new Pen(Color.Red, 2);
                        Pen bluePen = new Pen(Color.Blue, 2);
                        Font font = new Font("verdana", 13);

                        consoleGraphics.FillRectangle(Brushes.White, 0,0, 1500, 2000);
                        consoleGraphics.DrawLine(whitePen, new Point(20, 70), new Point(300, 70));
                        consoleGraphics.DrawLine(whitePen, new Point(30, 70), new Point(30, 300));
                        consoleGraphics.DrawString("X", font, Brushes.White, 310, 10);
                        consoleGraphics.DrawString("Y", font, Brushes.White, 40, 250);


                        int y = 3;
                        int x = 100;
                        for (; x < 1300; x++)
                        {
                            y = (int)(grid1[0,x].Real*70 + 70);
                            consoleGraphics.DrawRectangle(redPen, x, y, 1, 1);
                        }

                        consoleGraphics.DrawLine(whitePen, new Point(20, 500), new Point(300,500));
                        consoleGraphics.DrawLine(whitePen, new Point(30, 500), new Point(30, 800));
                        consoleGraphics.DrawString("X", font, Brushes.White, 310, 10);
                        consoleGraphics.DrawString("Y", font, Brushes.White, 40, 250);
                        x = 100;
                        for (; x < 1300; x++)
                        {
                            y = (int)(grid2[0, x].Real*70 + 500);
                            consoleGraphics.DrawRectangle(bluePen, x, y, 1, 1);
                        }

                        consoleGraphics.DrawLine(whitePen, new Point(20, 1000), new Point(300, 1000));
                        consoleGraphics.DrawLine(whitePen, new Point(30, 1000), new Point(30, 1300));
                        consoleGraphics.DrawString("X", font, Brushes.Black, 310, 990);
                        consoleGraphics.DrawString("Y", font, Brushes.Black, 40, 1250);
                        x = 0;
                        for (; x < 6; x++)
                        {
                            y = (int)(mist[x]/10 + 1000);
                            consoleGraphics.DrawRectangle(bluePen, x*20 + 100, y, 5, 5);
                        }


                        font.Dispose();
                        whitePen.Dispose();
                        redPen.Dispose();
                    }
                    b.Save("Charts.png");

        }

    }
}
