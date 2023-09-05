using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace AutomationObjectGenerator;

/// <summary>
/// Creates Constructor Wrappers for this record's ctors in the Lambdas class.
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public class GenerateLambdaAttribute : Attribute { }

//!++ TODO Generate these ctor wrappers according to [GenerateLambda] attribute
internal static partial class Lambdas
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationInfo NewPropertyDeclarationInfo(PropertyDeclarationSyntax syntax) => new(syntax);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodDeclarationInfo NewMethodDeclarationInfo(MethodDeclarationSyntax syntax) => new(syntax);
}