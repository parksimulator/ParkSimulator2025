# Documentación del Sistema BinaryFileStorage

## Descripción General

`BinaryFileStorage` es un sistema de almacenamiento binario diseñado para persistir y cargar escenas de simulación. Gestiona archivos binarios para diferentes tipos de objetos simulados: **Points**, **Facilities**, **Paths** y **Persons**.

---

## Arquitectura del Sistema

```mermaid
flowchart TD
    A[BinaryFileStorage] --> B[Lista de Escenas en Memoria]
    
    A --> C[Archivo Maestro: scenes.dat]
    
    A --> D[Archivos por Escena]
    
    D --> E[escenaX_points.dat]
    D --> F[escenaX_facilities.dat]
    D --> G[escenaX_paths.dat]
    D --> H[escenaX_persons.dat]
    
    style A fill:#4CAF50,color:#fff
    style B fill:#2196F3,color:#fff
    style C fill:#FF9800,color:#fff
    style D fill:#9C27B0,color:#fff
    style E fill:#FFE082
    style F fill:#81C784
    style G fill:#64B5F6
    style H fill:#F06292
```

## Estructura de Archivos

### 1. scenes.dat
Archivo maestro que contiene la lista de todas las escenas guardadas.

**Formato:**
```
[int: tamaño_id][string: scene_id]...
```

### 2. [escenaId]points.dat
Almacena información de los puntos de la escena.

**Formato por punto:**
```
[int: tamaño_id]
[string: point_id]
[int: tamaño_nombre]
[string: nombre]
[float: posición_X]
[float: posición_Y]
[float: posición_Z]
```

### 3. [escenaId]facilities.dat
Almacena información de las instalaciones.

**Formato por facility:**
```
[int: tamaño_id]
[string: facility_id]
[int: tamaño_nombre]
[string: nombre]
[float: power_consumed]
[int: num_entradas]
  └─ Por cada entrada:
     [int: tamaño_id_punto]
     [string: punto_id]
[int: num_salidas]
  └─ Por cada salida:
     [int: tamaño_id_punto]
     [string: punto_id]
```

### 4. [escenaId]paths.dat
Almacena información de los caminos entre puntos.

**Formato por path:**
```
[int: tamaño_id]
[string: path_id]
[int: tamaño_nombre]
[string: nombre]
[int: capacity_persons]
[int: tamaño_id_punto1]
[string: point1_id]
[int: tamaño_id_punto2]
[string: point2_id]
```

### 5. [escenaId]persons.dat
Almacena información de las personas en la simulación.

**Formato por persona:**
```
[int: tamaño_id]
[string: person_id]
[int: tamaño_nombre]
[string: nombre]
[int: edad]
[float: altura]
[float: peso]
[float: dinero]
[int: tamaño_id_facility]
[string: facility_id | "null"]
[int: tamaño_id_path]
[string: path_id | "null"]
```

## Flujo de Operaciones

### Inicialización del Sistema

```mermaid
flowchart TD
    A[Initialize] --> B{¿Existe scenes.dat?}
    B -->|No| C[Crear archivo vacío]
    B -->|Sí| D[Abrir archivo]
    C --> E[Cerrar archivo]
    D --> F[Leer tamaño del primer ID]
    F --> G{¿Hay datos?}
    G -->|Sí| H[Leer ID de escena]
    H --> I[Añadir a lista en memoria]
    I --> F
    G -->|No| J[Cerrar archivo]
    E --> K[Sistema inicializado]
    J --> K
```

### Guardar Escena

```mermaid
flowchart TD
    A[SaveScene] --> B[SavePointsFile]
    B --> C[SaveFacilitiesFile]
    C --> D[SavePathsFile]
    D --> E[SavePersonsFile]
    E --> F[SaveSceneIdFile]
    
    F --> G[Actualizar lista en memoria]
    G --> H[Abrir scenes.dat en modo Append]
    H --> I[Escribir ID de escena]
    I --> J[Cerrar archivo]
    J --> K[Escena guardada]
    
    style A fill:#4CAF50
    style K fill:#4CAF50
```

### Cargar Escena

```mermaid
flowchart TD
    A[LoadScene] --> B[LoadPointsFile]
    B --> C[LoadFacilitiesFile]
    C --> D[LoadPathsFile]
    D --> E[LoadPersonsFile]
    
    B --> B1[Crear Points en SimulatorCore]
    C --> C1[Buscar Points creados]
    C1 --> C2[Crear Facilities vinculadas]
    D --> D1[Buscar Points creados]
    D1 --> D2[Crear Paths vinculados]
    E --> E1[Buscar Facilities y Paths]
    E1 --> E2[Crear Persons vinculadas]
    
    E2 --> F[Escena cargada en memoria]
    
    style A fill:#2196F3
    style F fill:#2196F3
```

### Eliminar Escena

```mermaid
flowchart TD
    A[DeleteScene] --> B[Eliminar de lista en memoria]
    B --> C[Reescribir scenes.dat completo]
    C --> D[Eliminar storageId]
    D --> E[Eliminar storageIdpoints.dat]
    E --> F[Eliminar storageIdpersons.dat]
    F --> G[Eliminar storageidfacilities.dat]
    G --> H[Eliminar storageIdpaths.dat]
    H --> I[Escena eliminada]
    
    style A fill:#f44336
    style I fill:#f44336
```

## Dependencias entre Entidades

```mermaid
graph LR
    P[Points] --> F[Facilities]
    P --> PT[Paths]
    F --> PS[Persons]
    PT --> PS
    
    style P fill:#FFE082
    style F fill:#81C784
    style PT fill:#64B5F6
    style PS fill:#F06292
```

**Orden de carga:** Points → Facilities → Paths → Persons

**Orden de guardado:** Points → Facilities → Paths → Persons → SceneId

## Métodos Principales

### Initialize()
Inicializa el sistema de almacenamiento. Crea `scenes.dat` si no existe y carga todos los IDs de escenas en memoria.

### SaveScene(string storageId)
Guarda una escena completa creando/sobrescribiendo los archivos binarios correspondientes.

**Pasos:**
1. Guarda todos los Points
2. Guarda todas las Facilities
3. Guarda todos los Paths
4. Guarda todas las Persons
5. Registra el ID de la escena en scenes.dat

### LoadScene(string storageId)
Carga una escena completa desde los archivos binarios.

**Pasos:**
1. Carga Points (crea objetos Point)
2. Carga Facilities (requiere Points existentes)
3. Carga Paths (requiere Points existentes)
4. Carga Persons (requiere Facilities y Paths existentes)

### DeleteScene(string storageId)
Elimina una escena del sistema.

**Pasos:**
1. Elimina el ID de la lista en memoria
2. Reescribe scenes.dat sin el ID eliminado
3. Elimina todos los archivos asociados a la escena

### ListScenes()
Retorna la lista de IDs de todas las escenas guardadas.

## Consideraciones Importantes

### ⚠️ Manejo de Valores Nulos
Las referencias a Facilities y Paths en Persons pueden ser nulas. El sistema guarda la cadena `"null"` en UTF-8 cuando el valor es null.

### 🔗 Referencias entre Objetos
- **Facilities** referencian Points (entradas y salidas)
- **Paths** referencian dos Points (point1 y point2)
- **Persons** pueden referenciar una Facility y/o un Path

### 📝 Limitación Actual
Actualmente, el sistema solo soporta **una entrada y una salida** por Facility, aunque el formato de archivo permite múltiples entradas/salidas.

### 🔄 Orden de Carga Crítico
El orden de carga es fundamental debido a las dependencias. Los Points deben cargarse primero porque Facilities, Paths y Persons los referencian.

## Ejemplo de Uso

```csharp
// Inicializar el sistema
BinaryFileStorage storage = new BinaryFileStorage();
storage.Initialize();

// Guardar una escena
storage.SaveScene("escena01");

// Listar todas las escenas
List<string> scenes = storage.ListScenes();

// Cargar una escena
storage.LoadScene("escena01");

// Eliminar una escena
storage.DeleteScene("escena01");

// Finalizar
storage.Finish();
```

## Formato Binario General

Todos los archivos siguen un patrón similar:
1. **Tamaño del dato** (int de 4 bytes)
2. **Dato en sí** (string en UTF-8, int, o float)

Este patrón permite lectura secuencial y determinística de los datos.