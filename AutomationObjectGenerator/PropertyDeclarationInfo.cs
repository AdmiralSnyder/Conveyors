using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutomationObjectGenerator;

[GenerateLambda]
internal record struct PropertyDeclarationInfo(string Name, string Declaration)
{
    public PropertyDeclarationInfo(PropertyDeclarationSyntax syntax) : this(syntax.Identifier.Text, syntax.ToFullString()) { }
}