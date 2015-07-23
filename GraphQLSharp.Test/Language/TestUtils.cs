using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using GraphQLSharp.Language;
using GraphQLSharp.Language.Schema;

namespace GraphQLSharp.Test.Language
{
    public static class TestUtils
    {
        public static Lazy<String> ProjectDir = new Lazy<string>(() =>
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            var projectDir = Path.Combine(dirPath, @"..\..");
            projectDir = Path.GetFullPath(projectDir);
            return projectDir;
        });

        public static Lazy<String> KitchenSink = new Lazy<string>(() => 
            File.ReadAllText(Path.Combine(ProjectDir.Value, "Language", "kitchen-sink.graphql")));

        public static String ToLF(this String str)
        {
            return str.Replace("\r\n", "\n");
        }

        public static void ParseSchemaErr(string source)
        {
            Action action = () => SchemaParser.ParseSchema(source);
            action.ShouldThrow<SyntaxError>();
        }

        public static void ParseErr(string source, string message)
        {
            Action action = () => Parser.Parse(source);
            action.ShouldThrow<SyntaxError>().Where(err => err.Message.StartsWith(message));
        }

        public static void ParseErr(Source source, string message)
        {
            Action action = () => Parser.Parse(source);
            action.ShouldThrow<SyntaxError>().Where(err => err.Message.StartsWith(message));
        }

        public static void ParseNoErr(string source)
        {
            Action action = () => Parser.Parse(source);
            action.ShouldNotThrow();
        }
    }
}