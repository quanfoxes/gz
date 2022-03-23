using System.Collections.Generic;
using System.Linq;
using System;

public class ReorderInstruction : Instruction
{
	public List<Handle> Destinations { get; }
	public List<Format> Formats { get; }
	public List<Result> Sources { get; }
	public Type? ReturnType { get; }
	public bool Extracted { get; private set; } = false;

	public ReorderInstruction(Unit unit, List<Handle> destinations, List<Result> sources, Type? return_type) : base(unit, InstructionType.REORDER)
	{
		Dependencies = null;
		Destinations = destinations;
		Formats = Destinations.Select(i => i.Format).ToList();
		Sources = sources;
		ReturnType = return_type;
	}

	/// <summary>
	/// Returns how many bytes of the specified type are returned using the stack
	/// </summary>
	private int ComputeReturnOverflow(Type type, int overflow, List<Register> standard_parameter_registers, List<Register> decimal_parameter_registers)
	{
		foreach (var iterator in type.Variables)
		{
			var member = iterator.Value;

			if (member.Type!.IsPack)
			{
				overflow = ComputeReturnOverflow(member.Type, overflow, standard_parameter_registers, decimal_parameter_registers);
				continue;
			}

			// First, drain out the registers
			var register = member.Type!.Format.IsDecimal() ? decimal_parameter_registers.Pop() : standard_parameter_registers.Pop();
			if (register != null) continue;

			overflow += Assembler.Size.Bytes;
		}

		return overflow;
	}

	/// <summary>
	/// Returns how many bytes of the specified type are returned using the stack
	/// </summary>
	private int ComputeReturnOverflow(Type type)
	{
		var decimal_parameter_registers = Unit.MediaRegisters.Take(Calls.GetMaxMediaRegisterParameters()).ToList();
		var standard_parameter_registers = Calls.GetStandardParameterRegisters().Select(name => Unit.Registers.Find(r => r[Size.QWORD] == name)!).ToList();

		return ComputeReturnOverflow(type, 0, decimal_parameter_registers, standard_parameter_registers);
	}

	/// <summary>
	/// Evacuates variables that are located at the overflow zone of the stack
	/// </summary>
	private void EvacuateOverflowZone(Type type)
	{
		var overflow = Math.Max(ComputeReturnOverflow(type), Assembler.IsTargetWindows ? Calls.SHADOW_SPACE_SIZE : 0);

		foreach (var iterator in Unit.Scope!.Variables)
		{
			// Find all memory handles
			var value = iterator.Value;
			var instance = value.Value.Instance;
			if (instance != HandleInstanceType.STACK_VARIABLE && instance != HandleInstanceType.STACK_MEMORY && instance != HandleInstanceType.TEMPORARY_MEMORY && instance != HandleInstanceType.MEMORY) continue;

			var memory = value.Value.To<MemoryHandle>();

			// Ensure the memory address represents a stack address
			var start = memory.GetStart();
			if (start == null || start != Unit.GetStackPointer()) continue;

			// Ensure the memort address overlaps with the overflow
			var offset = memory.GetAbsoluteOffset();
			if (offset < 0 || offset >= overflow) continue;

			var variable = iterator.Key;

			// Try to get an available non-volatile register
			var destination = (Handle?)null;
			var register = Memory.GetNextRegister(Unit, variable.Type!.Format.IsDecimal(), Trace.GetDirectives(Unit, value));

			// Use the non-volatile register, if one was found
			if (register != null)
			{
				destination = new RegisterHandle(register);
			}
			else
			{
				// Since there are no non-volatile registers available, the value must be relocated to safe stack location
				destination = References.CreateVariableHandle(Unit, variable);
			}

			Unit.Append(new MoveInstruction(Unit, new Result(destination, variable.Type!.GetRegisterFormat()), value)
			{
				Description = $"Evacuate an important value into '{destination}'",
				Type = MoveType.RELOCATE
			});
		}
	}

	public override void OnBuild()
	{
		if (ReturnType != null && ReturnType.IsPack) EvacuateOverflowZone(ReturnType);

		var instructions = new List<Instruction>();

		for (var i = 0; i < Destinations.Count; i++)
		{
			var source = Sources[i];
			var destination = new Result(Destinations[i], Formats[i]);

			instructions.Add(new MoveInstruction(Unit, destination, source) { IsDestinationProtected = true });
		}

		instructions = Memory.Align(Unit, instructions.Cast<MoveInstruction>().ToList());
		
		Extracted = true;
		Unit.Append(instructions);
	}

	public override Result[] GetResultReferences()
	{
		if (Extracted) return Array.Empty<Result>();
		return Sources.ToArray();
	}
}