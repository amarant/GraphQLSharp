using System;
using System.Collections.Immutable;

namespace GraphQLSharp.Language.Schema
{
    public class SchemaParser : Parser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaParser"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        private SchemaParser(Source source, ParseOptions options) : base(source, options)
        {
        }

        /// <summary>
        /// Given a GraphQL schema source, parses it into a SchemaDocument.
        /// Throws GraphQLError if a syntax error is encountered.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static SchemaDocument ParseSchema(Source source, ParseOptions options = null)
        {
            var parser = new SchemaParser(source, options);
            return parser.ParseSchemaDocument();
        }

        /// <summary>
        /// Given a GraphQL schema source, parses it into a SchemaDocument.
        /// Throws GraphQLError if a syntax error is encountered.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static SchemaDocument ParseSchema(String source, ParseOptions options = null)
        {
            return ParseSchema(new Source(source), options);
        }

        /// <summary>
        /// Parses the schema document.
        /// SchemaDocument : SchemaDefinition+
        /// </summary>
        /// <returns></returns>
        private SchemaDocument ParseSchemaDocument()
        {
            var start = Token.Start;
            var definitions = ImmutableArray<SchemaDefinition>.Empty;
            do
            {
                definitions = definitions.Add(ParseSchemaDefinition());
            } while (!Skip(TokenKind.EOF));

            return new SchemaDocument
            {
                Definitions = definitions,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the schema definition.
        /// SchemaDefinition :
        ///   - TypeDefinition
        ///   - InterfaceDefinition
        ///   - UnionDefinition
        ///   - ScalarDefinition
        ///   - EnumDefinition
        ///   - InputObjectDefinition
        /// </summary>
        /// <returns></returns>
        private SchemaDefinition ParseSchemaDefinition()
        {
            if (!Peek(TokenKind.NAME))
            {
                throw Unexpected();
            }
            switch (Token.Value)
            {
                case "type":
                    return ParseTypeDefinition();
                case "interface":
                    return ParseInterfaceDefinition();
                case "union":
                    return ParseUnionDefinition();
                case "scalar":
                    return ParseScalarDefinition();
                case "enum":
                    return ParseEnumDefinition();
                case "input":
                    return ParseInputObjectDefinition();
                default:
                    throw Unexpected();
            }
        }

        /// <summary>
        /// Parses the type definition.
        /// TypeDefinition : TypeName ImplementsInterfaces? { FieldDefinition+ }
        /// 
        /// TypeName : Name
        /// </summary>
        /// <returns></returns>
        private TypeDefinition ParseTypeDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("type");
            var name = ParseName();
            var interfaces = ParseImplementsInterfaces();
            var fields = Any(TokenKind.BRACE_L, ParseFieldDefinition, TokenKind.BRACE_R);
            return new TypeDefinition
            {
                Name = name,
                Interfaces = interfaces,
                Fields = fields,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the implements interfaces.
        /// ImplementsInterfaces : `implements` NamedType+
        /// </summary>
        /// <returns></returns>
        private ImmutableArray<NamedType> ParseImplementsInterfaces()
        {
            var types = ImmutableArray<NamedType>.Empty;
            if (Token.Value == "implements")
            {
                Advance();
                do
                {
                    types = types.Add(ParseNamedType());
                } while (!Peek(TokenKind.BRACE_L));
            }

            return types;
        }

        /// <summary>
        /// Parses the field definition.
        /// FieldDefinition : FieldName ArgumentsDefinition? : Type
        /// 
        /// FieldName : Name
        /// </summary>
        /// <returns></returns>
        private FieldDefinition ParseFieldDefinition()
        {
            var start = Token.Start;
            var name = ParseName();
            var args = ParseArgumentDefs();
            Expect(TokenKind.COLON);
            var type = ParseType();
            return new FieldDefinition
            {
                Name = name,
                Arguments = args,
                Type = type,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the argument defs.
        /// ArgumentsDefinition : ( ArgumentDefinition+ )
        /// </summary>
        /// <returns></returns>
        private ImmutableArray<ArgumentDefinition> ParseArgumentDefs()
        {
            if (!Peek(TokenKind.PAREN_L))
            {
                return ImmutableArray<ArgumentDefinition>.Empty;
            }
            return Many(TokenKind.PAREN_L, ParseArgumentDef, TokenKind.PAREN_R);
        }

        /// <summary>
        /// Parses the argument definition.
        /// ArgumentDefinition : ArgumentName : Value[Const] DefaultValue?
        /// 
        /// ArgumentName : Name
        /// 
        /// DefaultValue : = Value[Const]
        /// </summary>
        /// <returns></returns>
        private ArgumentDefinition ParseArgumentDef()
        {
            var start = Token.Start;
            var name = ParseName();
            Expect(TokenKind.COLON);
            var type = ParseType();
            IValue defaultValue = null;
            if (Skip(TokenKind.EQUALS))
            {
                defaultValue = ParseConstValue();
            }
            return new ArgumentDefinition
            {
                Name = name,
                Type = type,
                DefaultValue = defaultValue,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the interface definition.
        /// InterfaceDefinition : `interface` TypeName { Fields+ }
        /// </summary>
        /// <returns></returns>
        private SchemaDefinition ParseInterfaceDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("interface");
            var name = ParseName();
            var fields = Any(TokenKind.BRACE_L, ParseFieldDefinition, TokenKind.BRACE_R);
            return new InterfaceDefinition
            {
                Name = name,
                Fields = fields,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the union definition.
        /// UnionDefinition : `union` TypeName = UnionMembers
        /// </summary>
        /// <returns></returns>
        private SchemaDefinition ParseUnionDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("union");
            var name = ParseName();
            Expect(TokenKind.EQUALS);
            var types = ParseUnionMembers();
            return new UnionDefinition
            {
                Name = name,
                Types = types,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the union members.
        /// UnionMembers :
        ///   - NamedType
        ///   - UnionMembers | NamedType
        /// </summary>
        /// <returns></returns>
        private ImmutableArray<NamedType> ParseUnionMembers()
        {
            var members = ImmutableArray<NamedType>.Empty;
            do
            {
                members = members.Add(ParseNamedType());
            } while (Skip(TokenKind.PIPE));
            return members;
        }

        /// <summary>
        /// Parses the scalar definition.
        /// ScalarDefinition : `scalar` TypeName
        /// </summary>
        /// <returns></returns>
        private SchemaDefinition ParseScalarDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("scalar");
            var name = ParseName();
            return new ScalarDefinition
            {
                Name = name,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the enum definition.
        /// EnumDefinition : `enum` TypeName { EnumValueDefinition+ }
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private SchemaDefinition ParseEnumDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("enum");
            var name = ParseName();
            var values = Many(TokenKind.BRACE_L, ParseEnumValueDefinition, TokenKind.BRACE_R);
            return new EnumDefinition
            {
                Name = name,
                Values = values,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the enum value definition.
        /// EnumValueDefinition : EnumValue
        /// 
        /// EnumValue : Name
        /// </summary>
        /// <returns></returns>
        private EnumValueDefinition ParseEnumValueDefinition()
        {
            var start = Token.Start;
            var name = ParseName();
            return new EnumValueDefinition
            {
                Name = name,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the input object definition.
        /// InputObjectDefinition : `input` TypeName { InputFieldDefinition+ }
        /// </summary>
        /// <returns></returns>
        private SchemaDefinition ParseInputObjectDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("input");
            var name = ParseName();
            var fields = Any(TokenKind.BRACE_L, ParseInputFieldDefinition, TokenKind.BRACE_R);
            return new InputObjectDefinition
            {
                Name = name,
                Fields = fields,
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the input field definition.
        /// InputFieldDefinition : FieldName : Type
        /// </summary>
        /// <returns></returns>
        private InputFieldDefinition ParseInputFieldDefinition()
        {
            var start = Token.Start;
            var name = ParseName();
            Expect(TokenKind.COLON);
            var type = ParseType();
            return new InputFieldDefinition
            {
                Name = name,
                Type = type,
                Location = GetLocation(start),
            };
        }
    }
}
