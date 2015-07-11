﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using GraphQLSharp.Language;
using Xunit;

namespace GraphQLSharp.Test.Language
{
    public class ParserTests
    {
        private static void ParseErr(string source, string message)
        {
            Action action = () => Parser.Parse(source);
            action.ShouldThrow<SyntaxError>().Where(err => err.Message.StartsWith(message));
        }

        private void ParseErr(Source source, string message)
        {
            Action action = () => Parser.Parse(source);
            action.ShouldThrow<SyntaxError>().Where(err => err.Message.StartsWith(message));
        }

        private void ParseNoErr(string source)
        {
            Action action = () => Parser.Parse(source);
            action.ShouldNotThrow();
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

        private string _projectDir;
        public String ProjectDir
        {
            get
            {
                if (_projectDir == null)
                {
                    var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
                    var dirPath = Path.GetDirectoryName(codeBasePath);
                    _projectDir = Path.Combine(dirPath, @"..\..");
                    _projectDir = Path.GetFullPath(_projectDir);
                }
                return _projectDir;
            }
        }

        [Fact]
        public void ParsesKitchenSink()
        {
            var kitchenSink = File.ReadAllText(Path.Combine(ProjectDir, "Language", "kitchen-sink.graphql"));
            Parser.Parse(kitchenSink);
        }

        [Fact]
        public void ParseCreatesAst()
        {
            var source = new Source("{\n  node(id: 4) {\n    id,\n    name\n  }\n}\n");
            var result = Parser.Parse(source);
            result.ShouldBeEquivalentTo(new Document
            {
                Location = new Location(0, 41, source),
                Definitions = new List<IDefinition>
                {
                    new OperationDefinition
                    {
                        Location = new Location(0, 40, source),
                        Operation = OperationType.Query,
                        Name = null,
                        VariableDefinitions = null,
                        Directives = new List<Directive>(),
                        SelectionSet = new SelectionSet
                        {
                            Location = new Location(0, 40, source),
                            Selections = new List<ISelection>
                            {
                                new Field
                                {
                                    Location = new Location(4, 38, source),
                                    Alias = null,
                                    Name = new Name
                                    {
                                        Location = new Location(4, 8, source),
                                        Value = "node",
                                    },
                                    Arguments = new List<Argument>
                                    {
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
                                        },
                                    },
                                    Directives = new List<Directive>(),
                                    SelectionSet = new SelectionSet
                                    {
                                        Location = new Location(16, 38, source),
                                        Selections = new List<ISelection>
                                        {
                                            new Field
                                            {
                                                Location = new Location(22, 24, source),
                                                Alias = null,
                                                Name = new Name
                                                {
                                                    Location = new Location(22, 24, source),
                                                    Value = "id",
                                                },
                                                Arguments = new List<Argument>(),
                                                Directives = new List<Directive>(),
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
                                                Arguments = new List<Argument>(),
                                                Directives = new List<Directive>(),
                                                SelectionSet = null,
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            });
            
        }
    }
}