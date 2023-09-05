using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutomationObjectGenerator
{
    internal record MethodDeclarationInfo
    (
        string ReturnType,
        string Name, 
        string Declaration,
        string[]/*ImmutableArray<string>*/ Arguments,
        string[]/*ImmutableArray<string>*/ ArgumentTypes,
        string[]/*ImmutableArray<string>*/ ArgumentNames,
        bool IsVoid
    )
    {
        public MethodDeclarationInfo(MethodDeclarationSyntax syntax) : this
        (
            syntax.ReturnType.ToFullString(),
            syntax.Identifier.Text,
            syntax.ToFullString(),
            syntax.ParameterList.Parameters.Select(p => p.ToFullString()).ToArray(),
            syntax.ParameterList.Parameters.Select(p => p.Type.ToFullString()).ToArray(),
            syntax.ParameterList.Parameters.Select(p => p.Identifier.Text).ToArray(),
            syntax.ReturnType is PredefinedTypeSyntax pds && pds.Keyword.IsKind(SyntaxKind.VoidKeyword)
        )
        { }
    }
}