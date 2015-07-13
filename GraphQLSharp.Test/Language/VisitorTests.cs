using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using FluentAssertions;
using GraphQLSharp.Language;
using Xunit;
using Xunit.Abstractions;

namespace GraphQLSharp.Test.Language
{
    public class VisitorTests
    {
        private readonly ITestOutputHelper _output;

        public VisitorTests(ITestOutputHelper output)
        {
            _output = output;
        }


        public class ToStringVisitor : TreeWalker
        {
            private readonly ITestOutputHelper _output;

            public ToStringVisitor(ITestOutputHelper output)
            {
                _output = output;
            }

            public override INode DefaultVisit(INode node)
            {
                _output.WriteLine("+" + node.GetType().Name);
                return null;
            }
        }

        public class ToStringStackWalker : StackWalker
        {
            private readonly ITestOutputHelper _output;

            public ToStringStackWalker(ITestOutputHelper output)
            {
                _output = output;
            }

            public override INode Enter(INode node)
            {
                _output.WriteLine("+" + node.GetType().Name);
                return node;
            }

            public override INode Leave(INode node)
            {
                _output.WriteLine("-" + node.GetType().Name);
                return node;
            }
        }

        [Fact]
        public void TestToStringVisitor()
        {
            var source = new Source("{\n  node(id: 4) {\n    id,\n    name\n  }\n}\n");
            var result = Parser.Parse(source);
            var visitor = new ToStringVisitor(_output);
            visitor.Visit(result);
        }

        [Fact]
        public void TestToStringStackWalker()
        {
            var source = new Source("{\n  node(id: 4) {\n    id,\n    name\n  }\n}\n");
            var result = Parser.Parse(source);
            var visitor = new ToStringStackWalker(_output);
            var visited = visitor.Visit(result);
            result.Should().Be(visited);
        }

        public class AllowsForEditingOnEnterStackWalker : StackWalker
        {
            public override Field EnterField(Field field)
            {
                if (field.Name.Value == "b")
                {
                    return null;
                }
                return field;
            }
        }

        [Fact]
        public void AllowsForEditingOnEnter()
        {
            var ast = Parser.Parse("{ a, b, c { a, b, c } }", new ParseOptions
            {
                NoLocation = true,
            });
            var visitor = new AllowsForEditingOnEnterStackWalker();
            var editedAst = visitor.Visit(ast);

            ast.ShouldBeEquivalentToDeepDynamic(Parser.Parse("{ a, b, c { a, b, c } }", new ParseOptions
                {
                    NoLocation = true,
                }));

            var expectation = Parser.Parse("{ a,    c { a,    c } }", new ParseOptions
            {
                NoLocation = true,
            });
            editedAst.ShouldBeEquivalentToDeepDynamic(expectation);

            var noexpectation = Parser.Parse("{ a, b, c { a,    c } }", new ParseOptions
            {
                NoLocation = true,
            });
            Action action = () =>
                ast.ShouldBeEquivalentToDeepDynamic(noexpectation);
            action.ShouldThrow<Exception>();
        }

        public class AllowsForEditingOnLeaveStackWalker : StackWalker
        {
            public override Field LeaveField(Field field)
            {
                if (field.Name.Value == "b")
                {
                    return null;
                }
                return field;
            }
        }

        [Fact]
        public void AllowsForEditingOnLeave()
        {
            var ast = Parser.Parse("{ a, b, c { a, b, c } }", new ParseOptions
            {
                NoLocation = true,
            });
            var visitor = new AllowsForEditingOnLeaveStackWalker();
            var editedAst = visitor.Visit(ast);

            ast.ShouldBeEquivalentToDeepDynamic(Parser.Parse("{ a, b, c { a, b, c } }", new ParseOptions
            {
                NoLocation = true,
            }));

            var expectation = Parser.Parse("{ a,    c { a,    c } }", new ParseOptions
            {
                NoLocation = true,
            });
            editedAst.ShouldBeEquivalentToDeepDynamic(expectation);

            var noexpectation = Parser.Parse("{ a, b, c { a,    c } }", new ParseOptions
            {
                NoLocation = true,
            });
            Action action = () =>
                ast.ShouldBeEquivalentToDeepDynamic(noexpectation);
            action.ShouldThrow<Exception>();
        }

        public class AddField : StackWalker
        {
            public static Field AddedField = new Field
            {
                Name = new Name
                {
                    Value = "__typename",
                },
            };

            public bool DidVisitAddedField = false;

            public override Field EnterField(Field field)
            {
                if (field.Name.Value == "a")
                {
                    var selectionSet = new SelectionSet
                    {
                        Selections = field.SelectionSet.Selections.Add(AddedField),
                    };
                    return new Field
                    {
                        Alias = field.Alias,
                        Name = field.Name,
                        Arguments = field.Arguments,
                        Directives = field.Directives,
                        SelectionSet = selectionSet,
                    };
                }
                else if (field == AddedField)
                {
                    DidVisitAddedField = true;
                }
                return field;
            }
        }

        [Fact]
        public void VisitsEditedNode()
        {
            var ast = Parser.Parse("{ a { x } }", new ParseOptions
            {
                NoLocation = true,
            });
            var visitor = new AddField();
            visitor.Visit(ast);
            visitor.DidVisitAddedField.Should().BeTrue();
        }
    }
}
