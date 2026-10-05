# IV. REFERENTE TEÓRICO

> *Nota de trabajo (eliminar antes de generar el archivo .docx):* borrador en Markdown pendiente de revisión por Edgar y de la formalización final en Norma APA (7.ª edición). Las fuentes en línea fueron consultadas el 30 de septiembre de 2026.

El presente capítulo establece las bases conceptuales sobre las que se sustenta el desarrollo del sistema SOK (System Optimization Key). Su propósito es definir, con el rigor académico requerido, los fundamentos teóricos y técnicos que permiten comprender el problema plantiado, la solución propuesta y las decisiones de diseño adoptadas a lo largo del trabajo. Cada concepto se presenta de forma objetiva y se relaciona con su aplicación concreta en el proyecto, de modo que el lector pueda comprender la fundamentación técnica de las funcionalidades que conforman el programa.

---

## 4.1 Sistemas operativos Windows 10 y Windows 11

Un sistema operativo es el conjunto de programas que gestiona los recursos de hardware de una computadora y proporciona servicios a los programas de usuario, actuando como intermediario entre el usuario, las aplicaciones y el equipamiento físico (Stallings, 2017). Entre sus funciones esenciales se encuentran la administración del procesador, la gestión de la memoria, el control de los dispositivos de entrada y salida y la provisión de una interfaz de interacción.

Windows 10 y Windows 11 son sistemas operativos de Microsoft construidos sobre la arquitectura Windows NT, la cual separa la ejecución en dos modos: modo usuario y modo kernel. En modo kernel se ejecutan el núcleo (kernel), el ejecutivo (executive), el controlador de capa abstracta de hardware (HAL) y los controladores de dispositivos, que son los componentes con acceso directo al hardware; en modo usuario se ejecutan las aplicaciones y los subsistemas que las apoyan, las cuales deben solicitar los servicios del sistema mediante llamadas controladas (Russinovich et al., 2022). Esta separación es fundamental para la estabilidad y la seguridad del sistema, pues impide que un error en una aplicación afecte directamente el núcleo operativo.

Las laptops de la I.E. N.° 82081 ejecutan Windows 10, versión del sistema operativo cuyo ciclo de soporte estándar concluyó en octubre de 2025, condición que hace especialmente relevante mantener los equipos correctamente configurados y actualizados. Dado que el proyecto se orienta a mejorar el rendimiento percibido por los docentes, el conocimiento de la arquitectura de Windows resulta indispensable para identificar qué componentes del sistema pueden ser configurados de manera segura, cuáles consumen recursos de forma innecesaria y qué acciones de optimización pueden aplicarse sin comprometer la estabilidad del equipo.

## 4.2 La API de Windows (Win32)

La API de Windows, comúnmente denominada API Win32, es el conjunto de funciones, estructuras de datos, constantes y mensajes que el sistema operativo expone para que las aplicaciones soliciten sus servicios (Microsoft, 2026b). Estas funciones se encuentran distribuidas en bibliotecas de enlaces dinámicos (DLL) como `kernel32.dll`, `user32.dll`, `advapi32.dll` y `psapi.dll`, entre otras. Cada DLL agrupa funciones relacionadas con un dominio específico del sistema: `kernel32.dll` gestiona procesos, memoria y archivos; `advapi32.dll` proporciona servicios de seguridad, registro y control de servicios; y `psapi.dll` ofrece funciones para el análisis de procesos y su consumo de memoria (Petzold, 2012).

Las funciones de la API Win32 constituyen el punto de entrada mediante el cual cualquier programa —independientemente del lenguaje en que esté escrito— interactúa con el núcleo de Windows. Operaciones aparentemente sencillas, como consultar la memoria disponible, detener un servicio del sistema o modificar una clave de configuración, se ejecutan invocando estas funciones nativas.

En SOK, la API Win32 representa el nivel de acceso más bajo del sistema. Las funciones `GlobalMemoryStatusEx`, `OpenProcess` y `EmptyWorkingSet` (de `psapi.dll`) son las que permiten implementar el módulo de liberación de memoria, mientras que las funciones relacionadas con el registro y los servicios sustentan los módulos de telemetría y gestión de servicios. Sin este conocimiento, el programa no podría ejecutar ninguna de las operaciones de optimización que lo definen.

## 4.3 El mecanismo P/Invoke

La invocación de código nativo (P/Invoke, por sus siglas en inglés *Platform Invocation Services*) es el mecanismo del framework .NET que permite a los programas escritos en C# llamar a funciones expuestas por bibliotecas dinámicas nativas, es decir, funciones escritas en C o C++ que forman parte del sistema operativo o de terceros (Microsoft, 2026c). El mecanismo se emplea mediante el atributo `DllImport`, con el cual se declara la firma de la función nativa: el nombre de la biblioteca, el nombre de la función, el tipo de convención de llamada y los tipos de los parámetros, incluyendo el proceso de marshaling, que es la conversión de tipos gestionados (.NET) a tipos no gestionados (Win32) y viceversa (Richter, 2012).

P/Invoke resuelve una limitación central de la plataforma administrada: el código C# se ejecuta sobre el Common Language Runtime (CLR) y no tiene acceso directo al sistema operativo, por lo que cualquier operación no cubierta por las bibliotecas estándar de .NET debe solicitarse a las funciones nativas correspondientes.

En el proyecto SOK, el mecanismo P/Invoke es la puerta de entrada al bajo nivel de Windows. La declaración de las firmas nativas se centraliza en la clase `NativeMethods.cs`, dentro de la capa de infraestructura, siguiendo la buena práctica de concentrar todas las invocaciones en un único punto para facilitar su revisión, documentación y prueba. Este diseño permite que el resto del sistema trabaje con abstracciones de alto nivel sin conocer los detalles del intercambio con el sistema operativo.

## 4.4 Windows Registry

El registro de Windows (Windows Registry) es una base de datos jerárquica en la que el sistema operativo y las aplicaciones almacenan configuraciones de hardware, de software y de preferencias del usuario (Microsoft, 2026d). Su estructura se organiza en claves (carpetas) y valores (datos), agrupadas en colmenas (hives) principales: `HKEY_LOCAL_MACHINE` (HKLM) contiene la configuración del equipo, válida para todos los usuarios; `HKEY_CURRENT_USER` (HKCU) almacena las preferencias del usuario autenticado; y `HKEY_CLASSES_ROOT` (HKCR) gestiona las asociaciones de archivos y objetos COM (Russinovich et al., 2022).

El registro es el principal repositorio de configuración de Windows: desde el nivel de telemetría y privacidad hasta el comportamiento de los servicios de inicio, gran parte de lo que el sistema hace "por defecto" está definido en claves del registro. Por ello, modificar claves del registro es una técnica habitual de ajuste y optimización, pero también implica riesgo: una clave mal editada puede impedir el arranque de un servicio o la ejecución de una aplicación.

SOK aplica sobre el registro su regla de oro: toda modificación se precede de un respaldo. Antes de alterar cualquier clave, el programa exporta su valor previo mediante archivos `.reg`, de modo que la operación sea reversible. Esta estrategia transforma una acción potencialmente riesgosa en un procedimiento controlado y auditable, requisito indispensable tratándose de equipos institucionales que soportan actividades educativas.

## 4.5 Windows Management Instrumentation (WMI)

Windows Management Instrumentation (WMI) es la implementación de Microsoft de la infraestructura de gestión basada en el modelo común de información (CIM) para el sistema operativo Windows, y permite consultar y administrar componentes locales o remotos del equipo de forma estructurada (Microsoft, 2026e). WMI organiza la información en clases, cada una de las cuales representa un recurso del sistema —procesadores, discos, servicios, procesos en ejecución, memoria— y dispone de propiedades consultables y métodos invocables.

Las consultas se formulan en lenguaje WQL (WMI Query Language), de sintaxis similar a SQL, o se realizan directamente sobre las clases mediante el espacio de nombres `System.Management` de .NET. Ejemplos de clases frecuentes son `Win32_OperatingSystem`, que describe el sistema operativo en ejecución; `Win32_Processor`, que expone características de la CPU; `Win32_Service`, que informa el estado de los servicios; y `Win32_PerfFormattedData_PerfOS_Memory`, que reporta indicadores de memoria en tiempo real.

Para SOK, WMI es la fuente primaria de información del panel de control (dashboard). El componente `SystemInfoProvider` utiliza esta infraestructura para obtener las especificaciones del equipo y los indicadores de rendimiento que se muestran al docente, ofreciendo así una visión técnica precisa sin requerir conocimientos especializados de su parte. Asimismo, WMI facilita la evaluación diagnóstica inicial de los equipos, aspecto previsto en el área problemática del informe.

## 4.6 Service Control Manager (SCM)

En Windows, un servicio es un programa que se ejecuta en segundo plano, de forma independiente de la sesión del usuario y con capacidad de iniciarse automáticamente con el arranque del sistema. La administración de estos servicios corresponde al Service Control Manager (SCM), componente del ejecutivo de Windows encargado de iniciar, detener, pausar y consultar el estado de los servicios, así como de definir su tipo de inicio (Microsoft, 2026f). Los tipos de inicio habituales son automático, automático (diferido), manual y deshabilitado; y los estados más comunes son en ejecución, detenido y pendiente (Russinovich et al., 2022).

Windows 10 incluye un extenso catálogo de servicios, muchos de los cuales están orientados a funcionalidades que no todos los usuarios aprovechan: telemetría, indexación, impresión, servicios remotos o componentes específicos de plataformas como Xbox. Deshabilitar adecuadamente los servicios que un entorno no utiliza libera memoria y capacidad de procesamiento, siempre que la operación se realice con conocimiento de las dependencias entre servicios.

Este fundamento sustenta el módulo de gestión de servicios de SOK. El programa presenta un catálogo de servicios no esenciales con su estado actual y permite al usuario detenerlos o deshabilitarlos de forma individual o mediante perfiles predefinidos. Antes de cualquier cambio, se respalda el tipo de inicio original del servicio, lo que garantiza la reversibilidad de la operación y el cumplimiento de la política de seguridad del sistema.

## 4.7 Memoria de acceso aleatorio, working set y standby list

La memoria de acceso aleatorio (RAM) es la memoria física donde el sistema operativo y las aplicaciones cargan los datos en uso para su ejecución inmediata, a diferencia de los medios de almacenamiento, cuyo acceso es considerablemente más lento. Dado que la RAM es un recurso finito, Windows administra la memoria mediante un modelo de memoria virtual: cada proceso trabaja sobre un espacio de direcciones propio, y el sistema traduce esas direcciones lógicas a páginas de memoria física, sacando a disco (paginación) los datos que no se usan recientemente (Russinovich et al., 2022).

Dos conceptos resultan centrales para el proyecto. El *working set* (conjunto de trabajo) de un proceso es el conjunto de páginas de memoria física que dicho proceso tiene cargadas en ese momento; un proceso con un working set excesivo presiona la memoria disponible del sistema. La *standby list* (lista de espera) es, por su parte, el conjunto de páginas que fueron utilizadas pero que ya no están activas y que Windows conserva en caché para acelerar su reutilización futura: aunque ocupan espacio, estas páginas pueden liberarse automáticamente cuando otro proceso las requiere (Kokosa, 2019).

La función nativa `EmptyWorkingSet` obliga a un proceso a liberar sus páginas residentes, reduciendo de forma inmediata su consumo de RAM (Microsoft, 2026m), mientras que `GlobalMemoryStatusEx` reporta el porcentaje de memoria en uso a nivel global. SOK emplea estas funciones para implementar la liberación manual de memoria con un clic y la liberación automática cuando el uso supera un umbral configurado, opciones pensadas para los momentos en que los docentes abren múltiples aplicaciones simultáneamente.

## 4.8 Telemetría en sistemas operativos

La telemetría hace referencia a la recolección y transmisión automática de datos sobre el uso de un sistema o servicio con fines de diagnóstico, mejora del producto o análisis estadístico. En el contexto de los sistemas operativos modernos, la telemetría comprende la recopilación de información relativa al rendimiento, los errores, la configuración y los patrones de uso del equipo, la cual se envía al fabricante del sistema (Microsoft, 2026g). Microsoft clasifica los datos de diagnóstico de Windows en niveles de seguridad obligatoria, requerida, opcional y complementaria, y permite administrarlos mediante configuraciones del sistema, políticas de grupo y el registro de Windows.

La telemetría es objeto de debate técnico y normativo: si bien aporta información útil para la corrección de fallos, su activación consume ancho de banda, recursos de procesamiento y espacio en disco, y plantea interrogantes sobre la privacidad de los usuarios. En el ámbito peruano, la protección de los datos personales se rige por la Ley N.° 29733, Ley de Protección de Datos Personales, y su reglamento, normativa que refuerza la pertinencia de ofrecer a los usuarios control sobre la información que sus equipos recopilan y comparten (Congreso de la República del Perú, 2011).

Este fundamento sustenta el primer módulo funcional de SOK. El programa expone un listado de telemetrías de Windows —servicios, claves de registro, identificadores de publicidad y configuraciones de privacidad— con su estado actual, y permite desactivarlas de manera individual o conjunta, respaldando previamente cada configuración. De este modo, se dota al docente de una herramienta de control transparente sobre el comportamiento de su equipo.

## 4.9 Unidades de estado sólido (SSD) y comando TRIM

Las unidades de estado sólido (SSD) almacenan información en chips de memoria flash NAND, carecen de componentes mecánicos móviles y ofrecen tiempos de acceso y velocidades de transferencia superiores a los de los discos duros tradicionales (HDD). No obstante, sus células de almacenamiento admiten un número limitado de ciclos de escritura, por lo que incorporan mecanismos internos de administración como el reparto equitativo de escrituras (*wear leveling*) y la sobraprovisionamiento, orientados a prolongar su vida útil (Microsoft, 2026h).

El comando TRIM es una extensión del protocolo de comandos ATA que permite al sistema operativo informar a la unidad qué bloques de datos ya no se consideran en uso, de modo que el SSD pueda optimizar internamente su administración y mantener su rendimiento con el paso del tiempo. Sin TRIM, la unidad debe realizar operaciones de borrado previo al escribir, lo que degrada progresivamente la velocidad de escritura. Complementariamente, la tecnología SMART permite consultar indicadores de salud y vida estimada de la unidad.

Si bien el módulo de optimización de discos se encuentra diferido como evolución posterior al alcance mínimo viable del proyecto, este fundamento resulta relevante para la justificación técnica de SOK: uno de los problemas reportados por los docentes es la demora en el encendido y la carga de aplicaciones, síntoma directamente asociado al rendimiento del almacenamiento. El referente permite comprender por qué el abordaje del proyecto se concentra primero en la configuración del sistema y la memoria, reservando la optimización de almacenamiento para una etapa posterior.

## 4.10 PowerShell

PowerShell es una línea de comandos y lenguaje de scripting orientado a objetos de Microsoft, diseñado para la administración de sistemas y la automatización de tareas. A diferencia de los intérpretes de texto tradicionales, PowerShell opera sobre objetos .NET y ofrece una estructura de comandos denominada cmdlet, así como un potente mecanismo de canalización (*pipeline*) que permite encadenar la salida de un comando como entrada de otro (Microsoft, 2026i). Desde la versión 7, PowerShell se ejecuta sobre la plataforma .NET moderna, lo que facilita su integración con aplicaciones escritas en C#.

La relevancia de PowerShell en el proyecto es doble. Por un lado, reproduce el modelo mental de las herramientas de referencia que SOK unifica, ya que gran parte de las rutinas de ajuste general que estas ejecutan se expresan naturalmente como scripts de PowerShell. Por otro, la inclusión del SDK de PowerShell dentro del programa permite invocar scripts embebidos desde C#, de manera que las acciones de optimización queden documentadas como scripts revisables y no como operaciones opacas. Esta transparencia es especialmente valiosa en un contexto institucional, donde cada modificación al sistema debe poder ser comprendida y verificada.

## 4.11 Lenguaje C# y plataforma .NET 8

C# es un lenguaje de programación orientado a objetos, de tipado fuerte y estático, desarrollado por Microsoft como parte de la plataforma .NET. El código C# se compila a lenguaje intermedio (IL) y se ejecuta en tiempo de ejecución sobre el Common Language Runtime (CLR), entorno que se encarga de la validación de tipos, la recolección de basura y la compilación justo a tiempo (JIT) al código nativo de la máquina (Troelsen & Japikse, 2022). Esta arquitectura combina la productividad de un lenguaje administrado con un rendimiento próximo al del código nativo.

.NET 8 es la versión de largo soporte (*long-term support*, LTS) de la plataforma, con soporte extendido hasta noviembre de 2026, y ofrece mejoras significativas en rendimiento, seguridad y experiencia de desarrollo respecto de versiones anteriores (Microsoft, 2026j). La plataforma incluye bibliotecas de clases para acceso a datos, redes, archivos, criptografía y, fundamentalmente para este proyecto, espacios de nombres de interoperabilidad con el sistema operativo Windows.

C# constituye el lenguaje con mayor dominio del perfil del egresado, proveniente de experiencias previas en ASP.NET Core, Xamarin.Forms y Unity, y su elección para SOK responde a la necesidad de maximizar las competencias existentes para concentrar el esfuerzo de aprendizaje en las brechas realmente nuevas, como el acceso de bajo nivel a Windows. El programa se estructura sobre .NET 8 mediante una solución de Visual Studio organizada en capas independientes.

## 4.12 Windows Presentation Foundation (WPF)

Windows Presentation Foundation (WPF) es el framework de Microsoft para la creación de aplicaciones de escritorio en Windows. Su característica distintiva es la separación entre la definición de la interfaz, expresada en XAML (Extensible Application Markup Language), un lenguaje de marcado declarativo, y la lógica de la aplicación, implementada en C#. WPF incorpora un sistema de data binding que enlaza automáticamente los elementos de la interfaz con los datos de la aplicación, así como plantillas de control, estilos y recursos que permiten personalizar la apariencia sin alterar la estructura del programa (MacDonald, 2010; Microsoft, 2026k).

Entre sus ventajas se encuentran la composición acelerada por hardware, la granularidad del sistema de layouts y su integración natural con el patrón MVVM, todo lo cual favorece aplicaciones mantenibles y con interfaces ricas.

Para SOK, WPF es la tecnología de presentación elegida. El conocimiento previo de XAML adquirido en Xamarin.Forms durante la formación técnica se transfiere a WPF en gran medida, dado que ambos comparten sintaxis y filosofía declarativa. La interfaz del programa —panel de control con indicadores en tiempo real, listados de telemetrías y servicios con interruptores de estado, medidor de memoria y guía integrada— se materializa en vistas XAML enlazadas a view models, siguiendo la arquitectura descrita en el capítulo de diseño del proyecto.

## 4.13 Patrón arquitectónico MVVM

El patrón MVVM (Model-View-ViewModel) es un patrón arquitectónico de presentación que separa la interfaz de usuario de la lógica de negocio mediante tres componentes: la *View* (vista), encargada de la representación visual y la interacción con el usuario; el *Model*, que representa los datos y las reglas de negocio del dominio; y el *ViewModel*, que actúa como intermediario: expone los datos y los comandos que la vista consume mediante enlaces de datos, y traduce las acciones del usuario en operaciones sobre el modelo (Microsoft, 2026l).

La separación que propone MVVM se apoya en dos mecanismos de WPF: `INotifyPropertyChanged`, que notifica a la vista cuando un dato del view model cambia, e `ICommand`, que permite ejecutar acciones desde la interfaz sin acoplarla a la lógica. El resultado es un código desacoplado, altamente testeable —los view models pueden probarse sin interfaz gráfica— y fácil de mantener. La CommunityToolkit.Mvvm simplifica su implementación mediante atributos generadores de código como `ObservableObject` y `RelayCommand`.

La arquitectura de SOK adopta MVVM de forma transversal: cada módulo del programa se organiza en una vista XAML y su view model correspondiente, y los view models interactúan con los servicios de las capas inferiores a través de interfaces, sin conocer los detalles de implementación. Esta decisión facilita las pruebas unitarias previstas en el plan de trabajo y garantiza que la complejidad del acceso al sistema operativo no contamine la capa de presentación.

## 4.14 SQLite

SQLite es un sistema de gestión de bases de datos relacional embebido, de código abierto y sin servidor, que almacena toda la base de datos en un único archivo en el disco. A diferencia de los SGBD cliente-servidor como MySQL o SQL Server, SQLite se integra directamente en la aplicación, lo que elimina la configuración de un servicio, simplifica la distribución y lo convierte en una opción natural para aplicaciones de escritorio (SQLite, 2026). SQLite implementa transacciones ACID —atomicidad, consistencia, aislamiento y durabilidad— y soporta la mayor parte del estándar SQL.

Su uso se extiende a aplicaciones móviles, navegadores, dispositivos embebidos y programas de escritorio donde se requiere persistencia de datos sin la complejidad de una infraestructura de servidor. En el ecosistema .NET, el acceso a SQLite se realiza habitualmente mediante bibliotecas como Microsoft.Data.Sqlite, frecuentemente combinadas con un mapeador ORM ligero como Dapper.

En SOK, SQLite sustenta el almacenamiento local de tres tipos de información: los perfiles de optimización creados por el usuario, el registro histórico de acciones ejecutadas con su resultado y los metadatos de los respaldos generados. Se trata de datos transaccionales y de escaso volumen, para los que un motor embebido resulta óptimo. Además, la experiencia previa del perfil del egresado con SQLite en el ciclo IV de la formación técnica reduce la curva de aprendizaje de esta componente.

## 4.15 Metodología Scrum

Scrum es un marco de trabajo (*framework*) para el desarrollo incremental de productos complejos, formulado por Ken Schwaber y Jeff Sutherland. Scrum no define un proceso prescriptivo, sino un conjunto de roles, eventos y artefactos que sostienen el trabajo de un equipo autoorganizado en iteraciones cortas denominadas *sprints*, cada una de las cuales produce un incremento potencialmente entregable del producto (Schwaber & Sutherland, 2020).

Los roles definidos son el *Product Owner*, responsable de priorizar el trabajo; el *Scrum Master*, encargado de velar por el correcto funcionamiento del marco; y el equipo de desarrollo. Los eventos son el *Sprint* y sus reuniones de planificación, revisión y retrospectiva, más la reunión diaria (*daily Scrum*). Los artefactos son el *Product Backlog*, lista priorizada de todo lo que requiere el producto; el *Sprint Backlog*, el conjunto de elementos seleccionados para el sprint en curso; y el *Incremento*, la suma de los elementos terminados. Cada elemento se expresa como una historia de usuario, estimada en *story points*, y se considera terminado cuando cumple la Definición de Hecho (*Definition of Done*) establecida por el equipo.

El desarrollo de SOK se organiza en cinco sprints de dos semanas cada uno, con un product backlog de epics y historias de usuario documentado en la sección de construcción del informe. La adopción de Scrum responde a la necesidad de estructurar el trabajo de un desarrollador único, hacer visible el avance mediante entregas incrementales y mantener la trazabilidad entre el plan de trabajo y las secciones del informe TAP.

## 4.16 Lenguaje de Modelado Unificado (UML)

El Lenguaje de Modelado Unificado (UML, *Unified Modeling Language*) es un lenguaje visual de modelado, desarrollado por Grady Booch, James Rumbaugh e Ivar Jacobson y normalizado por el Object Management Group, utilizado para especificar, visualizar, construir y documentar los artefactos de un sistema software (Booch et al., 2005; Object Management Group, 2026). UML no es un método de desarrollo, sino un lenguaje de notación común que puede emplearse con diversas metodologías.

UML 2.x define catorce tipos de diagramas, divididos en diagramas estructurales —que describen la estática del sistema— y de comportamiento —que describen su dinamismo—. Para el diseño de SOK se seleccionan los diagramas de casos de uso, que modelan las interacciones entre los actores (en este caso, el docente usuario) y las funcionalidades del sistema; de clases, que representan las clases del dominio, sus atributos, métodos y relaciones de asociación, herencia y composición; y de secuencia, que ilustran el intercambio de mensajes entre objetos a lo largo del tiempo para una operación específica, como la ejecución de una acción de optimización con respaldo previo.

Estos diagramas constituyen los entregables de la fase de diseño del proyecto y se integran en la sección de análisis y diseño del informe TAP, proporcionando una representación formal del sistema complementaria a su descripción textual.

## 4.17 Normas APA para la formalización académica

Las Normas APA son las directrices de estilo y formato elaboradas por la American Psychological Association para la escritura y presentación de trabajos académicos y científicos, cuya referencia vigente es la séptima edición del *Publication Manual of the American Psychological Association* (American Psychological Association, 2020). El sistema APA define entre otros aspectos la forma de citar las fuentes en el texto —cita narrativa y entre paréntesis con autor y año—, la estructura de la lista de referencias al final del documento, el formato de tablas y figuras, la escritura inclusiva y las normas de presentación del manuscrito.

Su propósito es garantizar la claridad, la consistencia y la trazabilidad de las fuentes, de modo que el lector pueda verificar el origen de cada afirmación. En el presente trabajo, las Normas APA regulan la citación de toda fuente consultada, incluyendo documentación técnica en línea, bibliografía especializada y fuentes institucionales. Conforme a lo planificado, la formalización definitiva de todas las citas y referencias del informe se realizará en la etapa de revisión final, una vez completada la totalidad de las secciones del documento.

---

## Referencias bibliográficas (preliminares)

*Lista preliminar pendiente de depurar y formatear íntegramente en Norma APA 7.ª edición en la etapa de revisión final.*

American Psychological Association. (2020). *Publication manual of the American Psychological Association* (7.ª ed.). https://doi.org/10.1037/0000165-000

Booch, G., Rumbaugh, J., & Jacobson, I. (2005). *The Unified Modeling Language user guide* (2.ª ed.). Addison-Wesley.

Congreso de la República del Perú. (2011). *Ley N.° 29733, Ley de Protección de Datos Personales*.

Kokosa, K. (2019). *Pro .NET memory management: For .NET developers*. Apress.

MacDonald, M. (2010). *Pro WPF in C# 4: Windows Presentation Foundation in .NET 4*. Apress.

Microsoft. (2026b). *Windows API (Win32 y C++)*. https://learn.microsoft.com/en-us/windows/win32/

Microsoft. (2026c). *Platform invoke (P/Invoke)*. https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke

Microsoft. (2026d). *Microsoft Windows registry*. https://learn.microsoft.com/en-us/windows/win32/sysinfo/registry

Microsoft. (2026e). *Windows Management Instrumentation*. https://learn.microsoft.com/en-us/windows/win32/wmisdk/

Microsoft. (2026f). *Services*. https://learn.microsoft.com/en-us/windows/win32/services/

Microsoft. (2026g). *Configure Windows diagnostic data in your organization*. https://learn.microsoft.com/en-us/windows/privacy/configure-windows-diagnostic-data-in-your-organization

Microsoft. (2026h). *Reclaim disk space by using the TRIM command*. https://learn.microsoft.com/en-us/windows-server/storage/file-server/trim-command-ssd

Microsoft. (2026i). *PowerShell overview*. https://learn.microsoft.com/en-us/powershell/scripting/overview

Microsoft. (2026j). *What's new in .NET 8*. https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8

Microsoft. (2026k). *Windows Presentation Foundation for .NET*. https://learn.microsoft.com/en-us/dotnet/desktop/wpf/

Microsoft. (2026l). *MVVM toolkit*. https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/

Microsoft. (2026m). *EmptyWorkingSet function (psapi.h)*. https://learn.microsoft.com/en-us/windows/win32/api/psapi/nf-psapi-emptyworkingset

Object Management Group. (2026). *Unified Modeling Language (UML)*. https://www.omg.org/spec/UML/

Petzold, C. (2012). *Programming Windows* (6.ª ed.). Microsoft Press.

Richter, J. (2012). *CLR via C#* (4.ª ed.). Microsoft Press.

Russinovich, M. E., Solomon, D. A., & Ionescu, A. (2022). *Windows internals part 1: System architecture, processes, threads, security, and memory management* (7.ª ed.). Microsoft Press.

Schwaber, K., & Sutherland, J. (2020). *The Scrum Guide: The official guide to Scrum*. https://scrumguides.org/scrum-guide.html

SQLite. (2026). *SQLite documentation*. https://www.sqlite.org/docs.html

Stallings, W. (2017). *Operating systems: Internals and design principles* (8.ª ed.). Pearson.

Troelsen, A., & Japikse, P. (2022). *Pro C# 10 with .NET 6: Object-oriented, GUI, and .NET 6 programming* (11.ª ed.). Apress.
