using System;

public class GraphConnectionAttribute : Attribute
{

	public enum DirectionType
	{
		In,
		Out
	}

	public DirectionType Direction;

	public GraphConnectionAttribute(DirectionType direction)
	{
		Direction = direction;
	}
	
}
