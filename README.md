# Angry Birds 3D

El proyecto final consiste en hacer el juego de Angry Birds 3D en realidad aumentada y multijugador, utilizando Unity, marcadores Aruco, Vuforia y reconocimiento de voz.

## REQUISITOS

● 4 cámaras web: 
  ○ 2 cámaras para el jugador 1.
  ○ 2 cámaras para el jugador 2.
  
● 3 pdf impresos:
  ○ 1 pdf del marcador aruco para jugador 1.
  ○ 1 pdf del marcador aruco para jugador 2.
  ○ 1 pdf del marcador vuforia para el mapa.

## Instructivo General

1. Descargue el proyecto.
2. Verifique si tiene la versión de Unity 2021.3.10f1, si no fuere el caso, descárguela e instálela.
3. Descargue e imprima las siguientes images: La primera denominada aruco y las demás vuforia.

![aruco](https://github.com/Ajmyquira/AngrybirdsAR/blob/master/Assets/Img/aruco.jpeg)

![vuforia](https://github.com/Ajmyquira/AngrybirdsAR/blob/master/Assets/Img/vuforia.jpeg)

![vuforia](https://github.com/Ajmyquira/AngrybirdsAR/blob/master/Assets/Img/vuforia2.jpeg)

4. Debe tener dos cámaras conectas para poder jugar, esto es para cada jugador.
5. Al momento de iniciar el juego, se mostrará solo una cámara. Coloque la imagen de vuforia al frente de la cámara hasta que aparezca el mapa en realidad aumentada.
6. Colocar la imagen de aruco al frente de la segunda cámara para reconocer el marcador aruco posteriormente.

## Si eres el jugador 1 debes saber que tú colocas los objetos:

  ● Debes tener dos cámaras conectadas al juego: una cámara para  mostrar el mapa y otra para mostrar el marcador aruco.
  ● Tienes 3 objetos de vidrio, 3 objetos de madera y 3 cerditos para colocar en todo el mapa.
  ● Sin embargo, te aconsejamos colocarlos en el piso porque luego se activa la gravedad y se caen los objetos.
  
1. Enfocar una cámara al marcador vuforia del mapa para que aparezca.
2. Decir la palabra correspondiente para colocar el objeto que quieras y aparezca: “BLOCK” = bloque de madera, “GLASS” = bloque de vidrio y “GREEN” = cerdito.
3. Para colocar los objetos en el mapa debes mostrar a la otra cámara el marcador aruco correspondiente y moverlo.
4. Para cambiar de objeto y colocar el anterior en esa posición debes decir “DOWN”.

Recuerda que solo tienes 3 objetos de cada uno y una vez que coloques todos se activará la gravedad y caerán.

## Si eres el jugador 2 debes saber que tú derribas los objetos:

  ● Debes tener dos cámaras conectadas al juego: una cámara para mostrar el mapa y otra para mostrar el marcador aruco.
  ● Tienes 3 oportunidades para lanzar pájaros.
  ● Debes derribar a todos los cerditos para ganar.
  
1. Enfocar una cámara al marcador vuforia del mapa para que aparezca.
2. Debes mostrar a la otra cámara el marcador aruco correspondiente para que puedas mover el pájaro.
3. Debes decir “SUJETA” para que puedas mover el pájaro con el marcador aruco y apuntar a tu objetivo.
4. Debes decir “DISPARA” para que se pueda lanzar el pájaro a tu objetivo.

Recuerda que cada objeto que derribes tiene un puntaje correspondiente, pero para ganar debes derribar todos los cerditos.

Psdt: Para visualizar mejor el proyecto y tener como referencia cómo se juega según cada jugador, revisa en la carpeta de Videos del proyecto.
