using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Point : SimulatedObject
    {
        /// <summary>
        /// Coordenadas del punto
        /// </summary>
        public Vector3 Position { get; set; }

        internal Point()
        {
            Name = "Point";
            Type = SimulatedObjectType.Point;
        }

        
    }
}
