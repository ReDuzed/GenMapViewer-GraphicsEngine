using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public class rand
    {
        private int seed = int.MinValue;
        private Random random()
        {
            return new Random(seed); 
        }
        internal void Update()
        {
            if (seed < int.MaxValue)
                seed++;
            else seed = int.MinValue;
        }
        public int Next(int max)
        {
            return random().Next(0, max);
        }
        public int Next(int min, int max)
        {
            return random().Next(min, max);
        }
        public float NextFloat()
        {
            return (float)random().NextDouble();
        }
        public bool NextBool()
        {
            return NextFloat() < 0.50f;
        }
    }
}
