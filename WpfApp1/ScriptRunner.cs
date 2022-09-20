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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ConveyorApp;

internal class ScriptRunner
{
    private ScriptGlobalsBase Globals;
    private Script Script;
    private Task InitScriptTask;

    private Type TheObjectType;

    private Dispatcher Dispatcher;

    public void InitializeScriptingEnvironment<TRootObj>(TRootObj automationRoot, Dispatcher dispatcher, Button? runButton = null)
    {
        Dispatcher = dispatcher;

        using var loader = new InteractiveAssemblyLoader();

        Globals = new ScriptGlobals<TRootObj>() { TheObject = automationRoot };
        TheObjectType = typeof(TRootObj);
        var globalsType = typeof(ScriptGlobals<TRootObj>);
        Script = CSharpScript.Create("", ScriptOptions.Default.AddImports(globalsType.Namespace, typeof(Point).Namespace)
            .AddReferences(typeof(ScriptGlobals<TRootObj>).Assembly, typeof(Point).Assembly),
            globalsType, loader);

        InitScriptTask = Task.Run(async () =>
        {
            await Script.RunAsync(Globals);
            if (runButton is { })
            {
                await runButton.Dispatcher.InvokeAsync(() => runButton.IsEnabled = true);
            }
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


        string classDefinition = @$"using System;
using System.Linq.Expressions;
{string.Join(Environment.NewLine, usings.Select(u => $"using {u}"))}

class foo
{{
    //public static dynamic DynObj {{ get; set; }}
    public static bool Execute({TheObjectType.FullName} DynObj)
    {{
        {text}
return true;
    }}
}}";

        if (Script is not null)
        {

            var refsToAdd = references.Where(r => !Script.Options.MetadataReferences.Any(mr => mr.Display.EndsWith(r)));
            List<string> existingFiles = new();
            foreach (var file in refsToAdd)
            {
                if (File.Exists(file))
                {
                    var fullFile = System.IO.Path.GetFullPath(file);
                    existingFiles.Add(fullFile);
                }
                else
                {

                }
            }

            this.Script.Options.AddReferences(existingFiles.ToArray());
            var script = Script.ContinueWith(classDefinition);
            await Task.Run(async () => await Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    try
                    {
                        var x = await script.ContinueWith<bool>($"foo.Execute(TheObject)").RunAsync(Globals);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error running script");
                    }
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }));
        }
    }
}

public class ScriptGlobalsBase
{ }

public class ScriptGlobals<TTheObject> : ScriptGlobalsBase
{
    public TTheObject TheObject { get; set; }
}

