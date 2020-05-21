using System;
using System.Collections.Generic;

public class IfNode : Node
{
	public Context Context { get; set; }
	public Node? Successor { get; private set; }

	public Node Condition => First!;
	public Node Body => Last!;

	public IfNode(Context context, Node condition, Node body)
	{
		Context = context;

		Add(condition);
		Add(body);
	}

	public void AddSuccessor(Node successor)
	{
		if (Successor == null)
		{
			Successor = successor;
			Insert(Last!, Successor);
		}
		else if (Successor is IfNode node)
		{
			node.AddSuccessor(successor);
		}
		else
		{
			throw new Exception("Couldn't add successor to a (else) if node");
		}
	}

	public Node[] GetAllBranches()
	{
		var branches = new List<Node>() { Body };

		if (Successor == null)
		{
			return branches.ToArray();
		}
		
		if (Successor.Is(NodeType.ELSE_IF_NODE))
		{
			branches.AddRange(Successor.To<ElseIfNode>().GetAllBranches());
		}
		else
		{
			branches.Add(Successor);
		}
		
		return branches.ToArray();
	}

	public override NodeType GetNodeType()
	{
		return NodeType.IF_NODE;
	}
}