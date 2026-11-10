
namespace Maseya.UI.WinForms.Tests.Fakes;

using System;

internal sealed class FakeRangedValueSource : IRangedValueSource
{
    private int _minimum = Int32.MinValue;
    private int _maximum = Int32.MaxValue;

    public int Minimum
    {
        get
        {
            return _minimum;
        }

        set
        {
            SetRange(value, Math.Max(Maximum, value));
        }
    }

    public int Maximum
    {
        get
        {
            return _maximum;
        }

        set
        {
            SetRange(Math.Min(Minimum, value), value);
        }
    }

    public int Value
    {
        get;

        set
        {
            var newValue = Math.Clamp(value, Minimum, Maximum);

            if (field == newValue)
            {
                return;
            }

            field = newValue;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? RangeChanged;

    public event EventHandler? ValueChanged;

    public void SetRange(int min, int max)
    {
        max = Math.Max(max, min);

        if (Minimum == min && Maximum == max)
        {
            return;
        }

        _minimum = min;
        _maximum = max;

        Value = Math.Clamp(Value, min, max);

        RangeChanged?.Invoke(this, EventArgs.Empty);
    }
}
