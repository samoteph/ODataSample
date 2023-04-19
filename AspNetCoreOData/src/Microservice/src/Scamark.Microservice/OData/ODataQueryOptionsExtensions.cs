using System.Text;
using System.Text.RegularExpressions;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNetCore.OData.Query;

/// <summary>
/// Cette classe permet de gérer la correction des paramètres OData
/// Dont le mapping est différent
/// Cela transforme le filtre
/// id_decis => Id_Decis
/// </summary>
public static class ODataQueryOptionsExtensions
{
    public static IDictionary<string, string> ToSapQuery(this ODataQueryOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var dict = new Dictionary<string, string>();
        var filterExpr = options.Filter?.FilterClause?.Expression as BinaryOperatorNode;
        var filterRawValue = options.Filter?.RawValue;

        if (filterExpr != null && filterRawValue != null)
        {
            Traverse(filterExpr, ref filterRawValue);
        }

        if (options.Filter != null)
        {
            dict.Add("$filter", filterRawValue);
        }
        if (options.Skip != null)
        {
            dict.Add("$skip", options.Skip.Value.ToString());
        }
        if (options.Top != null)
        {
            dict.Add("$top", options.Top.Value.ToString());
        }
        else
        {
            dict.Add("$top", Scamark.Microservice.OData.Constants.CurrentMaxTop.ToString());
        }
        if (options.Count?.Value == true)
        {
            // c'est le bon paramètre pour un service OData V2
            dict.Add("$inlinecount", "allpages");
        }
        if (options.OrderBy != null)
        {
            // todo : faire le mapping sur orderby aussi
            dict.Add("$orderBy", options.OrderBy.RawValue.ToString());
        }
        if (options?.SelectExpand?.RawSelect != null)
        {
            dict.Add("$select", options?.SelectExpand?.RawSelect);
        }
        return dict;
    }

    private static void Traverse(BinaryOperatorNode node, ref string filterRawValue)
    {
        SingleValueNode current = node;
        var stack = new Stack<SingleValueNode>();

        while (current != null || stack.Any() == true)
        {
            while (current != null)
            {
                stack.Push(current);
                current = (current as BinaryOperatorNode)?.Left;
            }

            current = stack.Pop();
            if (current.Kind == QueryNodeKind.SingleValuePropertyAccess)
            {
                FixSapPropertyName(current as SingleValuePropertyAccessNode, ref filterRawValue);
            }
            else if (current.Kind == QueryNodeKind.Convert && (current as ConvertNode).Source.Kind == QueryNodeKind.SingleValuePropertyAccess)
            {
                FixSapPropertyName((current as ConvertNode).Source as SingleValuePropertyAccessNode, ref filterRawValue);
            }
            current = (current as BinaryOperatorNode)?.Right;
        }
    }

    private static void FixSapPropertyName(SingleValuePropertyAccessNode node, ref string filterRawValue)
    {
        var fixedPropertyName = ToSapCase(node.Property.Name);
        var findPattern = @$"( |^){node.Property.Name} ";
        filterRawValue = Regex.Replace(filterRawValue, findPattern, "$1" + fixedPropertyName + " ", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Met la première lettre de chaque mot en majuscule, afin que cela match
    /// avec la casse des propriétés SAP
    ///     numcent => Numcent
    ///     id_decis => Id_Decis
    /// </summary>
    /// <param name="s">La chaine de caractères source.</param>
    /// <returns>La chaine de caractères transformée</returns>
    private static string ToSapCase(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        var sb = new StringBuilder(s);
        sb[0] = char.ToUpper(sb[0]);
        for (var i = 1; i < sb.Length; i++)
        {
            if (sb[i] == '_' && (i + 1) < sb.Length)
            {
                sb[i + 1] = char.ToUpper(sb[i + 1]);
            }
        }
        return sb.ToString();
    }
}
