using System;

public struct Uint128
{
	public ulong High;
	public ulong Low;

	public Uint128(ulong value)
	{
		High = 0;
		Low = value;
	}

	public Uint128(Uint128 value)
	{
		High = value.High;
		Low = value.Low;
	}

	public Uint128(ulong high, ulong low)
	{
		High = high;
		Low = low;
	}

	public Uint128 ShiftLeft(int n)
	{
		if (n == 0) return this;
		if (n == 64) return new Uint128(Low, 0);

		if (n < 64)
		{
			return new Uint128((High << n) | (Low >> (64 - n)), Low << n);
		}

		return new Uint128(Low << (n - 64), 0);
	}

	public Uint128 ShiftRight(int n)
	{
		if (n == 0) return this;
		if (n == 64) return new Uint128(Low, 0);

		if (n < 64)
		{
			return new Uint128(High >> n, (High << (64 - n)) | (Low >> n));
		}

		return new Uint128(0, High >> (n - 64));
	}

	public void Enable(int bit)
	{
		if (bit >= 64) { High |= 1UL << (bit - 64); }
		else { Low |= 1UL << bit; }
	}

	public static Uint128 operator+(Uint128 left, ulong right)
	{
		var result = new Uint128(left.High, left.Low + right);
		if (result.Low < left.Low) return new Uint128(result.High + 1, result.Low);

		return result;
	}

	public static Uint128 operator+(Uint128 left, Uint128 right)
	{
		var result = new Uint128(left.High + right.High, left.Low + right.Low);
		if (result.Low < right.Low) return new Uint128(result.High + 1, result.Low);

		return result;
	}

	public static Uint128 operator-(Uint128 left, Uint128 right)
	{
		var result = new Uint128(left.High - right.High, left.Low - right.Low);
		if (left.Low < right.Low) return new Uint128(result.High - 1, result.Low);

		return result;
	}

	public static Uint128 operator-(Uint128 left, ulong right)
	{
		var result = new Uint128(left.High, left.Low - right);
		if (left.Low < right) return new Uint128(result.High - 1, result.Low);

		return result;
	}

	public static bool operator>=(Uint128 left, Uint128 right)
	{
		return left.High > right.High || (left.High == right.High && left.Low >= right.Low);
	}

	public static bool operator>(Uint128 left, Uint128 right)
	{
		return left.High > right.High || (left.High == right.High && left.Low > right.Low);
	}

	public static bool operator<=(Uint128 left, Uint128 right)
	{
		return left.High < right.High || (left.High == right.High && left.Low <= right.Low);
	}

	public static bool operator<(Uint128 left, Uint128 right)
	{
		return left.High < right.High || (left.High == right.High && left.Low < right.Low);
	}

	public static bool operator==(Uint128 left, Uint128 right)
	{
		return left.High == right.High && left.Low == right.Low;
	}

	public static bool operator!=(Uint128 left, Uint128 right)
	{
		return left.High != right.High || left.Low != right.Low;
	}

	public static Uint128 operator<<(Uint128 value, int amount)
	{
		return new Uint128(value).ShiftLeft(amount);
	}

	public static Uint128 operator>>(Uint128 value, int amount)
	{
		return new Uint128(value).ShiftRight(amount);
	}

	public override bool Equals(object? other)
	{
		return other is Uint128 value && this == value;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(High, Low);
	}

	/// <summary>
	/// Returns the index of the last bit set to one
	/// </summary>
	public int GetLastSetBitIndex()
	{
		// If the high part is not zero, the last bit must be in the high part
		if (High != 0)
		{
			for (var i = 63; i >= 0; i--)
			{
				if ((High & (1UL << i)) != 0) return i + 64;
			}
		}

		for (var i = 63; i >= 0; i--)
		{
			if ((Low & (1UL << i)) != 0) return i;
		}

		throw new ApplicationException("Value must not be zero");
	}

	public static Uint128 operator/(Uint128 dividend, Uint128 divisor)
	{ 
		if (divisor > dividend) return new Uint128(0);
		if (divisor == dividend) return new Uint128(1);

		var denominator = new Uint128(divisor);
		var quotient = new Uint128(0);

		// Left aligns the most significant bit of the denominator with the dividend
		var shift = dividend.GetLastSetBitIndex() - denominator.GetLastSetBitIndex();
		denominator <<= shift;
		
		for (var i = 0; i <= shift; i++)
		{
			quotient <<= 1;

			if (dividend >= denominator)
			{
				dividend -= denominator;
				quotient.Enable(0);
			}

			denominator >>= 1;
		}

		// NOTE: Remainder is now in the dividend
		return quotient;
	}
}