﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphQLSharp.Language
{
    /// <summary>
    /// Configuration options to control parser behavior.
    /// </summary>
    public class ParseOptions
    {
        /// <summary>
        /// By default, the parser creates AST nodes that know the location
        /// in the source that they correspond to. This configuration flag
        /// disables that behavior for performance or testing.    
        /// </summary>
        public bool? NoLocation { get; set; }
        /// <summary>
        /// By default, the parser creates AST nodes that contain a reference
        /// to the source that they were created from. This configuration flag
        /// disables that behavior for performance or testing.
        /// </summary>
        public bool? NoSource { get; set; }

        public ParseOptions()
        {
        }
    }

    public class Parser
    {
        public Source Source { get; set; }
        public ParseOptions Options { get; set; }
        private Lexer _lexToken;
        public int PrevEnd { get; set; }
        public Token Token { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// Returns the parser object that is used to store state throughout the
        /// process of parsing.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        public Parser(Source source, ParseOptions options)
        {
            Source = source;
            Options = options ?? new ParseOptions();
            _lexToken = new Lexer(source);
            PrevEnd = 0;
            Token = _lexToken.NextToken();
        }

        /// <summary>
        /// Given a GraphQL source, parses it into a Document.
        /// Throws GraphQLError if a syntax error is encountered.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static Document Parse(Source source, ParseOptions options = null)
        {
            var parser = new Parser(source, options);
            return parser.ParseDocument();
        }

        /// <summary>
        /// Given a GraphQL source, parses it into a Document.
        /// Throws GraphQLError if a syntax error is encountered.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static Document Parse(String source, ParseOptions options = null)
        {
            return Parse(new Source(source), options);
        }

        /// <summary>
        /// Returns a location object, used to identify the place in
        /// the source that created a given parsed object.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public Location GetLocation(int start)
        {
            if (this.Options.NoLocation == true)
            {
                return null;
            }

            if (this.Options.NoSource == true)
            {
                return new Location
                {
                    Start = start,
                    End = this.PrevEnd,
                };
            }

            return new Location
            {
                Source = this.Source,
                Start = start,
                End = this.PrevEnd,
            };
        }

        /// <summary>
        /// Moves the internal parser object to the next lexed token.
        /// </summary>
        public void Advance()
        {
            var prevEnd = this.Token.End;
            this.PrevEnd = prevEnd;
            this.Token = this._lexToken.NextToken(prevEnd);
        }

        /// <summary>
        /// Determines if the next token is of a given kind.
        /// </summary>
        /// <returns>If the next token is of a given kind.</returns>
        public bool Peek(TokenKind kind)
        {
            return this.Token.Kind == kind;
        }

        /// <summary>
        /// If the next token is of the given kind, return true after advancing
        /// the parser. Otherwise, do not change the parser state and return false.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public bool Skip(TokenKind kind)
        {
            var match = this.Token.Kind == kind;
            if (match)
            {
                Advance();
            }
            return match;
        }

        /// <summary>
        /// If the next token is a keyword with the given value, return that token after
        /// advancing the parser. Otherwise, do not change the parser state and return
        /// false.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        /// <exception cref="SyntaxError"></exception>
        public Token Expect(TokenKind kind)
        {
            var token = this.Token;
            if (token.Kind == kind)
            {
                Advance();
                return token;
            }
            throw new SyntaxError(Source, token.Start,
                $"Expected {TokenKindHelpers.GetTokenKindDesc(kind)}, found {token.GetTokenDesc()}");
        }

        /// <summary>
        /// If the next token is a keyword with the given value, return that token after
        /// advancing the parser. Otherwise, do not change the parser state and return
        /// false.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="SyntaxError"></exception>
        public Token ExpectKeyword(string value)
        {
            var token = Token;
            if (token.Kind == TokenKind.NAME
                && token.Value == value)
            {
                Advance();
                return Token;
            }
            throw new SyntaxError(Source, token.Start,
                $"Expected \"{value}\", found {token.GetTokenDesc()}");
        }

        /// <summary>
        /// Helper function for creating an error when an unexpected lexed token
        /// is encountered.
        /// </summary>
        /// <param name="atToken">At token.</param>
        /// <returns></returns>
        public SyntaxError Unexpected(Token atToken = null)
        {
            var token = atToken ?? this.Token;
            return new SyntaxError(
                this.Source,
                token.Start,
                $"Unexpected {token.GetTokenDesc()}");
        }

        /// <summary>
        /// Returns a possibly empty list of parse nodes, determined by
        /// the parseFn. This list begins with a lex token of openKind
        /// and ends with a lex token of closeKind. Advances the parser
        /// to the next lex token after the closing token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openKind">Kind of the open.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <param name="closeKind">Kind of the close.</param>
        /// <returns></returns>
        public ImmutableArray<T> Any<T>(TokenKind openKind, Func<T> parseFn, TokenKind closeKind)
        {
            Expect(openKind);
            var nodes = ImmutableArray<T>.Empty;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (!Skip(closeKind))
            {
                nodes = nodes.Add(parseFn());
            }
            return nodes;
        }

        /// <summary>
        /// Returns a non-empty list of parse nodes, determined by
        /// the parseFn. This list begins with a lex token of openKind
        /// and ends with a lex token of closeKind. Advances the parser
        /// to the next lex token after the closing token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openKind">Kind of the open.</param>
        /// <param name="parseFn">The parse function.</param>
        /// <param name="closeKind">Kind of the close.</param>
        /// <returns></returns>
        public ImmutableArray<T> Many<T>(TokenKind openKind, Func<T> parseFn, TokenKind closeKind)
        {
            Expect(openKind);
            var nodes = ImmutableArray.Create(parseFn());
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (!Skip(closeKind))
            {
                nodes = nodes.Add(parseFn());
            }
            return nodes;
        }

        /// <summary>
        /// Converts a name lex token into a name parse node.
        /// </summary>
        /// <returns></returns>
        public Name ParseName()
        {
            var token = Expect(TokenKind.NAME);
            return new Name
            {
                Value = token.Value,
                Location = GetLocation(token.Start),
            };
        }

        // Implements the parsing rules in the Document section.

        public Document ParseDocument()
        {
            var start = this.Token.Start;
            var definitions = ImmutableArray<IDefinition>.Empty;
            do
            {
                if (Peek(TokenKind.BRACE_L))
                {
                    definitions = definitions.Add(ParseOperationDefinition());
                } else if (Peek(TokenKind.NAME))
                {
                    if (this.Token.Value == "query" || this.Token.Value == "mutation")
                    {
                        definitions = definitions.Add(ParseOperationDefinition());
                    }
                    else if (this.Token.Value == "fragment")
                    {
                        definitions = definitions.Add(ParseFragmentDefinition());
                    }
                    else
                    {
                        throw Unexpected();
                    }
                }
                else
                {
                    throw Unexpected();
                }
            } while (!Skip(TokenKind.EOF));
            return new Document
            {
                Definitions = definitions,
                Location = GetLocation(start),
            };
        }

        // Implements the parsing rules in the Operations section.

        private OperationDefinition ParseOperationDefinition()
        {
            var start = this.Token.Start;
            if (Peek(TokenKind.BRACE_L))
            {
                return new OperationDefinition
                {
                    Operation = OperationType.Query,
                    Name = null,
                    VariableDefinitions = ImmutableArray<VariableDefinition>.Empty,
                    Directives = ImmutableArray<Directive>.Empty,
                    SelectionSet = ParseSelectionSet(),
                    Location = GetLocation(start),
                };
            }
            var operationToken = Expect(TokenKind.NAME);
            OperationType operation;
            switch (operationToken.Value)
            {
                case "query":
                    operation = OperationType.Query;
                    break;
                case "mutation":
                    operation = OperationType.Mutation;
                    break;
                default:
                    throw new Exception();
            }
            return new OperationDefinition
            {
                Operation = operation,
                Name = ParseName(),
                VariableDefinitions = ParseVariableDefinitions(),
                Directives = ParseDirectives(),
                SelectionSet = ParseSelectionSet(),
                Location = GetLocation(start),
            };
        }

        public ImmutableArray<VariableDefinition> ParseVariableDefinitions()
        {
            if (Peek(TokenKind.PAREN_L))
            {
                return Many(TokenKind.PAREN_L, ParseVariableDefinition, TokenKind.PAREN_R);
            }
            else
            {
                return ImmutableArray<VariableDefinition>.Empty;
            }
        }

        public VariableDefinition ParseVariableDefinition()
        {
            var start = Token.Start;
            var variable = ParseVariable();
            Expect(TokenKind.COLON);
            var type = ParseType();
            return new VariableDefinition
            {
                Variable = variable,
                Type = type,
                DefaultValue = Skip(TokenKind.EQUALS) ? ParseValue(true) : null,
                Location = GetLocation(start),
            };
        }

        private Variable ParseVariable()
        {
            var start = Token.Start;
            Expect(TokenKind.DOLLAR);
            return new Variable
            {
                Name = ParseName(),
                Location = GetLocation(start),
            };
        }

        private SelectionSet ParseSelectionSet()
        {
            var start = Token.Start;
            return new SelectionSet
            {
                Selections = Many(TokenKind.BRACE_L, ParseSelection, TokenKind.BRACE_R),
                Location = GetLocation(start),
            };
        }

        private ISelection ParseSelection()
        {
            if (Peek(TokenKind.SPREAD))
            {
                return ParseFragment();
            }
            else
            {
                return ParseField();
            }
        }

        /// <summary>
        /// Corresponds to both Field and Alias in the spec
        /// </summary>
        /// <returns></returns>
        private Field ParseField()
        {
            var start = Token.Start;
            var nameOrAlias = ParseName();
            Name name;
            Name alias;
            if (Skip(TokenKind.COLON))
            {
                alias = nameOrAlias;
                name = ParseName();
            }
            else
            {
                alias = null;
                name = nameOrAlias;
            }

            return new Field
            {
                Alias = alias,
                Name = name,
                Arguments = ParseArguments(),
                Directives = ParseDirectives(),
                SelectionSet = Peek(TokenKind.BRACE_L) ? ParseSelectionSet() : null,
                Location = GetLocation(start),
            };
        }

        private ImmutableArray<Argument> ParseArguments()
        {
            if (Peek(TokenKind.PAREN_L))
            {
                return Many(TokenKind.PAREN_L, ParseArgument, TokenKind.PAREN_R);
            }
            else
            {
                return ImmutableArray<Argument>.Empty;
            }
        }

        private Argument ParseArgument()
        {
            var start = Token.Start;
            var name = ParseName();
            Expect(TokenKind.COLON);
            var value = ParseValue(false);
            return new Argument
            {
                Name = name,
                Value = value,
                Location = GetLocation(start),
            };
        }

        // Implements the parsing rules in the Fragments section.

        /// <summary>
        /// Parses the fragment.
        /// Corresponds to both FragmentSpread and InlineFragment in the spec
        /// </summary>
        /// <returns></returns>
        private ISelection ParseFragment()
        {
            var start = Token.Start;
            Expect(TokenKind.SPREAD);
            if (Token.Value == "on")
            {
                Advance();
                return new InlineFragment
                {
                    TypeCondition = ParseNamedType(),
                    Directives = ParseDirectives(),
                    SelectionSet = ParseSelectionSet(),
                    Location = GetLocation(start),
                };
            }

            return new FragmentSpread
            {
                Name = ParseFragmentName(),
                Directives = ParseDirectives(),
                Location = GetLocation(start),
            };
        }

        /// <summary>
        /// Parses the name of the fragment.
        /// </summary>
        /// <returns></returns>
        private Name ParseFragmentName()
        {
            if (Token.Value == "on")
            {
                throw Unexpected();
            }
            return ParseName();
        }

        private FragmentDefinition ParseFragmentDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("fragment");
            var name = ParseFragmentName();
            ExpectKeyword("on");
            var typeCondition = ParseNamedType();
            return new FragmentDefinition
            {
                Name = name,
                TypeCondition = typeCondition,
                Directives = ParseDirectives(),
                SelectionSet = ParseSelectionSet(),
                Location = GetLocation(start),
            };
        }

        // Implements the parsing rules in the Values section.

        private IValue ParseVariableValue()
        {
            return ParseValue(false);
        }

        private IValue ParseConstValue()
        {
            return ParseValue(true);
        }

        private IValue ParseValue(bool isConst)
        {
            var token = Token;
            switch (token.Kind)
            {
                case TokenKind.BRACKET_L:
                    return ParseArray(isConst);
                case TokenKind.BRACE_L:
                    return ParseObject(isConst);
                case TokenKind.INT:
                    Advance();
                    return new IntValue
                    {
                        Value = token.Value,
                        Location = GetLocation(token.Start),
                    };
                case TokenKind.FLOAT:
                    Advance();
                    return new FloatValue
                    {
                        Value = token.Value,
                        Location = GetLocation(token.Start),
                    };
                case TokenKind.STRING:
                    Advance();
                    return new StringValue
                    {
                        Value = token.Value,
                        Location = GetLocation(token.Start),
                    };
                case TokenKind.NAME:
                    Advance();
                    switch (token.Value)
                    {
                        case "true":
                        case "false":
                            return new BooleanValue
                            {
                                Value = token.Value == "true",
                                Location = GetLocation(token.Start),
                            };
                    }
                    return new EnumValue
                    {
                        Value = token.Value,
                        Location = GetLocation(token.Start),
                    };
                case TokenKind.DOLLAR:
                    if (!isConst)
                    {
                        return ParseVariable();
                    }
                    break;
            }
            throw Unexpected();
        }

        private ArrayValue ParseArray(bool isConst)
        {
            var start = Token.Start;
            var value = isConst
                ? Any(TokenKind.BRACKET_L, ParseConstValue, TokenKind.BRACKET_R)
                : Any(TokenKind.BRACKET_L, ParseVariableValue, TokenKind.BRACKET_R);
            return new ArrayValue
            {
                Values = value,
                Location = GetLocation(start),
            };
        }

        private ObjectValue ParseObject(bool isConst)
        {
            var start = Token.Start;
            Expect(TokenKind.BRACE_L);
            var fieldNames = new Dictionary<String, bool>();
            var fields = ImmutableArray<ObjectField>.Empty;
            while (!Skip(TokenKind.BRACE_R))
            {
                fields = fields.Add(ParseObjectField(isConst, fieldNames));
            }
            return new ObjectValue
            {
                Fields = fields,
                Location = GetLocation(start),
            };
        }

        private ObjectField ParseObjectField(bool isConst,
            Dictionary<String, bool> fieldNames)
        {
            var start = Token.Start;
            var name = ParseName();
            if (fieldNames.ContainsKey(name.Value))
            {
                throw new SyntaxError(Source, start,
                    $"Duplicate input object field {name.Value}.");
            }
            fieldNames.Add(name.Value, true);
            Expect(TokenKind.COLON);
            var value = ParseValue(isConst);
            return new ObjectField
            {
                Name = name,
                Value = value,
                Location = GetLocation(start),
            };
        }

        // Implements the parsing rules in the Directives section.

        private ImmutableArray<Directive> ParseDirectives()
        {
            var directives = ImmutableArray<Directive>.Empty;
            while (Peek(TokenKind.AT))
            {
                directives = directives.Add(ParseDirective());
            }
            return directives;
        }

        private Directive ParseDirective()
        {
            var start = Token.Start;
            Expect(TokenKind.AT);
            return new Directive
            {
                Name = ParseName(),
                Arguments = ParseArguments(),
                Location = GetLocation(start),
            };
        }

        // Implements the parsing rules in the Types section.

        /// <summary>
        /// Parses the type.
        /// Handles the Type: NamedType, ListType, and NonNullType parsing rules.
        /// </summary>
        /// <returns></returns>
        private IType ParseType()
        {
            var start = Token.Start;
            INameOrListType type;
            if (Skip(TokenKind.BRACE_L))
            {
                var innerType = ParseType();
                Expect(TokenKind.BRACKET_R);
                type = new ListType
                {
                    Type = innerType,
                    Location = GetLocation(start),
                };
            }
            else
            {
                type = ParseNamedType();
            }
            if (Skip(TokenKind.BANG))
            {
                return new NonNullType
                {
                    Type = type,
                    Location = GetLocation(start),
                };
            }

            return type;
        }

        /// <summary>
        /// Parses a NamedType.
        /// </summary>
        /// <returns></returns>
        private NamedType ParseNamedType()
        {
            var start = Token.Start;
            return new NamedType
            {
                Name = ParseName(),
                Location = GetLocation(start),
            };
        }
    }
}
