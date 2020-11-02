﻿using System.Collections.Generic;
using System;

class AssignPattern : Pattern
{
	public const int PRIORITY = 18;

	public const int DESTINATION = 0;
	public const int OPERATOR = 1;
	public const int SOURCE = 2;

	// (a-z) = ...
	public AssignPattern() : base
	(
		TokenType.IDENTIFIER, TokenType.OPERATOR, TokenType.OBJECT
	)
	{ }

	public override int GetPriority(List<Token> tokens)
	{
		return PRIORITY;
	}

	public override bool Passes(Context context, PatternState state, List<Token> tokens)
	{
		return tokens[OPERATOR].To<OperatorToken>().Operator == Operators.ASSIGN;
	}

	public override Node Build(Context context, List<Token> tokens)
	{
		var destination = tokens[DESTINATION].To<IdentifierToken>();

		Variable? variable;

		if (!context.IsVariableDeclared(destination.Value))
		{
			if (destination.Value == Function.SELF_POINTER_IDENTIFIER || destination.Value == Lambda.SELF_POINTER_IDENTIFIER)
			{
				throw Errors.Get(destination.Position, $"Can not declare variable called '{destination.Value}' since the name is reserved");
			}

			var category = context.IsType ? VariableCategory.MEMBER : VariableCategory.LOCAL;
			var is_constant = !context.IsInsideFunction && !context.IsInsideType;

			variable = new Variable
			(
				context,
				Types.UNKNOWN,
				category,
				destination.Value,
				AccessModifier.PUBLIC | (is_constant ? AccessModifier.CONSTANT : 0)
			);

			return new VariableNode(variable);
		}

		variable = context.GetVariable(destination.Value)!;

		if (variable.IsMember)
		{
			var self = context.GetSelfPointer() ?? throw new ApplicationException("Missing self pointer");

			return new LinkNode(
				new VariableNode(self),
				new VariableNode(variable)
			);
		}

		return new VariableNode(variable);
	}

	public override int GetEnd()
	{
		return 1;
	}
}