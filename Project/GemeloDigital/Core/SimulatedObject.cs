using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class SimulatedObject
    {
        /// <summary>
        /// Identificador único del objeto
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del objeto simulado
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tipo del objeto. "Any" para objeto de la clase base.
        /// </summary>
        public SimulatedObjectType Type { get; set; }

        internal SimulatedObject()
        {
            Name = "Sin nombre";
            Type = SimulatedObjectType.Any;
            Id = Guid.NewGuid().ToString();
        }

        internal virtual float GetKPI(string kpi)
        {
            return 0;
        }

        internal virtual void StartKPIRecording(string name)
        {
            // Nothing to do
        }

        internal virtual void StopKPIRecording(string name)
        {
            // Nothing to do
        }
    }
}
