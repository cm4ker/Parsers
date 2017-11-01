﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Devsense.PHP.Text;

namespace Devsense.PHP.Syntax
{
    #region ISourceToken

    /// <summary>
    /// Provides information about a token.
    /// </summary>
    public interface ISourceToken
    {
        /// <summary>
        /// Token identifier.
        /// </summary>
        Tokens Token { get; }

        /// <summary>
        /// Token span within source text.
        /// Can be an invalid span.
        /// </summary>
        Span Span { get; }

        /// <summary>
        /// Optional token line number.
        /// </summary>
        int? Line { get; }

        /// <summary>
        /// Optional token category.
        /// </summary>
        TokenCategory? Category { get; }
    }

    /// <summary>
    /// <see cref="ISourceToken"/> immutable implementation.
    /// </summary>
    public sealed class SourceToken : ISourceToken
    {
        public Tokens Token { get; }

        public Span Span { get; }

        public int? Line { get; }

        public TokenCategory? Category { get; }

        public SourceToken(Tokens token, Span span, int line, TokenCategory category)
        {
            this.Token = token;
            this.Span = span;
            this.Line = (line >= 0) ? new int?(line) : null;
            this.Category = (int)category != -1 ? new TokenCategory?(category) : null;
        }

        public SourceToken(Tokens token, Span span, int line = -1)
            : this(token, span, line, (TokenCategory)(-1))
        { }
    }

    #endregion

    #region ISourceTokenProvider

    /// <summary>
    /// Providing set of tokens from source code.
    /// </summary>
    public interface ISourceTokenProvider
    {
        /// <summary>
        /// Gets set of tokens within given span.
        /// </summary>
        /// <returns>
        /// Set of tokens under the span.
        /// Cannot be <c>null</c>.
        /// </returns>
        IEnumerable<ISourceToken> GetTokens(Span span);

        /// <summary>
        /// Gets set of tokens matching given predicate at given span.
        /// </summary>
        /// <param name="span">Span.</param>
        /// <param name="predicate">Token to find at given span.</param>
        /// <param name="default">Default token to be returned if tokens are not available at given span.</param>
        /// <returns>Set of tokens matching given predicate. Gets <paramref name="default"/> in case tokens are not available at given span.</returns>
        ISourceToken GetTokenAt(Span span, Tokens predicate, ISourceToken @default);

        /// <summary>
        /// Gets set of tokens matching given predicate at given span.
        /// </summary>
        /// <param name="span">Span.</param>
        /// <param name="predicate">Token to find at given span.</param>
        /// <param name="default">Default set to be returned if tokens are not available at given span. Cannot be <c>null</c>.</param>
        /// <returns>Set of tokens matching given predicate. Gets <paramref name="default"/> in case tokens are not available at given span.</returns>
        IEnumerable<ISourceToken> GetTokens(Span span, Func<ISourceToken, bool> predicate, IEnumerable<ISourceToken> @default);
    }

    #endregion

    #region SourceTokenProviderFactory

    /// <summary>
    /// Factory of <see cref="ISourceTokenProvider"/>s.
    /// </summary>
    public class SourceTokenProviderFactory
    {
        sealed class EmptyProvider : ISourceTokenProvider
        {
            public IEnumerable<ISourceToken> GetTokens(Span span) => EmptyArray<ISourceToken>.Instance;
            public ISourceToken GetTokenAt(Span span, Tokens predicate, ISourceToken @default) => @default;
            public IEnumerable<ISourceToken> GetTokens(Span span, Func<ISourceToken, bool> predicate, IEnumerable<ISourceToken> @default) => @default;
        }

        sealed class ListProvider : ISourceTokenProvider
        {
            readonly ISourceToken[] _tokens;

            public ListProvider(IEnumerable<ISourceToken> tokens)
            {
                Debug.Assert(tokens != null);
                _tokens = tokens.ToArray();
            }

            public IEnumerable<ISourceToken> GetTokens(Span span)
            {
                foreach (var t in _tokens)
                {
                    if (t.Span.IntersectsWith(span))
                    {
                        yield return t;
                    }
                }
            }

            public ISourceToken GetTokenAt(Span span, Tokens predicate, ISourceToken @default)
            {
                return GetTokens(span).FirstOrDefault() ?? @default;
            }

            public IEnumerable<ISourceToken> GetTokens(Span span, Func<ISourceToken, bool> predicate, IEnumerable<ISourceToken> @default)
            {
                // return GetTokens(span).DefaultIfEmpty(@default);

                bool hasTokens = false;

                foreach (var t in GetTokens(span))
                {
                    hasTokens = true;

                    if (predicate(t)) yield return t;
                }

                if (!hasTokens)
                {
                    foreach (var t in @default) yield return t;
                }
            }
        }

        public static ISourceTokenProvider CreateEmptyProvider()
        {
            return new EmptyProvider();
        }

        public static ISourceTokenProvider CreateProvider(IEnumerable<ISourceToken> tokens)
        {
            if (tokens == null)
            {
                new ArgumentNullException(nameof(tokens));
            }

            return new ListProvider(tokens);
        }
    }

    #endregion
}