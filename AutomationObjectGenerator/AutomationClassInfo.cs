namespace AutomationObjectGenerator;

internal record AutomationClassInfo
(
    bool OK, 
    string Interface, 
    string TargetNode, 
    string UsingDeclarations, 
    List<PropertyDeclarationInfo> Properties, 
    List<MethodDeclarationInfo> Methods, 
    string? TargetNameSpace, 
    string? TargetClassName
)
{
    public static implicit operator (bool OK, string Interface, string TargetNode, string UsingDeclarations, List<PropertyDeclarationInfo> Properties, List<MethodDeclarationInfo> Methods, string? TargetNameSpace, string? TargetClassName)(AutomationClassInfo value) 
        => (value.OK, value.Interface, value.TargetNode, value.UsingDeclarations, value.Properties, value.Methods, value.TargetNameSpace, value.TargetClassName);

    public static implicit operator AutomationClassInfo((bool OK, string Interface, string TargetNode, string UsingDeclarations, List<PropertyDeclarationInfo> Properties, List<MethodDeclarationInfo> Methods, string? TargetNameSpace, string? TargetClassName) value) 
        => new AutomationClassInfo(value.OK, value.Interface, value.TargetNode, value.UsingDeclarations, value.Properties, value.Methods, value.TargetNameSpace, value.TargetClassName);
}