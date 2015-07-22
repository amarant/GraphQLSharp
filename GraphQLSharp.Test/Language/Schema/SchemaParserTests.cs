using System.Collections.Immutable;
using GraphQLSharp.Language;
using GraphQLSharp.Language.Schema;
using Xunit;

namespace GraphQLSharp.Test.Language.Schema
{
    public class SchemaParserTests
    {
        [Fact]
        public void SimpleType()
        {
            var body = new Source(
@"
type Hello {
  world: String
}".ToLF());
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new TypeDefinition
                    {
                        Name = new Name("Hello", new Location(6, 11, body)),
                        Fields = ImmutableArray.Create(
                            new FieldDefinition
                            {
                                Name = new Name("world", new Location(16, 21, body)),
                                Type = new NamedType("String", 23, 29, body),
                                Location = new Location(16, 29, body),
                            }),
                        Location = new Location(1, 31, body),
                    }),
                Location = new Location(1, 31, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleNonNullType()
        {
            var body = new Source(
@"
type Hello {
  world: String!
}".ToLF());
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new TypeDefinition
                    {
                        Name = new Name("Hello", new Location(6, 11, body)),
                        Fields = ImmutableArray.Create(
                            new FieldDefinition
                            {
                                Name = new Name("world", new Location(16, 21, body)),
                                Type = new NonNullType
                                {
                                    Type = new NamedType("String", 23, 29, body),
                                    Location = new Location(23, 30, body),
                                },
                                Location = new Location(16, 30, body),
                            }),
                        Location = new Location(1, 32, body),
                    }),
                Location = new Location(1, 32, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }
    }
}
