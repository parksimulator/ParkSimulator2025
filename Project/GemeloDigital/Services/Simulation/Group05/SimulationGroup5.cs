using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class SimulationGroup5 : Simulation
    {
        internal override void Initialize()
        {
            Console.WriteLine("DummySimulation: Initializing");
        }

        internal override void Finish()
        {
            Console.WriteLine("DummySimulation: Finish");
        }

        internal override void Start()
        {
            // Nothing to do
            Console.WriteLine("DummySimulation: Start");
        }

        internal override void Step()
        {
            // Nothing to do
            Console.WriteLine("DummySimulation: Step");
        }

        internal override void Stop()
        {
            // Nothing to do
            Console.WriteLine("DummySimulation: Stop");
        }
    }
}
