using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3
{
    internal class CalcFunctions
    {
        internal static double Add(double x, double y) { return x + y; }
        internal static double Subtract(double x, double y) {return x - y; }
        internal static double Multiply(double x, double y) { return x * y; }
        internal static double Divide(double x, double y) { return x / y; }
        internal static double Modulate (double x, double y) { return x % y; }
        internal static double Exponentiate(double x, double y) { return Math.Pow(x, y); }
    }
}
