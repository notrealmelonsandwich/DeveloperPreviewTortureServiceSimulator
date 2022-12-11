using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDeveloperPreviewTortureServiceSimulator.Utilities
{
    public static class RandomExtension
    {
        public static bool NextBool(this Random r, int truePercentage = 50)
        {
            return r.NextDouble() < truePercentage / 100.0;
        }
    }
}
