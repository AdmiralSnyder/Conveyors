using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace AutomationObjectGenerator
{
    [Generator]
    public class Class1 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var oneWhiteSpace = SyntaxFactory.Whitespace(" ");

            var partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(oneWhiteSpace);
            var publicToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(oneWhiteSpace);

            var getSet = SyntaxFactory.AccessorList(new(new[]
            {
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithTrailingTrivia(oneWhiteSpace)
                .WithLeadingTrivia(oneWhiteSpace),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithTrailingTrivia(oneWhiteSpace),
            }));

            var automationClasses = initContext.SyntaxProvider.ForAttributeWithMetadataName("WpfApp1.Generate2Attribute`1",
                (sn, ct) => true,
                //!++ TODO do not let the node escape!
                (gasc, ct) =>
                {
                    var interfaceSymbol = gasc.Attributes.First().AttributeClass.TypeArguments.FirstOrDefault();

                    MemberDeclarationSyntax ModifyMember(MemberDeclarationSyntax member)
                    {
                        member = FullyQualifyTypes(member);

                        if (member is MethodDeclarationSyntax method)
                        {
                            member = method.WithSemicolonToken(default);
                        }

                        //member = member.AddModifiers(publicToken);

                        if (member is PropertyDeclarationSyntax property)
                        {
                            member = property.WithAccessorList(null);
                        }
                        member = member.WithoutTrivia();

                        return member;
                    }

                    T WithTrivia<T>(T node, T target) where T : SyntaxNode => node
                        .WithLeadingTrivia(target.GetLeadingTrivia())
                        .WithTrailingTrivia(target.GetTrailingTrivia());


                    MemberDeclarationSyntax FullyQualifyTypes(MemberDeclarationSyntax memberDeclarationSyntaxNode)
                    {
                        TypeSyntax GetFullyQualifiedType(TypeSyntax ts)
                        => WithTrivia(SyntaxFactory.ParseTypeName(gasc.SemanticModel.GetTypeInfo(ts).Type.ToDisplayString()), ts);

                        //EventFieldDeclarationSyntax
                        if (memberDeclarationSyntaxNode is FieldDeclarationSyntax fds)
                        {
                            return fds.WithDeclaration(fds.Declaration.WithType(GetFullyQualifiedType(fds.Declaration.Type)));
                        }
                        else if (memberDeclarationSyntaxNode is MethodDeclarationSyntax mds)
                        {
                            TypeSyntax? returnType = default;
                            if (mds.ReturnType is { } rt)
                            {
                                returnType = GetFullyQualifiedType(mds.ReturnType);
                            }

                            mds = mds.WithParameterList(SyntaxFactory.ParameterList().AddParameters(mds.ParameterList.Parameters.Select(p => p.WithType(GetFullyQualifiedType(p.Type))).ToArray()));

                            if (returnType is { })
                            {
                                mds = mds.WithReturnType(returnType);
                            }
                            return mds;
                        }
                        else if (memberDeclarationSyntaxNode is PropertyDeclarationSyntax pds)
                        {
                            return pds.WithType(GetFullyQualifiedType(pds.Type)
                                .WithLeadingTrivia(pds.Type.GetLeadingTrivia()));
                        }

                        // KnownUIContexts.SolutionBuildingContext.UIContextChanged

                        //return memberDeclarationSyntaxNode.with

                        //return memberDeclarationSyntaxNode.WithLeadingTrivia(SyntaxFactory.Comment($"/*{memberDeclarationSyntaxNode.GetType().Name}*/"));

                        return memberDeclarationSyntaxNode;
                    }

                    var members = (interfaceSymbol.DeclaringSyntaxReferences.First().GetSyntax() as InterfaceDeclarationSyntax).Members.Select(ModifyMember);
                    var membersTexts = (members.Select(x => x.ToFullString()));
                    //members = members.WithLeadingTrivia(SyntaxFactory.Comment("KEKSE"));
                    //var interfaceSymbol = gasc.SemanticModel.GetDeclaredSymbol(interfaceSyntaxNode);
                    return
                    (
                        OK: gasc.TargetNode is ClassDeclarationSyntax cds,
                        Interface: interfaceSymbol.ToDisplayString(),
                        TargetNode: "Blub",
                        UsingDeclarations: "",
                        Properties: members.OfType<PropertyDeclarationSyntax>().Select(x => (Name: x.Identifier.Text, Declaration: x.ToFullString())).ToList(),
                        Methods: members.OfType<MethodDeclarationSyntax>().Select(x => (Name: x.Identifier.Text, Declaration: x.ToFullString(), Arguments: x.ParameterList.Parameters.Select(p => p.Identifier.Text).ToList())).ToList(),
                        TargetNameSpace: gasc.SemanticModel.GetDeclaredSymbol((gasc.TargetNode as ClassDeclarationSyntax))?.ContainingNamespace.Name,
                        TargetClassName: (gasc.TargetNode as ClassDeclarationSyntax)?.Identifier.Text
                    );
                });

            initContext.RegisterSourceOutput(automationClasses,
            (spc, tuple) =>
            {
                spc.AddSource(tuple.Interface + ".g.cs",
                    tuple.OK ?
                    $$"""
                    namespace {{tuple.TargetNameSpace}};

                    public partial class {{tuple.TargetClassName}} : {{tuple.Interface}}
                    {
                        {{tuple.Properties.Select(p => $"public {p.Declaration} {{ get; set; }}").JoinI1()}}

                        {{tuple.Methods.Select(m => $"public partial {m.Declaration};").JoinI1()}}

                        public static {{tuple.Interface}} CreateAutomationObject() => CreateAutomationObject(out _);

                        public static {{tuple.Interface}} CreateAutomationObject(out IAutomationContext context)
                        {
                            var result = new Automation{{tuple.TargetClassName}}<{{tuple.TargetClassName}}>();
                            context = result;
                            return result;
                        }
                    }



                    public partial class Automation{{tuple.TargetClassName}}<TWrapper> : {{tuple.Interface}}, IAutomationRoot, IAutomationContext
                    where TWrapper : {{tuple.Interface}}, IAutomationRoot, new()
                    {
                        private TWrapper AutomationObject { get; } = new();
                    
                        public void Init(object obj) => AutomationObject.Init(obj);

                        public bool IsAutomated { get; set; }
                        public Action<string> LogAction { get; set; }
                        
                        {{tuple.Properties.Select(p => $"public {p.Declaration} => AutomationObject.{p.Name}").JoinI1()}}
                    
                    {{tuple.Methods.Select(m => $$"""
                            public {{m.Declaration}}
                            {
                                                //, [CallerArgumentExpression("points")] string pointsArg = null
                                                //, [CallerArgumentExpression("isRunning")] string isRunningArg = null
                                                //, [CallerArgumentExpression("lanes")] string lanesArg = null
                                if (!IsAutomated)
                                {
                                    LogAction?.Invoke($"$.{{m.Name}}({{m.Arguments.Select(a => $"{{{a}.Out()}}").JoinComma()}})");
                                }
                                return AutomationObject.{{m.Name}}({{m.Arguments.JoinComma()}});
                            }
                        """).JoinNL()}}
                    }
                    """
                : "// ERROR IN GENERATOR");
            });
        }
    }

    public static class Tools
    {
        public static string JoinComma(this IEnumerable<string> strings) => string.Join(", ", strings);
        public static string JoinNL(this IEnumerable<string> strings) => string.Join(Environment.NewLine, strings);
        public static string JoinI1(this IEnumerable<string> strings) => string.Join(@"
    ", strings);
        public static string JoinI2(this IEnumerable<string> strings) => string.Join(@"
        ", strings);
        public static string JoinI3(this IEnumerable<string> strings) => string.Join(@"
        ", strings);

        public static string Trace(this string str)
        {
            File.WriteAllText(@"T:\GeneratorTrace\trace.txt", str);
            //System.Diagnostics.Trace.WriteLine(str);
            return str;
        }
    }
}