using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Mesh : SimulatedObject
    {
        /// <summary>
        /// Ruta de la malla
        /// </summary>
        public string ResourceId { get; set; }

        internal Mesh()
        {
            Name = "Mesh";
            Type = SimulatedObjectType.Mesh;
        }

    }
}
