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
            Options = options;
            _lexToken = new Lexer(source);
            PrevEnd = 0;
            Token = _lexToken.NextToken();
        }

        /// <summary>
        /// Given a GraphQL source, parses it into a Document. Throws on error.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        public static void Parse(Source source, ParseOptions options)
        {
            var parser = new Parser(source, options);
        }

        /// <summary>
        /// Given a GraphQL source, parses it into a Document. Throws on error.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        public static void Parse(String source, ParseOptions options)
        {
            Parse(new Source(source), options);
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
        public List<T> Any<T>(TokenKind openKind, Func<Parser, T> parseFn, TokenKind closeKind)
        {
            Expect(openKind);
            var nodes = new List<T>();
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (!Skip(closeKind))
            {
                nodes.Add(parseFn(this));
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
            var definitions = new List<IDefinition>();
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
                    VariableDefinitions = null,
                    Directives = new List<Directive>(),
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
                Directives = ParseDirective(),
                SelectionSet = ParseSelectionSet(),
                Location = GetLocation(start),
            };
        }

        public List<VariableDefinition> ParseVariableDefinitions()
        {
            if (Peek(TokenKind.PAREN_L))
            {
                return Many(TokenKind.PAREN_L, ParseVariableDefinition, TokenKind.PAREN_R);
            }
            else
            {
                return new List<VariableDefinition>();
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

        private List<Argument> ParseArguments()
        {
            if (Peek(TokenKind.PAREN_L))
            {
                return Many(TokenKind.PAREN_L, ParseArgument, TokenKind.PAREN_R);
            }
            else
            {
                return new List<Argument>();
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

        private List<Directive> ParseDirectives()
        {
            throw new NotImplementedException();
        }



        private ISelection ParseFragment()
        {
            throw new NotImplementedException();
        }

        private IValue ParseValue(bool b)
        {
            throw new NotImplementedException();
        }

        private IType ParseType()
        {
            throw new NotImplementedException();
        }




        private List<Directive> ParseDirective()
        {
            throw new NotImplementedException();
        }




        private IDefinition ParseFragmentDefinition()
        {
            throw new NotImplementedException();
        }


    }
}
