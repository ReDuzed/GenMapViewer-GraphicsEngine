using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    /*
    public class rand
    {
        int seed;
        private async void Seed()
        {
            await Task.Run(() =>
            {
                Action method = null;
                method = delegate ()
                {
                    if (seed < int.MaxValue - 19280)
                        seed += 19280;
                    else seed = 0;
                    graphic.Dispatcher.BeginInvoke(method, System.Windows.Threading.DispatcherPriority.Background);
                };
                graphic.Dispatcher.BeginInvoke(method, System.Windows.Threading.DispatcherPriority.Background);
            });
        }
        public static int Next(int min, int max)
        {
            if (seed < int.MaxValue - 812141)
                seed += new Random().Next(NextBool() ? 812141 : 800160);
            else seed = 0;
            return new Random((int)seed).Next(min, max);
        }
        public static bool NextBool()
        {
            if (seed < int.MaxValue - 819218)
                seed += new Random().Next(819218);
            else seed = 0;
            return new Random((int)seed).Next(0, 1) == 1;
        }
    }
*/
}
