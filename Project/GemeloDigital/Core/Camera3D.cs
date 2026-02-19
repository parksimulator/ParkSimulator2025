using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Camera3D : SimulatedObject
    {
        /// <summary>
        /// Posicion de la cámara
        /// </summary>
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Apertura
        /// </summary>
        public float FOV { get; set; }

        /// <summary>
        /// Distancia al plano cercano
        /// </summary>
        public float ZNear { get; set; }

        /// <summary>
        /// Distancia al plano lejano
        /// </summary>
        public float ZFar { get; set; }

        internal Camera3D()
        {
            Name = "Camera";
            Type = SimulatedObjectType.Camera3D;

            FOV = 60;
            ZNear = 0.1f;
            ZFar = 10000f;
        }

    }
}
