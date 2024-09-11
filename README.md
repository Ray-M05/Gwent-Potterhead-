# Gwent: Potterhead Edition
Autora: Rachel Mojena González

En **Gwent: Potterhead Edition**, los jugadores se adentran en una intensa confrontación por el dominio de Hogwarts, la prestigiosa escuela de magia y hechicería. Este juego coloca a los participantes en el rol de líderes emblemáticos de dos de las casas más influyentes de Hogwarts: **Gryffindor** y **Slytherin**.

Enfrentando las fuerzas del bien y del mal en una contienda decisiva, cada jugador dispone de un mazo de cartas que encapsulan personajes memorables, criaturas mágicas y hechizos poderosos del universo de Harry Potter. Estas cartas se clasifican en diversas categorías, cada una con habilidades y estrategias distintivas que pueden alterar el curso del juego de manera significativa.

Desarrollado con la tecnología **.NET 7.0** y utilizando **Unity** como interfaz gráfica, **Gwent: Potterhead Edition** integra la solidez del lenguaje **C#** con las capacidades visuales avanzadas de Unity, ofreciendo una experiencia de juego refinada y visualmente atractiva.


## Instrucciones del Juego:
Este informe proporciona una guía completa sobre las mecánicas del juego y cómo interactuar con los elementos dentro de **Gwent: Potterhead Edition**.

### 1. Inicio del Juego

Al iniciar el juego, se presenta un botón que redirige al jugador al menú principal. En este menú, el jugador selecciona el mazo con el que desea jugar. Recientemente, se ha añadido una nueva pestaña en este menú que permite a los jugadores agregar cartas o efectos personalizados a su mazo, brindando una mayor personalización y estrategia al juego.

En esta pantalla, los jugadores también deben ingresar sus nombres antes de proceder. Una vez que ambos jugadores han elegido sus mazos, la partida comienza.

### 2. Comienzo de la Partida

Al inicio de la partida, cada jugador roba 10 cartas de su mazo. Los jugadores tienen la opción de descartar hasta dos cartas y robar nuevas en su lugar, o bien pueden optar por mantener las cartas que recibieron inicialmente.

### 3. Desarrollo del Turno

Cada turno en **Gwent: Harry Potter Edition** se compone de las siguientes fases:

- **Jugar una carta**: El jugador puede jugar una carta haciendo clic en ella y luego seleccionando la posición en el tablero donde desea invocarla, siempre y cuando la carta pueda ser colocada en dicha zona.

- **Activar el poder del líder**: Si el líder del jugador posee un poder activo (no pasivo), este puede ser activado durante su turno.

- **Pasar el turno**: Si el jugador no desea realizar más acciones, puede pasar el turno al siguiente jugador.

### 4. Interacción con las Cartas

Durante el juego, al pasar el mouse sobre una carta, el jugador podrá ver información relevante sobre la misma, incluyendo:

- **Poder**: La fuerza actual de la carta.
- **Zona de Invocación**: Las áreas del tablero donde la carta puede ser invocada.
- **Efecto**: Una explicación detallada del efecto de la carta, si es que tiene uno.

### 5. Condiciones para Ganar

La ronda termina cuando ambos jugadores deciden pasar consecutivamente. El jugador con más puntos acumulados al final de la ronda es recompensado con una **Snitch de Oro**.

El primer jugador que logre acumular dos Snitch de Oro gana la partida, y su casa de Hogwarts correspondiente se mostrará triunfante en la pantalla final del juego.


## Gwent++ Update

En esta actualización del proyecto, se han añadido nuevas funcionalidades que mejoran significativamente la experiencia del usuario. Se ha introducido la funcionalidad para agregar cartas y efectos a los mazos, lo que expande las posibilidades de jugabilidad.


### Detalles Técnicos

- **Interactividad Mejorada**: Se ha implementado un botón dentro del juego (donde se eligen los mazos) que abre una ventana donde puede colocar la ruta de un archivo txt en un Lenguaje de Propósito Específico (DSL). Este DSL es procesado por un compilador personalizado. Esta nueva función permite a los jugadores añadir cartas y efectos directamente a los mazos, ampliando la experiencia de juego de manera dinámica.

- **Proceso de Compilación**: El compilador que se ha implementado lleva a cabo un proceso completo que incluye las fases de **lexer**, **parser**, **semantic check**, **evaluate** y **execute**. Este proceso garantiza que las cartas y efectos se manejen de manera eficiente y se integren correctamente en el sistema del juego(El proceso de compilacion se llevo en otro repositorio en consola que posee mas commits).

- **Integración con Unity**: Se ha logrado vincular este sistema con Unity de manera exitosa, permitiendo que el juego utilice los mazos personalizados y que las cartas y efectos se integren perfectamente en el entorno de desarrollo de Unity.



## Proceso de Compilación

El proyecto de compilación se centra en el desarrollo de un compilador robusto que procesa código fuente en varias fases, desde el análisis léxico hasta la evaluación final del código. A lo largo del proceso, el compilador convierte secuencias de tokens en una representación ejecutable, manejando estructuras complejas mediante un Árbol de Sintaxis Abstracta (AST).

### Fases del Proceso de Compilación

#### 1. Análisis Léxico
- **Objetivo**: Convertir el código fuente en una secuencia de tokens.
- **Proceso**: El lexer escanea el texto de entrada y lo divide en tokens, que son las unidades básicas de significado como palabras clave, operadores y literales.
- **Clases Involucradas**: La clase `Token` es fundamental, encapsulando el tipo y valor de cada token.

#### 2. Análisis Sintáctico
- **Objetivo**: Convertir la secuencia de tokens en una estructura de árbol (AST).
- **Proceso**: El parser analiza la gramática del código, organizando los tokens en una jerarquía que representa la estructura lógica del programa.
- **Algunas Estructuras Involucradas**: 
  - **AST**: El Árbol de Sintaxis Abstracta (AST) se construye durante esta fase, con nodos que representan expresiones, instrucciones, y más.
  - **Expression**: Base para distintas expresiones dentro del AST.
  - **InstructionBlock**: Agrupa y organiza las instrucciones que se ejecutarán secuencialmente.

#### 3. Análisis Semántico
- **Objetivo**: Validar la lógica y coherencia del código.
- **Proceso**: Durante esta fase, el compilador verifica que las variables y funciones estén correctamente definidas y que las operaciones sean válidas. 
- **Errores**: Se manejan mediante la clase `CompilingError`, que recopila y reporta los errores semánticos.

#### 4. Evaluación y Ejecución
- **Objetivo**: Evaluar y ejecutar el código representado por el AST.
- **Proceso**: En esta fase, las expresiones y estructuras del AST son evaluadas y ejecutadas en el orden correcto. La fase de evaluación asegura que los efectos y acciones se lleven a cabo según lo definido en el código.
- **Algunas Clases Involucradas**: 
  - **ForExpression**, **WhileExpression**: Encapsulan los bucles y sus evaluaciones.
  - **Selector**, **Predicate**: Manejan la lógica condicional y la selección de objetos en base a criterios definidos.

### Estructura de Datos y Métodos Clave

#### Árbol de Sintaxis Abstracta (AST)
- **Función**: El AST organiza el código en una estructura jerárquica que refleja la lógica del programa.
- **Componentes**: 
  - **Nodos Hoja**: Representan los tokens básicos (e.g., literales, identificadores).
  - **Nodos Internos**: Representan estructuras más complejas (e.g., cartas, efectos, bloques de instrucciones, bucles).

#### Clases que Heredan de `Expression`
Las clases que heredan de `Expression` representan diferentes tipos de nodos en el AST, cada uno encapsulando una parte específica del lenguaje:
- **`BinaryExpression`**: Representa operaciones binarias como suma, resta, asignaciones, etc.
- **`UnaryExpression`**: Representa operaciones unarias, como la negación lógica o aritmética.
- **`LiteralExpression`**: Encapsula valores literales como números, cadenas, o booleanos.

#### Clase `Carta`
La clase `Carta` es fundamental en la representación de entidades del juego. Cada instancia de `Carta` debe tener propiedades y comportamientos específicos que son evaluados y ejecutados por el compilador. 

#### Clase `Efecto`
`Efecto` representa las acciones que una carta puede desencadenar. Esta clase permite definir y ejecutar efectos que pueden afectar el estado del juego o modificar otras cartas. `Efecto` se asocia con `OnActivation` y otras fases del ciclo de vida de una carta, ejecutando las instrucciones definidas.

#### Integración de Cartas en Unity
**Identificación del Tipo de Carta**:
- El tipo de la carta (`Type`) se utiliza para determinar las características de la carta, como su imagen, efectos y unidad asociada.
- Por ejemplo, una carta del tipo "Clima" tendrá un efecto de `Weather` y utilizará una imagen específica asignada a cartas climáticas.

**Verificación de Rango y Poder**:
- Se realizan validaciones para asegurarse de que las cartas tienen rangos y poderes adecuados según su tipo.
- Cartas especiales como "Lider" o "Clima" no deben tener poder asignado, mientras que las unidades deben tener un rango válido.

**Asignación de Efectos**:
- Si la carta tiene efectos asociados, estos se agregan al objeto `UnityCard`. Los efectos se describen en un resumen que se almacena como parte de la descripción de la carta.

**Creación del Objeto `UnityCard`**:
- Finalmente, se crea un nuevo objeto `UnityCard` utilizando los datos procesados. Este objeto incluye todas las propiedades de la carta, como nombre, tipo, facción, efectos y otros atributos visuales y funcionales.
