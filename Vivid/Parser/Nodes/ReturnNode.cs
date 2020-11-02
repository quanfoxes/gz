using System;

public class ReturnNode : InstructionNode, IResolvable
{
	public Node? Value => First;

	public ReturnNode(Node? node) : base(Keywords.RETURN)
	{
		if (node != null)
		{
			Add(node);
		}
	}

	public Node? Resolve(Context context)
	{
		// Find the parent function where the return value can be assigned
		var function = context.GetFunctionParent() ?? throw new ApplicationException("Return statement was not inside a function");

		if (Value == null)
		{
			return null;
		}

		Resolver.Resolve(context, Value);

		//var current = function.ReturnType;
		var type = Value?.TryGetType();

		if (type == Types.UNKNOWN)
		{
			return null;
		}

		return null;
	}

	public override NodeType GetNodeType()
	{
		return NodeType.RETURN;
	}

	public Status GetStatus()
	{
		if (Value == null)
		{
			return Status.OK;
		}

		// Find the parent function where the return value can be assigned
		var function = FindContext().GetContext()!.GetFunctionParent();

		if (function == null)
		{
			return Status.Error("Return statement was not inside a function");
		}

		var expected = function.ReturnType;
		var actual = Value.TryGetType();

		if (Resolver.GetSharedType(expected, actual) == null)
		{
			return Status.Error($"Type '{actual}' is not compatible with the current return type '{expected?.ToString() ?? "none"}'");
		}

		return Status.OK;
	}
}