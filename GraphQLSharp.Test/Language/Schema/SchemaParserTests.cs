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

        [Fact]
        public void SimpleTypeInheritingInterface()
        {
            var body = new Source("type Hello implements World { }");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new TypeDefinition
                    {
                        Name = new Name("Hello", new Location(5, 10, body)),
                        Interfaces = ImmutableArray.Create(new NamedType("World", 22, 27, body)),
                        Location = new Location(0, 31, body),
                    }),
                Location = new Location(0, 31, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleTypeInheritingMultipleInterfaces()
        {
            var body = new Source("type Hello implements Wo, rld { }");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new TypeDefinition
                    {
                        Name = new Name("Hello", new Location(5, 10, body)),
                        Interfaces = ImmutableArray.Create(
                            new NamedType("Wo", 22, 24, body),
                            new NamedType("rld", 26, 29, body)),
                        Location = new Location(0, 33, body),
                    }),
                Location = new Location(0, 33, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SingleValueEnum()
        {
            var body = new Source("enum Hello { WORLD }");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new EnumDefinition
                    {
                        Name = new Name("Hello", new Location(5, 10, body)),
                        Values = ImmutableArray.Create(
                            new EnumValueDefinition("WORLD", new Location(13, 18, body))),
                        Location = new Location(0, 20, body),
                    }),
                Location = new Location(0, 20, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void DoubleValueEnum()
        {
            var body = new Source("enum Hello { WO, RLD }");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new EnumDefinition
                    {
                        Name = new Name("Hello", new Location(5, 10, body)),
                        Values = ImmutableArray.Create(
                            new EnumValueDefinition("WO", new Location(13, 15, body)),
                            new EnumValueDefinition("RLD", new Location(17, 20, body))),
                        Location = new Location(0, 22, body),
                    }),
                Location = new Location(0, 22, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleInterface()
        {
            var body = new Source(
                @"
interface Hello {
  world: String
}".ToLF());
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new InterfaceDefinition
                    {
                        Name = new Name("Hello", new Location(11, 16, body)),
                        Fields = ImmutableArray.Create(
                            new FieldDefinition
                            {
                                Name = new Name("world", new Location(21, 26, body)),
                                Type = new NamedType("String", 28, 34, body),
                                Location = new Location(21, 34, body),
                            }),
                        Location = new Location(1, 36, body),
                    }),
                Location = new Location(1, 36, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleFieldWithArg()
        {
            var body = new Source(
                @"
type Hello {
  world(flag: Boolean): String
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
                                Type = new NamedType("String", 38, 44, body),
                                Arguments = ImmutableArray.Create(
                                    new ArgumentDefinition
                                    {
                                        Name = new Name("flag", new Location(22, 26, body)),
                                        Type = new NamedType("Boolean", 28, 35, body),
                                        Location = new Location(22, 35, body),
                                    }),
                                Location = new Location(16, 44, body),
                            }),
                        Location = new Location(1, 46, body),
                    }),
                Location = new Location(1, 46, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }
    }
}