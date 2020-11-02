using System;
using System.Collections.Generic;

public class LexerPhase : Phase
{
	public override Status Execute(Bundle bundle)
	{
		string[] contents = bundle.Get("input_file_contents", Array.Empty<string>());

		if (contents.Length == 0)
		{
			return Status.Error("Nothing to tokenize");
		}

		List<Token>[] tokens = new List<Token>[contents.Length];

		for (int i = 0; i < contents.Length; i++)
		{
			int index = i;

			Run(() =>
			{
				string content = contents[index];

				try
				{
					tokens[index] = Lexer.GetTokens(content);
				}
				catch (Exception e)
				{
					return Status.Error(e.Message);
				}

				return Status.OK;
			});
		}

		bundle.Put("input_file_tokens", tokens);

		return Status.OK;
	}
}