using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Runge_Kutta
{
    class Program
    {
        delegate Complex Function(Complex x, Complex[] y);

        static void Main(string[] args)
        {
            var A = -3.0;
            var B = 2.0;
            var C = 1.0; 

            var Derivatives = new Function[4];
            Derivatives[0] = (x, y) => (2 * x * Complex.Pow(y[1], 1/B)*y[3]);
            Derivatives[1] = (x, y) => (2*B*x*Complex.Exp((B/C)*(y[2] - A))*y[3]);
            Derivatives[2] = (x, y) => (2*C*x*y[3]);
            Derivatives[3] = (x, y) => (-2*x*Complex.Log(y[0]));

            var y0 = new Complex[] {1,1,A,1};

            var grid = Method1(Derivatives, 1300, 0.1, y0, 0, 5);
            var acc = Math.Exp(Math.Sin(5*5));
            var acc1 = Math.Exp(B*Math.Sin(5*5));
            var acc2 = (C * Math.Sin(5 * 5) + A);
            var acc3 = Math.Cos(5 * 5);

            System.Console.Out.WriteLine("'My method', y0, 1300 steps, x=5:" + grid[0, 1300]);
            var grid2 = Method1(Derivatives, 1300, 1, y0, 0, 5);
            System.Console.Out.WriteLine("Heun's method, y0, 1300 steps, x=5:" + grid2[0, 1300]);
            System.Console.Out.WriteLine("Accurate y0, x=5:" + acc);
            System.Console.Out.WriteLine("Accurate y1, x=5:" + acc1);
            System.Console.Out.WriteLine("Accurate y2, x=5:" + acc2);
            System.Console.Out.WriteLine("Accurate y3, x=5:" + acc3);
            System.Console.Out.WriteLine("");



            double[] mist = new double[6];
            for (int i = 1; i <= 6; i++)
            {
                var gridl = Method1(Derivatives, 5*Math.Pow(2, i), 0.1, y0, 0, 5);
                mist[i-1] = Math.Sqrt(Math.Pow(gridl[0, (int)(5 * Math.Pow(2, i))].Real - Math.Exp(Math.Sin(5 * 5)), 2)
                    + Math.Pow(gridl[1, (int)(5 * Math.Pow(2, i))].Real - Math.Exp(B*Math.Sin(5 * 5)), 2)
                    + Math.Pow(gridl[2, (int)(5 * Math.Pow(2, i))].Real - (C * Math.Sin(5 * 5) + A), 2)
                    + Math.Pow(gridl[3, (int)(5 * Math.Pow(2, i))].Real - Math.Cos(5 * 5), 2)
                    );
            }
            Drawer.Draw(0, 0, grid, grid2, mist);

            // метод с выбором оптимального шага
            var gridh = Method1(Derivatives, 5.0 * 50, 0.1, y0, 0, 5);
            var gridh2 = Method1(Derivatives, 5.0 * 100, 0.1, y0, 0, 5);


            var Rn = new Complex[4];

            for (int i = 0; i < 4; i++)
                Rn[i] = (gridh[i, (int) (5.0*50)].Real - gridh2[i, (int) (5.0*100)].Real)/(1 - Math.Pow(2, -2));

            double tol = 1e-6;
            var ht = (1.0/50)*Math.Pow(tol/(Math.Abs( norma(Rn) )*4), 1/2.0);
            var gridht = Method1(Derivatives, (int)(Math.Ceiling(5 / ht)), 0.1, y0, 0, 5);

            var mistake = Math.Sqrt(Math.Pow(gridht[0, (int)(Math.Ceiling(5 / ht))].Real - acc, 2)
                + Math.Pow(gridht[1, (int)(Math.Ceiling(5 / ht))].Real - acc1, 2)
                + Math.Pow(gridht[2, (int)(Math.Ceiling(5 / ht))].Real - acc2, 2)
                + Math.Pow(gridht[3, (int)(Math.Ceiling(5 / ht))].Real - acc3, 2)
                );
            System.Console.Out.WriteLine("Hopt:" + ht);
            System.Console.Out.WriteLine("Mistake ht, x=5:" + mistake);
            System.Console.Out.WriteLine("Count of steps:" + Math.Ceiling(5 / ht));
            System.Console.Out.WriteLine("y0 ht, x=5:" + gridht[0, (int)(Math.Ceiling(5 / ht))]);
            System.Console.Out.WriteLine("y1 ht, x=5:" + gridht[1, (int)(Math.Ceiling(5 / ht))]);
            System.Console.Out.WriteLine("y2 ht, x=5:" + gridht[2, (int)(Math.Ceiling(5 / ht))]);
            System.Console.Out.WriteLine("y3 ht, x=5:" + gridht[3, (int)(Math.Ceiling(5 / ht))]);
            System.Console.Out.WriteLine("");




            // автоматический метод
            double rtol = 1e-6;
            double atol = 1e-12;
            var norm = Math.Pow(
                (Derivatives[0](0, y0)*Derivatives[0](0, y0)).Real +
                (Derivatives[1](0, y0)*Derivatives[1](0, y0)).Real +
                (Derivatives[2](0, y0)*Derivatives[2](0, y0)).Real +
                (Derivatives[3](0, y0)*Derivatives[3](0, y0)).Real, 1/2.0);

            var delta = Math.Pow(1/5.0, 3) + Math.Pow(norm, 3);
            var h1 = Math.Pow((rtol*norm + atol)/delta, 1.0/3);
            var autoanswer = AutoMethod1(Derivatives, h1, 0.01, y0, 0, 5, rtol, atol);
            var automistake = Math.Sqrt(Math.Pow(autoanswer[0].Real - acc, 2)
    + Math.Pow(autoanswer[1].Real - acc1, 2)
    + Math.Pow(autoanswer[2].Real - acc2, 2)
    + Math.Pow(autoanswer[3].Real - acc3, 2)
    );

            System.Console.Out.WriteLine("AutoMethod y0, x=5: " + autoanswer[0]);
            System.Console.Out.WriteLine("AutoMethod y1, x=5: " + autoanswer[1]);
            System.Console.Out.WriteLine("AutoMethod y2, x=5: " + autoanswer[2]);
            System.Console.Out.WriteLine("AutoMethod y3, x=5: " + autoanswer[3]);
            System.Console.Out.WriteLine("AutoMethod mistake: " + automistake);

            System.Console.In.Read();

        }

        // Двухэтапный метод второго порядка, зависящий от константы с2
        static Complex[,] Method1(Function[] derivatives, double steps, double c2, Complex[] y0, double xStart, double xEnd)
        {
            var h = (xEnd - xStart)/steps;
            var grid = new Complex[4, (int) steps+1];

            var x = xStart;
            var y = y0;

            for (int i = 1; i <= steps; i++)
            {
               
                y = MethodIteration(derivatives, h, c2, y, x);
                for (int j = 0; j < 4; j++)
                    grid[j, i] = y[j];
                x += h;

            }

            return grid;
        }


        // Метод с автовыбором шага
        static Complex[] AutoMethod1(Function[] derivatives, double h1, double c2, Complex[] y0, double xStart, double xEnd, double rtol, double atol)
       {
            var h = h1;
            var x = xStart;
           var maxStep = 0.01;
            var y = y0;

            int counter = 0;
           var rn = new Complex[4];
           var rn2 = new Complex[4];
            do
           {
               counter++;
                var yf = MethodIteration(derivatives, h, c2, y, x);
                var y1 = MethodIteration(derivatives, h / 2, c2, y, x);
                var y2 = MethodIteration(derivatives, h / 2, c2, y1, x + h / 2);

                for (int i = 0; i < 4; i++)
               {
                   rn[i] = (y2[i].Real - yf[i].Real)/(1.0 - Math.Pow(2, -2));
                   rn2[i] = (y2[i].Real - yf[i].Real) / ( Math.Pow(2, 2) - 1.0);
                }

               var rnorm = norma(rn);
               var yfnorm = norma(yf);

               if (rnorm > ((yfnorm * rtol + atol))*4)
               {
                   h /= 2;
                   //y = y0;
               } 
               else if ((yfnorm * rtol + atol) < rnorm)
               {
                    x += h;
                    h /= 2;
                    x += h;
                    for (int i = 0; i < 4; i++)
                        y[i] = y2[i] + rn2[i];
                }
               else if ((yfnorm * rtol + atol)/8.0 <= rnorm)
               {
                    x += h;
                    for (int i = 0; i < 4; i++)
                    y[i] = yf[i] + rn[i];
               }
               else
               {
                    x += h;
                    for (int i = 0; i < 4; i++)
                        y[i] = yf[i] + rn[i];
                }

               if (h > maxStep) h = maxStep;

                if ((x<xEnd) && ((x + h) > xEnd))
              {
                  h = xEnd - x;
              }
                
            } while (x < xEnd);

            System.Console.Out.WriteLine("AutoMethod count of steps: " + counter);
            return y;
        }

        // Евклидова норма
        static double norma(Complex[] v)
        {
            double n = 0;
            for (int i = 0; i < v.Length; i++)
                n += Math.Pow(v[i].Real, 2);
            return Math.Sqrt(n);
        }

        // Итерация двухэтапного метода Рунге-Кутты второго порядка, зависящая от константы с
        static Complex[] MethodIteration(Function[] derivatives, double h, double c, Complex[] y0, double x0)
        {
            double[] b = new double[2];

            Complex[] V1 = new Complex[4];
            Complex[] V2 = new Complex[4];
            Complex[] Vt = new Complex[4];

            b[1] = 1/(2*c);
            b[0] = 1 - b[1];

          for (int i = 0; i < 4; i++)
          V1[i] = derivatives[i](x0, y0);

            for (int i = 0; i < 4; i++)
                Vt[i] = y0[i] + c*h*V1[i];

            for (int i = 0; i < 4; i++)
          V2[i] = derivatives[i](x0 + c*h, Vt);

            var F = new Complex[4];

            for (int p = 0; p < 4; p++)
            {            
                F[p] = y0[p] + h*(b[0]*V1[p] + b[1] * V2[p]);
            }
            return F;
        }
    }
}
