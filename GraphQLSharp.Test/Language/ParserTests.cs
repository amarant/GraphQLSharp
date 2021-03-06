﻿using System.Collections.Immutable;
using GraphQLSharp.Language;
using Xunit;
using static GraphQLSharp.Test.Language.TestUtils;

namespace GraphQLSharp.Test.Language
{
    public class ParserTests
    {
        [Fact]
        public void AcceptsOptionToNotIncludeSource()
        {
            var result = Parser.Parse("{ field }", new ParseOptions { NoSource = true });
            result.ShouldBeEquivalentToDeepDynamic(
                new Document
                {
                    Location = new Location(0, 9),
                    Definitions = ImmutableArray.Create<IDefinition>(
                        new OperationDefinition
                        {
                            Location = new Location(0, 9),
                            Operation = OperationType.Query,
                            Name = null,
                            VariableDefinitions = ImmutableArray<VariableDefinition>.Empty,
                            Directives = ImmutableArray<Directive>.Empty,
                            SelectionSet = new SelectionSet
                            {
                                Location = new Location(0, 9),
                                Selections = ImmutableArray.Create<ISelection>(
                                new Field
                                {
                                    Location = new Location(2, 7),
                                    Alias = null,
                                    Name = new Name
                                    {
                                        Location = new Location(2, 7),
                                        Value = "field",
                                    },
                                    Arguments = ImmutableArray<Argument>.Empty,
                                    Directives = ImmutableArray<Directive>.Empty,
                                    SelectionSet = null,
                                })
                            }
                        })
                });
        }

        [Fact]
        public void ParseProvidesUsefulErrors()
        {
            ParseErr(
 @"{ ...MissingOn }
fragment MissingOn Type
", "Syntax Error GraphQL (2:20) Expected \"on\", found Name \"Type\"");
            ParseErr("{ field: {} }", "Syntax Error GraphQL (1:10) Expected Name, found {");
            ParseErr("notanoperation Foo { field }", "Syntax Error GraphQL (1:1) Unexpected Name \"notanoperation\"");
            ParseErr("...", "Syntax Error GraphQL (1:1) Unexpected ...");
        }

        [Fact]
        public void ParseProvidesUsefulErrorWhenUsingSource()
        {
            ParseErr(new Source("query", "MyQuery.graphql"), "Syntax Error MyQuery.graphql (1:6) Expected Name, found EOF");
        }

        [Fact]
        public void ParsesVariableInlineValues()
        {
            ParseNoErr("{ field(complex: { a: { b: [ $var ] } }) }");
        }

        [Fact]
        public void ParsesConstantDefaultValues()
        {
            ParseErr("query Foo($x: Complex = { a: { b: [ $var ] } }) { field }",
                "Syntax Error GraphQL (1:37) Unexpected $");
        }

        [Fact]
        public void DuplicateKeysInInputObjectIsSyntaxError()
        {
            ParseErr("{ field(arg: { a: 1, a: 2 }) }",
                "Syntax Error GraphQL (1:22) Duplicate input object field a.");
        }

        [Fact]
        public void ParsesKitchenSink()
        {
            var kitchenSink = TestUtils.KitchenSink.Value;
            Parser.Parse(kitchenSink);
        }

        [Fact]
        public void DoesNotAcceptFragmentsNamedOn()
        {
            ParseErr("fragment on on on { on }", "Syntax Error GraphQL (1:10) Unexpected Name \"on\"");
        }

        [Fact]
        public void DoesNotAcceptFragmentsSpreadOfOn()
        {
            ParseErr("{ ...on }", "Syntax Error GraphQL (1:9) Expected Name, found }");
        }

        [Fact]
        public void AllowsNonKeywordsAnywhereANameIsAllowed()
        {
            var nonKeywords = new[] {
              "on",
              "fragment",
              "query",
              "mutation",
              "true",
              "false",
            };
            foreach (var keyword in nonKeywords)
            {
                // You can't define or reference a fragment named `on`.
                var fragmentName = keyword == "on" ? "a" : keyword;
                ParseNoErr($@"query {keyword} {{
  ... {fragmentName}
  ... on {keyword} {{ field }}
}}
fragment {fragmentName} on Type {{
  {keyword}({keyword}: ${keyword}) @{keyword}({keyword}: {keyword})
}}");
            }
        }

        [Fact]
        public void ParseCreatesAst()
        {
            var source = new Source("{\n  node(id: 4) {\n    id,\n    name\n  }\n}\n");
            var result = Parser.Parse(source);
            result.ShouldBeEquivalentToDeepDynamic(new Document
            {
                Location = new Location(0, 41, source),
                Definitions = ImmutableArray.Create<IDefinition>(
                    new OperationDefinition
                    {
                        Location = new Location(0, 40, source),
                        Operation = OperationType.Query,
                        Name = null,
                        VariableDefinitions = ImmutableArray<VariableDefinition>.Empty,
                        Directives = ImmutableArray<Directive>.Empty,
                        SelectionSet = new SelectionSet
                        {
                            Location = new Location(0, 40, source),
                            Selections = ImmutableArray.Create<ISelection>(
                                new Field
                                {
                                    Location = new Location(4, 38, source),
                                    Alias = null,
                                    Name = new Name
                                    {
                                        Location = new Location(4, 8, source),
                                        Value = "node",
                                    },
                                    Arguments = ImmutableArray.Create(
                                        new Argument
                                        {
                                            Name = new Name
                                            {
                                                Location = new Location(9, 11, source),
                                                Value = "id",
                                            },
                                            Value = new IntValue
                                            {
                                                Location = new Location(13, 14, source),
                                                Value = "4",
                                            },
                                            Location = new Location(9, 14, source),
                                        }
                                    ),
                                    Directives = ImmutableArray<Directive>.Empty,
                                    SelectionSet = new SelectionSet
                                    {
                                        Location = new Location(16, 38, source),
                                        Selections = ImmutableArray.Create<ISelection>(
                                            new Field
                                            {
                                                Location = new Location(22, 24, source),
                                                Alias = null,
                                                Name = new Name
                                                {
                                                    Location = new Location(22, 24, source),
                                                    Value = "id",
                                                },
                                                Arguments = ImmutableArray<Argument>.Empty,
                                                Directives = ImmutableArray<Directive>.Empty,
                                                SelectionSet = null,
                                            },
                                            new Field
                                            {
                                                Location = new Location(30, 34, source),
                                                Alias = null,
                                                Name = new Name
                                                {
                                                    Location = new Location(30, 34, source),
                                                    Value = "name",
                                                },
                                                Arguments = ImmutableArray<Argument>.Empty,
                                                Directives = ImmutableArray<Directive>.Empty,
                                                SelectionSet = null,
                                            }
                                        )
                                    }
                                }
                            )
                        }
                    }
                )
            });
        }
    }
}
