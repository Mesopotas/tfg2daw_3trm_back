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
    HoraInicio VARCHAR(5),
    HoraFin VARCHAR(5)
);

CREATE TABLE Disponibilidades ( -- disponibilidad de puestos de trabajo o salas en una hora espcifica
    IdDisponibilidad INT IDENTITY(1,1) PRIMARY KEY,
    Fecha INT,
    Estado BIT DEFAULT 1, -- por defecto estará disponible
    IdTramoHorario INT,
    IdPuestoTrabajo INT,
    FOREIGN KEY (IdTramoHorario) REFERENCES TramosHorarios(IdTramoHorario),
    FOREIGN KEY (IdPuestoTrabajo) REFERENCES PuestosTrabajo(IdPuestoTrabajo)
);

CREATE TABLE DetallesReservas ( -- reserva puestos de trabajo
    IdDetalleReserva INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion VARCHAR(250),
    IdPuestoTrabajo INT,
    FOREIGN KEY (IdPuestoTrabajo) REFERENCES PuestosTrabajo(IdPuestoTrabajo)
);

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
    IdDetalleReserva INT,
    Precio DECIMAL(10,2) DEFAULT (0),
    FOREIGN KEY (IdReserva) REFERENCES Reservas(IdReserva),
    FOREIGN KEY (IdDetalleReserva) REFERENCES DetallesReservas(IdDetalleReserva)
);

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
