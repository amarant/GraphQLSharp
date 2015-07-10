using System;
using FluentAssertions;
using GraphQLSharp.Language;
using Xunit;

namespace GraphQLSharp.Test.Language
{
    public class LexerTests
    {
        private static void LexOne(String body,
            TokenKind kind, int start, int end, string value)
        {
            (new Lexer(new Source(body)))
                .NextToken(null)
                .ShouldBeEquivalentTo(new Token(kind, start, end, value));
        }

        private static void LexErr(String body, String message)
        {
            Action action = () => (new Lexer(new Source(body)))
                .NextToken(null);
            action.ShouldThrow<SyntaxError>().Where(err => err.Message.StartsWith(message));
        }

        [Fact]
        public void LexerSkipsWhitespace()
        {
            LexOne("\n\n    foo\n\n\n", TokenKind.NAME, 6, 9, "foo");
            LexOne("\n    #comment\n    foo#comment\n", TokenKind.NAME, 18, 21, "foo");
            LexOne(",,,foo,,,", TokenKind.NAME, 3, 6, "foo");
        }

        [Fact]
        public void LexerErrorsRespectWhitespace()
        {
            LexErr("\n\n    ?\n\n\n",
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
            LexOne("\"simple\"", TokenKind.STRING, 0, 8, "simple");
            LexOne("\" white space \"", TokenKind.STRING, 0, 15, " white space ");
            LexOne("\"quote \\\"\"", TokenKind.STRING, 0, 10, "quote \"");
            LexOne("\"escaped \\n\\r\\b\\t\\f\"", TokenKind.STRING, 0, 20, "escaped \n\r\b\t\f");
            LexOne("\"slashes \\\\ \\/\"", TokenKind.STRING, 0, 15, "slashes \\ /");
            LexOne("\"unicode \\u1234\\u5678\\u90AB\\uCDEF\"", TokenKind.STRING, 0, 34, "unicode \u1234\u5678\u90AB\uCDEF");
        }

        [Fact]
        public void LexerLexReportsUsefulStringErrors()
        {
            LexErr("\"no end quote", "Syntax Error GraphQL (1:14) Unterminated string");
            LexErr("\"multi\nline\"", "Syntax Error GraphQL (1:7) Unterminated string");
            LexErr("\"multi\rline\"", "Syntax Error GraphQL (1:7) Unterminated string");
            LexErr("\"multi\u2028line\"", "Syntax Error GraphQL (1:7) Unterminated string");
            LexErr("\"multi\u2029line\"", "Syntax Error GraphQL (1:7) Unterminated string");
            LexErr("\"bad \\z esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
            LexErr("\"bad \\x esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
            LexErr("\"bad \\u1 esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
            LexErr("\"bad \\u0XX1 esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
            LexErr("\"bad \\uXXXX esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
            LexErr("\"bad \\uFXXX esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
            LexErr("\"bad \\uXXXF esc\"", "Syntax Error GraphQL (1:7) Bad character escape sequence");
        }

        [Fact]
        public void LexerLexesNumbers()
        {
            LexOne("4", TokenKind.INT, 0, 1, "4");
            LexOne("4.123", TokenKind.FLOAT, 0, 5, "4.123");
            LexOne("-4", TokenKind.INT, 0, 2, "-4");
            LexOne("9", TokenKind.INT, 0, 1, "9");
            LexOne("0", TokenKind.INT, 0, 1, "0");
            LexOne("00", TokenKind.INT, 0, 1, "0");
            LexOne("-4.123", TokenKind.FLOAT, 0, 6, "-4.123");
            LexOne("0.123", TokenKind.FLOAT, 0, 5, "0.123");
            LexOne("-1.123e4", TokenKind.FLOAT, 0, 8, "-1.123e4");
            LexOne("-1.123e-4", TokenKind.FLOAT, 0, 9, "-1.123e-4");
            LexOne("-1.123e4567", TokenKind.FLOAT, 0, 11, "-1.123e4567");
        }

        [Fact]
        public void LexerLexReportsUsefulNumberErrors()
        {
            LexErr("+1", "Syntax Error GraphQL (1:1) Unexpected character \" + \"");
            LexErr("1.", "Syntax Error GraphQL (1:3) Invalid number");
            LexErr("1.A", "Syntax Error GraphQL (1:3) Invalid number");
            LexErr("-A", "Syntax Error GraphQL (1:2) Invalid number");
            LexErr("1.0e+4", "Syntax Error GraphQL (1:5) Invalid number");
            LexErr("1.0e", "Syntax Error GraphQL (1:5) Invalid number");
            LexErr("1.0eA", "Syntax Error GraphQL (1:5) Invalid number");
        }

        [Fact]
        public void LexerLexesPunctuation()
        {
            LexOne("!", TokenKind.BANG, 0, 1, null);
            LexOne("$", TokenKind.DOLLAR, 0, 1, null);
            LexOne("(", TokenKind.PAREN_L, 0, 1, null);
            LexOne(")", TokenKind.PAREN_R, 0, 1, null);
            LexOne("...", TokenKind.SPREAD, 0, 3, null);
            LexOne(":", TokenKind.COLON, 0, 1, null);
            LexOne("=", TokenKind.EQUALS, 0, 1, null);
            LexOne("@", TokenKind.AT, 0, 1, null);
            LexOne("[", TokenKind.BRACKET_L, 0, 1, null);
            LexOne("]", TokenKind.BRACKET_R, 0, 1, null);
            LexOne("{", TokenKind.BRACE_L, 0, 1, null);
            LexOne("|", TokenKind.PIPE, 0, 1, null);
            LexOne("}", TokenKind.BRACE_R, 0, 1, null);
        }

        [Fact]
        public void LexerLexReportsUsefulUnknownCharacterError()
        {
            LexErr("..", "Syntax Error GraphQL (1:1) Unexpected character \".\"");
            LexErr("?", "Syntax Error GraphQL (1:1) Unexpected character \"?\"");
            LexErr("\u203B", "Syntax Error GraphQL (1:1) Unexpected character \"\u203B\"");
        }
    }
}
