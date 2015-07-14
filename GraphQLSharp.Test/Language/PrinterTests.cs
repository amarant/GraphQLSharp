using System;
using GraphQLSharp.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphQLSharp.Test.Language
{
    [TestClass]
    public class PrinterTests
    {
        [TestMethod]
        public void DoesNotAlterAst()
        {
            var kitchenSink = FileUtils.KitchenSink.Value;
            var ast = Parser.Parse(kitchenSink);
            var astCopy = Parser.Parse(kitchenSink);
            var printer = new Printer();
            var print = printer.VisitDocument(ast);
            ast.ShouldBeEquivalentToDeepDynamic(astCopy);
        }
    }
}
