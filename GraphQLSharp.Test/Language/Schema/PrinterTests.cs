using FluentAssertions;
using GraphQLSharp.Language;
using GraphQLSharp.Language.Schema;
using Xunit;

namespace GraphQLSharp.Test.Language.Schema
{
    public class PrinterTests
    {
        [Fact]
        public void PrintsMinimalAst()
        {
            var ast = new ScalarDefinition
            {
                Name = new Name {Value = "foo"},
            };
            var schemaPrinter = new SchemaPrinter();
            schemaPrinter.Visit(ast).Should().Be("scalar foo");
        }

        [Fact]
        public void DoesNotAlterAst()
        {
            var schemaKitchenSink = TestUtils.SchemaKitchenSink.Value;
            var ast = SchemaParser.ParseSchema(schemaKitchenSink);
            var astCopy = SchemaParser.ParseSchema(schemaKitchenSink);
            var schemaPrinter = new SchemaPrinter();
            schemaPrinter.VisitSchemaDocument(ast);
            ast.ShouldBeEquivalentToDeepDynamic(astCopy);
        }

        [Fact]
        public void PrintsKitchenSink()
        {
            var schemaKitchenSink = TestUtils.SchemaKitchenSink.Value;
            var ast = SchemaParser.ParseSchema(schemaKitchenSink);
            var schemaPrinter = new SchemaPrinter();
            var printed = schemaPrinter.VisitSchemaDocument(ast);
            printed.Should().Be(
@"type Foo implements Bar {
  one: Type
  two(argument: InputType!): Type
  three(argument: InputType, other: String): Int
  four(argument: String = ""string""): String
  five(argument: [String] = [""string"", ""string""]): String
  six(argument: InputType = {key: ""value""}): Type
}

interface Bar {
  one: Type
  four(argument: String = ""string""): String
}

union Feed = Story | Article | Advert

scalar CustomScalar

enum Site {
  DESKTOP
  MOBILE
}

input InputType {
  key: String!
  answer: Int = 42
}
".ToLF());
        }
    }
}
