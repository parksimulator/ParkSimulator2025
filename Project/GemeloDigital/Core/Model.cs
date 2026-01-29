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
        /// Posicion del modelo
        /// </summary>
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }


        /// <summary>
        /// Material del modelo
        /// </summary>
        public Material material;
        public Mesh mesh;

        internal Model()
        {
            Name = "Model";
            Type = SimulatedObjectType.Model;
        }

    }
}
