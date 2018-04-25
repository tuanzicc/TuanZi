using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Maths
{
    public static class MathHelper
    {
        public static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
        }
    }
}
