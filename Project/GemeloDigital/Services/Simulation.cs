using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal abstract class Simulation
    {
        internal abstract void Initialize();
        internal abstract void Finish();
        internal abstract void Start();
        internal abstract void Step();
        internal abstract void Stop();
    }
}
