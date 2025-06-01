create table Usuarios
(
Id int not null primary key,
IdPerfil int null,
Login varchar(50) not null,
Senha varchar(max) null,
PrimeiroAcesso bit not null,
Email varchar(100) null,
Nome varchar(100) null,
Foto image null,
)

create table Motor
(
Id int not null primary key,
Modelo varchar(100) null,
Fabricante varchar(100) null,
Nome varchar(100) null,
TemperaturaSecagem decimal(18,2) null,
NumeroDeSerie varchar(50) null
)

create table Simulacao
(
Id int not null primary key,
Nome varchar(100) null,
DataCriacaoAlteracao datetime null,
IdMotor int not null,
IdUsuario int not null
)
alter table Simulacao add constraint FK_IdMotor Foreign Key(IdMotor) references Motor(Id)
alter table Simulacao add constraint FK_IdUsuario Foreign Key(IdUsuario) references Usuarios(Id)

create table Perfil
(
Id int not null primary key,
Nome varchar(100) not null,
Permissoes varchar(max) null,
)

