using System;
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


        public class ToStringVisitor : GenericVisitor
        {
            private readonly ITestOutputHelper _output;

            public ToStringVisitor(ITestOutputHelper output)
            {
                _output = output;
            }

            public override VisitAction Enter(INode node)
            {
                _output.WriteLine("+" + node.GetType().Name);
                return VisitAction.NoAction;
            }

            public override VisitAction Leave(INode node)
            {
                _output.WriteLine("-" + node.GetType().Name);
                return VisitAction.NoAction;
            }
        }

        [Fact]
        public void TestMethod1()
        {
            var source = new Source("{\n  node(id: 4) {\n    id,\n    name\n  }\n}\n");
            var result = Parser.Parse(source);
            var visitor = new ToStringVisitor(_output);
            result.Accept(visitor);
        }
    }
}
