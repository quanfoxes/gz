using System;
using System.Collections.Generic;

public class FunctionToken : Token
{
	public IdentifierToken Identifier { get; private set; }
	public ContentToken Parameters { get; private set; }
	public Node ParameterTree { get; private set; } = new Node();

	public string Name => Identifier.Value;

	public FunctionToken(IdentifierToken name, ContentToken parameters) : base(TokenType.FUNCTION)
	{
		Identifier = name;
		Parameters = parameters;
	}

	/// <summary>
	/// Returns function parameters as node tree
	/// </summary>
	/// <param name="context">Context used to parse</param>
	/// <returns>Parameters as node tree</returns>
	public Node GetParsedParameters(Context context)
	{
		if (ParameterTree.First != null)
		{
			return ParameterTree;
		}

		ParameterTree = new Node();

		for (int i = 0; i < Parameters.SectionCount; i++)
		{
			var tokens = Parameters.GetTokens(i);
			Parser.Parse(ParameterTree, context, tokens);
		}

		return ParameterTree;
	}

	/// <summary>
	/// Returns the parameter names
	/// </summary>
	/// <returns>List of parameter names</returns>
	public List<string> GetParameterNames(Context function_context)
	{
		var names = new List<string>();

		if (Parameters.IsEmpty)
		{
			return names;
		}

		ParameterTree = GetParsedParameters(function_context);

		var parameter = ParameterTree.First;

		while (parameter != null)
		{
			if (parameter is VariableNode variable_node)
			{
				names.Add(variable_node.Variable.Name);
			}
			else if (parameter is OperatorNode assign && assign.Operator == Operators.ASSIGN)
			{
				throw new NotImplementedException("Parameter default values aren't supported yet");
			}
			else if (parameter is UnresolvedIdentifier parameter_identifier)
			{
				names.Add(parameter_identifier.Value);
			}
			else
			{
				throw new NotImplementedException("Unknown parameter syntax");
			}

			parameter = parameter.Next;
		}

		return names;
	}

	public override bool Equals(object? obj)
	{
		return obj is FunctionToken token &&
			   base.Equals(obj) &&
			   EqualityComparer<IdentifierToken>.Default.Equals(Identifier, token.Identifier) &&
			   EqualityComparer<ContentToken>.Default.Equals(Parameters, token.Parameters) &&
			   Name == token.Name;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.GetHashCode(), Identifier, Parameters, Name);
	}

	public override object Clone()
	{
		var clone = (FunctionToken)MemberwiseClone();
		clone.Parameters = (ContentToken)Parameters.Clone();
		clone.Identifier = (IdentifierToken)Identifier.Clone();
		
		return clone;
	}
}
