// <copyright file="IntegerTextBox.cs" organization="Maseya">
//     Copyright (c) 2026 spel werdz rite (SWR).
//     Licensed under the Mozilla Public License 2.0.
//     See LICENSE in the project root for license information, or visit
//     https://www.mozilla.org/MPL/2.0/.
// </copyright>

namespace Maseya.UI.WinForms;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

/// <summary>
/// Represents a text box that displays and edits an integer value within a
/// specified range, with optional hexadecimal formatting and a suffix.
/// </summary>
/// <remarks>
/// <see cref="Value"/> is updated as the text changes when the text represents
/// a valid integer within the permitted range. Invalid or out-of-range text
/// does not update the value. When the control loses focus, parseable text is
/// applied to <see cref="Value"/>, which clamps it to the permitted range,
/// and the text is reformatted.
/// </remarks>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(ValueChanged))]
[DefaultBindingProperty(nameof(Value))]
public class IntegerTextBox : TextBox, IRangedValueSource
{
    private const int DefaultMinValue = Int32.MinValue;
    private const int DefaultMaxValue = Int32.MaxValue;

    private int _value;
    private int _minimum = DefaultMinValue;
    private int _maximum = DefaultMaxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerTextBox"/> class.
    /// </summary>
    public IntegerTextBox()
    {
        Suffix = String.Empty;
        FormatText();
    }

    /// <summary>
    /// Gets or sets the integer value represented by the text box.
    /// </summary>
    /// <value>
    /// The current integer value, constrained by <see cref="Minimum"/> and
    /// <see cref="Maximum"/>.
    /// </value>
    /// <remarks>
    /// Setting this property clamps the supplied value to the permitted range.
    /// If the value changes, <see cref="ValueChanged"/> is raised.
    /// </remarks>
    [Category("Value")]
    [Description("The integer value represented by the text box.")]
    [DefaultValue(0)]
    [Bindable(true)]
    public int Value
    {
        get
        {
            return _value;
        }

        set
        {
            SetValueInternal(Math.Clamp(value, Minimum, Maximum));
        }
    }

    /// <summary>
    /// Gets or sets the minimum permitted integer value.
    /// </summary>
    /// <value>The inclusive lower bound of the permitted range.</value>
    /// <remarks>
    /// Setting this property adjusts <see cref="Maximum"/> if necessary to
    /// ensure that the maximum is not less than the minimum. If the current
    /// <see cref="Value"/> falls outside the resulting range, it is clamped.
    /// <see cref="RangeChanged"/> is raised when the range changes.
    /// </remarks>
    [Category("Value")]
    [Description("The minimum permitted integer value.")]
    [DefaultValue(DefaultMinValue)]
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

    /// <summary>
    /// Gets or sets the maximum permitted integer value.
    /// </summary>
    /// <value>The inclusive upper bound of the permitted range.</value>
    /// <remarks>
    /// Setting this property adjusts <see cref="Minimum"/> if necessary to
    /// ensure that the minimum is not greater than the maximum. If the current
    /// <see cref="Value"/> falls outside the resulting range, it is clamped.
    /// <see cref="RangeChanged"/> is raised when the range changes.
    /// </remarks>
    [Category("Value")]
    [Description("The maximum permitted integer value.")]
    [DefaultValue(DefaultMaxValue)]
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

    /// <summary>
    /// Gets or sets a value indicating whether the integer is displayed and
    /// parsed in hexadecimal.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to use hexadecimal formatting and parsing;
    /// otherwise, <see langword="false"/> to use decimal formatting and parsing.
    /// </value>
    /// <remarks>
    /// Changing this property reformats the text immediately, including while
    /// the control has focus.
    /// Hexadecimal parsing uses <see cref="NumberStyles.HexNumber"/> and
    /// <see cref="CultureInfo.InvariantCulture"/>. Negative values are
    /// represented using their 32-bit two's-complement hexadecimal form.
    /// </remarks>
    [Category("Value")]
    [Description("Indicates whether the integer is displayed and parsed in hexadecimal.")]
    [DefaultValue(false)]
    public bool Hexadecimal
    {
        get;

        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            FormatText();
        }
    }

    /// <summary>
    /// Gets or sets the optional suffix appended to the displayed integer value.
    /// </summary>
    /// <value>
    /// The suffix to append, or an empty string if no suffix is used.
    /// Leading and trailing whitespace is removed when the property is set.
    /// </value>
    /// <remarks>
    /// The suffix is separated from the formatted integer by a space.
    /// During parsing, a matching suffix is removed without regard to case.
    /// Changing this property reformats the text when the control does not
    /// have focus.
    /// </remarks>
    [Category("Value")]
    [Description("The optional suffix appended to the displayed integer value.")]
    [DefaultValue("")]
    public string Suffix
    {
        get;

        set
        {
            if (field == value)
            {
                return;
            }

            field = value.Trim();

            TryFormatText();
        }
    }

    /// <summary>
    /// Gets the current value formatted according to <see cref="Hexadecimal"/>
    /// and <see cref="Suffix"/>.
    /// </summary>
    /// <value>
    /// The formatted integer value, optionally followed by the suffix.
    /// </value>
    /// <remarks>
    /// Decimal formatting uses the current culture. Hexadecimal formatting
    /// uses the invariant culture.
    /// </remarks>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string FormattedValue
    {
        get
        {
            var text = Hexadecimal
                ? Value.ToString("X", CultureInfo.InvariantCulture)
                : Value.ToString(CultureInfo.CurrentCulture);

            if (!String.IsNullOrEmpty(Suffix))
            {
                text += " " + Suffix;
            }

            return text;
        }
    }

    private bool IsTextValueUpdating
    {
        get;
        set;
    }

    /// <summary>
    /// Occurs when the <see cref="Value"/> property changes.
    /// </summary>
    [Category("Value")]
    [Description("Occurs when the integer value changes.")]
    public event EventHandler? ValueChanged;

    /// <summary>
    /// Occurs when the permitted range changes.
    /// </summary>
    /// <remarks>
    /// If the current <see cref="Value"/> is outside the new range, it is
    /// clamped before this event is raised. Consequently, <see cref="ValueChanged"/>
    /// may be raised before <see cref="RangeChanged"/>.
    /// </remarks>
    [Category("Behavior")]
    [Description("Occurs when the range of permitted values changes.")]
    public event EventHandler? RangeChanged;

    /// <summary>
    /// Reformats the text to match the current <see cref="Value"/>,
    /// <see cref="Hexadecimal"/>, and <see cref="Suffix"/> properties.
    /// </summary>
    /// <remarks>
    /// This method replaces the current text even if the control has focus.
    /// It does nothing if a text-to-value update is already in progress.
    /// </remarks>
    public void FormatText()
    {
        if (!IsTextValueUpdating)
        {
            IsTextValueUpdating = true;
            Text = FormattedValue;
            IsTextValueUpdating = false;
        }
    }

    /// <summary>
    /// Sets the permitted integer range.
    /// </summary>
    /// <param name="min">The inclusive lower bound of the range.</param>
    /// <param name="max">The inclusive upper bound of the range.</param>
    /// <remarks>
    /// If <paramref name="max"/> is less than <paramref name="min"/>,
    /// the maximum is adjusted to equal the minimum.
    /// If the current <see cref="Value"/> falls outside the resulting range,
    /// it is clamped. If the range changes, <see cref="RangeChanged"/> is raised.
    /// </remarks>
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

        OnRangeChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="RangeChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected virtual void OnRangeChanged(EventArgs e)
    {
        RangeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected virtual void OnValueChanged(EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        if (!IsTextValueUpdating &&
            TryParseText(out var value) &&
            value >= Minimum && value <= Maximum)
        {
            IsTextValueUpdating = true;
            SetValueInternal(value);
            IsTextValueUpdating = false;
        }

        base.OnTextChanged(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        if (TryParseText(out var value))
        {
            Value = value;
        }

        FormatText();

        base.OnLostFocus(e);
    }

    private void SetValueInternal(int value)
    {
        if (Value == value)
        {
            return;
        }

        _value = value;

        TryFormatText();

        OnValueChanged(EventArgs.Empty);
    }

    private bool TryParseText(out int value)
    {
        var text = Text.Trim();

        if (!String.IsNullOrEmpty(Suffix))
        {
            if (text.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
            {
                text = text[..^Suffix.Length].TrimEnd();
            }
        }

        return Hexadecimal
            ? Int32.TryParse(
                text,
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out value)
            : Int32.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.CurrentCulture,
                out value);
    }

    private void TryFormatText()
    {
        if (!Focused)
        {
            FormatText();
        }
    }
}
