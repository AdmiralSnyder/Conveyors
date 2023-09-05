using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AutomationObjectGenerator
{
    internal class InfoAndDiagnostics<TInfo>
    {
        public TInfo? Info { get; set; }

        public List<Diagnostic> Diagnostics = new();

        internal void AddDiagnostic(string title, string? text = null, Location? location = null)
        {
            Diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor("KEKSE", title, text, "GEN", DiagnosticSeverity.Error, true), location));
        }

        internal bool Convert<T>(SyntaxNode node, out T result)
            where T : SyntaxNode
        {
            if (ConvertObj(node, out T convResult, () => node.GetLocation()))
            {
                result = convResult;
                return true;
            }
            else
            {
                result = default!;
                return false;
            }
        }

        private bool ConvertObj<T>(object obj, out T result, Func<Location?> getLocation, [CallerArgumentExpression(nameof(obj))] string objExpr = null)
        {
            if (obj is T converted)
            {
                result = converted;
                return true;
            }
            else
            {
                AddDiagnostic($"Unexpected type of {objExpr} - expected: {typeof(T).Name}");
                result = default;
                return false;
            }
        }

        internal bool IsNotNull<T>(T? obj, out T result, [CallerArgumentExpression(nameof(obj))] string objExpr = null)
            where T : class
        {
            if ((result = obj!) is not null)
            {
                return true;
            }
            else
            {
                AddDiagnostic($"{objExpr} should not be null");
                return false;
            }
        }
    }
}