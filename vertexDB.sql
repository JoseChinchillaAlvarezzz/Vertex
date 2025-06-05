USE vertex;
GO

-- Tabla: roles
CREATE TABLE roles (
	id INT IDENTITY(1,1) PRIMARY KEY,
	rol VARCHAR(50) NOT NULL UNIQUE
);

-- Tabla: usuarios
CREATE TABLE usuarios (
	id INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(50) NOT NULL,
	apellido VARCHAR(50) NOT NULL,
	email VARCHAR(75) NOT NULL UNIQUE,
	telefono CHAR(9) NOT NULL UNIQUE,
	contrasenia VARCHAR(250) NOT NULL,
	rol_id INT,
	FOREIGN KEY (rol_id) REFERENCES roles(id)
);

-- Tabla: clientes
CREATE TABLE clientes (
	id INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(50) NOT NULL,
	apellido VARCHAR(50) NOT NULL,
	email VARCHAR(75) NOT NULL UNIQUE,
	telefono CHAR(9) NOT NULL UNIQUE,
	contrasenia VARCHAR(250) NOT NULL,
	empresa VARCHAR(100),
	contactoprincipal VARCHAR(75),
	nombrecontacto VARCHAR(50)
);

-- Tabla: categorias
CREATE TABLE categorias (
	id INT IDENTITY(1,1) PRIMARY KEY,
	categoria VARCHAR(50) NOT NULL UNIQUE
);

-- Tabla: prioridades
CREATE TABLE prioridades (
	id INT IDENTITY(1,1) PRIMARY KEY,
	prioridad VARCHAR(50) NOT NULL UNIQUE
);

-- Tabla: estados_tickets
CREATE TABLE estados_tickets (
	id INT IDENTITY(1,1) PRIMARY KEY,
	estado VARCHAR(50) NOT NULL UNIQUE
);

-- Tabla: tickets
CREATE TABLE tickets (
	id INT IDENTITY(1,1) PRIMARY KEY,
	titulo VARCHAR(50) NOT NULL,
	descripcion VARCHAR(300) NOT NULL,
	aplicacion VARCHAR(50),
	fechacreacion DATE NOT NULL,
	usuario_id INT NOT NULL,
	categoria_id INT,
	estado_ticket_id INT NOT NULL,
	prioridad_id INT NOT NULL,
	cliente_id INT,
	FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
	FOREIGN KEY (categoria_id) REFERENCES categorias(id),
	FOREIGN KEY (estado_ticket_id) REFERENCES estados_tickets(id),
	FOREIGN KEY (prioridad_id) REFERENCES prioridades(id),
	FOREIGN KEY (cliente_id) REFERENCES clientes(id)
);

-- Tabla: asignaciones
CREATE TABLE asignaciones (
	id INT IDENTITY(1,1) PRIMARY KEY,
	fechaasignacion DATE NOT NULL,
	ticket_id INT NOT NULL,
	usuario_id INT NOT NULL,
	FOREIGN KEY (ticket_id) REFERENCES tickets(id),
	FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Tabla: cambios_estado
CREATE TABLE cambios_estado (
	id INT IDENTITY(1,1) PRIMARY KEY,
	fechacambio DATE NOT NULL,
	ticket_id INT NOT NULL,
	FOREIGN KEY (ticket_id) REFERENCES tickets(id)
);

-- Tabla: comentarios
CREATE TABLE comentarios (
	id INT IDENTITY(1,1) PRIMARY KEY,
	titulo VARCHAR(50) NOT NULL,
	comentario VARCHAR(300) NOT NULL,
	ticket_id INT NOT NULL,
	usuario_id INT,
	cliente_id INT,
	FOREIGN KEY (ticket_id) REFERENCES tickets(id),
	FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
	FOREIGN KEY (cliente_id) REFERENCES clientes(id)
);

-- Tabla: estados_tareas
CREATE TABLE estados_tareas (
	id INT IDENTITY(1,1) PRIMARY KEY,
	estado VARCHAR(50) NOT NULL UNIQUE
);

-- Tabla: tareas
CREATE TABLE tareas (
	id INT IDENTITY(1,1) PRIMARY KEY,
	titulo VARCHAR(50) NOT NULL,
	descripcion VARCHAR(300) NOT NULL,
	ticket_id INT NOT NULL,
	estado_tarea_id INT NOT NULL,
	FOREIGN KEY (ticket_id) REFERENCES tickets(id),
	FOREIGN KEY (estado_tarea_id) REFERENCES estados_tareas(id)
);

-- Roles
INSERT INTO roles (rol) VALUES
('Administrador'), ('T�cnico');

--Categor�as
INSERT INTO categorias (categoria) VALUES
('Redes'), ('Hardware'), ('Software');

-- Prioridades
INSERT INTO prioridades (prioridad) VALUES
('Alta'), ('Media'), ('Baja'), ('Cr�tica');

-- Estados de tickets
INSERT INTO estados_tickets (estado) VALUES
('Pendiente'), ('En progreso'), ('En espera'), ('Resuelto'), ('Cerrado'), ('Cancelado');

-- Estados de tareas
INSERT INTO estados_tareas (estado) VALUES
('Pendiente'), ('En ejecuci�n'), ('Completada');

-- Usuarios
INSERT INTO usuarios (nombre, apellido, email, telefono, contrasenia, rol_id) VALUES
('Gabriel', 'Moreno', 'gabriel.moreno@catolica.edu.sv', '7011-2233', '123', 1),
('Jos�', 'Chinchilla', 'jose.chinchilla@catolica.edu.sv', '7011-2234', '123', 1),
('No�', 'L�pez', 'noe.lopez@catolica.edu.sv', '7011-2235', '123', 2),
('Eduardo', 'Echeverr�a', 'eduardo.echeverria@catolica.edu.sv', '7011-2236', '123', 2),
('Carlos', 'Rivas', 'carlos.rivas@catolica.edu.sv', '7011-2237', '123', 2);

-- Clientes
INSERT INTO clientes (nombre, apellido, email, telefono, contrasenia, empresa, contactoprincipal, nombrecontacto) VALUES
('Ana', 'L�pez', 'josedavidchinchillaa@gmail.com', '6011-2233', '123', 'DICOEN', 'ana@cliente.com', 'Pedro Mart�nez'),
('Mario', 'Rivas', 'gabrielmorenoxv@gmail.com', '6011-3344', '123', 'Little Caesars', 'mario@cliente.com', 'Luisa Vega'),
('Laura', 'G�mez', 'ee653522@gmail.com', '6011-4455', '123', 'La Sihua', 'laura@cliente.com', 'Carlos P�rez'),
('Jos�', 'M�ndez', 'carlossalesianoprro@gmail.com', '6011-5566', '123', 'Walmart', 'jose@cliente.com', 'Ana Torres'),
('Sof�a', 'Hern�ndez', 'gaboguardado5@gmail.com', '6011-6677', '123', 'DICOEN', 'sofia@cliente.com', 'Luis Fern�ndez'),
('Ricardo', 'Vargas', 'rutilia6919@gmail.com', '6011-7788', '123', 'Taqueria La Chismosa', 'ricardo@cliente.com', 'Elena Ram�rez'),
('Patricia', 'Ruiz', 'nogus19lopez@gmail.com', '6011-8899', '123', 'TodoTicket', 'patricia@cliente.com', 'Miguel G�mez');

-- Tickets
INSERT INTO tickets (titulo, descripcion, aplicacion, fechacreacion, usuario_id, categoria_id, estado_ticket_id, prioridad_id, cliente_id) VALUES
('Fallo de VPN', 'No puedo conectarme a la VPN de la empresa.', null, '27/05/2025', 1, 1, 2, 3, 4),
('Problema de sonido', 'No se escucha nada al reproducir videos.', null, '15/05/2025', 2, 2, 4, 2, 6),
('Pantalla azul', 'Mi computadora muestra pantalla azul cada cierto tiempo.', 'Windows', '19/05/2025', 1, 3, 1, 1, 2),
('Error en Excel', 'No puedo abrir archivos .xlsm.', 'Excel', '09/05/2025', 2, 3, 3, 4, 5),
('Internet lento', 'La conexi�n a internet es muy inestable.', null, '20/05/2025', 1, 1, 6, 1, 7),
('Impresora desconectada', 'La impresora est� fuera de l�nea.', null, '11/05/2025', 2, 2, 2, 3, 1),
('Sistema no arranca', 'El equipo no enciende.', 'Windows', '17/05/2025', 1, 2, 5, 4, 3),
('Correo bloqueado', 'No puedo enviar correos desde Outlook.', 'Outlook', '26/05/2025', 2, 3, 1, 2, 7),
('Red sin acceso', 'No puedo acceder a recursos compartidos.', null, '05/05/2025', 1, 1, 4, 1, 2),
('Error de inicio', 'El sistema se queda en la pantalla de inicio.', 'Windows', '24/05/2025', 1, 2, 3, 2, 4),
('Corte de red', 'Perd� conexi�n a internet de repente.', null, '14/05/2025', 2, 1, 6, 3, 6),
('Actualizaci�n fallida', 'Error al actualizar el sistema.', 'Windows', '29/05/2025', 1, 3, 2, 1, 3),
('Problema con router', 'El router no distribuye se�al.', null, '10/05/2025', 2, 1, 5, 4, 5),
('Disco da�ado', 'El disco duro hace ruidos extra�os.', null, '22/05/2025', 2, 2, 1, 3, 1),
('Pantalla parpadea', 'La pantalla parpadea al mover el mouse.', null, '08/05/2025', 1, 2, 4, 2, 7),
('Software congelado', 'El programa se queda congelado frecuentemente.', 'PlatyPlus', '12/05/2025', 2, 3, 6, 4, 2),
('Reinicio inesperado', 'El sistema se reinicia solo.', 'Windows', '25/05/2025', 1, 2, 3, 1, 4),
('Clave olvidada', 'No puedo acceder a mi cuenta.', 'Gmail', '03/05/2025', 2, 3, 2, 2, 6),
('Se�al d�bil', 'La red inal�mbrica es muy d�bil.', null, '06/05/2025', 1, 1, 1, 3, 3),
('Teclado no responde', 'El teclado dej� de funcionar.', null, '13/05/2025', 2, 2, 5, 4, 5),
('Error en navegador', 'El navegador se cierra solo.', 'Chrome', '04/05/2025', 1, 3, 4, 2, 1),
('No imprime', 'El documento se queda en cola de impresi�n.', null, '16/05/2025', 2, 2, 6, 3, 7),
('Conexi�n perdida', 'La red LAN se desconecta constantemente.', null, '18/05/2025', 1, 1, 2, 4, 2),
('Fallo de mouse', 'El puntero se mueve solo.', null, '21/05/2025', 2, 2, 3, 1, 6),
('Licencia expirada', 'El software solicita renovaci�n de licencia.', 'Office', '23/05/2025', 1, 3, 1, 2, 3);

--Asignaciones
INSERT INTO asignaciones (fechaasignacion, ticket_id, usuario_id) VALUES
('28/05/2025', 1, 3), ('28/05/2025', 1, 4), ('16/05/2025', 2, 4), ('20/05/2025', 4, 2),
('21/05/2025', 5, 5), ('12/05/2025', 6, 3), ('18/05/2025', 7, 4), ('27/05/2025', 8, 2),
('06/05/2025', 9, 3), ('06/05/2025', 9, 4), ('06/05/2025', 9, 5), ('25/05/2025', 10, 5), 
('15/05/2025', 11, 3), ('15/05/2025', 11, 4), ('30/05/2025', 12, 4), ('11/05/2025', 13, 5), 
('11/05/2025', 13, 3), ('23/05/2025', 14, 5), ('09/05/2025', 15, 1), ('13/05/2025', 16, 4),
('26/05/2025', 17, 5), ('04/05/2025', 19, 3), ('04/05/2025', 19, 5), ('14/05/2025', 20, 4), 
('17/05/2025', 21, 3), ('17/05/2025', 21, 4), ('29/05/2025', 22, 5), ('19/05/2025', 23, 3), 
('19/05/2025', 23, 4), ('19/05/2025', 23, 5), ('22/05/2025', 24, 3), ('24/05/2025', 25, 1);

--Cambios de estado de ticket
INSERT INTO cambios_estado (fechacambio, ticket_id) VALUES
('28/05/2025', 2),
('20/05/2025', 4), ('21/05/2025', 4), ('22/05/2025', 4), ('23/05/2025', 4),
('21/05/2025', 5), ('22/05/2025', 5), ('23/05/2025', 5),
('18/05/2025', 6),
('18/05/2025', 7), ('19/05/2025', 7), ('22/05/2025', 7),
('06/05/2025', 9), ('08/05/2025', 9), ('09/05/2025', 9),
('25/05/2025', 10), ('27/05/2025', 10), ('28/05/2025', 10), ('29/05/2025', 10),
('15/05/2025', 11), ('16/05/2025', 11), ('18/05/2025', 11), ('19/05/2025', 11),
('30/05/2025', 12), 
('12/05/2025', 13), ('13/05/2025', 13), ('15/05/2025', 13),
('10/05/2025', 15), ('11/05/2025', 15), ('12/05/2025', 15), ('13/05/2025', 15),
('14/05/2025', 16), ('15/05/2025', 16), ('16/05/2025', 16),
('27/05/2025', 17), ('28/05/2025', 17),
('04/05/2025', 18),
('15/05/2025', 20), ('16/05/2025', 20), ('18/05/2025', 20),
('20/05/2025', 21), ('21/05/2025', 21), ('22/05/2025', 21),
('30/05/2025', 22), 
('20/05/2025', 23), ('21/05/2025', 23), ('22/05/2025', 23), ('23/05/2025', 23),
('25/05/2025', 24), ('26/05/2025', 24), ('27/05/2025', 24), ('28/05/2025', 24),
('25/05/2025', 25); 

-- Comentarios
INSERT INTO comentarios (titulo, comentario, ticket_id, usuario_id, cliente_id) VALUES
('Verificaci�n inicial', 'Revis� el punto de red como indicaba el t�tulo y se observa actividad. Podr�a ser un problema intermitente.', 2, 2, null),
('Cambio de impresora', 'Se identific� que el error era por mala conexi�n de red. La impresora ya funciona.', 4, 3, null),
('Prueba de servicio', 'Valid� el ticket y confirm� que la red inal�mbrica tiene cobertura completa en el �rea.', 5, 4, null),
('Atenci�n a solicitud', 'El equipo solicit� la instalaci�n de software especializado. Proced� a coordinar con TI.', 6, 5, null),
('Seguimiento', 'Se revisaron los cables de red y se sustituy� uno da�ado. Se espera respuesta del cliente.', 7, 3, null),
('Verificaci�n inicial', 'Valid� que el proyector est� correctamente instalado, el problema era configuraci�n.', 9, 1, null),
('Revisi�n de hardware', 'Se limpi� la impresora y se comprob� que funciona correctamente.', 10, 2, null),
('Mantenimiento', 'Revis� el ticket de falla en el esc�ner. La l�mpara estaba sucia y ya se limpi�.', 11, 1, null),
('Soluci�n de red', 'Se detect� que la baja velocidad era por configuraci�n err�nea en el switch. Se corrigi�.', 13, 4, null),
('Atenci�n t�cnica', 'Se reinstal� el controlador del esc�ner y funciona bien.', 15, 3, null),
('Validaci�n', 'Verifiqu� que la red tiene conectividad total, tras ajustes en la configuraci�n.', 16, 2, null),
('Mejora aplicada', 'Se aument� la capacidad del servidor como solicitaba el �rea de sistemas.', 17, 5, null),
('Seguimiento del cliente', 'El cliente inform� que persiste el problema. Se hizo ajuste en el router.', 20, 4, null),
('Revisi�n t�cnica', 'Se ajustaron par�metros del proyector y se verific� correcta proyecci�n.', 21, 3, null),
('Revisi�n avanzada', 'El usuario inform� problema intermitente. Se configur� nuevo perfil de red.', 23, 2, null);

--Tareas
INSERT INTO tareas (titulo, descripcion, ticket_id, estado_tarea_id) VALUES
('Revisar punto de red', 'Inspeccionar el cableado y la conectividad del punto de red.', 2, 1),
('Reiniciar impresora', 'Reiniciar la impresora de red y verificar conectividad.', 4, 2),
('Verificar cobertura wifi', 'Revisar y medir la intensidad de la se�al de wifi en la zona del ticket.', 5, 1),
('Instalar software', 'Instalar software especializado requerido por el cliente.', 6, 3),
('Cambiar cable de red', 'Sustituir cable defectuoso detectado en el �rea.', 7, 2),
('Configurar proyector', 'Ajustar configuraci�n del proyector para correcta visualizaci�n.', 9, 1),
('Limpiar impresora', 'Realizar limpieza interna y externa de la impresora para mejorar impresi�n.', 10, 2),
('Limpieza del esc�ner', 'Desarmar y limpiar el esc�ner para eliminar obstrucciones.', 11, 1),
('Ajustar switch', 'Modificar configuraci�n del switch para mejorar velocidad de red.', 13, 3),
('Actualizar driver', 'Actualizar el controlador del esc�ner a la �ltima versi�n.', 15, 2),
('Revisar conectividad', 'Validar conexi�n completa a la red de �rea local.', 16, 1),
('Aumentar capacidad', 'Ampliar capacidad de almacenamiento en el servidor.', 17, 2),
('Configurar router', 'Ajustar configuraci�n de router para mejorar estabilidad.', 20, 3),
('Ajustar proyecci�n', 'Verificar proyecci�n del proyector y calibrar imagen.', 21, 1),
('Configurar perfil de red', 'Crear nuevo perfil de red para solucionar intermitencias.', 23, 2);
