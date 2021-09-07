using System;

namespace SensingBase.Utils
{
    public class RandomEx
    {
        public RandomEx()
        {
            r1 = new Random(unchecked((int)DateTime.Now.Ticks));
            r2 = new Random(~unchecked((int)DateTime.Now.Ticks));
        }

        private Random r1, r2;

        public double NextDouble()
        {
            double v1 = r1.NextDouble();
            double v2 = r2.NextDouble();

            double r = Math.Sqrt(-2.0 * Math.Log(v2));
            if (r > 1.0)
                return NextDouble();

            return Math.Abs(r * Math.Cos(2 * Math.PI * v1));
        }
    }
}
