# Challenge
### Introducción
Este proyecto esta basado en C# utilizando librerias  .Net Framework version 4.7.2 para la creacion del servicio, Newtonsoft.json para poder deserializar la informacion obtenida desde la API publica proporcionada.  Tambien se utilizar el paquete Microsoft.Data.SqlClient para la conexion a la base datos local. 

### Ruta de conexion a base de datos

Dentro del proyecto, en la clase Conexion.cs se podra configurar la ruta hacia la BD local en la que queremos volcar los datos de los productos.

    Data Source="nombre servidor Local";Initial Catalog="DBPRUEBAS";Integrated Security=True;TrustServerCertificate=True

*NOTA: Database Security Server hace no Valide el certificado cuando TrustServerCertificate=True. Esto era necesario ya que no me dejaba alojar los datos en la BD*

### Como consumir la API 

URL : [https://apilab.distecna.com/Product](https://apilab.distecna.com)
Parametros: x-apikey 
Valor: 081ae9a0-bb54-4621-8475-9e49f18d2413

### Comando para consumir api desde cualquier consola
`curl -X GET "https://api.distecna.com/Product" -H  "accept: text/plain" -H  "x-apikey: 081ae9a0-bb54-4621-8475-9e49f18d2413"`

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
### Resultados en la BD

Si el servicio fue ejecutado correctamente, en la base de datos local podremos visualizar los datos cargados en la tabla PRODUCT mediante la siguiente consulta apuntando a DBPRUEBAS:
```sql
select * from product
```
### Solicitud al Web Service
En la clase Worker.cs obtenemos los datos que nos brinda la API de la siguiente forma.  Primero se llamamos a la clase HttpClient para hacer las conexiones y luego de esa clase utilizamos el método  GetAsync para hacer la peticion al webService junto con el parametro que tambien tomamos del método client para enviarselo junto con el método Get. Posteriomente si la respuesta es satisfactoria se utilizara la clase ProductListResponse para crear una lista con los productos Deserializados gracias al JsonConvert. Una vez que se termina de armar la lista se recorre la misma producto por producto y en cada iteracion se iran agregando o actualizando los productos segun corresponda.

En caso de no tener un response satisfactorio el mismo se imprime por consola con un mensaje: "Error al consumir Api {response.StatusCode}"

```csharp
private async void GetProduct()
        {   

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-apikey", "081ae9a0-bb54-4621-8475-9e49f18d2413");
                var response = await client.GetAsync("https://apilab.distecna.com/Product");
                if (response.IsSuccessStatusCode)
                {
                    //_logger.LogInformation(await response.Content.ReadAsStringAsync());

                    ProductListResponse products = JsonConvert.DeserializeObject<ProductListResponse>(await response.Content.ReadAsStringAsync());
                    foreach (Product product in products.products)
                    {
                        _logger.LogInformation($"Code: {product.code}, Sku: {product.sku}, Stock: {product.stock}, Currency: {product.currency}, Price: {product.price}, Iva: {product.iva}, Ii: {product.ii}");

                        ProductData.Registrar(product);//Guarda los datos
                        ProductData.Actualizar(product);
                    }

                }
                
                else
                {
                    _logger.LogInformation($"Error al consumir Api {response.StatusCode}");
                }
            }
        }
```

### Configuracion de intervalos de ejecución
Para que el servicio sea configurable para ser ejecutado cada cierto tiempo de forma automática, en la clase Worker.cs  se puede modificar el siguiente método: 

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

Para insertar o actualizar un producto, se dispone tanto del SP registrar y tambien actualizar, los cuales pueden ser llamados desde la clase Worker en el método GetProduct(), dentro del foreach donde se recorre la lista de productos

```csharp
foreach (Product product in products.products)
{
	_logger.LogInformation($"Code: {product.code}, Sku: {product.sku}, Stock: {product.stock}, Currency: {product.currency}, Price: {product.price}, Iva: {product.iva}, Ii: {product.ii}");

	ProductData.Registrar(product);//Guarda los datos
	ProductData.Actualizar(product);
}


```
