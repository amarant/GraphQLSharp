using System;
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
    }

    public abstract class ParserCore
    {
        public Source Source { get; set; }
        protected Lexer _lexToken;
        public ParseOptions Options { get; set; }
        public int PrevEnd { get; set; }
        public Token Token { get; set; }

        /// <summary>
        /// Returns a location object, used to identify the place in
        /// the source that created a given parsed object.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        protected Location GetLocation(int start)
        {
            if (Options.NoLocation == true)
            {
                return null;
            }

            if (Options.NoSource == true)
            {
                return new Location
                {
                    Start = start,
                    End = PrevEnd,
                };
            }

            return new Location
            {
                Source = Source,
                Start = start,
                End = PrevEnd,
            };
        }

        /// <summary>
        /// Moves the internal parser object to the next lexed token.
        /// </summary>
        protected void Advance()
        {
            var prevEnd = Token.End;
            PrevEnd = prevEnd;
            Token = _lexToken.NextToken(prevEnd);
        }

        /// <summary>
        /// Determines if the next token is of a given kind.
        /// </summary>
        /// <returns>If the next token is of a given kind.</returns>
        protected bool Peek(TokenKind kind)
        {
            return Token.Kind == kind;
        }

        /// <summary>
        /// If the next token is of the given kind, return true after advancing
        /// the parser. Otherwise, do not change the parser state and return false.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        protected bool Skip(TokenKind kind)
        {
            var match = Token.Kind == kind;
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
        protected Token Expect(TokenKind kind)
        {
            var token = Token;
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
        protected Token ExpectKeyword(string value)
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
        protected SyntaxError Unexpected(Token atToken = null)
        {
            var token = atToken ?? Token;
            return new SyntaxError(
                Source,
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
        protected ImmutableArray<T> Any<T>(TokenKind openKind, Func<T> parseFn, TokenKind closeKind)
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
        protected ImmutableArray<T> Many<T>(TokenKind openKind, Func<T> parseFn, TokenKind closeKind)
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
    }
}
