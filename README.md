# Sistema de Gestión Académica

¡Bienvenido a mi portafolio! Este es un sistema robusto de control escolar desarrollado en **C#** y **SQL Server**, estructurado bajo una **Arquitectura en Capas** para garantizar la escalabilidad, mantenibilidad y la separación limpia de responsabilidades.

El proyecto pone un foco crítico en las **buenas prácticas de desarrollo** y la **ciberseguridad corporativa**, implementando mecanismos avanzados para la protección de datos según los estándares modernos de la industria.

---

## Características Clave

### Módulo de Seguridad Avanzada (Autenticación y Control)
* **Criptografía Avanzada (Hash + Salt):** Las contraseñas no se almacenan en texto plano. Se implementó un algoritmo de hash criptográfico **SHA-256** combinado con un **Salt aleatorio único por usuario** generado por proveedores criptográficos nativos.
* **Mitigación de Ataques de Fuerza Bruta:** Control estricto de intentos fallidos. Al registrarse **3 intentos erróneos**, la cuenta se bloquea de forma automática y temporal por **15 minutos**, registrando la marca de tiempo exacta en la base de datos.
* **Mensajes de Error Genéricos:** Diseño de interfaz defensivo. Ante un fallo, el sistema devuelve un mensaje genérico ("Usuario o contraseña incorrectos") para mitigar ataques de enumeración de usuarios.
* **Protección contra SQL Injection:** Toda la persistencia de datos (Capa DAL) utiliza **consultas 100% parametrizadas** mediante ADO.NET, eliminando cualquier vulnerabilidad por concatenación de strings.

### Control de Accesos y Menús Dinámicos
* **Seguridad Basada en Roles (RBAC):** Sistema preparado para la gestión de permisos según el rol del usuario (Administrador, Docente, Secretaria).
* **UI Dinámica:** Los menús de la interfaz gráfica (WinForms) se habilitan, deshabilitan o se ocultan dinámicamente en tiempo de ejecución evaluando el rol del usuario autenticado en la sesión activa.

---

## Arquitectura del Software
El sistema está desacoplado de forma estricta en los siguientes proyectos:

1.  **Capa de Presentación (UI):** Interfaces de usuario construidas en Windows Forms, encargadas exclusivamente de la captura de datos y validaciones de formato en controles.
2.  **Capa de Negocio (BLL):** Contiene la lógica core del sistema, las reglas de negocio académicas y los motores de encriptación/validación de bloqueos.
3.  **Capa de Datos (DAL):** Encargada de la comunicación directa con SQL Server utilizando ADO.NET puro, optimizando el rendimiento de las transacciones.
4.  **Capa de Entidades:** Clases POCO que representan el modelo del dominio y sirven para el transporte seguro de objetos entre capas.

---

## Tecnologías Utilizadas
* **Lenguaje:** C# (.NET Framework / .NET Core)
* **Base de Datos:** SQL Server 2019 o superior
* **Arquitectura:** N-Tier Architecture (En Capas)
* **Criptografía:** `System.Security.Cryptography` (SHA-256)
* **Control de Versiones:** Git & GitHub