using System;

public static class RegisterFlag
{
    public const int VOLATILE = 1;
    public const int RESERVED = 2;
    public const int RETURN = 4;
    public const int BASE_POINTER = 8;
    public const int STACK_POINTER = 16;
}

public class Register
{
    public String Name { get; private set; }

    private Result? _Value { get; set; } = null;
    public Result? Handle 
    { 
        get => _Value;
        set { _Value = value; IsUsed = true; }
    }

    public int Flags { get; private set; }
    
    public bool IsUsed { get; private set; } = false;
    public bool IsVolatile => Flag.Has(Flags, RegisterFlag.VOLATILE);
    public bool IsReserved => Flag.Has(Flags, RegisterFlag.RESERVED);
    public bool IsReturnRegister => Flag.Has(Flags, RegisterFlag.RETURN);
    public bool IsReleasable => Handle == null || Handle.IsReleasable();

    public Register(string name, params int[] flags) 
    {
        Name = name;
        Flags = Flag.Combine(flags);
    }

    public bool IsAvailable(int position)
    {
        return Handle == null || !Handle.IsValid(position);
    }

    public void Reset(bool full = false)
    {
        _Value = null;
        
        if (full)
        {
            IsUsed = false;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}