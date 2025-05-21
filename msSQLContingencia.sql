-- INSTALACION DE IMAGENES Y CONTENEDORES PARA DOCKER

-- Descarga de la imagen con la version correcta de la BBDD de MicrosoftSQL

-- docker pull mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04

-- Creacion del contenedor para la imagen de msSQL

-- docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong@Passw0rd>" -dp 1433:1433 mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04

-- Datos
---- Usuario: sa
---- Contraseña: iJJE3JODS_sjUSjsdjO


-- CONSULTAS PARA LA CREACION DE LA BASE DE DATOS

/* PARA VER LAS TABLAS DE LA BBDD
USE CoWorkingDB;
SELECT * FROM information_schema.tables
WHERE table_type = 'BASE TABLE';
*/

-- Creación de la base de datos
CREATE DATABASE CoworkingDB;
USE CoworkingDB;

CREATE TABLE Sedes ( -- ubicacion fisica de la oficina
    IdSede INT IDENTITY(1,1) PRIMARY KEY,
    Pais VARCHAR(100),
    Ciudad VARCHAR(100),
    Direccion VARCHAR(250),
    CodigoPostal VARCHAR(5),
    Planta VARCHAR(100),
    URL_Imagen VARCHAR(250), -- imagen de la localizacion
    Latitud  VARCHAR(100),
    Longitud  VARCHAR(100),
    Observaciones VARCHAR(100)
);

CREATE TABLE TiposPuestosTrabajo ( -- define los tipos de puestos como silla, mesa, etc y su precio base a aplicar
    IdTipoPuestoTrabajo INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100), -- silla sala privada, silla sala publica y cada una un precio variable
    Imagen_URL VARCHAR(250), -- imagen de la silla por ejemplo
    Descripcion VARCHAR(200),
    Precio DECIMAL(10,2) -- relativo a tipossala y todo
);

INSERT INTO TiposPuestosTrabajo(Nombre, Imagen_URL, Descripcion, Precio) -- insert inicial de silla, tendrá id 1, link de imagen provisional, pendiente cambiarlo
VALUES ('Silla comun en sala publica', 'https://e7.pngegg.com/pngimages/488/752/png-clipart-chair-desk-table-school-chair-angle-furniture-thumbnail.png', 'Silla comun', 10.00);
INSERT INTO TiposPuestosTrabajo(Nombre, Imagen_URL, Descripcion, Precio) -- insert inicial de silla, tendrá id 1, link de imagen provisional, pendiente cambiarlo
VALUES ('Silla comun en sala privada', 'https://e7.pngegg.com/pngimages/488/752/png-clipart-chair-desk-table-school-chair-angle-furniture-thumbnail.png', 'Silla comun', 15.00);

CREATE TABLE TiposSalas ( -- privada o comun
    IdTipoSala INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
    NumeroMesas INT, -- NUMERO DE MESAS
    CapacidadAsientos INT, -- NUMERO DE ASIENTOS POR MESA 
    EsPrivada BIT DEFAULT 0, -- DEFAULT SERÁ FALSO, OSEA, PUBLICA
    Descripcion VARCHAR(100),
    IdTipoPuestoTrabajo INT,
    FOREIGN KEY (IdTipoPuestoTrabajo) REFERENCES TiposPuestosTrabajo(IdTipoPuestoTrabajo)
);

CREATE TABLE Salas ( -- salas dentro de cada sede
    IdSala INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50),
    URL_Imagen VARCHAR(250), --  imagen de la sala por dentro
    Capacidad INT,
    IdTipoSala INT,
    IdSede INT,
    Bloqueado BIT DEFAULT 0, -- para el rol del admin de bloquear puestos de trabajo
    FOREIGN KEY (IdSede) REFERENCES Sedes(IdSede),
    FOREIGN KEY (IdTipoSala) REFERENCES TiposSalas(IdTipoSala)
);

CREATE TABLE CaracteristicasSala ( -- complementos de la sala
    IdCaracteristica INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100), -- Ejemplo: "Internet", "Proyector"
    Descripcion VARCHAR(250),
    PrecioAniadido DECIMAL(3,2), -- será un %, eg. si es 1,5: el precio total de las sillas + su 1,5%, 

)

CREATE TABLE SalaConCaracteristicas( -- tabla intermedia para evitar la relacion N:N, cada sala 
    IdSalaConCaracteristica INT IDENTITY(1,1) PRIMARY KEY, 
    IdSala INT,
    IdCaracteristica INT,
    FOREIGN KEY (IdSala) REFERENCES Salas(IdSala),
    FOREIGN KEY (IdCaracteristica) REFERENCES CaracteristicasSala(IdCaracteristica),


) 


CREATE TABLE ZonasTrabajo ( -- aforo dentro de cada sala y sus detalles
    IdZonaTrabajo INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion VARCHAR(250),
    IdSala INT,
    FOREIGN KEY (IdSala) REFERENCES Salas(IdSala)
);

CREATE TABLE PuestosTrabajo ( -- puestos de trabajo dentro de cada zona de trabajo, en principio de base solo para sillas a escoger, ampliandolo en un futuro a poder elegir lotes de mesas
    IdPuestoTrabajo INT IDENTITY(1,1) PRIMARY KEY,
    NumeroAsiento INT, -- será un identificador secundario (REPETIBLE) para la silla para pintarla en el fetch
    CodigoMesa INT, -- será el identificador de mesas, codigo 1 -> mesa 1, codigo 2 -> mesa 2, si hago 4x4 mesas, habrá 4 codigos asignados a 4 mesas cada uno
    URL_Imagen VARCHAR(250), -- la imagen del componente, como mesas, sillas, etc para el fetch
    Disponible BIT DEFAULT 1, -- por defecto estará disponible para reservar
    IdZonaTrabajo INT,
    IdSala INT,
    Bloqueado BIT DEFAULT 0, -- para el rol del admin de bloquear puestos de trabajo
    FOREIGN KEY (IdZonaTrabajo) REFERENCES ZonasTrabajo(IdZonaTrabajo),
    FOREIGN KEY (IdSala) REFERENCES Salas(IdSala)
    
);

CREATE TABLE TramosHorarios ( -- intervalos de tiempo en los que hay disponibilidad
    IdTramoHorario INT IDENTITY(1,1) PRIMARY KEY,
    HoraInicio TIME, -- time guardará la hora pero no el dia
    HoraFin TIME
);

CREATE TABLE Disponibilidades ( -- disponibilidad de puestos de trabajo o salas en una hora espcifica
    IdDisponibilidad INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATE, -- dare guardará el dia sin hora, se complementará con el tramo horario
    Estado BIT DEFAULT 1, -- por defecto estará disponible
    IdTramoHorario INT,
    IdPuestoTrabajo INT,
    FOREIGN KEY (IdTramoHorario) REFERENCES TramosHorarios(IdTramoHorario),
    FOREIGN KEY (IdPuestoTrabajo) REFERENCES PuestosTrabajo(IdPuestoTrabajo)
);
/*
CREATE TABLE DetallesReservas ( -- ELIMINAR, Lineas cubrirá esa función
    IdDetalleReserva INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion VARCHAR(250),
    IdPuestoTrabajo INT,
    FOREIGN KEY (IdPuestoTrabajo) REFERENCES PuestosTrabajo(IdPuestoTrabajo)
);
*/
CREATE TABLE Roles ( -- roles de usuario (admin y cliente de base)
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
    Descripcion VARCHAR(255)
);

-- INSERT 2 PRIMEROS ROLES INICIALES
INSERT INTO Roles (Nombre, Descripcion)
VALUES ('Admin', 'Rol con privilegios avanzados para gestión');

INSERT INTO Roles (Nombre, Descripcion)
VALUES ('Cliente', 'Rol limitado para consumidores');

CREATE TABLE Usuarios ( -- personas registradas en la plataforma
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100),
    Apellidos NVARCHAR(255),
    Email NVARCHAR(255),
    Contrasenia NVARCHAR(255),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    IdRol INT DEFAULT 2, -- rol de id 2 por defecto será Usuario normal (cliente)
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);

CREATE TABLE Reservas ( -- reservas realizadas por los usuarios
    IdReserva INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT,
    Fecha DATETIME,
    Descripcion VARCHAR(250),
    PrecioTotal DECIMAL(10,2),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);

CREATE TABLE Lineas ( -- reserva relacionada con sus detalles especificos, aquello que veria el usuario a modo factura
    IdLinea INT IDENTITY(1,1) PRIMARY KEY,
    IdReserva INT,
    IdPuestoTrabajo INT,
    Precio DECIMAL(10,2) DEFAULT (0),
    FOREIGN KEY (IdReserva) REFERENCES Reservas(IdReserva),
    FOREIGN KEY (IdPuestoTrabajo) REFERENCES PuestosTrabajo(IdPuestoTrabajo)
);




-- inserts tramos horarios, como base serian estos en todos los dias q este abierto
INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('08:00', '09:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('09:00', '10:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('10:00', '11:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('11:00', '12:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('12:00', '13:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('13:00', '14:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('14:00', '15:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('15:00', '16:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('16:00', '17:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('17:00', '18:00');

INSERT INTO TramosHorarios (HoraInicio, HoraFin)
VALUES ('18:00', '19:00');



-- inserts de prueba


-- 5 Inserts para la tabla Sedes
INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, URL_Imagen, Latitud, Longitud, Observaciones)
VALUES ('España', 'Madrid', 'Calle Gran Vía 32', '28013', 'Planta 4', 'https://ejemplo.com/imagenes/sede_madrid.jpg', '40.4200', '-3.7050', 'Edificio céntrico con vistas al centro');

INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, URL_Imagen, Latitud, Longitud, Observaciones)
VALUES ('España', 'Barcelona', 'Passeig de Gràcia 77', '08008', 'Planta 2', 'https://ejemplo.com/imagenes/sede_barcelona.jpg', '41.3950', '2.1609', 'Zona comercial Premium');

INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, URL_Imagen, Latitud, Longitud, Observaciones)
VALUES ('España', 'Valencia', 'Calle Colón 15', '46004', 'Planta 1', 'https://ejemplo.com/imagenes/sede_valencia.jpg', '39.4699', '-0.3773', 'Cerca del centro histórico');

INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, URL_Imagen, Latitud, Longitud, Observaciones)
VALUES ('España', 'Sevilla', 'Avenida de la Constitución 10', '41004', 'Planta 3', 'https://ejemplo.com/imagenes/sede_sevilla.jpg', '37.3886', '-5.9953', 'Al lado de la Catedral');

INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, URL_Imagen, Latitud, Longitud, Observaciones)
VALUES ('España', 'Bilbao', 'Gran Vía Don Diego López de Haro 12', '48001', 'Planta 5', 'https://ejemplo.com/imagenes/sede_bilbao.jpg', '43.2630', '-2.9350', 'Vistas al Guggenheim');

-- 5 Inserts para la tabla TiposPuestosTrabajo (2 ya existen en su script original)
INSERT INTO TiposPuestosTrabajo (Nombre, Imagen_URL, Descripcion, Precio)
VALUES ('Mesa individual', 'https://ejemplo.com/imagenes/mesa_individual.png', 'Mesa individual para una persona', 20.00);

INSERT INTO TiposPuestosTrabajo (Nombre, Imagen_URL, Descripcion, Precio)
VALUES ('Mesa compartida', 'https://ejemplo.com/imagenes/mesa_compartida.png', 'Mesa para cuatro personas', 12.00);

INSERT INTO TiposPuestosTrabajo (Nombre, Imagen_URL, Descripcion, Precio)
VALUES ('Espacio premium', 'https://ejemplo.com/imagenes/espacio_premium.png', 'Espacio con monitor adicional y mejores vistas', 25.00);

-- 5 Inserts para la tabla TiposSalas
INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo)
VALUES ('Sala común general', 10, 4, 0, 'Espacio abierto compartido', 1);

INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo)
VALUES ('Sala privada pequeña', 2, 4, 1, 'Sala privada para reuniones pequeñas', 2);

INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo)
VALUES ('Sala privada mediana', 4, 4, 1, 'Sala privada para equipos medianos', 2);

INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo)
VALUES ('Sala premium', 5, 2, 1, 'Sala con servicios exclusivos', 5);

INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo)
VALUES ('Sala de conferencias', 1, 10, 1, 'Sala para presentaciones', 2);

-- 5 Inserts para la tabla Salas
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Coworking Madrid Central', 'https://ejemplo.com/imagenes/sala_madrid1.jpg', 40, 1, 1, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Reuniones Barcelona', 'https://ejemplo.com/imagenes/sala_barcelona1.jpg', 8, 2, 2, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Espacio Valencia Creativo', 'https://ejemplo.com/imagenes/sala_valencia1.jpg', 16, 3, 3, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Premium Sevilla', 'https://ejemplo.com/imagenes/sala_sevilla1.jpg', 10, 4, 4, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Auditorio Bilbao', 'https://ejemplo.com/imagenes/sala_bilbao1.jpg', 10, 5, 5, 0);


INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Madrid Premium', 'https://ejemplo.com/imagenes/sala_madrid2.jpg', 8, 2, 1, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Madrid Reuniones', 'https://ejemplo.com/imagenes/sala_madrid3.jpg', 12, 3, 1, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Madrid Conferencias', 'https://ejemplo.com/imagenes/sala_madrid4.jpg', 4, 5, 1, 0);

-- Para la sede 2 (Barcelona) - Ya existe 1 sala, añadimos 3 más
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Barcelona Coworking', 'https://ejemplo.com/imagenes/sala_barcelona2.jpg', 12, 1, 2, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Barcelona Premium', 'https://ejemplo.com/imagenes/sala_barcelona3.jpg', 4, 4, 2, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Barcelona Conferencias', 'https://ejemplo.com/imagenes/sala_barcelona4.jpg', 8, 5, 2, 0);

-- Para la sede 3 (Valencia) - Ya existe 1 sala, añadimos 3 más
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Valencia Coworking', 'https://ejemplo.com/imagenes/sala_valencia2.jpg', 8, 1, 3, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Valencia Premium', 'https://ejemplo.com/imagenes/sala_valencia3.jpg', 4, 4, 3, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Valencia Conferencias', 'https://ejemplo.com/imagenes/sala_valencia4.jpg', 12, 5, 3, 0);

-- Para la sede 4 (Sevilla) - Ya existe 1 sala, añadimos 3 más
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Sevilla Coworking', 'https://ejemplo.com/imagenes/sala_sevilla2.jpg', 12, 1, 4, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Sevilla Reuniones', 'https://ejemplo.com/imagenes/sala_sevilla3.jpg', 8, 3, 4, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Sevilla Conferencias', 'https://ejemplo.com/imagenes/sala_sevilla4.jpg', 4, 5, 4, 0);

-- Para la sede 5 (Bilbao) - Ya existe 1 sala, añadimos 3 más
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Bilbao Coworking', 'https://ejemplo.com/imagenes/sala_bilbao2.jpg', 8, 1, 5, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Bilbao Reuniones', 'https://ejemplo.com/imagenes/sala_bilbao3.jpg', 4, 2, 5, 0);

INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado)
VALUES ('Sala Bilbao Premium', 'https://ejemplo.com/imagenes/sala_bilbao4.jpg', 12, 4, 5, 0);

-- 5 Inserts para la tabla CaracteristicasSala
INSERT INTO CaracteristicasSala (Nombre, Descripcion, PrecioAniadido)
VALUES ('WiFi Alta Velocidad', 'Conexión fibra 1Gbps', 0.10);

INSERT INTO CaracteristicasSala (Nombre, Descripcion, PrecioAniadido)
VALUES ('Proyector 4K', 'Proyector de alta definición con HDMI', 0.15);

INSERT INTO CaracteristicasSala (Nombre, Descripcion, PrecioAniadido)
VALUES ('Pizarra Digital', 'Pizarra interactiva con conexión a dispositivos', 0.12);

INSERT INTO CaracteristicasSala (Nombre, Descripcion, PrecioAniadido)
VALUES ('Sistema de Videoconferencia', 'Equipo completo para reuniones virtuales', 0.20);

INSERT INTO CaracteristicasSala (Nombre, Descripcion, PrecioAniadido)
VALUES ('Café Premium', 'Servicio de café de especialidad incluido', 0.05);

-- 5 Inserts para la tabla SalaConCaracteristicas
INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES (1, 1);

INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES (2, 2);

INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES (3, 3);

INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES (4, 4);

INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES (5, 5);

-- 5 Inserts para la tabla ZonasTrabajo
INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona General Madrid', 1);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Reuniones Barcelona', 2);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Creativa Valencia', 3);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Premium Sevilla', 4);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Conferencias Bilbao', 5);


INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Premium Madrid', 6);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Reuniones Madrid', 7);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Conferencias Madrid', 8);

-- Para las nuevas salas de Barcelona
INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Coworking Barcelona', 9);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Premium Barcelona', 10);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Conferencias Barcelona', 11);

-- Para las nuevas salas de Valencia
INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Coworking Valencia', 12);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Premium Valencia', 13);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Conferencias Valencia', 14);

-- Para las nuevas salas de Sevilla
INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Coworking Sevilla', 15);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Reuniones Sevilla', 16);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Conferencias Sevilla', 17);

-- Para las nuevas salas de Bilbao
INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Coworking Bilbao', 18);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Reuniones Bilbao', 19);

INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES ('Zona Premium Bilbao', 20);

-- 5 Inserts para la tabla PuestosTrabajo
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_madrid1.jpg', 1, 1, 1, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_madrid1.jpg', 1, 1, 1, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_madrid1.jpg', 1, 1, 1, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_madrid1.jpg', 1, 1, 1, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_madrid1.jpg', 1, 1, 1, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_madrid1.jpg', 1, 1, 1, 0);


INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona1.jpg', 1, 2, 2, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona1.jpg', 1, 2, 2, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona1.jpg', 1, 2, 2, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona1.jpg', 1, 2, 2, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona1.jpg', 1, 2, 2, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona1.jpg', 1, 2, 2, 0);


INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_valencia1.jpg', 1, 3, 3, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_valencia1.jpg', 1, 3, 3, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_valencia1.jpg', 1, 3, 3, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_valencia1.jpg', 1, 3, 3, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_valencia1.jpg', 1, 3, 3, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla1.jpg', 1, 4, 4, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla1.jpg', 1, 4, 4, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla1.jpg', 1, 4, 4, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla1.jpg', 1, 4, 4, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla1.jpg', 1, 4, 4, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla1.jpg', 1, 4, 4, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);

INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES (1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao1.jpg', 1, 5, 5, 0);


INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_madrid_premium.jpg', 1, 6, 6, 0);

-- Para la sala 7 (Madrid Reuniones) - 12 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(1, 3, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(2, 3, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(3, 3, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0),
(4, 3, 'https://ejemplo.com/imagenes/puesto_madrid_reuniones.jpg', 1, 7, 7, 0);

-- Para la sala 8 (Madrid Conferencias) - 4 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_madrid_conf.jpg', 1, 8, 8, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_madrid_conf.jpg', 1, 8, 8, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_madrid_conf.jpg', 1, 8, 8, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_madrid_conf.jpg', 1, 8, 8, 0);

-- Para la sala 9 (Barcelona Coworking) - 12 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(1, 3, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(2, 3, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(3, 3, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0),
(4, 3, 'https://ejemplo.com/imagenes/puesto_barcelona_cowork.jpg', 1, 9, 9, 0);

-- Para la sala 10 (Barcelona Premium) - 4 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_premium.jpg', 1, 10, 10, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_premium.jpg', 1, 10, 10, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_premium.jpg', 1, 10, 10, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_premium.jpg', 1, 10, 10, 0);

-- Para la sala 11 (Barcelona Conferencias) - 8 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_barcelona_conf.jpg', 1, 11, 11, 0);

-- Para la sala 12 (Valencia Coworking) - 8 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_valencia_cowork.jpg', 1, 12, 12, 0);

-- Para la sala 13 (Valencia Premium) - 4 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_valencia_premium.jpg', 1, 13, 13, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_valencia_premium.jpg', 1, 13, 13, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_valencia_premium.jpg', 1, 13, 13, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_valencia_premium.jpg', 1, 13, 13, 0);

-- Para la sala 14 (Valencia Conferencias) - 12 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(1, 3, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(2, 3, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(3, 3, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0),
(4, 3, 'https://ejemplo.com/imagenes/puesto_valencia_conf.jpg', 1, 14, 14, 0);

-- Para la sala 15 (Sevilla Coworking) - 12 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(1, 3, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(2, 3, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(3, 3, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0),
(4, 3, 'https://ejemplo.com/imagenes/puesto_sevilla_cowork.jpg', 1, 15, 15, 0);

-- Para la sala 16 (Sevilla Reuniones) - 8 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_sevilla_reuniones.jpg', 1, 16, 16, 0);

-- Para la sala 17 (Sevilla Conferencias) - 4 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_conf.jpg', 1, 17, 17, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_conf.jpg', 1, 17, 17, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_conf.jpg', 1, 17, 17, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_sevilla_conf.jpg', 1, 17, 17, 0);

-- Para la sala 18 (Bilbao Coworking) - 8 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_cowork.jpg', 1, 18, 18, 0);

-- Para la sala 19 (Bilbao Reuniones) - 4 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_reuniones.jpg', 1, 19, 19, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_reuniones.jpg', 1, 19, 19, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_reuniones.jpg', 1, 19, 19, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_reuniones.jpg', 1, 19, 19, 0);

-- Para la sala 20 (Bilbao Premium) - 12 asientos
INSERT INTO PuestosTrabajo (NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, IdZonaTrabajo, IdSala, Bloqueado)
VALUES 
(1, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(2, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(3, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(4, 1, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(1, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(2, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(3, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(4, 2, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(1, 3, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(2, 3, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(3, 3, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0),
(4, 3, 'https://ejemplo.com/imagenes/puesto_bilbao_premium.jpg', 1, 20, 20, 0);



-- 5 Inserts para la tabla Usuarios
INSERT INTO Usuarios (Nombre, Apellidos, Email, Contrasenia, FechaRegistro, IdRol)
VALUES ('Admin', 'Principal', 'admin@coworking.com', 'Pass123$Admin', GETDATE(), 1);

INSERT INTO Usuarios (Nombre, Apellidos, Email, Contrasenia, FechaRegistro, IdRol)
VALUES ('Juan', 'García Pérez', 'juan.garcia@email.com', 'Pass123$Juan', GETDATE(), 2);

INSERT INTO Usuarios (Nombre, Apellidos, Email, Contrasenia, FechaRegistro, IdRol)
VALUES ('María', 'López Martínez', 'maria.lopez@email.com', 'Pass123$Maria', GETDATE(), 2);

INSERT INTO Usuarios (Nombre, Apellidos, Email, Contrasenia, FechaRegistro, IdRol)
VALUES ('Carlos', 'Rodríguez Sánchez', 'carlos.rodriguez@email.com', 'Pass123$Carlos', GETDATE(), 2);

INSERT INTO Usuarios (Nombre, Apellidos, Email, Contrasenia, FechaRegistro, IdRol)
VALUES ('Laura', 'Fernández Díaz', 'laura.fernandez@email.com', 'Pass123$Laura', GETDATE(), 2);



/* AÑADIR PROXIMAMENTE CONFORME TODO ESTE HECHO 
CREATE TABLE Descuentos ( 
    IdDescuento INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion VARCHAR(200),
    Porcentaje DECIMAL(5,2),
    MinHoras INT,
    MinAsientos INT
);
*/


/* BORRAR TODAS LA BBDD
USE master;
DROP DATABASE CoworkingDB;

*/
