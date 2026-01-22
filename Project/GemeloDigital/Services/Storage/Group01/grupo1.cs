using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace GemeloDigital
{
    /*
     Una clase abstracta NO se puede usar directamente

    Cuando una clase es abstracta, significa:

    No puedes crear objetos de esa clase.

    Solo sirve como modelo / plantilla para que otras clases la hereden.
     
     */
    internal class FileStorage : Storage
    {
        string folderName = "Scenes";//guarda el nombre de la carpeta donde se van a guardar los ficheros

        internal override void Initialize()//hace arrancar el simulador, override sobreescribe la versión concreta de este método que viene definido en la clase padre
                                           //internal → solo se puede usar dentro del mismo proyecto
        {
            if (!Directory.Exists(folderName))//comprueba si la carpeta existe y si no la crea
            {                                       //Es necesario para que al guardar no falle por “ruta inexistente”.(NO ENTIENDO ESA PARTE)
                Directory.CreateDirectory(folderName);
            }
        }

        internal override void Finish()//se llama cuando el simulador termina
        {
            //Aquí no necesitamos cerrar nada porque usamos FileStream solo dentro de cada operación y lo cerramos al final.
            // No necesitamos hacer nada
        }

        //devuelve al programa la lista de escenas guardadas
        internal override List<string> ListScenes()
        {
            List<string> list = new List<string>();

            if (!Directory.Exists(folderName))//Si la carpeta Scenes no existe, devuelve la lista vacía.
            {
                return list;
            }

            string[] files = Directory.GetFiles(folderName, "*.bin");//Busca todos los ficheros que terminen en .bin dentro de Scenes.
            /*
             Para cada fichero:

            fullPath = ruta completa (Scenes/Escena1.bin).

            Path.GetFileNameWithoutExtension(fullPath) → "Escena1".

            Lo añade a la lista.
             */
            for (int i = 0; i < files.Length; i++)//este bucle guarda la ruta completa de cada file en fullPath
            {
                string fullPath = files[i];//fullPath=ruta completa
                                           //.Path sirve para trabajar con rutas de archivos
                string name = System.IO.Path.GetFileNameWithoutExtension(fullPath);//transforma--> Scenes/Escena1.bin en -->Escena1"
                list.Add(name);//Añade "Escena1" a la lista de escenas que el menú mostrará al usuario
            }

            return list;
        }

        internal override void DeleteScene(string storageId)//borrar el fichero de una escena concreta.
        {
            string filePath = System.IO.Path.Combine(folderName, storageId + ".bin");//storageId es el nombre de la escena (por ejemplo "Escena1"---- Construye la ruta → Scenes/Escena1.bin.

            if (File.Exists(filePath))//Si existe, lo borra.
            {
                File.Delete(filePath);
            }
        }//se quita la extencion para mostrar nombres limpios de las escenas

        // =======================
        //  GUARDAR ESCENA
        // =======================
        internal override void SaveScene(string storageId)
        {
            if (!Directory.Exists(folderName))//Crea el nombre del fichero (Scenes/<nombre>.bin).
            {
                Directory.CreateDirectory(folderName);
            }

            string filePath = System.IO.Path.Combine(folderName, storageId + ".bin");

            // Obtenemos los objetos de la escena
            List<SimulatedObject> objects = SimulatorCore.GetObjects();

            // ordenamos para que los Point vayan primero
            List<SimulatedObject> orderedObjects = new List<SimulatedObject>();

            // Points primero
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is Point)
                {
                    orderedObjects.Add(objects[i]);
                }
            }

            // Facilities
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is Facility)
                {
                    orderedObjects.Add(objects[i]);
                }
            }

            // Paths
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is Path)
                {
                    orderedObjects.Add(objects[i]);
                }
            }

            // Persons
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is Person)
                {
                    orderedObjects.Add(objects[i]);
                }
            }

            FileStream fichero = new FileStream(filePath, FileMode.Create, FileAccess.Write);// crea el fichero que va a guardar la escena

            // Datos de cada objeto
            for (int i = 0; i < orderedObjects.Count; i++)//escribe cada objeto
            {
                SimulatedObject obj = orderedObjects[i]; //Coge el objeto número i de la lista orderedObjects y guárdalen una variable llamada obj de tipo SimulatedObject

                // Tipo (int)
                int typeInt = (int)obj.Type;//que tipo es?
                byte[] bytesType = BitConverter.GetBytes(typeInt);
                fichero.Write(bytesType);

                // Id (string: longitud + bytes) 
                string id = obj.Id;//Coge el Id del objeto (obj) de la escena
                if (id == null) id = "";//por seguridad
                byte[] idBytes = Encoding.UTF8.GetBytes(id);//Convierte el texto id a bytes, usando codificación UTF-8
                int idLength = idBytes.Length;//Guarda cuántos bytes tiene el id
                byte[] idLengthBytes = BitConverter.GetBytes(idLength);//Convierte el número idLength (que es un int) a bytes
                fichero.Write(idLengthBytes);
                fichero.Write(idBytes);

                // 4.3) Name (string: longitud + bytes)
                string name = obj.Name;
                if (name == null) name = "";
                byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                int nameLength = nameBytes.Length;
                byte[] nameLengthBytes = BitConverter.GetBytes(nameLength);
                fichero.Write(nameLengthBytes);
                fichero.Write(nameBytes);

                // 4.4) Datos extra según el tipo
                if (obj is Point)
                {
                    Point p = (Point)obj;

                    // Position.X
                    byte[] bx = BitConverter.GetBytes(p.Position.X);
                    fichero.Write(bx);

                    // Position.Y
                    byte[] by = BitConverter.GetBytes(p.Position.Y);
                    fichero.Write(by);

                    // Position.Z
                    byte[] bz = BitConverter.GetBytes(p.Position.Z);
                    fichero.Write(bz);
                }
                else if (obj is Person)
                {
                    Person per = (Person)obj;

                    // Age (int)
                    byte[] bAge = BitConverter.GetBytes(per.Age);
                    fichero.Write(bAge);

                    // Height (float)
                    byte[] bH = BitConverter.GetBytes(per.Height);
                    fichero.Write(bH);

                    // Weight (float)
                    byte[] bW = BitConverter.GetBytes(per.Weight);
                    fichero.Write(bW);

                    // Money (float)
                    byte[] bM = BitConverter.GetBytes(per.Money);
                    fichero.Write(bM);
                }
                else if (obj is Facility)
                {
                    Facility fac = (Facility)obj;

                    // Guardamos solo 1 entrada y 1 salida (versión sencilla)
                    string entranceId = "";
                    if (fac.Entrances != null && fac.Entrances.Count > 0 && fac.Entrances[0] != null && fac.Entrances[0].Id != null)
                    {
                        entranceId = fac.Entrances[0].Id;
                    }

                    string exitId = "";
                    if (fac.Exits != null && fac.Exits.Count > 0 && fac.Exits[0] != null && fac.Exits[0].Id != null)
                    {
                        exitId = fac.Exits[0].Id;
                    }

                    // entranceId
                    byte[] eBytes = Encoding.UTF8.GetBytes(entranceId);
                    int eLen = eBytes.Length;
                    byte[] eLenBytes = BitConverter.GetBytes(eLen);
                    fichero.Write(eLenBytes);
                    fichero.Write(eBytes);

                    // exitId
                    byte[] xBytes = Encoding.UTF8.GetBytes(exitId);
                    int xLen = xBytes.Length;
                    byte[] xLenBytes = BitConverter.GetBytes(xLen);
                    fichero.Write(xLenBytes);
                    fichero.Write(xBytes);

                    // PowerConsumed
                    byte[] powerBytes = BitConverter.GetBytes(fac.PowerConsumed);
                    fichero.Write(powerBytes);
                }
                else if (obj is Path)
                {
                    Path path = (Path)obj;//guarda el camino con sus puntos de inicio y fin

                    string p1Id = "";
                    if (path.Point1 != null && path.Point1.Id != null)
                    {
                        p1Id = path.Point1.Id;
                    }

                    string p2Id = "";
                    if (path.Point2 != null && path.Point2.Id != null)
                    {
                        p2Id = path.Point2.Id;
                    }

                    // p1Id
                    byte[] p1Bytes = Encoding.UTF8.GetBytes(p1Id);
                    int p1Len = p1Bytes.Length;
                    byte[] p1LenBytes = BitConverter.GetBytes(p1Len);
                    fichero.Write(p1LenBytes);
                    fichero.Write(p1Bytes);

                    // p2Id
                    byte[] p2Bytes = Encoding.UTF8.GetBytes(p2Id);
                    int p2Len = p2Bytes.Length;
                    byte[] p2LenBytes = BitConverter.GetBytes(p2Len);
                    fichero.Write(p2LenBytes);
                    fichero.Write(p2Bytes);
                }
            }

            fichero.Close();
        }

        // =======================
        //  CARGAR ESCENA
        // =======================
        internal override void LoadScene(string storageId)//Leer la escena del fichero
        {
            string filePath = System.IO.Path.Combine(folderName, storageId + ".bin");

            if (!File.Exists(filePath))
            {
                // si no existe, no hacemos nada
                return;
            }

            FileStream fichero = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Para reconstruir relaciones, guardamos los objetos por Id
            Dictionary<string, SimulatedObject> byId = new Dictionary<string, SimulatedObject>();//Guarda los objetos a medida que los creamos

            byte[] typeBytes = new byte[sizeof(int)];
            int leidos = fichero.Read(typeBytes);

            //Leer todos los objetos
            while (leidos > 0)
            {
                int typeInt = BitConverter.ToInt32(typeBytes);
                SimulatedObjectType type = (SimulatedObjectType)typeInt;

                //Id (string: longitud + bytes)
                byte[] idLenBytes = new byte[sizeof(int)];
                fichero.Read(idLenBytes);
                int idLen = BitConverter.ToInt32(idLenBytes);

                byte[] idBytes = new byte[idLen];
                if (idLen > 0)
                {
                    fichero.Read(idBytes);
                }
                string id = Encoding.UTF8.GetString(idBytes);

                // Name (string: longitud + bytes)
                byte[] nameLenBytes = new byte[sizeof(int)];
                fichero.Read(nameLenBytes);
                int nameLen = BitConverter.ToInt32(nameLenBytes);

                byte[] nameBytes = new byte[nameLen];
                if (nameLen > 0)
                {
                    fichero.Read(nameBytes);
                }
                string name = Encoding.UTF8.GetString(nameBytes);

                SimulatedObject obj = null;

                // Datos extra según el tipo
                if (type == SimulatedObjectType.Point)
                {
                    // X
                    byte[] bx = new byte[sizeof(float)];
                    fichero.Read(bx);
                    float x = BitConverter.ToSingle(bx);

                    // Y
                    byte[] byBytes = new byte[sizeof(float)];
                    fichero.Read(byBytes);
                    float y = BitConverter.ToSingle(byBytes);

                    // Z
                    byte[] bz = new byte[sizeof(float)];
                    fichero.Read(bz);
                    float z = BitConverter.ToSingle(bz);

                    Point p = SimulatorCore.CreatePoint();
                    p.Position = new Vector3(x, y, z);
                    obj = p;
                }
                else if (type == SimulatedObjectType.Person)
                {
                    // Age
                    byte[] bAge = new byte[sizeof(int)];
                    fichero.Read(bAge);
                    int age = BitConverter.ToInt32(bAge);

                    // Height
                    byte[] bH = new byte[sizeof(float)];
                    fichero.Read(bH);
                    float height = BitConverter.ToSingle(bH);

                    // Weight
                    byte[] bW = new byte[sizeof(float)];
                    fichero.Read(bW);
                    float weight = BitConverter.ToSingle(bW);

                    // Money
                    byte[] bM = new byte[sizeof(float)];
                    fichero.Read(bM);
                    float money = BitConverter.ToSingle(bM);

                    Person per = SimulatorCore.CreatePerson();
                    per.Age = age;
                    per.Height = height;
                    per.Weight = weight;
                    per.Money = money;

                    obj = per;
                }
                else if (type == SimulatedObjectType.Facility)
                {
                    // entranceId
                    byte[] eLenBytes = new byte[sizeof(int)];
                    fichero.Read(eLenBytes);
                    int eLen = BitConverter.ToInt32(eLenBytes);

                    byte[] eBytes = new byte[eLen];
                    if (eLen > 0)
                    {
                        fichero.Read(eBytes);
                    }
                    string entranceId = Encoding.UTF8.GetString(eBytes);

                    // exitId
                    byte[] xLenBytes = new byte[sizeof(int)];
                    fichero.Read(xLenBytes);
                    int xLen = BitConverter.ToInt32(xLenBytes);

                    byte[] xBytes = new byte[xLen];
                    if (xLen > 0)
                    {
                        fichero.Read(xBytes);
                    }
                    string exitId = Encoding.UTF8.GetString(xBytes);

                    // PowerConsumed
                    byte[] powerBytes = new byte[sizeof(float)];
                    fichero.Read(powerBytes);
                    float power = BitConverter.ToSingle(powerBytes);

                    Point entrancePoint = null;
                    Point exitPoint = null;

                    if (entranceId != "" && byId.ContainsKey(entranceId))
                    {
                        entrancePoint = (Point)byId[entranceId];
                    }
                    if (exitId != "" && byId.ContainsKey(exitId))
                    {
                        exitPoint = (Point)byId[exitId];
                    }

                    if (entrancePoint == null)
                    {
                        entrancePoint = SimulatorCore.CreatePoint();
                    }
                    if (exitPoint == null)
                    {
                        exitPoint = SimulatorCore.CreatePoint();
                    }

                    Facility fac = SimulatorCore.CreateFacility(entrancePoint, exitPoint);
                    fac.PowerConsumed = power;

                    obj = fac;
                }
                else if (type == SimulatedObjectType.Path)
                {
                    // p1Id
                    byte[] p1LenBytes = new byte[sizeof(int)];
                    fichero.Read(p1LenBytes);
                    int p1Len = BitConverter.ToInt32(p1LenBytes);

                    byte[] p1Bytes = new byte[p1Len];
                    if (p1Len > 0)
                    {
                        fichero.Read(p1Bytes);
                    }
                    string p1Id = Encoding.UTF8.GetString(p1Bytes);

                    // p2Id
                    byte[] p2LenBytes = new byte[sizeof(int)];
                    fichero.Read(p2LenBytes);
                    int p2Len = BitConverter.ToInt32(p2LenBytes);

                    byte[] p2Bytes = new byte[p2Len];
                    if (p2Len > 0)
                    {
                        fichero.Read(p2Bytes);
                    }
                    string p2Id = Encoding.UTF8.GetString(p2Bytes);

                    Point p1 = null;
                    Point p2 = null;

                    if (p1Id != "" && byId.ContainsKey(p1Id))
                    {
                        p1 = (Point)byId[p1Id];
                    }
                    if (p2Id != "" && byId.ContainsKey(p2Id))
                    {
                        p2 = (Point)byId[p2Id];
                    }

                    if (p1 == null)
                    {
                        p1 = SimulatorCore.CreatePoint();
                    }
                    if (p2 == null)
                    {
                        p2 = SimulatorCore.CreatePoint();
                    }

                    Path path = SimulatorCore.CreatePath(p1, p2);
                    obj = path;
                }
                else
                {
                    // Tipo desconocido
                    obj = null;
                }

                // Asignar Id y Name y guardarlo en el diccionario
                if (obj != null)
                {
                    obj.Id = id;
                    obj.Name = name;

                    if (!byId.ContainsKey(id))
                    {
                        byId.Add(id, obj);
                    }
                }

                typeBytes = new byte[sizeof(int)];
                leidos = fichero.Read(typeBytes);
            }

            fichero.Close();
        }
    }
}
