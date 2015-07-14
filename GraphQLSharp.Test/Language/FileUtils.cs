using System;
using System.IO;
using System.Reflection;

namespace GraphQLSharp.Test.Language
{
    public class FileUtils
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
    }
}