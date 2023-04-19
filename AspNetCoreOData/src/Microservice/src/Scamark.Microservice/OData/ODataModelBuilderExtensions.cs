using System.Linq.Expressions;
using Microsoft.OData.ModelBuilder;

namespace Scamark.Microservice.OData;

public static class ODataModelBuilderExtensions
{
    private const int DefaultPageSize = 1000;

    public static EntityTypeConfiguration<T> CreateDefaultEntity<T>(this ODataModelBuilder builder) where T : class
    {
        return builder.CreateEntity<T>(typeof(T).Name).ConfigurePage();
    }

    public static EntityTypeConfiguration<T> CreateEntity<T>(this ODataModelBuilder builder) where T : class
    {
        return builder.CreateEntity<T>(typeof(T).Name);
    }

    public static EntityTypeConfiguration<T> CreateEntity<T>(this ODataModelBuilder builder, string name) where T : class
    {
        return builder.EntitySet<T>(name).EntityType;
    }

    public static EntityTypeConfiguration<T> ConfigurePage<T>(this EntityTypeConfiguration<T> entity) where T : class
    {
        return entity.ConfigurePage(DefaultPageSize);
    }

    public static EntityTypeConfiguration<T> ConfigurePage<T>(this EntityTypeConfiguration<T> entity, int? pageSize) where T : class
    {
        entity.Filter().Count().Page(maxTopValue: pageSize * 10, pageSizeValue: pageSize);
        return entity;
    }

    public static FunctionConfiguration AddCollectionFunction<T>(this EntityTypeConfiguration<T> entity, string name) where T : class
    {
        return entity.Collection.Function(name);
    }

    public static ActionConfiguration AddCollectionAction<T>(this EntityTypeConfiguration<T> entity, string name) where T : class
    {
        return entity.Collection.Action(name);
    }

    public static FunctionConfiguration ReturnsFromEntitySet<T>(this FunctionConfiguration function) where T : class
    {
        return function.ReturnsFromEntitySet<T>(typeof(T).Name);
    }

    public static FunctionConfiguration ReturnsCollectionFromEntitySet<T>(this FunctionConfiguration function) where T : class
    {
        return function.ReturnsCollectionFromEntitySet<T>(typeof(T).Name);
    }

    public static ActionConfiguration ReturnsCollectionFromEntitySet<T>(this ActionConfiguration function) where T : class
    {
        return function.ReturnsCollectionFromEntitySet<T>(typeof(T).Name);
    }

    public static FunctionConfiguration AddParameter<T>(this FunctionConfiguration function, string name)
    {
        function.Parameter<T>(name).Required();
        return function;
    }

    public static FunctionConfiguration AddParameter<T>(this FunctionConfiguration function, string name, string defaultValue)
    {
        function.Parameter<T>(name).Optional().HasDefaultValue(defaultValue);
        return function;
    }

    public static FunctionConfiguration AddCollectionParameter<T>(this FunctionConfiguration function, string name)
    {
        function.CollectionParameter<T>(name);
        return function;
    }

    public static ActionConfiguration AddParameter<T>(this ActionConfiguration action, string name)
    {
        action.Parameter<T>(name).Required();
        return action;
    }

    //
    // Summary:
    //     Configures the key property(s) for this entity type.
    //
    // Parameters:
    //   keyDefinitionExpression:
    //     A lambda expression representing the property to be used as the primary key.
    //     The order is set with the order of the properties itself.
    //     For example, in C# t => t.Id.
    //
    // Type parameters:
    //   TKey:
    //     The type of key.
    //
    // Returns:
    //     Returns itself so that multiple calls can be chained.
    public static EntityTypeConfiguration<TEntityType> HasOrderedKey<TEntityType, TKey>(
        this EntityTypeConfiguration<TEntityType> source,
        Expression<Func<TEntityType, TKey>> keyDefinitionExpression) where TEntityType : class
    {
        var properties = PropertySelectorVisitor.GetSelectedProperties(keyDefinitionExpression).ToArray();
        int order = 1;

        foreach (var selectedProperty in properties)
        {
            PropertyConfiguration property = null;

            if (selectedProperty.PropertyType == typeof(int))
            {
                var func = CreatePropertyAccessor<TEntityType, int>(selectedProperty.Name);
                property = source.Property(func);
            }
            else if (selectedProperty.PropertyType == typeof(string))
            {
                var func = CreatePropertyAccessor<TEntityType, string>(selectedProperty.Name);
                property = source.Property(func);
            }
            else if (selectedProperty.PropertyType == typeof(byte))
            {
                var func = CreatePropertyAccessor<TEntityType, byte>(selectedProperty.Name);
                property = source.Property(func);
            }

            if (property != null)
            {
                // se base sur l'ordre de la propriété déclaré
                property.Order = order++;
            }
            else
            {
                throw new InvalidOperationException($"HasOrderedKey cannot be called on a key of type {selectedProperty.PropertyType.FullName}, only int / string  byte are supported.");
            }
        }

        var entityConfiguration = source.HasKey(keyDefinitionExpression);

        return entityConfiguration;
    }

    private static Expression<Func<TIn, TOut>> CreatePropertyAccessor<TIn, TOut>(string propertyName)
    {
        var param = Expression.Parameter(typeof(TIn));
        var body = Expression.PropertyOrField(param, propertyName);
        return Expression.Lambda<Func<TIn, TOut>>(body, param);
    }
}
