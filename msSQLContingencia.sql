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
    PrecioAniadido DECIMAL(3,2),

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
    TramoReservado VARCHAR(100),
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


-- INSERTS PARA SEDES (5 en España con información completa)
INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, URL_Imagen, Latitud, Longitud, Observaciones)
VALUES 
('España', 'Madrid', 'Calle Gran Vía 28', '28013', 'Planta 4', 'https://example.com/images/madrid.jpg', '40.4200', '-3.7025', 'Edificio histórico con vistas a Gran Vía'),
('España', 'Barcelona', 'Passeig de Gràcia 92', '08008', 'Planta 2', 'https://example.com/images/barcelona.jpg', '41.3950', '2.1534', 'Junto a Casa Milà'),
('España', 'Valencia', 'Calle Colón 60', '46004', 'Planta 3', 'https://example.com/images/valencia.jpg', '39.4697', '-0.3774', 'Zona comercial principal'),
('España', 'Sevilla', 'Avenida de la Constitución 12', '41004', 'Planta 1', 'https://example.com/images/sevilla.jpg', '37.3886', '-5.9953', 'Cerca de la Catedral'),
('España', 'Bilbao', 'Gran Vía Don Diego López de Haro 38', '48009', 'Planta 5', 'https://example.com/images/bilbao.jpg', '43.2630', '-2.9350', 'Vista al río Nervión');

-- ACTUALIZAR TIPOS DE PUESTOS DE TRABAJO (ajustar precios según requerimientos)
UPDATE TiposPuestosTrabajo SET Precio = 2.00 WHERE IdTipoPuestoTrabajo = 1; -- Publica 2€/hora
UPDATE TiposPuestosTrabajo SET Precio = 5.00 WHERE IdTipoPuestoTrabajo = 2; -- Privada 5€/hora

-- INSERTAR TIPOS DE SALAS (4 tipos según requerimientos)
INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo)
VALUES 
('Sala Grande Pública', 10, 40, 0, 'Sala grande de trabajo colaborativo', 1),
('Sala Mediana Pública', 8, 32, 0, 'Sala mediana de trabajo colaborativo', 1),
('Sala Pequeña Pública', 3, 12, 0, 'Sala pequeña de trabajo colaborativo', 1),
('Sala Privada', 5, 20, 1, 'Sala privada para equipos', 2);

-- CARACTERÍSTICAS DE SALAS (5 características)
INSERT INTO CaracteristicasSala (Nombre, Descripcion, PrecioAniadido)
VALUES 
('Ordenadores incorporados', 'Puestos con ordenadores de alta gama instalados', 0.50),
('Cocina equipada', 'Cocina con todo lo necesario', 0.00),
('Habitaciones individuales para llamadas', 'Cabinas insonorizadas para videoconferencias', 0.25),
('Lockers para usuarios', 'Taquillas seguras para pertenencias personales', 0.15),
('Puerta por llave magnética', 'Acceso seguro mediante tarjeta magnética', 0.20);

-- INSERTAR SALAS (4 salas por sede, cada una de un tipo)
-- Para la sede de Madrid (IdSede = 1)
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede)
VALUES 
('Madrid Grande', 'https://example.com/images/madrid_grande.jpg', 40, 1, 1),
('Madrid Mediana', 'https://example.com/images/madrid_mediana.jpg', 32, 2, 1),
('Madrid Pequeña', 'https://example.com/images/madrid_pequena.jpg', 12, 3, 1),
('Madrid Privada', 'https://example.com/images/madrid_privada.jpg', 20, 4, 1);

-- Para la sede de Barcelona (IdSede = 2)
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede)
VALUES 
('Barcelona Grande', 'https://example.com/images/barcelona_grande.jpg', 40, 1, 2),
('Barcelona Mediana', 'https://example.com/images/barcelona_mediana.jpg', 32, 2, 2),
('Barcelona Pequeña', 'https://example.com/images/barcelona_pequena.jpg', 12, 3, 2),
('Barcelona Privada', 'https://example.com/images/barcelona_privada.jpg', 20, 4, 2);

-- Para la sede de Valencia (IdSede = 3)
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede)
VALUES 
('Valencia Grande', 'https://example.com/images/valencia_grande.jpg', 40, 1, 3),
('Valencia Mediana', 'https://example.com/images/valencia_mediana.jpg', 32, 2, 3),
('Valencia Pequeña', 'https://example.com/images/valencia_pequena.jpg', 12, 3, 3),
('Valencia Privada', 'https://example.com/images/valencia_privada.jpg', 20, 4, 3);

-- Para la sede de Sevilla (IdSede = 4)
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede)
VALUES 
('Sevilla Grande', 'https://example.com/images/sevilla_grande.jpg', 40, 1, 4),
('Sevilla Mediana', 'https://example.com/images/sevilla_mediana.jpg', 32, 2, 4),
('Sevilla Pequeña', 'https://example.com/images/sevilla_pequena.jpg', 12, 3, 4),
('Sevilla Privada', 'https://example.com/images/sevilla_privada.jpg', 20, 4, 4);

-- Para la sede de Bilbao (IdSede = 5)
INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede)
VALUES 
('Bilbao Grande', 'https://example.com/images/bilbao_grande.jpg', 40, 1, 5),
('Bilbao Mediana', 'https://example.com/images/bilbao_mediana.jpg', 32, 2, 5),
('Bilbao Pequeña', 'https://example.com/images/bilbao_pequena.jpg', 12, 3, 5),
('Bilbao Privada', 'https://example.com/images/bilbao_privada.jpg', 20, 4, 5);

-- INSERTAR RELACIÓN SALA CON CARACTERÍSTICAS (al menos una característica por sala)
-- Para Madrid
INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES 
(1, 1), -- Madrid Grande: Ordenadores incorporados
(1, 4), -- Madrid Grande: Lockers para usuarios
(2, 2), -- Madrid Mediana: Asientos libres
(3, 2), -- Madrid Pequeña: Asientos libres
(3, 3), -- Madrid Pequeña: Habitaciones individuales para llamadas
(4, 5); -- Madrid Privada: Puerta por llave magnética

-- Para Barcelona
INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES 
(5, 1), -- Barcelona Grande: Ordenadores incorporados
(6, 2), -- Barcelona Mediana: Asientos libres
(6, 3), -- Barcelona Mediana: Habitaciones individuales para llamadas
(7, 2), -- Barcelona Pequeña: Asientos libres
(8, 5); -- Barcelona Privada: Puerta por llave magnética

-- Para Valencia
INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES 
(9, 1),  -- Valencia Grande: Ordenadores incorporados
(10, 2), -- Valencia Mediana: Asientos libres
(11, 3), -- Valencia Pequeña: Habitaciones individuales para llamadas
(12, 4), -- Valencia Privada: Lockers para usuarios
(12, 5); -- Valencia Privada: Puerta por llave magnética

-- Para Sevilla
INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES 
(13, 1), -- Sevilla Grande: Ordenadores incorporados
(14, 2), -- Sevilla Mediana: Asientos libres
(15, 3), -- Sevilla Pequeña: Habitaciones individuales para llamadas
(16, 5); -- Sevilla Privada: Puerta por llave magnética

-- Para Bilbao
INSERT INTO SalaConCaracteristicas (IdSala, IdCaracteristica)
VALUES 
(17, 1), -- Bilbao Grande: Ordenadores incorporados
(18, 2), -- Bilbao Mediana: Asientos libres
(19, 3), -- Bilbao Pequeña: Habitaciones individuales para llamadas
(20, 4), -- Bilbao Privada: Lockers para usuarios
(20, 5); -- Bilbao Privada: Puerta por llave magnética

-- INSERTAR ZONAS DE TRABAJO (una por sala)
INSERT INTO ZonasTrabajo (Descripcion, IdSala)
VALUES 
-- Madrid
('Zona amplia con mesas espaciosas', 1),
('Zona bien iluminada con mesas medianas', 2),
('Zona tranquila con pocas mesas', 3),
('Zona privada con mesas para equipos', 4),
-- Barcelona
('Zona amplia con vistas a la ciudad', 5),
('Zona céntrica con mesas medianas', 6),
('Zona tranquila y acogedora', 7),
('Zona privada para reuniones de equipo', 8),
-- Valencia
('Zona amplia con luz natural', 9),
('Zona mediterránea con mesas medianas', 10),
('Zona compacta y funcional', 11),
('Zona privada con decoración moderna', 12),
-- Sevilla
('Zona amplia con estilo andaluz', 13),
('Zona acogedora con mesas medianas', 14),
('Zona íntima con pocas mesas', 15),
('Zona privada con aire acondicionado', 16),
-- Bilbao
('Zona amplia con estilo industrial', 17),
('Zona moderna con mesas medianas', 18),
('Zona pequeña pero bien equipada', 19),
('Zona privada con vistas a la ciudad', 20);




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

/* PARA LOS ASIENTOS Y TRAMOS HORARIOS 

EN CMD ESCRIBIR ESTE COMANDO
curl -k -X POST https://localhost:7179/api/PuestosTrabajo/generarAsientosDeSalas

Y LUEGO ESTE ( TARDA 2-3 MINS YA QUE HAY 520 ASIENTOS EN TOTAL Y CARGA LAS DISPONIBILIDADES DE CADA UNO TODO EL AÑO)

curl -k -X POST https://localhost:7179/api/Disponibilidades/add/2025




*/

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
