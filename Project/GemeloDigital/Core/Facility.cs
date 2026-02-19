using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class Facility : SimulatedObject
    {
        /// <summary>
        /// Entradas de la instalación
        /// </summary>
        public List<Point> Entrances { get { return entrances; } set { entrances = value; } }

        /// <summary>
        /// Salidas de la instalación 
        /// </summary>
        public List<Point> Exits { get { return exits; } set { exits = value; } }

        /// <summary>
        /// Consumo en KW/h
        /// </summary>
        public float PowerConsumed { get; set; }

        List<Point> entrances;
        List<Point> exits;

        internal Facility(Point entrance, Point exit)
        {
            Name = "Facility";
            Type = SimulatedObjectType.Facility;

            entrances = new List<Point>();
            exits = new List<Point>();

            entrances.Add(entrance);
            exits.Add(exit);
        }

        internal override float GetKPI(string kpi)
        {
            return 0;
        }

        internal override void StartKPIRecording(string name)
        {
            base.StartKPIRecording(name);

        }

        internal override void StopKPIRecording(string name)
        {
            base.StopKPIRecording(name);

        }
    }
}
