using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Scamark.Microservice.OData;

internal class PropertySelectorVisitor : ExpressionVisitor
{
    private List<PropertyInfo> _properties = new List<PropertyInfo>();

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Class is internal, virtual call okay")]
    internal PropertySelectorVisitor(Expression exp)
    {
        Visit(exp);
    }

    public PropertyInfo Property
    {
        get
        {
            return _properties.SingleOrDefault();
        }
    }

    public ICollection<PropertyInfo> Properties
    {
        get
        {
            return _properties;
        }
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        var pinfo = node.Member as PropertyInfo;
        if (pinfo == null)
        {
            throw new InvalidOperationException();
        }

        if (node.Expression.NodeType != ExpressionType.Parameter)
        {
            throw new InvalidOperationException();
        }

        _properties.Add(pinfo);
        return node;
    }

    public static PropertyInfo GetSelectedProperty(Expression exp)
    {
        return new PropertySelectorVisitor(exp).Property;
    }

    public static ICollection<PropertyInfo> GetSelectedProperties(Expression exp)
    {
        return new PropertySelectorVisitor(exp).Properties;
    }

    public override Expression Visit(Expression exp)
    {
        if (exp == null)
        {
            return exp;
        }

        switch (exp.NodeType)
        {
            case ExpressionType.New:
            case ExpressionType.MemberAccess:
            case ExpressionType.Lambda:
                return base.Visit(exp);
            default:
                throw new InvalidOperationException();
        }
    }

    protected override Expression VisitLambda<T>(Expression<T> lambda)
    {
        if (lambda == null)
        {
            throw new ArgumentNullException("lambda");
        }

        if (lambda.Parameters.Count != 1)
        {
            throw new InvalidOperationException();
        }

        var body = Visit(lambda.Body);

        if (body != lambda.Body)
        {
            return Expression.Lambda(lambda.Type, body, lambda.Parameters);
        }
        return lambda;
    }
}
