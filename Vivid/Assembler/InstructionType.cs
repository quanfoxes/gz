public enum InstructionType
{
	ADDITION,
	ALLOCATE_REGISTER,
	ALLOCATE_STACK,
	DEBUG_BREAK,
	ATOMIC_EXCHANGE_ADDITION,
	BITWISE,
	CALL,
	COMPARE,
	CONVERT,
	CREATE_PACK,
	DIVISION,
	EVACUATE,
	EXCHANGE,
	EXTEND_NUMERATOR,
	GET_CONSTANT,
	GET_MEMORY_ADDRESS,
	GET_OBJECT_POINTER,
	GET_RELATIVE_ADDRESS,
	GET_VARIABLE,
	INITIALIZE,
	JUMP,
	LABEL,
	LABEL_MERGE,
	LOAD_SHIFTED_CONSTANT,
	LONG_MULTIPLICATION,
	MOVE,
	MULTIPLICATION,
	MULTIPLICATION_SUBTRACTION,
	NO_OPERATION,
	NORMAL,
	TEMPORARY_COMPARE,
	REORDER,
	REQUIRE_VARIABLES,
	RETURN,
	LOCK_STATE,
	SET_VARIABLE,
	SINGLE_PARAMETER,
	SUBTRACT,
	DEBUG_START,
	DEBUG_FRAME_OFFSET,
	DEBUG_END,
	ENTER_SCOPE
}