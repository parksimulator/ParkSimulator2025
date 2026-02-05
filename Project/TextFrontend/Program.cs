using System.Numerics;


namespace GemeloDigital
{
    internal partial class Program
    {
        const int messageWaitTime = 2000;
        const string tab = "  ";

        static void Main(string[] args)
        {
            SimulatorCore.Initialize();

            int option = -1;
            int menu = 0;

            SimulatedObject selectedObject = null;
            List<string> monitorizedGeneralKPIs = new List<string>();
            List<SimulatedObject> monitorizedObjects = new List<SimulatedObject>();
            List<string> monitorizedObjectKPIs = new List<string>();

            bool showObjectPropertiesInList = false;


            while(menu != 0 || option != 0)
            {
                Console.WriteLine(".--------------------------------------.");
                Console.WriteLine("|                                      |");
                Console.WriteLine("|            PARK SIMULATOR            |");
                Console.WriteLine("|                                      |");
                Console.WriteLine(".--------------------------------------.");
                Console.WriteLine();

                Console.WriteLine("-------- Simulación  ---------");
                Console.WriteLine();
                Console.WriteLine(tab + tab + "Estado: " + SimulatorCore.State);
                Console.WriteLine(tab + tab + "Pasos : " + SimulatorCore.Steps);
                Console.WriteLine(tab + tab + "Tiempo(h): " + SimulatorCore.Time);
                Console.WriteLine();


                List<SimulatedObject> objects = SimulatorCore.GetObjects();

                Console.WriteLine("-------- Escena (" + objects.Count + " objetos)  ---------");
                Console.WriteLine();
                for(int i = 0; i < objects.Count; i++)
                {
                    SimulatedObject o = objects[i];

                    Console.WriteLine(tab + i + ": " + ObjectToString(o, !showObjectPropertiesInList));
                }
                Console.WriteLine();
                Console.WriteLine("-------  Objeto seleccionado   --------");
                Console.WriteLine();

                if(selectedObject == null) { Console.WriteLine(tab + "Ningún objeto seleccionado"); }
                else { PrintObject(selectedObject); }

                Console.WriteLine();
                Console.WriteLine("-------  KPIS   --------");
                Console.WriteLine();
                if(monitorizedGeneralKPIs.Count == 0 && monitorizedObjectKPIs.Count == 0)
                {
                    Console.WriteLine(tab + tab + "No se está monitorizando ningún KPI");
                }
                else
                {
                    for(int i = 0; i < monitorizedGeneralKPIs.Count; i++)
                    {
                        string kpi = monitorizedGeneralKPIs[i];
                        Console.WriteLine(tab + tab + kpi + ": " + SimulatorCore.GetGeneralKPI(kpi));
                    }                
                    Console.WriteLine();
                    for(int i = 0; i < monitorizedObjectKPIs.Count; i++)
                    {
                        string kpi = monitorizedObjectKPIs[i];
                        SimulatedObject obj = monitorizedObjects[i];
                        Console.WriteLine(tab + tab + obj.Name + ": "+ kpi + ": " + SimulatorCore.GetObjectKPI(obj, kpi));
                    }                
                }
                Console.WriteLine();
                if(menu == 0)
                {
                    Console.WriteLine("-------------- Menú principal --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Escena");
                    Console.WriteLine(tab + tab + "2.- Objetos");
                    Console.WriteLine(tab + tab + "3.- Simulación");
                    Console.WriteLine(tab + tab + "4.- KPIs");
                    Console.WriteLine(tab + tab + "5.- Opciones");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Salir");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 5);

                    if(option == 1) { menu = 1; }
                    else if(option == 2) { menu = 2; }
                    else if(option == 3) { menu = 3; }
                    else if(option == 4) { menu = 4; }
                    else if(option == 5) { menu = 5; }

                }
                else if(menu == 1)
                {
                    Console.WriteLine("-------------- Escena --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Nueva escena");
                    Console.WriteLine(tab + tab + "2.- Cargar escena.");
                    Console.WriteLine(tab + tab + "3.- Guardar escena.");
                    Console.WriteLine(tab + tab + "4.- Eliminar escena.");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Atrás");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 4);

                    if(option == 1)
                    {
                        SimulatorCore.NewScene();
                        selectedObject = null;
                    }
                    else if(option == 2)
                    {
                        string? scene = PickScene();

                        if(scene != null)
                        {
                            SimulatorCore.LoadScene(scene);
                            selectedObject = null;
                        }
                    }
                    else if(option == 3)
                    {
                        string scene = AskString("Nombre de escena");
                        SimulatorCore.SaveScene(scene);
                    }
                    else if(option == 4)
                    {
                        string scene = PickScene();
                        SimulatorCore.DeleteScene(scene);
                    }
                    else
                    {
                        menu = 0;
                        option = -1;
                    }
                }
                else if(menu == 2)
                {
                    Console.WriteLine("-------------- Objetos --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Seleccionar");
                    Console.WriteLine(tab + tab + "2.- Crear.");
                    Console.WriteLine(tab + tab + "3.- Modificar.");
                    Console.WriteLine(tab + tab + "4.- Eliminar.");
                    Console.WriteLine(tab + tab + "5.- Generador de personas");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Atrás");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 5);


                    if(option == 1)
                    {
                        if(SimulatorCore.CountObjectsOfType(SimulatedObjectType.Any) <= 0)
                        {
                            Console.WriteLine("No hay objetos para seleccionar en la escena");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            selectedObject = PickObject("Objeto", "Listado", SimulatedObjectType.Any);
                        }
                    }
                    else if(option == 2) { menu = 22; }
                    else if(option == 3)
                    {
                        if(selectedObject == null)
                        {
                            Console.WriteLine("Selecciona un objeto primero.");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            selectedObject.Name = AskString("Nombre");

                            if(selectedObject.Type == SimulatedObjectType.Person)
                            {
                                Person p = SimulatorCore.AsPerson(selectedObject);
                                AskPersonProperties(p);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Facility)
                            {
                                Facility f = SimulatorCore.AsFacility(selectedObject);
                                AskFacilityProperties(f, false);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Point)
                            {
                                Point p = SimulatorCore.AsPoint(selectedObject);
                                AskPointProperties(p);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Path)
                            {
                                Path p = SimulatorCore.AsPath(selectedObject);
                                AskPathProperties(p);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Camera3D)
                            {
                                Camera3D c = SimulatorCore.AsCamera3D(selectedObject);
                                AskCamera3DProperties(c);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Object3D)
                            {
                                Object3D o = SimulatorCore.AsObject3D(selectedObject);
                                AskObject3DProperties(o);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Model)
                            {
                                Model m = SimulatorCore.AsModel(selectedObject);
                                AskModelProperties(m);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Texture)
                            {
                                Texture t = SimulatorCore.AsTexture(selectedObject);
                                AskTextureProperties(t);
                            }
                            else if(selectedObject.Type == SimulatedObjectType.Shader)
                            {
                                Shader s = SimulatorCore.AsShader(selectedObject);
                                AskShaderProperties(s);
                            }

                        }
                    }
                    else if(option == 4)
                    {
                        if(selectedObject == null)
                        {
                            Console.WriteLine("Selecciona un objeto primero");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            SimulatorCore.DeleteObject(selectedObject);
                            selectedObject = null;
                            menu = 0;
                        }

                    }
                    else if(option == 5)
                    {
                        PersonGenerator generator = SimulatorCore.CreatePersonGeneratorUtility();

                        int amount = AskIntegerBetween("Cantidad", 10, 1000);

                        Path path = null;
                        Facility facility = null;
                        if(SimulatorCore.CountObjectsOfType(SimulatedObjectType.Point) > 0)
                        {
                            SimulatedObject obj = PickObjectOrNull("Ubicar a todos en camino", "Camino", SimulatedObjectType.Path);
                            path = SimulatorCore.AsPath(obj);
                        }

                        if(path == null && SimulatorCore.CountObjectsOfType(SimulatedObjectType.Facility) > 0)
                        {
                            SimulatedObject obj = PickObjectOrNull("Ubicar a todos en instalación", "Instalaciones", SimulatedObjectType.Facility);
                            facility = SimulatorCore.AsFacility(obj);
                        }

                        for(int i = 0; i < amount; i ++)
                        {
                            Person p = generator.GeneratePerson();
                            p.IsAtFacility = facility;
                            p.IsAtPath = path;
                        }
                        
                    }
                    else
                    {
                        menu = 0;
                        option = -1;
                    }

                }
                else if(menu == 22)
                {
                    Console.WriteLine("-------------- Crear objeto --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Persona");
                    Console.WriteLine(tab + tab + "2.- Instalación");
                    Console.WriteLine(tab + tab + "3.- Punto");
                    Console.WriteLine(tab + tab + "4.- Camino");
                    Console.WriteLine(tab + tab + "5.- Cámara3D");
                    Console.WriteLine(tab + tab + "6.- Objeto3D");
                    Console.WriteLine(tab + tab + "7.- Material");
                    Console.WriteLine(tab + tab + "8.- Modelo");
                    Console.WriteLine(tab + tab + "9.- Shader");
                    Console.WriteLine(tab + tab + "10.- Textura");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Atrás");
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 10);

                    if(option == 1)
                    {
                        Person p = SimulatorCore.CreatePerson();

                        AskPersonProperties(p);
                    }
                    else if(option == 2)
                    {
                        if(SimulatorCore.CountObjectsOfType(SimulatedObjectType.Point) < 2)
                        {
                            Console.WriteLine("Se necesitan al menos dos puntos para poder crear una instalación");
                            Thread.Sleep(messageWaitTime);

                        }
                        else
                        {
                            SimulatedObject o1 = PickObject("Entrada", "Puntos", SimulatedObjectType.Point);
                            SimulatedObject o2 = PickObject("Salida", "Puntos", SimulatedObjectType.Point);

                            Point p1 = SimulatorCore.AsPoint(o1);
                            Point p2 = SimulatorCore.AsPoint(o2); 

                            Facility f = SimulatorCore.CreateFacility(p1, p2);

                            f.Name = AskString("Nombre");

                            AskFacilityProperties(f, true);

                            menu = 0;

                        }


                    }
                    else if(option == 3)
                    {
                        Point p = SimulatorCore.CreatePoint();
                        p.Name = AskString("Nombre");
                        
                        AskPointProperties(p);

                        menu = 0;

                    }
                    else if(option == 4)
                    {
                        List<SimulatedObject> points = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);

                        if(points.Count > 1)
                        {
                            bool done = false;

                            Point p1 = null;
                            Point p2 = null;

                            while(!done)
                            {
                                SimulatedObject o1 = PickObject("Punto 1", "Puntos", SimulatedObjectType.Point);
                                SimulatedObject o2 = PickObject("Punto 2", "Puntos", SimulatedObjectType.Point);

                                p1 = SimulatorCore.AsPoint(o1);
                                p2 = SimulatorCore.AsPoint(o2);

                                if(p1 != p2) {  done = true; }
                                else { Console.WriteLine("Los puntos deben ser distintos"); }
                            }

                            Path path = SimulatorCore.CreatePath(p1, p2);
                            path.Name = AskString("Nombre");                            
                        
                        }
                        else
                        {
                            Console.WriteLine("No hay al menos dos puntos que unir en un camino");
                            Thread.Sleep(messageWaitTime);
                        }
                    }
                    else if(option == 5)
                    {
                        List<SimulatedObject> cameras = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Camera3D);

                        if(cameras.Count > 0)
                        {
                            Console.WriteLine("No puede existir más de una cámara en la escena");
                        }
                        else
                        {
                            Camera3D c = SimulatorCore.CreateCamera3D();
                            c.Name = AskString("Nombre");

                            AskCamera3DProperties(c);
                            
                        }
                    }
                    else if(option == 6)
                    {
                        Object3D m = SimulatorCore.CreateObject3D();
                        m.Name = AskString("Nombre");

                        AskObject3DProperties(m);

                    }
                    else if(option == 7)
                    {
                        Material m = SimulatorCore.CreateMaterial();
                        m.Name = AskString("Nombre");

                        AskMaterialProperties(m);

                    }
                    else if(option == 8)
                    {
                        Model m = SimulatorCore.CreateModel();
                        m.Name = AskString("Nombre");

                        AskModelProperties(m);

                    }
                    else if(option == 9)
                    {
                        Shader s = SimulatorCore.CreateShader();
                        s.Name = AskString("Nombre");

                        AskShaderProperties(s);

                    }
                    else if(option == 10)
                    {
                        Texture t = SimulatorCore.CreateTexture();
                        t.Name = AskString("Nombre");

                        AskTextureProperties(t);

                    }
                    else if(option == 0)
                    {
                        menu = 2;
                    }
                }
                else if(menu == 3)
                {
                    Console.WriteLine("-------------- Simulación --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Arrancar");
                    Console.WriteLine(tab + tab + "2.- Paso.");
                    Console.WriteLine(tab + tab + "3.- Parar.");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Atrás.");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 3);

                    if(option == 1)
                    {
                        if(SimulatorCore.State != SimulatorState.Stopped)
                        {
                            Console.WriteLine("La simulación ya está corriendo");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            SimulatorCore.Start();
                        }
                    }
                    else if(option == 2)
                    {
                        if(SimulatorCore.State != SimulatorState.Running)
                        {
                            Console.WriteLine("La simulación debe estar corriendo");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            SimulatorCore.Step();
                        }
                    }
                    else if(option == 3)
                    {
                        if(SimulatorCore.State != SimulatorState.Running)
                        {
                            Console.WriteLine("La simulación debe estar corriendo");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            SimulatorCore.Stop();
                        }
                    }
                    else
                    {
                        menu = 0;
                        option = -1;

                    }
                }
                else if(menu == 4)
                {
                    Console.WriteLine("-------------- KPIs --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Monitorizar KPI general");
                    Console.WriteLine(tab + tab + "2.- Monitorizar KPI de objeto.");
                    Console.WriteLine(tab + tab + "3.- Dejar de monitorizar KPI general.");
                    Console.WriteLine(tab + tab + "4.- Dejar de monitorizar KPI de objeto.");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Atrás.");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 4);

                    if(option == 1)
                    {
                        string kpi = AskString("KPI");

                        if(monitorizedGeneralKPIs.IndexOf(kpi) >= 0)
                        {
                            Console.WriteLine("El KPI general ya está siendo monitorizado");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            monitorizedGeneralKPIs.Add(kpi);
                        }

                    }
                    else if(option == 2)
                    {
                        if(SimulatorCore.CountObjectsOfType(SimulatedObjectType.Any) <= 0)
                        {
                            Console.WriteLine("No hay objetos a los que pedir un KPI");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            SimulatedObject obj = PickObject("Objeto", "Objeto", SimulatedObjectType.Any);

                            string kpi = AskString("KPI");

                            bool found = false;
                            for(int i = 0; i < monitorizedObjects.Count; i ++)
                            {
                                if(monitorizedObjects[i] == obj && monitorizedObjectKPIs[i] == kpi)
                                {
                                    found = true;
                                }
                            }

                            if(found)
                            {
                                Console.WriteLine("El KPI de objeto ya está siendo monitorizado");
                                Thread.Sleep(messageWaitTime);
                            }
                            else
                            {
                                monitorizedObjects.Add(obj);
                                monitorizedObjectKPIs.Add(kpi);
                            }

                        }
                    }
                    else if(option == 3)
                    {
                        string kpi = AskString("KPI");

                        if(monitorizedGeneralKPIs.IndexOf(kpi) < 0)
                        {
                            Console.WriteLine("El KPI general no está siendo monitorizado");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            monitorizedGeneralKPIs.Remove(kpi);
                        }

                    }
                    else if(option == 4)
                    {
                        SimulatedObject obj = PickObject("Objeto", "Objeto", SimulatedObjectType.Any);

                        string kpi = AskString("KPI");

                        bool found = false;
                        int foundIndex = -1;
                        for(int i = 0; i < monitorizedObjects.Count; i ++)
                        {
                            if(monitorizedObjects[i] == obj && monitorizedObjectKPIs[i] == kpi)
                            {
                                found = true;
                                foundIndex = i;
                            }
                        }

                        if(!found)
                        {
                            Console.WriteLine("El KPI de objeto no está siendo monitorizado");
                            Thread.Sleep(messageWaitTime);
                        }
                        else
                        {
                            monitorizedObjects.RemoveAt(foundIndex);
                            monitorizedObjectKPIs.RemoveAt(foundIndex);
                        }
                    }
                    else
                    {
                        menu = 0;
                        option = -1;
                    }
                }
                else if(menu == 5)
                {
                    Console.WriteLine("-------------- Opciones --------------");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "1.- Mostrar propiedades en el listado de objetos");
                    Console.WriteLine(tab + tab + "2.- Ocultar propiedades en el listado de objetos");
                    Console.WriteLine();
                    Console.WriteLine(tab + tab + "0.- Atrás.");
                    Console.WriteLine();

                    option = AskIntegerBetween("Opción", 0, 2);

                    if(option == 1) { showObjectPropertiesInList = true; menu = 0; option = -1; }
                    else if(option == 2) { showObjectPropertiesInList = false; menu = 0; option = -1; }
                    else { menu = 0; option = -1; }
                }

                Console.Clear();


            }

            SimulatorCore.Finish();

        }


    }
}
