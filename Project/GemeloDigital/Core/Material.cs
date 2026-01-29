using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Material : SimulatedObject
    {
        /// <summary>
        /// Color
        /// </summary>
        public Vector3 Color { get; set; }

        /// <summary>
        /// Texture
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Shader
        /// </summary>
        public Shader Shader { get; set; }

        internal Material()
        {
            Name = "Material";
            Type = SimulatedObjectType.Material;
        }

    }
}
