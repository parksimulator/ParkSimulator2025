using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    /// <summary>
    /// Tipo de objeto simulado. "Any" se utiliza para
    /// indicar la clase base.
    /// </summary>
    public enum SimulatedObjectType
    {
        Any,
        Facility,
        Person,
        Point,
        Path,
        Camera,
        Model,
        Material,
        Texture,
        Shader,
        Mesh
    }

    /// <summary>
    /// Estado de la simulación. Ciertas operaciones
    /// sólo pueden hacerse en ciertos estados.
    /// </summary>
    public enum SimulatorState
    {
        Stopped,
        Running
    }
}
