using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.UriParser;
using System;

namespace Microsoft.AspNetCore.OData.WebHost.Tools;

public static class ODataQueryOptionsExtension
{
    public static bool FilterContainsDynamicProperties<T>(this ODataQueryOptions<T> options)
    {
        bool isDynamicPropertyFound = false;

        if (options.Filter != null)
        {
            var expression = options.Filter.FilterClause.Expression;

            // Verification que la propriété n'existe pas dans T (peut être pas utile si le fait d'être un ConvertNode suffit à le definir comme un noeud de dictionnaire)
            var typeAttribute = typeof(T);

            isDynamicPropertyFound = FindPropertyInExpression(expression, (nodeWithProperty) =>
            {
                // on verifie si la propriété existe par reflexion
                if (typeAttribute.GetProperty(nodeWithProperty.Name) == null)
                {
                    // on ne continue pas
                    return false;
                }

                // on continue
                return true;
            });
        }

        return isDynamicPropertyFound;
    }

    /// <summary>
    /// Parcours récursivement les elements de l'expression stocké sous forme d'arbre binaire
    /// </summary>
    /// <param name="nodeExpression"></param>
    /// <param name="nodeWithProperty"></param>
    /// <returns></returns>

    private static bool FindPropertyInExpression(QueryNode nodeExpression, Func<SingleValueOpenPropertyAccessNode, bool> nodeWithProperty)
    {
        // Propriété
        if(nodeExpression is SingleValueOpenPropertyAccessNode)
        {
            bool mustContinue = nodeWithProperty.Invoke((SingleValueOpenPropertyAccessNode)nodeExpression);
        
            if(mustContinue == false)
            {
                // c'est trouvé
                return true;
            }
        }
        // contient les paramètres d'une fonction
        else if (nodeExpression is SingleValueFunctionCallNode)
        {
            var source = (SingleValueFunctionCallNode)nodeExpression;

            foreach (QueryNode parameter in source.Parameters)
            {
                // on continue le foreach tant que FindExpression renvoie false
                if( FindPropertyInExpression(parameter, nodeWithProperty) )
                {
                    // sinon on quitte
                    return true;
                }
            }
        }
        // Par exemple And
        else if(nodeExpression is BinaryOperatorNode) 
        {
            var binaryOperatorNode = (BinaryOperatorNode)nodeExpression;

            if (FindPropertyInExpression(binaryOperatorNode.Left, nodeWithProperty))
            {
                // sinon on quitte
                return true;
            }

            if (FindPropertyInExpression(binaryOperatorNode.Right, nodeWithProperty))
            {
                // sinon on quitte
                return true;
            }
        }
        // Fonction par exemple Contains
        else if(nodeExpression is ConvertNode)
        {
            var convertNode = (ConvertNode)nodeExpression;

            if (FindPropertyInExpression(convertNode.Source, nodeWithProperty))
            {
                // sinon on quitte
                return true;
            }
        }

        return false;
    }
}
