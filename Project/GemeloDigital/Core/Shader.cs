using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Shader : SimulatedObject
    {
        /// <summary>
        /// Ruta del shader
        /// </summary>
        public string ResourceId { get; set; }

        internal Shader()
        {
            Name = "Shader";
            Type = SimulatedObjectType.Shader;
        }

    }
}
