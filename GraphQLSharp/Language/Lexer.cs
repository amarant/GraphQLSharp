using System;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLSharp.Language
{
    /// <summary>
    /// A representation of a lexed Token. Value is optional, is it is
    /// not needed for punctuators like BANG or PAREN_L.
    /// </summary>
    public class Token
    {
        public TokenKind Kind { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public String Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="value">The value.</param>
        public Token(TokenKind kind, int start, int end, string value = null)
        {
            Kind = kind;
            Start = start;
            End = end;
            Value = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetTokenDesc();
        }

        /// <summary>
        /// A helper function to describe a token as a string for debugging
        /// </summary>
        /// <returns></returns>
        public string GetTokenDesc()
        {
            return String.IsNullOrEmpty(Value)
                ? TokenKindHelpers.GetTokenKindDesc(Kind)
                : String.Format("{0} \"{1}\"", TokenKindHelpers.GetTokenKindDesc(Kind), Value);
        }
    }

    /// <summary>
    /// An enum describing the different kinds of tokens that the lexer emits.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TokenKind
    {
      EOF= 1,
      BANG= 2,
      DOLLAR= 3,
      PAREN_L= 4,
      PAREN_R= 5,
      SPREAD= 6,
      COLON= 7,
      EQUALS= 8,
      AT= 9,
      BRACKET_L= 10,
      BRACKET_R= 11,
      BRACE_L= 12,
      PIPE= 13,
      BRACE_R= 14,
      NAME= 15,
      VARIABLE= 16,
      INT= 17,
      FLOAT= 18,
      STRING= 19,
    }

    public static class TokenKindHelpers
    {
        public static String[] TokenDescription = {
            "", // placeholder because EOF start at 1 to follow graphql-js
            "EOF",
            "!",
            "$",
            "(",
            ")",
            "...",
            ":",
            "=",
            "@",
            "[",
            "]",
            "{",
            "|",
            "}",
            "Name",
            "Variable",
            "Int",
            "Float",
            "String",
        };

        /// <summary>
        /// Gets the token kind desc.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns></returns>
        public static string GetTokenKindDesc(TokenKind kind)
        {
            if (kind >= TokenKind.EOF && kind <= TokenKind.STRING)
            {
                return TokenDescription[(int)kind];
            }
            return null;
        }
    }

    public class Lexer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lexer"/> class.
        /// Given a Source object, this returns a Lexer for that source.
        /// A Lexer is a function that acts like a generator in that every time
        /// it is called, it returns the next token in the Source. Assuming the
        /// source lexes, the final Token emitted by the lexer will be of kind
        /// EOF, after which the lexer will repeatedly return EOF tokens whenever
        /// called.
        /// 
        /// The argument to the lexer function is optional, and can be used to
        /// rewind or fast forward the lexer to a new position in the source.
        /// </summary>
        public Lexer(Source source)
        {
            Source = source;
            PrevPosition = 0;
        }

        public Source Source { get; private set; }
        public int PrevPosition { get; set; }

        public Token NextToken(int? resetPosition = null)
        {
            var token = ReadToken(
              Source,
              resetPosition ?? PrevPosition
            );
            PrevPosition = token.End;
            return token;
        }

        /// <summary>
        /// Gets the next token from the source starting at the given position.
        /// 
        /// This skips over whitespace and comments until it finds the next lexable
        /// token, then lexes punctuators immediately or calls the appropriate helper
        /// fucntion for more complicated tokens.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fromPosition">The from position.</param>
        /// <returns></returns>
        private Token ReadToken(Source source, int fromPosition)
        {
            var body = source.Body;
            var bodyLength = body.Length;

            var position = PositionAfterWhitespace(body, fromPosition);

            if (position >= bodyLength)
            {
                return new Token(TokenKind.EOF, position, position);
            }

            var code = body[position];
            switch ((int)code) {
            // !
            case 33: return new Token(TokenKind.BANG, position, position + 1);
            // $
            case 36: return new Token(TokenKind.DOLLAR, position, position + 1);
            // (
            case 40: return new Token(TokenKind.PAREN_L, position, position + 1);
            // )
            case 41: return new Token(TokenKind.PAREN_R, position, position + 1);
            // .
            case 46:
                if (position + 2 < bodyLength &&
                    body[position + 1] == 46 &&
                    body[position + 2] == 46) {
                return new Token(TokenKind.SPREAD, position, position + 3);
                }
                break;
            // :
            case 58: return new Token(TokenKind.COLON, position, position + 1);
            // =
            case 61: return new Token(TokenKind.EQUALS, position, position + 1);
            // @
            case 64: return new Token(TokenKind.AT, position, position + 1);
            // [
            case 91: return new Token(TokenKind.BRACKET_L, position, position + 1);
            // ]
            case 93: return new Token(TokenKind.BRACKET_R, position, position + 1);
            // {
            case 123: return new Token(TokenKind.BRACE_L, position, position + 1);
            // |
            case 124: return new Token(TokenKind.PIPE, position, position + 1);
            // }
            case 125: return new Token(TokenKind.BRACE_R, position, position + 1);
            // A-Z
            case 65: case 66: case 67: case 68: case 69: case 70: case 71: case 72:
            case 73: case 74: case 75: case 76: case 77: case 78: case 79: case 80:
            case 81: case 82: case 83: case 84: case 85: case 86: case 87: case 88:
            case 89: case 90:
            // _
            case 95:
            // a-z
            case 97: case 98: case 99: case 100: case 101: case 102: case 103: case 104:
            case 105: case 106: case 107: case 108: case 109: case 110: case 111:
            case 112: case 113: case 114: case 115: case 116: case 117: case 118:
            case 119: case 120: case 121: case 122:
                return ReadName(source, position);
            // -
            case 45:
            // 0-9
            case 48: case 49: case 50: case 51: case 52:
            case 53: case 54: case 55: case 56: case 57:
                return ReadNumber(source, position, code);
            // "
            case 34: return ReadString(source, position);
            }

            throw new SyntaxError(source, position,
                String.Format("Unexpected character \"{0}\"", char.ConvertFromUtf32(code)));
        }

        /// <summary>
        /// Reads from body starting at startPosition until it finds a non-whitespace
        /// or commented character, then returns the position of that character for
        /// lexing.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="startPosition">The start position.</param>
        /// <returns></returns>
        private static int PositionAfterWhitespace(String body, int startPosition)
        {
            var bodyLength = body.Length;
            var position = startPosition;
            while (position < bodyLength) {
                var code = body[position];
                // Skip whitespace
                if (
                    code == 32 || // space
                    code == 44 || // comma
                    code == 160 || // '\xa0'
                    code == 0x2028 || // line separator
                    code == 0x2029 || // paragraph separator
                    code > 8 && code < 14 // whitespace
                ) {
                    ++position;
                // Skip comments
                } else if (code == 35) { // #
                    ++position;
                    while (
                        position < bodyLength &&
                        (code = body[position]) != 10 &&
                        code != 13 && code != 0x2028 && code != 0x2029
                    ) {
                        ++position;
                    }
                } else {
                    break;
                }
            }
            return position;
        }

        /// <summary>
        /// Get the current code safely.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        private static int SafeCode(String body, int position)
        {
            if (position < body.Length)
            {
                return body[position];
            }
            return -1;
        }

        /// <summary>
        /// Reads a number token from the source file, either a float
        /// or an int depending on whether a decimal point appears.
        /// 
        /// Int:   -?(0|[1-9][0-9]*)
        /// Float: -?(0|[1-9][0-9]*)\.[0-9]+(e-?[0-9]+)?
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <param name="firstCode">The first code.</param>
        /// <returns></returns>
        private static Token ReadNumber(Source source, int start, int firstCode)
        {
            var code = firstCode;
            var body = source.Body;
            var position = start;
            var isFloat = false;

            if (code == 45) { // -
                code = SafeCode(body, ++position);
            }

            if (code == 48) { // 0
                code = SafeCode(body, ++position);
            } 
            else if (code >= 49 && code <= 57) 
            { // 1 - 9
                do {
                    code = SafeCode(body, ++position);
                } while (code >= 48 && code <= 57); // 0 - 9
            } 
            else
            {
                throw new SyntaxError(source, position, "Invalid number");
            }

            if (code == 46) { // .
                isFloat = true;

                code = SafeCode(body, ++position);
                if (code >= 48 && code <= 57) { // 0 - 9
                    do {
                        code = SafeCode(body, ++position);
                    } while (code >= 48 && code <= 57); // 0 - 9
                } else {
                    throw new SyntaxError(source, position, "Invalid number");
                }

                if (code == 101) { // e
                    code = SafeCode(body, ++position);
                    if (code == 45) { // -
                        code = SafeCode(body, ++position);
                    }
                    if (code >= 48 && code <= 57) { // 0 - 9
                        do {
                            code = SafeCode(body, ++position);
                        } while (code >= 48 && code <= 57); // 0 - 9
                    } else {
                        throw new SyntaxError(source, position, "Invalid number");
                    }
                }
            }

            return new Token(
                isFloat ? TokenKind.FLOAT : TokenKind.INT,
                start,
                position,
                body.Substring(start, position - start)
            );           
        }

        /// <summary>
        /// Reads a string token from the source file.
        /// "([^"\\\u000A\u000D\u2028\u2029]|(\\(u[0-9a-fA-F]{4}|["\\/bfnrt])))*"
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        private static Token ReadString(Source source, int start)
        {
            var body = source.Body;
            var position = start + 1;
            var chunkStart = position;
            var code = 0;
            var value = "";

            while (
                position < body.Length &&
                (code = body[position]) != 34 &&
                code != 10 && code != 13 && code != 0x2028 && code != 0x2029
            ) {
                ++position;
                if (code == 92) { // \
                    value += body.Substring(chunkStart, position -1 - chunkStart);
                    code = body[position];
                    switch (code) {
                        case 34: value += '"'; break;
                        case 47: value += '/'; break;
                        case 92: value += '\\'; break;
                        case 98: value += '\b'; break;
                        case 102: value += '\f'; break;
                        case 110: value += '\n'; break;
                        case 114: value += '\r'; break;
                        case 116: value += '\t'; break;
                        case 117:
                            var charCode = UniCharCode(
                                body[position + 1],
                                body[position + 2],
                                body[position + 3],
                                body[position + 4]
                            );
                            if (charCode < 0) {
                                throw new SyntaxError(source, position, "Bad character escape sequence");
                            }
                            value += char.ConvertFromUtf32(charCode);
                            position += 4;
                        break;
                    default:
                        throw new SyntaxError(source, position, "Bad character escape sequence");
                    }
                    ++position;
                    chunkStart = position;
                }
            }

            if (code != 34) {
                throw new SyntaxError(source, position, "Unterminated string");
            }

            value += body.Substring(chunkStart, position - chunkStart);
            return new Token(TokenKind.STRING, start, position + 1, value);            
        }
        
        /// <summary>
        /// Converts four hexidecimal chars to the integer that the
        /// string represents. For example, uniCharCode('0','0','0','f')
        /// will return 15, and uniCharCode('0','0','f','f') returns 255.
        /// 
        /// Returns a negative number on error, if a char was invalid.
        ///
        /// This is implemented by noting that char2hex() returns -1 on error,
        /// which means the result of ORing the char2hex() will also be negative.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        private static int UniCharCode(char a, char b, char c, char d) {
          return Char2Hex(a) << 12 | Char2Hex(b) << 8 | Char2Hex(c) << 4 | Char2Hex(d);
        }
        
        /// <summary>
        /// Converts a hex character to its integer value.
        /// '0' becomes 0, '9' becomes 9
        /// 'A' becomes 10, 'F' becomes 15
        /// 'a' becomes 10, 'f' becomes 15
        /// Returns -1 on error.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <returns></returns>
        private static int Char2Hex(char a) {
          return (
            a >= 48 && a <= 57 ? a - 48 : // 0-9
            a >= 65 && a <= 70 ? a - 55 : // A-F
            a >= 97 && a <= 102 ? a - 87 : // a-f
            -1
          );
        }
        
        /// <summary>
        /// Reads an alphanumeric + underscore name from the source.
        /// [_A-Za-z][_0-9A-Za-z]*
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        private static Token ReadName(Source source, int position) {
          var body = source.Body;
          var bodyLength = body.Length;
          var end = position + 1;
          char code;
          while (
            end != bodyLength &&
            (
              (code = body[end]) == 95 || // _
              code >= 48 && code <= 57 || // 0-9
              code >= 65 && code <= 90 || // A-Z
              code >= 97 && code <= 122 // a-z
            )
          ) 
          {
            ++end;
          }
          return new Token(
            TokenKind.NAME,
            position,
            end,
            body.Substring(position, end - position)
          );
        }
    }
}
