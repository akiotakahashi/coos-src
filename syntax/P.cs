// $ANTLR 2.7.6 (2005-12-22): "signature.g" -> "P.cs"$

namespace CooS.Format.CLI.Syntax
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using IToken                   = antlr.IToken;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
	public 	class P : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int NAME = 4;
		public const int NEWLINE = 5;
		
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected P(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public P(TokenBuffer tokenBuf) : this(tokenBuf,1)
		{
		}
		
		protected P(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public P(TokenStream lexer) : this(lexer,1)
		{
		}
		
		public P(ParserSharedInputState state) : base(state,1)
		{
			initialize();
		}
		
	public void startRule() //throws RecognitionException, TokenStreamException
{
		
		IToken  n = null;
		
		try {      // for error handling
			n = LT(1);
			match(NAME);
			System.out.println("Hi there, " + n.getText());
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_0_);
		}
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""NAME""",
		@"""NEWLINE"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	
}
}
