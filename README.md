# Challenge
### Introducción
Este proyecto esta basado en C# utilizando librerias  .Net Framework version 4.7.2 para la creacion del servicio, Newtonsoft.json para poder deserializar la informacion obtenida desde la API publica proporcionada.  Tambien se utilizar el paquete Microsoft.Data.SqlClient para la conexion a la base datos local. 

### Ruta de conexion a base de datos

Dentro del proyecto, en la clase Conexion.cs se podra configurar la ruta hacia la BD local en la que queremos volcar los datos de los productos.

    Data Source="nombre servidor Local";Initial Catalog="DBPRUEBAS";Integrated Security=True;TrustServerCertificate=True

*NOTA: Database Security Server hace no Valide el certificado cuando TrustServerCertificate=True. Esto era necesario ya que no me dejaba alojar los datos en la BD*

### Creación de la base de datos local y la tabla

Para crear la base local en la cual vamos a volcar los datos de que la API nos provee necesitamos ejecutar en SQL Server Managment lo siguiente:
```sql
use master
go
IF NOT EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE NAME = 'DBPRUEBAS')
CREATE DATABASE DBPRUEBAS

GO 

USE DBPRUEBAS

GO

if not exists (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'PRODUCT')
create table PRODUCT(
Id int primary key identity(1,1),
Code varchar(60),
Sku varchar(60),
Stock varchar(60),
Currency varchar(60),
Price varchar(60),
Iva varchar(60),
Ii varchar(60)
)

go

select * from dbo.product
```

### Creacion de los SP: registrar - actualizar - obtener - listar - eliminar
El siguiente codigo sirve para la creacion de los SP dentro de la base DBPRUEBAS que en el caso de que ya existan los eliminara y volvera a crear 

```sql
go
use DBPRUEBAS
go
--************************ VALIDAMOS SI EXISTE EL PROCEDIMIENTO ************************--

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_registrar')
DROP PROCEDURE usp_registrar

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_actualizar')
DROP PROCEDURE usp_actualizar

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_obtener')
DROP PROCEDURE usp_obtener

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_listar')
DROP PROCEDURE usp_listar

go

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_eliminar')
DROP PROCEDURE usp_eliminar

go

--************************ PROCEDIMIENTOS PARA CREAR ************************--


create procedure usp_registrar(
@code varchar(60),
@sku varchar(60),
@stock varchar(60),
@currency varchar(60),
@price varchar(60),
@iva varchar(60),
@ii varchar(60)
)
as
begin

IF NOT EXISTS (SELECT * FROM PRODUCT WHERE @code = Code)

insert into PRODUCT(Code,Sku,Stock,Currency,Price,Iva,Ii)
values
(
@code,
@sku,
@stock,
@currency,
@price,
@iva,
@ii
)

end


go

create procedure usp_actualizar(
@code varchar(60),
@sku varchar(60),
@stock varchar(60),
@currency varchar(60),
@price varchar(60),
@iva varchar(60),
@ii varchar(60)
)
as
begin

IF EXISTS (SELECT * FROM PRODUCT WHERE @code = Code)
update PRODUCT set 
Code = @code,
Sku = @sku,
Stock = @stock,
Currency = @currency,
Price = @price,
Iva = @iva,
Ii = @ii

where Code = @code
end

go

create procedure usp_obtener(@code int)
as
begin

select * from PRODUCT where Code = @code
end

go
create procedure usp_listar
as
begin

select * from PRODUCT
end


go

go

create procedure usp_eliminar(
@code int
)
as
begin

delete from PRODUCT where Code = @code

end

go
```
### Resultados

Si el servicio fue ejecutado correctamente, en la base de datos local podremos visualizar los datos cargados en la tabla PRODUCT mediante la siguiente consulta:
```sql
select * from product
```

### Configuracion de intervalos de ejecución
Para que el servicio sea configurable para ser ejecutado cada cierto tiempo de forma automática, en la clase Worker.cs  se puede modificar el siguiente metodo: 

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {   
            while (!stoppingToken.IsCancellationRequested)
            {
                // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                GetProduct();
                await Task.Delay(600000, stoppingToken); // 10 minutos = 600000 milisegundos 
            }
        }
```
### Insertar o actualizar productos

Para insertar o actualizar un producto, se dispone tanto del SP registrar y tambien actualizar, los cuales pueden ser llamados desde la clase Worker en el metodo GetProduct(), dentro del foreach donde se recorre la lista de productos

```csharp
foreach (Product product in products.products)
{
	_logger.LogInformation($"Code: {product.code}, Sku: {product.sku}, Stock: {product.stock}, Currency: {product.currency}, Price: {product.price}, Iva: {product.iva}, Ii: {product.ii}");

	ProductData.Registrar(product);//Guarda los datos
	ProductData.Actualizar(product);
}
```
