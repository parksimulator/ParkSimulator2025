using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal partial class Storage02 : Storage
    {
        internal override void LoadScene(string storageId)
        {

            Console.WriteLine("Storage02: Load simulation" + storageId + ".sb");

            if (!File.Exists($"saves/{storageId}.sb"))
            {
                Console.WriteLine($"La escena {storageId} no existe");
                Console.ReadLine();
                return;
            }

            FileStream fileLoad = new FileStream("saves/" + storageId + ".sb", FileMode.Open, FileAccess.Read);

            bytes = new byte[sizeof(int)];
            fileLoad.Read(bytes); 
            int countPoint = BitConverter.ToInt32(bytes); 

            for (int i = 0; i < countPoint; i++)
            {
                Point pointTemporal = SimulatorCore.CreatePoint(); 
                var posTemporal = pointTemporal.Position;

                bytes = new byte[16];
                fileLoad.Read(bytes);
                pointTemporal.Id = new Guid(bytes).ToString();

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                int longitudName = BitConverter.ToInt32(bytes);

                bytes = new byte[longitudName]; 
                fileLoad.Read(bytes); 
                pointTemporal.Name = System.Text.Encoding.UTF8.GetString(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                posTemporal.X = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                posTemporal.Y = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                posTemporal.Z = BitConverter.ToSingle(bytes);


            }

            fileLoad.Read(bytes);
            int countPath = BitConverter.ToInt32(bytes); 

            for (int i = 0; i < countPath; i++)
            {
                string pointID1;
                string pointID2;
                int longitudName;

                bytes = new byte[16];
                fileLoad.Read(bytes);
                pointID1 = new Guid(bytes).ToString();
                fileLoad.Read(bytes);
                pointID2 = new Guid(bytes).ToString();

                SimulatedObject p1 = SimulatorCore.FindObjectById(pointID1.ToString());
                SimulatedObject p2 = SimulatorCore.FindObjectById(pointID2.ToString());

                Point point1 = SimulatorCore.AsPoint(p1);
                Point point2 = SimulatorCore.AsPoint(p2);

                Path pathTemporal = SimulatorCore.CreatePath(point1, point2);

                fileLoad.Read(bytes);
                pathTemporal.Id = new Guid(bytes).ToString();

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                longitudName = BitConverter.ToInt32(bytes);

                bytes = new byte[longitudName];
                fileLoad.Read(bytes);
                pathTemporal.Name = System.Text.Encoding.UTF8.GetString(bytes);

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                pathTemporal.CapacityPersons = BitConverter.ToInt32(bytes);
            }

            fileLoad.Read(bytes);
            int countFacility = BitConverter.ToInt32(bytes); 

            for (int i = 0; i < countFacility; i++)
            {
                int longitudName;
                int cantidadEntradas;
                int cantidadSalidas;
                List<Point> listaEntradas = new List<Point>();
                List<Point> listaSalidas = new List<Point>();

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                cantidadEntradas = BitConverter.ToInt32(bytes);

                for (int e = 0; e < cantidadEntradas; e++)
                {
                    bytes = new byte[16]; // leemos ID
                    fileLoad.Read(bytes);
                    string idTemporal = new Guid(bytes).ToString();
                    SimulatedObject objetoReferencia = SimulatorCore.FindObjectById(idTemporal);
                    Point puntoTemporal = SimulatorCore.AsPoint(objetoReferencia);
                    listaEntradas.Add(puntoTemporal);
                }

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                cantidadSalidas = BitConverter.ToInt32(bytes);

                for (int s = 0; s < cantidadSalidas; s++)
                {
                    bytes = new byte[16]; 
                    fileLoad.Read(bytes);
                    string idTemporal = new Guid(bytes).ToString();
                    SimulatedObject objetoReferencia = SimulatorCore.FindObjectById(idTemporal);
                    Point puntoTemporal = SimulatorCore.AsPoint(objetoReferencia);
                    listaSalidas.Add(puntoTemporal);
                }

                Facility facilityTemporal = SimulatorCore.CreateFacility(listaEntradas[0], listaSalidas[0]); 

                for (int e = 1; e < listaEntradas.Count; e++)
                {
                    facilityTemporal.Entrances.Add(listaEntradas[e]);
                }
                for (int s = 1; s < listaSalidas.Count; s++)
                {
                    facilityTemporal.Exits.Add(listaSalidas[s]);
                }

                fileLoad.Read(bytes);
                facilityTemporal.Id = new Guid(bytes).ToString();

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                longitudName = BitConverter.ToInt32(bytes);

                bytes = new byte[longitudName];
                fileLoad.Read(bytes);
                facilityTemporal.Name = System.Text.Encoding.UTF8.GetString(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                facilityTemporal.PowerConsumed = BitConverter.ToSingle(bytes); 
            }

            fileLoad.Read(bytes);
            int countPerson = BitConverter.ToInt32(bytes); 

            for (int i = 0; i < countPerson; i++)
            {
                Person personTemporal = SimulatorCore.CreatePerson();

                bytes = new byte[16];
                fileLoad.Read(bytes);
                personTemporal.Id = new Guid(bytes).ToString();

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                int longitudNombre = BitConverter.ToInt32(bytes);

                bytes = new byte[longitudNombre];
                fileLoad.Read(bytes);
                personTemporal.Name = System.Text.Encoding.UTF8.GetString(bytes);

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                personTemporal.Age = BitConverter.ToInt32(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                personTemporal.Height = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                personTemporal.Weight = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                personTemporal.Money = BitConverter.ToInt32(bytes);

                string idFacility;
                bytes = new byte[16];
                fileLoad.Read(bytes);
                Guid facilityGuid = new Guid(bytes);

                if (facilityGuid != Guid.Empty)
                {
                    SimulatedObject objetoReferencia = SimulatorCore.FindObjectById(facilityGuid.ToString());
                    Facility facilityTemporal = SimulatorCore.AsFacility(objetoReferencia);
                    personTemporal.IsAtFacility = facilityTemporal;
                }
                else
                {
                    personTemporal.IsAtFacility = null;
                }

                string idPath;
                bytes = new byte[16];
                fileLoad.Read(bytes);
                Guid pathGuid = new Guid(bytes);

                if (pathGuid != Guid.Empty)
                {
                    SimulatedObject objetoReferencia = SimulatorCore.FindObjectById(pathGuid.ToString());
                    Path pathTemporal = SimulatorCore.AsPath(objetoReferencia);
                    personTemporal.IsAtPath = pathTemporal;
                }
                else
                {
                    personTemporal.IsAtPath = null;
                }
            }

            fileLoad.Close();

        }

    }
}

