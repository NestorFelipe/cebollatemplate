using Domain.Entities.Atributes;
using Domain.Entities.BaseModel;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Infraestructure.Persistence.Contexts.Generic;

public class BaseContext : DbContext
{

    public BaseContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Property(x => x.FechaCreacion).IsModified = false;
                    entry.Property(x => x.UsuarioCreacion).IsModified = false;
                    break;
                default:
                    break;
            }

        }



        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Obtener un listado a partir de una tabla funcion postgresql
    /// </summary>
    /// <typeparam name="T">Estructura exacta de la tabla funcion, esta debe contener un campo de tipo unica como llave primaria</typeparam>
    /// <param name="param">Listado de parametros de la funcion tabla, debe tener el orden y el tipo exacto de la tabla funcion</param>
    /// <returns>Listado de entidades con la estructura especificada</returns>
    /// <exception cref="NullReferenceException">El error se da por que no se definio el atributo NameFunction en el modelo o entidad</exception>
    public IQueryable<T> GetFunction<T>(params object[] param) where T : FunctionBaseTable
    {
        var vQuery = string.Empty;
        var atributo = typeof(T).GetCustomAttribute<NameFunction>()!;

        try
        {
            if (atributo != null)
            {
                vQuery = $"SELECT * FROM {atributo.Name}";

                if (param.Length == 0)
                    return Set<T>().FromSqlRaw(string.Concat(vQuery, "()"));

                var vStr = "(";
                for (int i = 0; i < param.Length; i++)
                {
                    vStr += "{" + i.ToString() + "}, ";
                }
                vStr = vStr.Substring(0, vStr.Length - 2) + ")";

                vQuery += vStr;
            }
            else
            {
                throw new NullReferenceException("Debe definir el nombre del procedimiento almacenado desde el atributo NameFunction de la entidad");
            }

            return Set<T>().FromSqlRaw(string.Format(vQuery, param));
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<int> InsertMergeAsync<T>(IEnumerable<T> entities, string[] keyColumns, string currentUser) where T : AuditableBaseEntity
    {
        if (entities == null || !entities.Any())
            throw new ArgumentException("La lista de entidades no puede estar vacía.", nameof(entities));

        if (keyColumns == null || keyColumns.Length == 0)
            throw new ArgumentException("Debe proporcionar al menos una columna clave.", nameof(keyColumns));

        if (string.IsNullOrEmpty(currentUser))
            throw new ArgumentException("El usuario actual no puede estar vacío.", nameof(currentUser));

        // Obtener el nombre de la tabla desde el decorador [Table]
        var tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
        if (string.IsNullOrEmpty(tableName))
            throw new InvalidOperationException($"La entidad {typeof(T).Name} no tiene un decorador [Table] definido.");

        // Obtener las propiedades de la entidad excluyendo claves primarias y campos de auditoría
        var type = typeof(T);
        var properties = type.GetProperties()
            .Where(p => p.CanRead && !p.GetMethod!.IsStatic) // Propiedades legibles
            .Where(p => p.GetCustomAttribute<KeyAttribute>() == null) // Excluir claves primarias
            .Where(p => !new[] { nameof(AuditableBaseEntity.FechaModificacion), nameof(AuditableBaseEntity.UsuarioModificacion) }.Contains(p.Name)) // Excluir campos de auditoría
            .ToArray();

        if (!properties.Any())
            throw new InvalidOperationException("No se encontraron propiedades para insertar.");

        // Conversión de CamelCase a snake_case
        string ConvertToSnakeCase(string input) =>
            string.Concat(input.Select((c, i) =>
                i > 0 && char.IsUpper(c) ? $"_{char.ToLower(c)}" : char.ToLower(c).ToString()));

        // Generar lista de columnas en snake_case
        var columns = string.Join(", ", properties.Select(p => ConvertToSnakeCase(p.Name)));

        // Generar valores dinámicamente
        foreach (var entity in entities)
        {
            entity.FechaCreacion = DateTime.UtcNow;
            entity.UsuarioCreacion = currentUser;
        }

        var valores = string.Join(", ",
            entities.Select(entity =>
            {
                var propertyValues = properties
                    .Select(p =>
                    {
                        var value = p.GetValue(entity);
                        return value switch
                        {
                            string str => $"'{str.Replace("'", "''")}'",
                            DateTime dt => $"'{dt:yyyy-MM-dd}'",
                            null => "NULL",
                            _ => value.ToString()
                        };
                    });

                return $"({string.Join(", ", propertyValues)})";
            }));

        // Crear las claves únicas en snake_case
        var conflictColumns = string.Join(", ", keyColumns.Select(ConvertToSnakeCase));

        // SQL dinámico para insertar y manejar conflictos
        var sqlInsert = $@"
        INSERT INTO {tableName} ({columns})
        VALUES {valores}
        ON CONFLICT ({conflictColumns}) DO NOTHING";

        //// SQL dinámico para contar los registros insertados
        //var sqlCount = $@"
        //SELECT COUNT(*) 
        //FROM (VALUES {valores}) AS temp({columns})
        //WHERE NOT EXISTS (
        //    SELECT 1 
        //    FROM {tableName} 
        //    WHERE {string.Join(" AND ", keyColumns.Select(k => $"temp.{ConvertToSnakeCase(k)} = {tableName}.{ConvertToSnakeCase(k)}"))}
        //)";

        // Insertar registros y contar el total de nuevos
        //await Database.ExecuteSqlRawAsync(sqlInsert);
        var totalInserted = await Database.ExecuteSqlRawAsync(sqlInsert);

        return totalInserted;
    }

    public async Task<List<T>> InsertBulk<T>(List<T> entities, string currentUser) where T : AuditableBaseEntity
    {
        List<T> registrosInsertados = new List<T>();
        try
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("La lista de entidades no puede estar vacía.", nameof(entities));

            if (string.IsNullOrEmpty(currentUser))
                throw new ArgumentException("El usuario actual no puede estar vacío.", nameof(currentUser));

            // Obtener el nombre de la tabla desde el decorador [Table]
            var tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidOperationException($"La entidad {typeof(T).Name} no tiene un decorador [Table] definido.");

            var type = typeof(T);
            var properties = type.GetProperties()
                .Where(p => p.CanRead && !p.GetMethod!.IsStatic) // Propiedades legibles
                .Where(p => p.GetCustomAttribute<KeyAttribute>() == null) // Excluir claves primarias
                .Where(p => !new[] { nameof(AuditableBaseEntity.FechaModificacion), nameof(AuditableBaseEntity.UsuarioModificacion) }.Contains(p.Name)) // Excluir campos de auditoría
                .Where(p => !typeof(IEnumerable).IsAssignableFrom(p.PropertyType) || p.PropertyType == typeof(string)) // Excluir listas, pero permitir strings
                .Where(p => !p.GetMethod!.IsVirtual) // Excluir propiedades virtuales
                .ToArray();

            if (!properties.Any())
                throw new InvalidOperationException("No se encontraron propiedades para insertar.");

            // Conversión de CamelCase a snake_case
            string ConvertToSnakeCase(string input) =>
                string.Concat(input.Select((c, i) =>
                    i > 0 && char.IsUpper(c) ? $"_{char.ToLower(c)}" : char.ToLower(c).ToString()));

            // Generar lista de columnas en snake_case
            var columns = string.Join(", ", properties.Select(p => ConvertToSnakeCase(p.Name)));

            // seteo de valores de auditoría
            entities = entities.Select(ent => { ent.FechaCreacion = DateTime.Now; ent.UsuarioCreacion = currentUser; return ent; }).ToList();

            var primaryKeyProperty = type.GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);

            var outputColumns = properties.Select(p => $"INSERTED.{ConvertToSnakeCase(p.Name)}").ToList();

            // Agrega la columna `IDENTITY` al `OUTPUT`
            if (primaryKeyProperty != null)
            {
                outputColumns.Insert(0, $"INSERTED.{ConvertToSnakeCase(primaryKeyProperty.Name)}");
            }

            outputColumns.Insert(0, $"INSERTED.fecha_modificacion");
            outputColumns.Insert(0, $"INSERTED.usuario_modificacion");

            // Definir el tamaño del lote (1000 filas por inserción)
            const int batchSize = 1000;
            int totalBatches = (int)Math.Ceiling((double)entities.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                // Obtener el lote actual
                var batch = entities.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                // Construir valores para el lote
                var valores = string.Join(", ",
                    batch.Select(entity =>
                    {
                        var propertyValues = properties
                            .Select(p =>
                            {
                                var value = p.GetValue(entity);
                                return value switch
                                {
                                    string str => $"'{str.Replace("'", "''")}'",
                                    DateTime dt => $"'{dt:yyyy-MM-dd}'",
                                    null => "NULL",
                                    _ => value.ToString()
                                };
                            });

                        return $"({string.Join(", ", propertyValues)})";
                    }));

                // Construir la consulta de inserción masiva para el lote
                var query = $@"INSERT INTO [dbo].[{tableName}] ({columns})
                           OUTPUT {string.Join(", ", outputColumns)} 
                           VALUES {string.Join(", ", valores)}";

                // Ejecutar la consulta SQL para el lote actual y mapear los resultados
                var rawResults = await Set<T>().FromSqlRaw(query).ToListAsync();

                // Guardar los registros insertados en la lista general
                registrosInsertados.AddRange(rawResults);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message); // Relanzamos la excepción
        }

        return registrosInsertados;
    }

}
