using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Texture : SimulatedObject
    {
        /// <summary>
        /// Ruta de la textura
        /// </summary>
        public string ResourceId { get; set; }

        internal Texture()
        {
            Name = "Texture";
            Type = SimulatedObjectType.Texture;
        }

    }
}
