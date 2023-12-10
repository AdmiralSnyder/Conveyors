using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptingLib;

public class ScriptRunner
{
    private ScriptGlobalsBase Globals;
    private Script Script;
    private Task InitScriptTask;
    private Type TheObjectType;
    private Action? BeforeExecAction;
    private Action? AfterExecAction;
    private Action<Exception>? ErrorAction;

    public void InitializeScriptingEnvironment<TRootObj>(TRootObj automationRoot, Action? initDoneAction, 
        Action? beforeExecAction, Action? afterExecAction, Action<Exception>? errorAction, 
        Type[] namespaceTypes, Type[] assemblyTypes)
    {
        BeforeExecAction = beforeExecAction; 
        AfterExecAction = afterExecAction;
        ErrorAction = errorAction;
        using var loader = new InteractiveAssemblyLoader();

        Globals = new ScriptGlobals<TRootObj>() { TheObject = automationRoot };
        TheObjectType = typeof(TRootObj);
        var globalsType = typeof(ScriptGlobals<TRootObj>);
        Script = CSharpScript.Create("", ScriptOptions.Default
            .AddImports(globalsType.Namespace)
            .AddImports(namespaceTypes.Select(t => t.Namespace))
            .AddReferences(typeof(ScriptGlobals<TRootObj>).Assembly)
            .AddReferences(assemblyTypes.Select(t => t.Assembly)),
            globalsType, loader);

        InitScriptTask = Task.Run(async () =>
        {
            await Script.RunAsync(Globals);
            initDoneAction?.Invoke();
        });
    }

    public async Task RunScript(string text)
    {
        var lines = text.Split(Environment.NewLine);

        var references = lines.Where(l => l.StartsWith("//#r ")).Select(l => l[5..].Trim());
        var usings = lines.Where(l => l.StartsWith("//#u ")).Select(l => l[5..].Trim());

        if (!text.EndsWith(";"))
        {
            text += ";";
        }

        text = text.Replace("$", "DynObj");

        string classDefinition = $$"""
            using System;
            using System.Linq.Expressions;
            {{string.Join(Environment.NewLine, usings.Select(u => $"using {u}"))}}

            class foo
            {
                //public static dynamic DynObj { get; set; }
                public static bool Execute({{TheObjectType.FullName}} DynObj)
                {
                    {{text}}
                    return true;
                }
            }
            """;

        if (Script is not null)
        {

            var refsToAdd = references.Where(r => !Script.Options.MetadataReferences.Any(mr => mr.Display.EndsWith(r)));
            List<string> existingFiles = [];
            foreach (var file in refsToAdd)
            {
                if (File.Exists(file))
                {
                    var fullFile = Path.GetFullPath(file);
                    existingFiles.Add(fullFile);
                }
                else
                {

                }
            }

            Script.Options.AddReferences(existingFiles.ToArray());
            var script = Script.ContinueWith(classDefinition);
            BeforeExecAction?.Invoke();
            try
            {
                var x = await script.ContinueWith<bool>($"foo.Execute(TheObject)").RunAsync(Globals);
            }
            catch (Exception ex)
            {
                ErrorAction?.Invoke(ex);
            }
            AfterExecAction?.Invoke();
        }
    }
}

public class ScriptGlobalsBase { }

public class ScriptGlobals<TTheObject> : ScriptGlobalsBase
{
    public TTheObject TheObject { get; set; }
}

