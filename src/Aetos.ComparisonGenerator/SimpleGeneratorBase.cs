using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Aetos.ComparisonGenerator
{
    internal class SimpleGeneratorBase
    {
        public virtual string TransformText()
        {
            return this.GenerationEnvironment.ToString();
        }

        private bool _endsWithNewLine = true;

        public void Write(
            string? text)
        {
            if (text is null)
            {
                return;
            }

            string newLine = "\r\n";
            int newLineLen = newLine.Length;

            var buffer = this.GenerationEnvironment;

            int length = text.Length;
            int start = 0;

            while (start < length)
            {
                if (this._endsWithNewLine)
                {
                    buffer.Append(this._currentIndent);
                }

                int index = text.IndexOf(newLine, start, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    buffer.Append(text, start, length - start);
                    this._endsWithNewLine = false;
                    break;
                }

                buffer.Append(text, start, index - start + newLineLen);

                start = index + newLineLen;
                this._endsWithNewLine = true;
            }
        }

        private string _currentIndent = string.Empty;

        private readonly Stack<string> _indents = new();

        public void PushIndent(
            string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this._indents.Push(text);
            this._currentIndent = string.Join(null, this._indents);
        }

        public void PopIndent()
        {
            this._indents.Pop();
            this._currentIndent = string.Join(null, this._indents);
        }

        protected readonly StringBuilder GenerationEnvironment = new();

        protected readonly Helper ToStringHelper;

        public struct Helper
        {
            [SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "By design.")]
            public string ToStringWithCulture(
                object? obj)
            {
                return obj switch {
                    null => string.Empty,
                    string s => s,
                    IConvertible c => c.ToString(_invariantCulture),
                    IFormattable f => f.ToString(null, _invariantCulture),
                    var other => other.ToString()
                };
            }

            private static readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;
        }
    }
}
