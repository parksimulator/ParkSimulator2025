using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    public class SimulatorCore
    {
        /// <summary>
        /// Devuelve la versión del simulador
        /// </summary>
        public static string Version { get { return version; } }

        /// <summary>
        /// Devuelve el estado del simulador
        /// </summary>
        public static SimulatorState State { get { return state; }  }

        /// <summary>
        /// Devuelve los pasos que se han simulado desde el último start
        /// </summary>
        public static int Steps { get { return steps; } }

        /// <summary>
        /// Devuelve el tiempo en horas que se ha simulado desde el último start
        /// </summary>
        public static float Time { get { return steps * Constants.hoursPerStep; } }

        const string version = "6";
        static SimulatorState state;
        static int steps;

        static List<SimulatedObject> simulatedObjects;

        static Storage storage;
        static Render render;
        static Resources resources;

        /// <summary>
        /// Inicia el simulador. Debe llamarse
        /// una vez antes de utilizar cualquier otra
        /// función de esta clase.
        /// </summary>
        public static void Initialize()
        {
            simulatedObjects = new List<SimulatedObject>();

            state = SimulatorState.Stopped;

            storage = new Cargarguardar();
            render = new GLRender();
            resources = new FileResources();

            resources.Initialize();
            storage.Initialize();
            render.Initialize();
        }

        /// <summary>
        /// Arranca una simulación. El estado del simulador
        /// debe ser Stopped
        /// </summary>
        public static void Start()
        {
            steps = 0;

            for(int i = 0; i< simulatedObjects.Count; i++)
            {
                simulatedObjects[i].Start();
            }

            state = SimulatorState.Running;
        }

        /// <summary>
        /// Ejecuta un paso de simulación. El estado del simulador
        /// debe ser Running
        /// </summary>
        public static void Step()
        {

            for (int i = 0; i < simulatedObjects.Count; i++)
            {
                simulatedObjects[i].Step();
            }

            steps++;
        }

        /// <summary>
        /// Detiene la simulación. El estado del simulador debe
        /// ser running
        /// </summary>
        public static void Stop()
        {
            for (int i = 0; i < simulatedObjects.Count; i++)
            {
                simulatedObjects[i].Stop();
            }

            state = SimulatorState.Stopped;
        }


        /// <summary>
        /// Finaliza el simulador. No pueden llamarse a
        /// más funciones de la clase después de ésta.
        /// </summary>
        public static void Finish()
        {
            render.Finish();
            storage.Finish();
            resources.Finish();


            simulatedObjects = null;

        }


        /// <summary>
        /// Crea una persona con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>La persona creada</returns>
        public static Person CreatePerson()
        {
            Person p = new Person();
            simulatedObjects.Add(p);

            return p;
        }

        /// <summary>
        /// Crea un objeto que permite crear personas fácilmente
        /// con unas propiedades aleatorizadas
        /// </summary>
        /// <returns>El generador de personas</returns>
        public static PersonGenerator CreatePersonGeneratorUtility()
        {
            PersonGenerator g = new PersonGenerator();

            return g;
        }

        /// <summary>
        /// Crea una instalación con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>La instalación creada</returns>
        public static Facility CreateFacility(Point entrance, Point exit)
        {
            Facility f = new Facility(entrance, exit);
            simulatedObjects.Add(f);

            return f;
        }

        /// <summary>
        /// Crea un punto con propiedades por defecto y lo
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>El punto creado</returns>
        public static Point CreatePoint()
        {
            Point p = new Point();
            simulatedObjects.Add(p);

            return p;
        }

        /// <summary>
        /// Crea un camino entre los dos puntos, que deben ser diferentes
        /// </summary>
        /// <param name="p1">Primer punto que conecta el camino</param>
        /// <param name="p2">Segundo punto que conecta el camino</param>
        /// <returns>El camino creado</returns>
        public static Path CreatePath(Point p1, Point p2)
        {
            Path p = new Path(p1, p2);
            simulatedObjects.Add(p);

            return p;
        }

        /// <summary>
        /// Crea una cámara con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>La cámara creada</returns>
        public static Camera CreateCamera()
        {
            Camera c = new Camera();
            simulatedObjects.Add(c);

            return c;
        }

        /// <summary>
        /// Crea una textura con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>La textura creada</returns>
        public static Texture CreateTexture()
        {
            Texture t = new Texture();
            simulatedObjects.Add(t);

            return t;
        }

        /// <summary>
        /// Crea un material con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>El material creado</returns>
        public static Material CreateMaterial()
        {
            Material m = new Material();
            simulatedObjects.Add(m);

            return m;
        }

        /// <summary>
        /// Crea un modelo con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>El modelo creado</returns>
        public static Model CreateModel()
        {
            Model m = new Model();
            simulatedObjects.Add(m);

            return m;
        }

        /// <summary>
        /// Crea una malla con propiedades por defecto y la 
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>La malla creada</returns>
        public static Mesh CreateMesh()
        {
            Mesh m = new Mesh();
            simulatedObjects.Add(m);

            return m;
        }

        /// <summary>
        /// Crea un shader con propiedades por defecto y lo
        /// añade al listado de objetos simulados
        /// </summary>
        /// <returns>El shader creado</returns>
        public static Shader CreateShader()
        {
            Shader s = new Shader();
            simulatedObjects.Add(s);

            return s;
        }

        /// <summary>
        /// Elimina un objeto del listado de objetos simulados
        /// </summary>
        /// <param name="obj">El objeto a eliminar</param>
        public static void DeleteObject(SimulatedObject obj)
        {
            simulatedObjects.Remove(obj);
        }

        /// <summary>
        /// Devuelve el listado de objetos simulados
        /// </summary>
        /// <returns>El listado de objetos simulados</returns>
        public static List<SimulatedObject> GetObjects()
        {
            return simulatedObjects;
        }


        /// <summary>
        /// Cuenta los objetos de un cierto tipo que existen en la lista
        /// de objetos simulados
        /// </summary>
        /// <param name="type">Tipo de objeto a contar</param>
        /// <returns>Cantidad de objetos encontrados</returns>
        public static int CountObjectsOfType(SimulatedObjectType type)
        {
            int count = 0;

            for(int i = 0; i < simulatedObjects.Count; i++)
            {
                SimulatedObject o = simulatedObjects[i];
                if(o.Type == type || type == SimulatedObjectType.Any) { count ++; }
            }

            return count;
        }

        /// <summary>
        /// Busca los objetos de un cierto tipo y los devuelve en una lista
        /// </summary>
        /// <param name="type">Tipo de objetos a recopilar</param>
        /// <returns>Objetos del tipo encontrados</returns>
        public static List<SimulatedObject> FindObjectsOfType(SimulatedObjectType type)
        {
            List<SimulatedObject> objects = new List<SimulatedObject>();
            
            for(int i = 0; i < simulatedObjects.Count; i++)
            {
                SimulatedObject o = simulatedObjects[i];
                if(o.Type == type || type == SimulatedObjectType.Any) { objects.Add(o); }
            }

            return objects;
        }

        /// <summary>
        /// Convierte el objeto en una persona.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en persona</returns>
        public static Person AsPerson(SimulatedObject obj)
        {
            return (Person)obj;
        }

        /// <summary>
        /// Convierte el objeto en una instalación.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en instalación</returns>
        public static Facility AsFacility(SimulatedObject obj)
        {
            return (Facility)obj;
        }

        /// <summary>
        /// Convierte el objeto en un punto.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en punto</returns>
        public static Point AsPoint(SimulatedObject obj)
        {
            return (Point)obj;
        }

        /// <summary>
        /// Convierte el objeto en un camino.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en un camino</returns>
        public static Path AsPath(SimulatedObject obj)
        {
            return (Path)obj;
        }

        /// <summary>
        /// Convierte el objeto en una cámara.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en una camara</returns>
        public static Camera AsCamera(SimulatedObject obj)
        {
            return (Camera)obj;
        }

        /// <summary>
        /// Convierte el objeto en una textura.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en una textura</returns>
        public static Texture AsTexture(SimulatedObject obj)
        {
            return (Texture)obj;
        }

        /// <summary>
        /// Convierte el objeto en un material.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en un material</returns>
        public static Material AsMaterial(SimulatedObject obj)
        {
            return (Material)obj;
        }

        /// <summary>
        /// Convierte el objeto en un modelo.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en un modelo</returns>
        public static Model AsModel(SimulatedObject obj)
        {
            return (Model)obj;
        }

        /// <summary>
        /// Convierte el objeto en una malla.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en una malla</returns>
        public static Mesh AsMesh(SimulatedObject obj)
        {
            return (Mesh)obj;
        }

        /// <summary>
        /// Convierte el objeto en un shader.
        /// </summary>
        /// <param name="obj">El objeto a convertir</param>
        /// <returns>El objeto convertido en un shader</returns>
        public static Shader AsShader(SimulatedObject obj)
        {
            return (Shader)obj;
        }


        /// <summary>
        /// Devuelve el valor de un KPI general
        /// </summary>
        /// <param name="kpi">Nombre del KPI</param>
        /// <returns>Valor del KPI</returns>
        public static float GetGeneralKPI(string kpi)
        {
            float total = 0;

            for(int i = 0; i < simulatedObjects.Count; i++)
            {
                total += simulatedObjects[i].GetKPI(kpi);
            }

            return total;
        }

        /// <summary>
        /// Devuelve el valor de un KPI de un objeto
        /// </summary>
        /// <param name="obj">El objeto</param>
        /// <param name="kpi">Nombre del KPI</param>
        /// <returns></returns>
        public static float GetObjectKPI(SimulatedObject obj, string kpi)
        {
            return obj.GetKPI(kpi);
        }

        /// <summary>
        /// Inicia la grabación de un KPI general
        /// </summary>
        /// <param name="kpi"></param>
        public static void StartGeneralKPIRecording(string kpi)
        {
            //...
        }

        /// <summary>
        /// Detiene la grabación de un KPI general
        /// </summary>
        /// <param name="kpi"></param>
        public static void StopGeneralKPIRecording(string kpi)
        {
            //...
        }

        /// <summary>
        /// Inicia la grabación de un KPI de objeto.
        /// </summary>
        /// <param name="simObj">El objeto</param>
        /// <param name="kpi">El KPI</param>
        public static void StartObjectKPIRecording(SimulatedObject simObj, string kpi)
        {
            simObj.StartKPIRecording(kpi);
        }

        /// <summary>
        /// Detiene la grabación de un KPI de objeto.
        /// </summary>
        /// <param name="simObj"></param>
        /// <param name="kpi"></param>
        public static void StopObjectKPIRecording(SimulatedObject simObj, string kpi)
        {
            simObj.StopKPIRecording(kpi);
        }

        /// <summary>
        /// Deja la simulación en su estado inicial.
        /// La simulación debe estar parada para poder llamar
        /// a esta función
        /// </summary>
        public static void NewScene()
        {
            simulatedObjects.Clear();
        }

        /// <summary>
        /// Carga una simulación del storage
        /// La simulación debe estar parada para poder llamar
        /// a esta función
        /// </summary>
        public static void LoadScene(string storageId)
        {
            simulatedObjects.Clear();

            storage.LoadScene(storageId);
        }

        /// <summary>
        /// Guarda una simulación en el storage
        /// La simulación debe estar parada para poder llamar
        /// a esta función
        /// </summary>
        public static void SaveScene(string storageId)
        {
            storage.SaveScene(storageId);
        }

        /// <summary>
        /// Devuelve la lista de simulaciones almacenadas
        /// </summary>
        /// <returns></returns>
        public static List<string> ListScenes()
        {
            return storage.ListScenes();
        }

        /// <summary>
        /// Elimina una simulación almacenada
        /// </summary>
        public static void DeleteScene(string storageId)
        {
            storage.DeleteScene(storageId);
        }

        internal static SimulatedObject? FindObjectById(string id)
        {
            SimulatedObject? result = null;
            int i = 0;

            while(result == null && i < simulatedObjects.Count)
            {
                SimulatedObject o = simulatedObjects[i];
                if(o.Id == id) { result = o; } 
                else { i++; }
            }

            return result;
        }

        internal static Point CreatePointWithId(string id)
        {
            Point p = CreatePoint();
            p.Id = id;

            return p;
        }

        internal static Path CreatePathWithId(string id, Point point1, Point point2)
        {
            Path p = CreatePath(point1, point2);
            p.Id = id;

            return p;
        }

        internal static Facility CreateFacilityWithId(string id, Point entrance, Point exit)
        {
            Facility f = CreateFacility(entrance, exit);
            f.Id = id;

            return f;
        }

        internal static Person CreatePersonWithId(string id)
        {
            Person p = CreatePerson();
            p.Id = id;

            return p;
        }
        
    }
}
