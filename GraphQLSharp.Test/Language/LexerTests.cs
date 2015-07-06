using System;
using FluentAssertions;
using GraphQLSharp.Language;
using Xunit;

namespace GraphQLSharp.Test.Language
{
    public class LexerTests
    {
        private static Token LexOne(String body)
        {
            return (new Lexer(new Source(body))).NextToken(null);
        }

        [Fact]
        public void LexerSkipsWhitespace()
        {
            var firstToken = LexOne("\n\n    foo\n\n\n");
            firstToken.ShouldBeEquivalentTo(new Token(TokenKind.NAME, 6, 9, "foo"));

            firstToken = LexOne("\n    #comment\n    foo#comment\n");
            firstToken.ShouldBeEquivalentTo(new Token(TokenKind.NAME, 18, 21, "foo"));

            firstToken = LexOne(",,,foo,,,");
            firstToken.ShouldBeEquivalentTo(new Token(TokenKind.NAME, 3, 6, "foo"));
        }

        [Fact]
        public void LexerErrorsRespectWhitespace()
        {
            Action act = () => LexOne("\n\n    ?\n\n\n");
            act.ShouldThrow<SyntaxError>()
                .WithMessage(
                "Syntax Error GraphQL (3:5) Unexpected character \"?\"\n" +
                "\n" +
                "2: \n" +
                "3:     ?\n" +
                "       ^\n" +
                "4: \n");
        }

        [Fact]
        public void LexerLexesStrings()
        {
            LexOne("\"simple\"").ShouldBeEquivalentTo(
                new Token(TokenKind.STRING, 0, 8, "simple"));

            LexOne("\" white space \"").ShouldBeEquivalentTo(
                new Token(TokenKind.STRING, 0, 15, " white space "));

            LexOne("\"quote \\\"\"").ShouldBeEquivalentTo(
                new Token(TokenKind.STRING, 0, 10, "quote \\\""));

            var lexOne = LexOne("\"escaped \\n\\r\\b\\t\\f\"");
            lexOne.ShouldBeEquivalentTo(
                new Token(TokenKind.STRING, 0, 20, "escaped \n\r\b\t\f"));
        }
    }
}
