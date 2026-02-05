using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Model : SimulatedObject
    {
        /// <summary>
        /// Ruta de la malla
        /// </summary>
        public string ResourceId { get; set; }

        internal Model()
        {
            Name = "Mesh";
            Type = SimulatedObjectType.Model;
        }

    }
}
