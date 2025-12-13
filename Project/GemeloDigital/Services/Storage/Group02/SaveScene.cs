using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal partial class Storage02 : Storage
    {
        internal override void SaveScene(string storageId)
        {
            if (!File.Exists("saves/" + storageId+".sb"))
            {
                Console.WriteLine("No existe la escena, creandola...");
                Thread.Sleep(1000);
            }

            FileStream ficha = new FileStream("saves/"+storageId+".sb", FileMode.Create,FileAccess.Write);

            //puntos
                //sacar puntos
                List<SimulatedObject> points = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);
                //cantidad de puntos
                    bytes = new byte[sizeof(int)];
                    bytes = BitConverter.GetBytes(points.Count);
                    ficha.Write(bytes);
                //puntos
                foreach (SimulatedObject point in points)
                {
                    Point p = SimulatorCore.AsPoint(point);

                    //id
                    bytes = Guid.Parse(p.Id).ToByteArray(); //convertir guid a bytes
                    ficha.Write(bytes);
                    
                    //grandaria nombre
                    bytes = new byte[sizeof(int)];
                    bytes = BitConverter.GetBytes(p.Name.Length);
                    ficha.Write(bytes);
                    //nombre
                    bytes = System.Text.Encoding.UTF8.GetBytes(p.Name);
                    ficha.Write(bytes);

                    //x
                    bytes = BitConverter.GetBytes(p.Position.X);
                    ficha.Write(bytes);
                    //y
                    bytes = BitConverter.GetBytes(p.Position.Y);
                    ficha.Write(bytes);
                    //z
                    bytes = BitConverter.GetBytes(p.Position.Z);
                    ficha.Write(bytes);
                }
            Console.WriteLine("\n");

            //paths
                //sacar paths
                List<SimulatedObject> paths = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Path);
                //cantidad de paths
                bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(paths.Count);
                ficha.Write(bytes);
                //paths
                foreach (SimulatedObject path in paths)
                {
                    Path p = SimulatorCore.AsPath(path);
                    //guid1
                    bytes = Guid.Parse(p.Point1.Id).ToByteArray(); 
                    ficha.Write(bytes);
                    //guid2
                    bytes = Guid.Parse(p.Point2.Id).ToByteArray(); 
                    ficha.Write(bytes);
                    //id
                    bytes = Guid.Parse(p.Id).ToByteArray(); 
                    ficha.Write(bytes);
                    //grandaria nombre
                    bytes = new byte[sizeof(int)];
                    bytes = BitConverter.GetBytes(p.Name.Length);
                    ficha.Write(bytes);
                    //nombre0
                    bytes = System.Text.Encoding.UTF8.GetBytes(p.Name);
                    ficha.Write(bytes);
                    //capacity
                    bytes = new byte[sizeof(int)];
                    bytes = BitConverter.GetBytes(p.CapacityPersons);
                    ficha.Write(bytes);
                }

            //facilities
                //sacar facilities
                List<SimulatedObject> facs = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Facility);
                //cantidad de facs
                bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(facs.Count);
                ficha.Write(bytes);
                //facs
                foreach (SimulatedObject fac in facs)
                {
                    Facility f = SimulatorCore.AsFacility(fac);
                    //cantidad entradas
                    int cantidad = f.Entrances.Count;
                    bytes = BitConverter.GetBytes(cantidad);
                    ficha.Write(bytes);
                    //entradas
                    foreach(Point p in f.Entrances)
                    { 
                        bytes = Guid.Parse(p.Id).ToByteArray();
                        ficha.Write(bytes);
                    }
                    //cantidad salidas
                    cantidad = f.Exits.Count;
                    bytes = BitConverter.GetBytes(cantidad);
                    ficha.Write(bytes);
                    //salidas
                    foreach(Point p in f.Exits)
                    { 
                        bytes = Guid.Parse(p.Id).ToByteArray();
                        ficha.Write(bytes);
                    }
                    //id
                    bytes = Guid.Parse(f.Id).ToByteArray();
                    ficha.Write(bytes);
                    //grandaria nombre
                    bytes = new byte[sizeof(int)];
                    bytes = BitConverter.GetBytes(f.Name.Length);
                    ficha.Write(bytes);
                    //nombre
                    bytes = System.Text.Encoding.UTF8.GetBytes(f.Name);
                    ficha.Write(bytes);
                    //powerconsumed
                    bytes = BitConverter.GetBytes(f.PowerConsumed);
                    ficha.Write(bytes);
                                        
                }

            //personas
                //sacar facilities
                List<SimulatedObject> pers = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Person);
                //cantidad de facs
                bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(pers.Count);
                ficha.Write(bytes);
                //facs
                foreach (SimulatedObject per in pers)
                {
                    Person p = SimulatorCore.AsPerson(per);
                    //id
                    bytes = Guid.Parse(p.Id).ToByteArray();
                    ficha.Write(bytes);
                    //grandaria nombre
                    bytes = new byte[sizeof(int)];
                    bytes = BitConverter.GetBytes(p.Name.Length);
                    ficha.Write(bytes);
                    //nombre
                    bytes = System.Text.Encoding.UTF8.GetBytes(p.Name);
                    ficha.Write(bytes);
                    //age
                    bytes = BitConverter.GetBytes(p.Age);
                    ficha.Write(bytes);
                    //height
                    bytes = BitConverter.GetBytes(p.Height);
                    ficha.Write(bytes);
                    //weight
                    bytes = BitConverter.GetBytes(p.Weight);
                    ficha.Write(bytes);
                    //money
                    bytes = BitConverter.GetBytes(p.Money);
                    ficha.Write(bytes);
                    //id facility
                    if(p.IsAtFacility!=null)//en personas esto puede ser NULL
                    { 
                          bytes = Guid.Parse(p.IsAtFacility.Id).ToByteArray();
                          ficha.Write(bytes);
                    }else ficha.Write(new byte[16]);//escribir espacio de guid vacio
                    //id path
                    if(p.IsAtPath!=null) //aqui igual
                    {
                          bytes = Guid.Parse(p.IsAtPath.Id).ToByteArray();
                          ficha.Write(bytes);
                    }else ficha.Write(new byte[16]);
                }
                
            //guardar ficha
            ficha.Close();
            lista_storages.Add(storageId);//meter ficha a la lista
        }
    }
}
