using System;
using MonoGame.Framework;

namespace MonoGame.Testing
{
    public readonly ref struct PLCToken
    {
        public PLCTokenType Type { get; }
        public ReadOnlySpan<char> Value { get; }
        public int Line { get; }
        public int Column { get; }

        public PLCToken(PLCTokenType type, ReadOnlySpan<char> value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }
    }

    public enum PLCTokenType
    {
        Instruction,
        Argument,
        Comment,
        Spacing,
        Unknown,
        EndOfData
    }

    public delegate (bool Continue, bool Abort) ReadWhilePredicate(int offset, char value);

    public struct PLCTokenizer
    {
        public string CommentPrefix { get; }

        public int Line { get; set; }

        public PLCTokenizer(string commentPrefix)
        {
            if (string.IsNullOrWhiteSpace(commentPrefix))
                throw new ArgumentEmptyException(nameof(commentPrefix));

            CommentPrefix = commentPrefix;
            Line = 0;
        }

        public PLCToken ReadToken(ReadOnlySpan<char> text, out int charsConsumed)
        {
            bool hasFoundChar = false;

            string commentPrefix = CommentPrefix;
            bool inComment = false;
            int matchingCommentChars = 0;

            (bool Continue, bool Abort) InstructionPredicate(int offset, char value)
            {
                if (hasFoundChar && char.IsDigit(value))
                    return (false, true);

                if (char.IsLetter(value))
                {
                    hasFoundChar = true;
                    return (true, false);
                }
                return (false, false);
            }

            static (bool Continue, bool Abort) ArgumentPredicate(int offset, char value)
            {
                return (char.IsLetterOrDigit(value), false);
            }

            (bool Continue, bool Abort) CommentPredicate(int offset, char value)
            {
                if (inComment)
                {
                    if (value == '\n')
                        return (false, false);
                    return (true, false);
                }

                if (offset < commentPrefix.Length &&
                    value == commentPrefix[offset])
                {
                    matchingCommentChars++;
                    if (matchingCommentChars == commentPrefix.Length)
                        inComment = true;
                    
                    return (true, false);
                }
                return (false, true);
            }

            static (bool Continue, bool Abort) SpacingPredicate(int offset, char value)
            {
                return (char.IsWhiteSpace(value), false);
            }

            var type = PLCTokenType.EndOfData;


            if ((charsConsumed = ReadWhile(CommentPredicate, text)) != 0)
                type = PLCTokenType.Comment;
            else if ((charsConsumed = ReadWhile(InstructionPredicate, text)) != 0)
                type = PLCTokenType.Instruction;
            else if ((charsConsumed = ReadWhile(ArgumentPredicate, text)) != 0)
                type = PLCTokenType.Argument;
            else if ((charsConsumed = ReadWhile(SpacingPredicate, text)) != 0)
                type = PLCTokenType.Spacing;
            else if (!text.IsEmpty)
            {
                charsConsumed = text.Length;
                type = PLCTokenType.Unknown;
            }
            return new PLCToken(type, text.Slice(0, charsConsumed), 0, 0);
        }

        public static int ReadWhile(ReadWhilePredicate predicate, ReadOnlySpan<char> text)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            int i = 0;
            for (; i < text.Length; i++)
            {
                var (Continue, Abort) = predicate.Invoke(i, text[i]);
                if (Abort)
                    return 0;
                if (!Continue)
                    break;
            }
            return i;
        }
    }
}
