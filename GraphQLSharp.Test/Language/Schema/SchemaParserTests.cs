using System.Collections.Immutable;
using GraphQLSharp.Language;
using GraphQLSharp.Language.Schema;
using Xunit;
using static GraphQLSharp.Test.Language.TestUtils;

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
                                    new InputValueDefinition
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

        [Fact]
        public void SimpleFieldWithArgWithDefaultValue()
        {
            var body = new Source(
                @"
type Hello {
  world(flag: Boolean = true): String
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
                                Type = new NamedType("String", 45, 51, body),
                                Arguments = ImmutableArray.Create(
                                    new InputValueDefinition
                                    {
                                        Name = new Name("flag", new Location(22, 26, body)),
                                        Type = new NamedType("Boolean", 28, 35, body),
                                        DefaultValue = new BooleanValue
                                        {
                                            Value = true,
                                            Location = new Location(38, 42, body),
                                        },
                                        Location = new Location(22, 42, body),
                                    }),
                                Location = new Location(16, 51, body),
                            }),
                        Location = new Location(1, 53, body),
                    }),
                Location = new Location(1, 53, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleFieldWithListArg()
        {
            var body = new Source(
                @"
type Hello {
  world(things: [String]): String
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
                                Type = new NamedType("String", 41, 47, body),
                                Arguments = ImmutableArray.Create(
                                    new InputValueDefinition
                                    {
                                        Name = new Name("things", new Location(22, 28, body)),
                                        Type = new ListType
                                        {
                                            Type = new NamedType("String", 31, 37, body),
                                            Location = new Location(30, 38, body),
                                        },
                                        DefaultValue = null,
                                        Location = new Location(22, 38, body),
                                    }),
                                Location = new Location(16, 47, body),
                            }),
                        Location = new Location(1, 49, body),
                    }),
                Location = new Location(1, 49, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleFieldWithTwoArgs()
        {
            var body = new Source(
                @"
type Hello {
  world(argOne: Boolean, argTwo: Int): String
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
                                Type = new NamedType("String", 53, 59, body),
                                Arguments = ImmutableArray.Create(
                                    new InputValueDefinition
                                    {
                                        Name = new Name("argOne", new Location(22, 28, body)),
                                        Type = new NamedType("Boolean", 30, 37, body),
                                        DefaultValue = null,
                                        Location = new Location(22, 37, body),
                                    },
                                    new InputValueDefinition
                                    {
                                        Name = new Name("argTwo", new Location(39, 45, body)),
                                        Type = new NamedType("Int", 47, 50, body),
                                        DefaultValue = null,
                                        Location = new Location(39, 50, body),
                                    }),
                                Location = new Location(16, 59, body),
                            }),
                        Location = new Location(1, 61, body),
                    }),
                Location = new Location(1, 61, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleUnion()
        {
            var body = new Source("union Hello = World");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new UnionDefinition
                    {
                        Name = new Name("Hello", new Location(6, 11, body)),
                        Types = ImmutableArray.Create(
                            new NamedType("World", 14, 19, body)),
                        Location = new Location(0, 19, body),
                    }),
                Location = new Location(0, 19, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void UnionWithTwoTypes()
        {
            var body = new Source("union Hello = Wo | Rld");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new UnionDefinition
                    {
                        Name = new Name("Hello", new Location(6, 11, body)),
                        Types = ImmutableArray.Create(
                            new NamedType("Wo", 14, 16, body),
                            new NamedType("Rld", 19, 22, body)),
                        Location = new Location(0, 22, body),
                    }),
                Location = new Location(0, 22, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }


        [Fact]
        public void Scalar()
        {
            var body = new Source("scalar Hello");
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new ScalarDefinition
                    {
                        Name = new Name("Hello", new Location(7, 12, body)),
                        Location = new Location(0, 12, body),
                    }),
                Location = new Location(0, 12, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }


        [Fact]
        public void SimpleInputObject()
        {
            var body = new Source(
@"
input Hello {
  world: String
}".ToLF());
            var doc = SchemaParser.ParseSchema(body);
            var expected = new SchemaDocument
            {
                Definitions = ImmutableArray.Create<SchemaDefinition>(
                    new InputObjectDefinition
                    {
                        Name = new Name("Hello", new Location(7, 12, body)),
                        Fields = ImmutableArray.Create(
                            new InputValueDefinition
                            {
                                Name = new Name("world", new Location(17, 22, body)),
                                Type = new NamedType("String", 24, 30, body),
                                Location = new Location(17, 30, body),
                            }),
                        Location = new Location(1, 32, body),
                    }),
                Location = new Location(1, 32, body),
            };
            doc.ShouldBeEquivalentToDeepDynamic(expected);
        }

        [Fact]
        public void SimpleInputObjectWithArgsShouldFail()
        {
            var body = @"
input Hello {
  world(foo: Int): String
}".ToLF();
            ParseSchemaErr(body);
        }

        [Fact]
        public void RejectQueryKeywords()
        {
            var body = "query Foo { field }";
            ParseSchemaErr(body);
        }

        [Fact]
        public void RejectQueryShorthand()
        {
            var body = "{ field }";
            ParseSchemaErr(body);
        }
    }
}