using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public class rand
    {
        public rand()
        {
            seed = DateTime.Now.Millisecond;
            r = random();
        }
        private int seed;
        private Random r;
        private Random random()
        {
            return new Random(seed); 
        }
        internal void Update()
        {
            if (seed + 19753 < int.MaxValue)
                seed += 19753;
            else seed = int.MinValue;
        }
        public int Next(int max)
        {
            return r.Next(0, max);
        }
        public int Next(int min, int max)
        {
            return r.Next(min, max);
        }
        public float NextFloat()
        {
            return (float)r.NextDouble();
        }
        public bool NextBool()
        {
            return Next(2) == 1;
        }
    }
}
