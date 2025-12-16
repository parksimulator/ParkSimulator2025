//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Numerics;
//using System.Text;

//namespace GemeloDigital
//{
//    internal class Cargarguardar : Storage
//    {
//        List<string> list = new List<string>();

//        internal override void Initialize()
//        {
//            list = new List<string>();
//            string currentFolder = Directory.GetCurrentDirectory();
//            string[] files = Directory.GetFiles(currentFolder, "*.dat");

//            foreach (string file in files)
//            {
//                string cleanName = System.IO.Path.GetFileNameWithoutExtension(file);
//                list.Add(cleanName);
//            }

//            Console.WriteLine("Sistema de guardado inicializado." + list.Count + "escenas encontradas.");
//        }

//        internal override void Finish()
//        {
//            Console.WriteLine("DummyStorage: Finish");
//        }

        
//        private string GetCorrectFilename(string storageId)
//        {
//            if (storageId.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
//            {
//                return storageId;
//            }
//            return storageId + ".dat";
//        }

        

//        internal override void SaveScene(string storageId)
//        {
//            Console.WriteLine("Grupo 6: Save simulation " + storageId);
//            string filename = GetCorrectFilename(storageId);
//            Console.WriteLine("Guardando archivo en: " + System.IO.Path.GetFullPath(filename));

//            FileStream fichero = new FileStream(filename, FileMode.Create, FileAccess.Write);
//            byte[] bytes;

//            List<SimulatedObject> allObjects = SimulatorCore.GetObjects();
//            List<Point> points = new List<Point>();
//            List<Facility> facilities = new List<Facility>();
//            List<Path> paths = new List<Path>();
//            List<Person> persons = new List<Person>();

//            foreach (var obj in allObjects)
//            {
//                if (obj.Type == SimulatedObjectType.Point) points.Add(SimulatorCore.AsPoint(obj));
//                else if (obj.Type == SimulatedObjectType.Facility) facilities.Add(SimulatorCore.AsFacility(obj));
//                else if (obj.Type == SimulatedObjectType.Path) paths.Add(SimulatorCore.AsPath(obj));
//                else if (obj.Type == SimulatedObjectType.Person) persons.Add(SimulatorCore.AsPerson(obj));
//            }

//            // Guardar Points
//            bytes = BitConverter.GetBytes(points.Count); fichero.Write(bytes, 0, bytes.Length);
//            foreach (var po in points)
//            {
//                byte[] idBytes = Encoding.UTF8.GetBytes(po.Id);
//                fichero.Write(BitConverter.GetBytes(idBytes.Length), 0, sizeof(int));
//                fichero.Write(idBytes, 0, idBytes.Length);
//                byte[] nameBytes = Encoding.UTF8.GetBytes(po.Name);
//                fichero.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
//                fichero.Write(nameBytes, 0, nameBytes.Length);
//                fichero.Write(BitConverter.GetBytes(po.Position.X), 0, sizeof(float));
//                fichero.Write(BitConverter.GetBytes(po.Position.Y), 0, sizeof(float));
//                fichero.Write(BitConverter.GetBytes(po.Position.Z), 0, sizeof(float));
//            }
//            // Guardar Facilities
//            bytes = BitConverter.GetBytes(facilities.Count);
//            fichero.Write(bytes, 0, bytes.Length);
//            foreach (var f in facilities)
//            {
//                byte[] idBytes = Encoding.UTF8.GetBytes(f.Id);
//                fichero.Write(BitConverter.GetBytes(idBytes.Length), 0, sizeof(int));
//                fichero.Write(idBytes, 0, idBytes.Length);
//                byte[] nameBytes = Encoding.UTF8.GetBytes(f.Name);
//                fichero.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
//                fichero.Write(nameBytes, 0, nameBytes.Length);
//                fichero.Write(BitConverter.GetBytes(f.PowerConsumed), 0, sizeof(float));
//                fichero.Write(BitConverter.GetBytes(f.Entrances.Count), 0, sizeof(int));
//                foreach (var ent in f.Entrances)
//                {
//                    byte[] pid = Encoding.UTF8.GetBytes(ent.Id);
//                    fichero.Write(BitConverter.GetBytes(pid.Length), 0, sizeof(int));
//                    fichero.Write(pid, 0, pid.Length);
//                }
//                fichero.Write(BitConverter.GetBytes(f.Exits.Count), 0, sizeof(int));
//                foreach (var ext in f.Exits)
//                {
//                    byte[] pid = Encoding.UTF8.GetBytes(ext.Id);
//                    fichero.Write(BitConverter.GetBytes(pid.Length), 0, sizeof(int));
//                    fichero.Write(pid, 0, pid.Length);
//                }
//            }
//            // Guardar Paths
//            bytes = BitConverter.GetBytes(paths.Count);
//            fichero.Write(bytes, 0, bytes.Length);
//            foreach (var pa in paths)
//            {
//                byte[] idBytes = Encoding.UTF8.GetBytes(pa.Id);
//                fichero.Write(BitConverter.GetBytes(idBytes.Length), 0, sizeof(int));
//                fichero.Write(idBytes, 0, idBytes.Length);
//                byte[] nameBytes = Encoding.UTF8.GetBytes(pa.Name);
//                fichero.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
//                fichero.Write(nameBytes, 0, nameBytes.Length);
//                byte[] p1 = Encoding.UTF8.GetBytes(pa.Point1.Id);
//                fichero.Write(BitConverter.GetBytes(p1.Length), 0, sizeof(int));
//                fichero.Write(p1, 0, p1.Length);
//                byte[] p2 = Encoding.UTF8.GetBytes(pa.Point2.Id);
//                fichero.Write(BitConverter.GetBytes(p2.Length), 0, sizeof(int));
//                fichero.Write(p2, 0, p2.Length);
//                fichero.Write(BitConverter.GetBytes(pa.CapacityPersons), 0, sizeof(int));
//            }
//            // Guardar Persons
//            bytes = BitConverter.GetBytes(persons.Count);
//            fichero.Write(bytes, 0, bytes.Length);
//            foreach (var p in persons)
//            {
//                byte[] idBytes = Encoding.UTF8.GetBytes(p.Id);
//                fichero.Write(BitConverter.GetBytes(idBytes.Length), 0, sizeof(int));
//                fichero.Write(idBytes, 0, idBytes.Length);
//                byte[] nameBytes = Encoding.UTF8.GetBytes(p.Name);
//                fichero.Write(BitConverter.GetBytes(nameBytes.Length), 0, sizeof(int));
//                fichero.Write(nameBytes, 0, nameBytes.Length);
//                fichero.Write(BitConverter.GetBytes(p.Age), 0, sizeof(int));
//                fichero.Write(BitConverter.GetBytes(p.Height), 0, sizeof(float));
//                fichero.Write(BitConverter.GetBytes(p.Weight), 0, sizeof(float));
//                fichero.Write(BitConverter.GetBytes(p.Money), 0, sizeof(float));
//                string fid = (p.IsAtFacility != null) ? p.IsAtFacility.Id : ""; byte[] fBytes = Encoding.UTF8.GetBytes(fid);
//                fichero.Write(BitConverter.GetBytes(fBytes.Length), 0, sizeof(int));
//                if (fBytes.Length > 0) fichero.Write(fBytes, 0, fBytes.Length);
//                string pid = (p.IsAtPath != null) ? p.IsAtPath.Id : ""; byte[] pBytes = Encoding.UTF8.GetBytes(pid);
//                fichero.Write(BitConverter.GetBytes(pBytes.Length), 0, sizeof(int));
//                if (pBytes.Length > 0) fichero.Write(pBytes, 0, pBytes.Length);
//            }

//            fichero.Close();

//            // Actualizamos la lista
//            string cleanName = storageId;
//            if (storageId.EndsWith(".dat")) cleanName = storageId.Substring(0, storageId.Length - 4);

//            if (!list.Contains(cleanName))
//            {
//                list.Add(cleanName);
//            }

//            Console.WriteLine("Escena guardada correctamente");
//        }

//        internal override void DeleteScene(string storageId)
//        {
//            Console.WriteLine("Deleting simulation " + storageId);
//            list.Remove(storageId);
//            string filename = GetCorrectFilename(storageId);
//            if (File.Exists(filename))
//            {
//                File.Delete(filename);
//            }
//        }

//        internal override List<string> ListScenes()
//        {
//            return list;
//        }
//    }
//}