using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Given a GraphQL source, parses it into a Document. Throws on error.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static Node Parse(Source source, ParseOptions options = null)
        {
            var parser = new Parser(source, options);
            return parser.ParseDocument();
        }

        /// <summary>
        /// Given a GraphQL source, parses it into a Document. Throws on error.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static Node Parse(String source, ParseOptions options = null)
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
            throw new SyntaxError(this.Source, token.Start,
                String.Format("Expected {0}, found {1}",
                    TokenKindHelpers.GetTokenKindDesc(kind),
                    token.GetTokenDesc()));
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
                String.Format("Expected \"{0}\", found {1}", value, token.GetTokenDesc()));
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
                String.Format("Unexpected {0}", token.GetTokenDesc()));
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
        public List<T> Any<T>(TokenKind openKind, Func<T> parseFn, TokenKind closeKind)
        {
            Expect(openKind);
            var nodes = new List<T>();
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (!Skip(closeKind))
            {
                nodes.Add(parseFn());
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
        public List<T> Many<T>(TokenKind openKind, Func<T> parseFn, TokenKind closeKind)
        {
            Expect(openKind);
            var nodes = new List<T>{parseFn()};
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (!Skip(closeKind))
            {
                nodes.Add(parseFn());
            }
            return nodes;
        }

        /// <summary>
        /// Converts a name lex token into a name parse node.
        /// </summary>
        /// <returns></returns>
        public Node ParseName()
        {
            var token = Expect(TokenKind.NAME);
            return Node.CreateName(token.Value, GetLocation(token.Start));
        }

        // Implements the parsing rules in the Document section.

        public Node ParseDocument()
        {
            var start = this.Token.Start;
            var definitions = new List<Node>();
            do
            {
                if (Peek(TokenKind.BRACE_L))
                {
                    definitions.Add(ParseOperationDefinition());
                } else if (Peek(TokenKind.NAME))
                {
                    if (this.Token.Value == "query" || this.Token.Value == "mutation")
                    {
                        definitions.Add(ParseOperationDefinition());
                    }
                    else if (this.Token.Value == "fragment")
                    {
                        definitions.Add(ParseFragmentDefinition());
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
            return Node.CreateDocument(
                definitions: definitions,
                location: GetLocation(start)
            );
        }

        // Implements the parsing rules in the Operations section.

        private Node ParseOperationDefinition()
        {
            var start = this.Token.Start;
            if (Peek(TokenKind.BRACE_L))
            {
                return Node.CreateOperationDefinition(
                    operation: OperationType.Query,
                    name: null,
                    variableDefinitions: null,
                    directives: new List<Node>(),
                    selectionSet: ParseSelectionSet(),
                    location: GetLocation(start)
                );
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
            return Node.CreateOperationDefinition(
                operation: operation,
                name: ParseName(),
                variableDefinitions: ParseVariableDefinitions(),
                directives: ParseDirectives(),
                selectionSet: ParseSelectionSet(),
                location: GetLocation(start)
            );
        }

        public List<Node> ParseVariableDefinitions()
        {
            if (Peek(TokenKind.PAREN_L))
            {
                return Many(TokenKind.PAREN_L, ParseVariableDefinition, TokenKind.PAREN_R);
            }
            else
            {
                return new List<Node>();
            }
        }

        public Node ParseVariableDefinition()
        {
            var start = Token.Start;
            var variable = ParseVariable();
            Expect(TokenKind.COLON);
            var type = ParseType();
            return Node.CreateVariableDefinition(
                variable: variable,
                type: type,
                defaultValue: Skip(TokenKind.EQUALS) ? ParseValue(true) : null,
                location: GetLocation(start)
            );
        }

        private Node ParseVariable()
        {
            var start = Token.Start;
            Expect(TokenKind.DOLLAR);
            return Node.CreateVariable(
                name: ParseName(),
                location: GetLocation(start)
            );
        }

        private Node ParseSelectionSet()
        {
            var start = Token.Start;
            return Node.CreateSelectionSet(
                selections: Many(TokenKind.BRACE_L, ParseSelection, TokenKind.BRACE_R),
                location: GetLocation(start));
        }

        private Node ParseSelection()
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
        private Node ParseField()
        {
            var start = Token.Start;
            var nameOrAlias = ParseName();
            Node name;
            Node alias;
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

            return Node.CreateField(
                alias: alias,
                name: name,
                arguments: ParseArguments(),
                directives: ParseDirectives(),
                selectionSet: Peek(TokenKind.BRACE_L) ? ParseSelectionSet() : null,
                location: GetLocation(start));
        }

        private List<Node> ParseArguments()
        {
            if (Peek(TokenKind.PAREN_L))
            {
                return Many(TokenKind.PAREN_L, ParseArgument, TokenKind.PAREN_R);
            }
            else
            {
                return new List<Node>();
            }
        }

        private Node ParseArgument()
        {
            var start = Token.Start;
            var name = ParseName();
            Expect(TokenKind.COLON);
            var value = ParseValue(false);
            return Node.CreateArgument(
                name: name,
                value: value,
                location: GetLocation(start));
        }

        // Implements the parsing rules in the Fragments section.

        /// <summary>
        /// Parses the fragment.
        /// Corresponds to both FragmentSpread and InlineFragment in the spec
        /// </summary>
        /// <returns></returns>
        private Node ParseFragment()
        {
            var start = Token.Start;
            Expect(TokenKind.SPREAD);
            if (Token.Value == "on")
            {
                Advance();
                return Node.CreateInlineFragment(
                    typeCondition: ParseName(),
                    directives: ParseDirectives(),
                    selectionSet: ParseSelectionSet(),
                    location: GetLocation(start));
            }

            return Node.CreateFragmentSpread(
                name: ParseName(),
                directives: ParseDirectives(),
                location: GetLocation(start));
        }

        private Node ParseFragmentDefinition()
        {
            var start = Token.Start;
            ExpectKeyword("fragment");
            var name = ParseName();
            ExpectKeyword("on");
            var typeCondition = ParseName();
            return Node.CreateFragmentDefinition(
                name = name,
                typeCondition = typeCondition,
                directives: ParseDirectives(),
                selectionSet: ParseSelectionSet(),
                location: GetLocation(start));
        }

        // Implements the parsing rules in the Values section.

        private Node ParseVariableValue()
        {
            return ParseValue(false);
        }

        private Node ParseConstValue()
        {
            return ParseValue(true);
        }

        private Node ParseValue(bool isConst)
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
                    return Node.CreateIntValue(
                        value: token.Value,
                        location: GetLocation(token.Start));
                case TokenKind.FLOAT:
                    Advance();
                    return Node.CreateFloatValue(
                        value: token.Value,
                        location: GetLocation(token.Start));
                case TokenKind.STRING:
                    Advance();
                    return Node.CreateStringValue(
                        value: token.Value,
                        location: GetLocation(token.Start));
                case TokenKind.NAME:
                    Advance();
                    switch (token.Value)
                    {
                        case "true":
                        case "false":
                            return Node.CreateBooleanValue(
                                value: token.Value == "true",
                                location: GetLocation(token.Start));
                    }
                    return Node.CreateEnumValue(
                        value: token.Value,
                        location: GetLocation(token.Start));
                case TokenKind.DOLLAR:
                    if (!isConst)
                    {
                        return ParseVariable();
                    }
                    break;
            }
            throw Unexpected();
        }

        private Node ParseArray(bool isConst)
        {
            var start = Token.Start;
            var value = isConst
                ? Any(TokenKind.BRACKET_L, ParseConstValue, TokenKind.BRACKET_R)
                : Any(TokenKind.BRACKET_L, ParseVariableValue, TokenKind.BRACKET_R);
            return Node.CreateArrayValue(
                values: value,
                location: GetLocation(start));
        }

        private Node ParseObject(bool isConst)
        {
            var start = Token.Start;
            Expect(TokenKind.BRACE_L);
            var fieldNames = new Dictionary<String, bool>();
            var fields = new List<Node>();
            while (!Skip(TokenKind.BRACE_R))
            {
                fields.Add(ParseObjectField(isConst, fieldNames));
            }
            return Node.CreateObjectValue(
                fields: fields,
                location: GetLocation(start));
        }

        private Node ParseObjectField(bool isConst,
            Dictionary<String, bool> fieldNames)
        {
            var start = Token.Start;
            var name = ParseName();
            if (fieldNames.ContainsKey((String) name["Value"]))
            {
                throw new SyntaxError(Source, start,
                    String.Format("Duplicate input object field {0}.", name["Value"]));
            }
            fieldNames.Add((String) name["Value"], true);
            Expect(TokenKind.COLON);
            var value = ParseValue(isConst);
            return Node.CreateObjectField(
                name: name,
                value: value,
                location: GetLocation(start));
        }

        // Implements the parsing rules in the Directives section.

        private List<Node> ParseDirectives()
        {
            var directives = new List<Node>();
            while (Peek(TokenKind.AT))
            {
                directives.Add(ParseDirective());
            }
            return directives;
        }

        private Node ParseDirective()
        {
            var start = Token.Start;
            Expect(TokenKind.AT);
            return Node.CreateDirective(
                name: ParseName(),
                value: Skip(TokenKind.COLON) ? ParseValue(false) : null,
                location: GetLocation(start));
        }

        // Implements the parsing rules in the Types section.



        /// <summary>
        /// Parses the type.
        /// Handles the Type: TypeName, ListType, and NonNullType parsing rules.
        /// </summary>
        /// <returns></returns>
        private Node ParseType()
        {
            var start = Token.Start;
            Node type;
            if (Skip(TokenKind.BRACE_L))
            {
                var innerType = ParseType();
                Expect(TokenKind.BRACKET_R);
                type = Node.CreateListType(
                    type: innerType,
                    location: GetLocation(start));
            }
            else
            {
                type = ParseName();
            }
            if (Skip(TokenKind.BANG))
            {
                return Node.CreateNonNullType(
                    type: type,
                    location: GetLocation(start));
            }

            return type;
        }
    }
}
