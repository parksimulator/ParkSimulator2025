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

            if (!File.Exists($"saves/{storageId}"))
            {
                Console.WriteLine($"La escena {storageId} no existe");
                Console.ReadLine();
                return;
            }

            FileStream fileLoad = new FileStream("saves/" + storageId + ".sb", FileMode.Open, FileAccess.Read);  // point - facility-pat-person


            int countObject = 0; // Tamaño del bloque del objeto

            bytes = new byte[sizeof(int)];
            fileLoad.Read(bytes); // se acaba el fichero
            countObject = BitConverter.ToInt32(bytes); // Bloque 1 : points

            for (int i = 0; i < countObject; i++)
            {
                //guid + longitud nombre + nombre + x + y+ z
                // fichaTemporal

                Point pointTemporal = SimulatorCore.CreatePoint(); // en el momento que hago esta variable, es el enlace a meter las cosas en el simulatorCore.
                var posTemporal = pointTemporal.Position;

                bytes = new byte[16];
                pointTemporal.Id = fileLoad.Read(bytes).ToString(); // funsiona siuhhh

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                int sizeName = BitConverter.ToInt32(bytes); // tamaño

                bytes = new byte[sizeName]; // sizeName
                fileLoad.Read(bytes); // name
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
            countObject = BitConverter.ToInt32(bytes); // Bloque 2 : path gui1,guid2, id camino, longitud, nombre, capacity,distancia

            for (int i = 0; i < countObject; i++)
            {
                string pointID1;
                string pointID2;
                int longitudName;

                // leer point1 y leer point2
                bytes = new byte[16];
                fileLoad.Read(bytes);
                pointID1 = System.Text.Encoding.UTF8.GetString(bytes);
                fileLoad.Read(bytes);
                pointID2 = System.Text.Encoding.UTF8.GetString(bytes);

                SimulatedObject p1 = SimulatorCore.FindObjectById(pointID1.ToString());
                SimulatedObject p2 = SimulatorCore.FindObjectById(pointID2.ToString());

                Point point1 = SimulatorCore.AsPoint(p1);
                Point point2 = SimulatorCore.AsPoint(p2);

                Path pathTemporal = SimulatorCore.CreatePath(point1,point2);

                fileLoad.Read(bytes);
                pathTemporal.Id = System.Text.Encoding.UTF8.GetString(bytes);


                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                longitudName = BitConverter.ToInt32(bytes);
               
                bytes = BitConverter.GetBytes(longitudName);
                fileLoad.Read(bytes);
                pathTemporal.Name = System.BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                pathTemporal.CapacityPersons = BitConverter.ToInt32(bytes);

                // distancia


            }

            fileLoad.Read(bytes);
            countObject = BitConverter.ToInt32(bytes); // Bloque 3 : facility // Punto1, Punto 2, CreateFacility, id;longitud, Name, PowerConsumed

            for (int i = 0; i < countObject; i++)
            {
                string pointID1;
                string pointID2;
                int longitudName;

                // leer point1 y leer point2
                bytes = new byte[16];
                fileLoad.Read(bytes);
                pointID1 = System.Text.Encoding.UTF8.GetString(bytes);
                fileLoad.Read(bytes);
                pointID2 = System.Text.Encoding.UTF8.GetString(bytes);

                SimulatedObject p1 = SimulatorCore.FindObjectById(pointID1.ToString());
                SimulatedObject p2 = SimulatorCore.FindObjectById(pointID2.ToString());

                Point point1 = SimulatorCore.AsPoint(p1);
                Point point2 = SimulatorCore.AsPoint(p2);

                Facility facilityTemporal = SimulatorCore.CreateFacility(point1,point2); // CAMBIAR , LEER CANTIDAD DE PUNTOS ENTRADAS/SALIDA, FOR, Y CREAR LISTA DE ENTRADA / SALIDA

                fileLoad.Read(bytes);
                facilityTemporal.Id = System.Text.Encoding.UTF8.GetString(bytes);

                bytes = new byte[sizeof(int)];
                fileLoad.Read(bytes);
                longitudName = BitConverter.ToInt32(bytes);

                bytes = BitConverter.GetBytes(longitudName);
                fileLoad.Read(bytes);
                facilityTemporal.Name = System.BitConverter.ToString(bytes);

                bytes = new byte[sizeof(float)];
                fileLoad.Read(bytes);
                facilityTemporal.PowerConsumed = BitConverter.ToSingle(bytes); // si que hay que cargar, get y set,  si solo pone get solo es lecutra

         

            }

            fileLoad.Read(bytes);
            countObject = BitConverter.ToInt32(bytes); // Bloque 4 : person

            for (int i = 0; i < countObject; i++)
            {
                SimulatorCore.CreatePerson();
            }

            fileLoad.Close();
            // Point tiene posicion X posicion Y y posicion Z, lee 3 floats // Guardar las cosas en la lista de puntos



            // Crear punto 




            // add.Lista(punto) // simulatedObjects.add(point)





            // Facility



        }




        //foreach(var escena in listaEscena)
        //{
        //    Console.WriteLine("Escribe el nombre del fichero"); 
        //    Console.WriteLine($"Escena {escena}");

        //    string fichero = Console.ReadLine();
        //}

        //FileStream fileLoad = new FileStream(fichero, FileMode.Open, FileAccess.Read);
    }
}

