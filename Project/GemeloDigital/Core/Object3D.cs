using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Object3D : SimulatedObject
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
        public Material Material { get; set; }
        public Model Model { get; set; }

        internal Object3D()
        {
            Name = "Object3D";
            Type = SimulatedObjectType.Object3D;
        }

    }
}
