using FluentAssertions;
using GraphQLSharp.Language;
using Xunit;

namespace GraphQLSharp.Test.Language
{
    public class PrinterTests
    {
        [Fact]
        public void DoesNotAlterAst()
        {
            var kitchenSink = FileUtils.KitchenSink.Value;
            var ast = Parser.Parse(kitchenSink);
            var astCopy = Parser.Parse(kitchenSink);
            var printer = new Printer();
            var print = printer.VisitDocument(ast);
            ast.ShouldBeEquivalentToDeepDynamic(astCopy);
        }

        [Fact]
        public void PrintsMinimalAst()
        {
            var ast = new Field
            {
                Name = new Name
                {
                    Value = "foo",
                },
            };
            var printer = new Printer();
            printer.Visit(ast).Should().Be("foo");
        }

        [Fact]
        public void PrintsKitchenSink()
        {
            var kitchenSink = FileUtils.KitchenSink.Value;
            var ast = Parser.Parse(kitchenSink);
            var printer = new Printer();
            var printed = printer.VisitDocument(ast);
            printed.Should().Be(
@"query queryName($foo: ComplexType, $site: Site = MOBILE) {
  whoever123is: node(id: [123, 456]) {
    id
    ... on User @defer {
      field2 {
        id
        alias: field1(first: 10, after: $foo) @include(if: $foo) {
          id
          ...frag
        }
      }
    }
  }
}

mutation likeStory {
  like(story: 123) @defer {
    story {
      id
    }
  }
}

fragment frag on Friend {
  foo(size: $size, bar: $b, obj: {key: ""value""})
}

{
  unnamed(truthy: true, falsey: false)
  query
}
".Replace("\r\n", "\n"));
        }
    }
}
