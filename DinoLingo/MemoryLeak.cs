using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DinoLingo
{
    public static class MemoryLeak
    {
        static bool debug = true;
        static long totalMemoryPrev = -1;
        

        public static void TrackMemory()
        {
            if (!debug) return;
            double d_memory_percent;

            if (totalMemoryPrev < 0)
            {
                totalMemoryPrev = GC.GetTotalMemory(false);
                d_memory_percent = 100;
            }
            else
            {
                long new_memory = GC.GetTotalMemory(false);
                d_memory_percent = (new_memory - totalMemoryPrev) / (double) totalMemoryPrev * 100;
                totalMemoryPrev = new_memory;
            }
            Debug.WriteLine(String.Format("Total memory: {0}, memory increased: {1}%", totalMemoryPrev, d_memory_percent));
        }
    }
}
