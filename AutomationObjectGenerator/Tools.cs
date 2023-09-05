namespace AutomationObjectGenerator
{
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