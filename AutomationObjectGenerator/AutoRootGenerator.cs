using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutomationObjectGenerator;

using static AutomationObjectGenerator.Lambdas;

[Generator]
public class AutoRootGenerator : IIncrementalGenerator
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

        //var automationClassesNoFAWMN = initContext.SyntaxProvider.CreateSyntaxProvider(
        //    (sn, ct) => sn is ClassDeclarationSyntax cds && cds.AttributeLists.Any(al => al.Attributes.Any(a => a.Name is GenericNameSyntax gns 
        //    && gns.Identifier.Text.StartsWith("Generate2") && gns.TypeArgumentList.Arguments.Count == 1))
        //    ,
        //    GetAutomationTypeInfo);

        InfoAndDiagnostics<AutomationClassInfo> GetAutomationTypeInfo(GeneratorSyntaxContext gsc, CancellationToken ct)
        {
            var result = new InfoAndDiagnostics<AutomationClassInfo>();
            if (result.Convert(gsc.Node, out ClassDeclarationSyntax cds))
            {

                if (cds.AttributeLists.SelectMany(al => al.Attributes).First(a => a.Name.ToString() is { } name && (name.StartsWith("Generate2"))) is { } nameAttr)
                {
                    if (result.Convert(nameAttr.Name, out GenericNameSyntax gns))
                    {
                        var singleArg = gns.TypeArgumentList.Arguments[0];
                        if (result.IsNotNull(gsc.SemanticModel.GetTypeInfo(singleArg).Type, out var automationInterface))
                        {
                            result.Info = GetAutomationTypeInfo3(cds, gsc.SemanticModel, automationInterface, ct);
                        }
                    }
                }
                else
                {
                    result.AddDiagnostic("nameAttr problem");
                }
            }
            return result;
        }

        InfoAndDiagnostics<AutomationClassInfo> GetAutomationTypeInfo2(GeneratorAttributeSyntaxContext gasc, CancellationToken ct)
        {
            InfoAndDiagnostics<AutomationClassInfo> result = new()
            {
                Info = GetAutomationTypeInfo3(gasc.TargetNode as ClassDeclarationSyntax, gasc.SemanticModel, gasc.Attributes.First().AttributeClass.TypeArguments.FirstOrDefault(), ct)
            };
            return result;
        }

        AutomationClassInfo GetAutomationTypeInfo3(ClassDeclarationSyntax targetNode, SemanticModel semanticModel, ITypeSymbol automationInterface, CancellationToken ct)
        {

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
                    => WithTrivia(SyntaxFactory.ParseTypeName(semanticModel.GetTypeInfo(ts).Type.ToDisplayString()), ts);

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

            var members = (automationInterface.DeclaringSyntaxReferences.First().GetSyntax() as InterfaceDeclarationSyntax).Members.Select(ModifyMember);
            var membersTexts = (members.Select(x => x.ToFullString()));
            //members = members.WithLeadingTrivia(SyntaxFactory.Comment("KEKSE"));
            //var interfaceSymbol = gasc.SemanticModel.GetDeclaredSymbol(interfaceSyntaxNode);

            return new AutomationClassInfo(
                    targetNode is ClassDeclarationSyntax cds,
                    automationInterface.ToDisplayString(),
                    TargetNode: "Blub",
                    UsingDeclarations: "",
                    members.OfType<PropertyDeclarationSyntax>().Select(NewPropertyDeclarationInfo).ToList(),
                    members.OfType<MethodDeclarationSyntax>().Select(NewMethodDeclarationInfo).ToList(),
                    semanticModel.GetDeclaredSymbol(targetNode)?.ContainingNamespace.Name,
                    targetNode?.Identifier.Text
                );
        }


        var automationClasses = initContext.SyntaxProvider.ForAttributeWithMetadataName("GenerationLib.Generate2Attribute`1",
            (sn, ct) => true,
            //!++ TODO do not let the node escape!
            GetAutomationTypeInfo2);

        initContext.RegisterSourceOutput(automationClasses, (spc, infoAndDiagnostics) =>
        {
            foreach (var diag in infoAndDiagnostics.Diagnostics)
            {
                //throw new Exception("wummp" + diag.ToString());
                spc.ReportDiagnostic(diag);
            }

            var tuple = infoAndDiagnostics.Info;
            if (tuple is not null)
            {
                spc.AddSource(tuple.Interface + ".g.cs",
                    tuple.OK ?
                    $$"""
                    using System.Runtime.CompilerServices;
                    using AutomationLib;

                    namespace {{tuple.TargetNameSpace}};
                    /*123456*/

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
                        public System.Action<string> LogAction { get; set; }
                        
                        {{tuple.Properties.Select(p => $"public {p.Declaration} => AutomationObject.{p.Name};").JoinI1()}}
                    
                    {{tuple.Methods.Select(m => $$"""
                        
                          //public {{m.ReturnType}} {{m.Name}}({{m.Arguments.JoinComma()}}, {{m.ArgumentNames.Select(an => $@"[CallerArgumentExpression(""{an}"")] string {an}Arg = null").JoinComma()}})
                            public {{m.ReturnType}} {{m.Name}}({{m.Arguments.JoinComma()}})
                            {
                                if (!IsAutomated)
                                {
                                  //LogAction?.Invoke($"$.{{m.Name}}({{m.ArgumentNames.Select(a => $"{a}Arg").JoinComma()}})");
                                    LogAction?.Invoke($"$.{{m.Name}}({{m.ArgumentNames.Select(a => $"{{{a}.Out()}}").JoinComma()}})");
                                }
                                {{(m.IsVoid ? "" : "return ")}}AutomationObject.{{m.Name}}({{m.ArgumentNames.JoinComma()}});
                            }
                        """).JoinNL()}}
                    }
                    """
                : "// ERROR IN GENERATOR");
            }
        });
    }
}