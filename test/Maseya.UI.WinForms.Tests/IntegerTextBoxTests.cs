// <copyright file="IntegerTextBoxTests.cs" organization="Maseya">
//     Copyright (c) 2026 spel werdz rite (SWR).
//     Licensed under the Mozilla Public License 2.0.
//     See LICENSE in the project root for license information, or visit
//     https://www.mozilla.org/MPL/2.0/.
// </copyright>

namespace Maseya.UI.WinForms.Tests;

public class IntegerTextBoxTests
{
    [StaFact]
    public void Constructor_InitializesValidRange()
    {
        using var control = new IntegerTextBox();

        Assert.True(control.Minimum <= control.Value);
        Assert.True(control.Value <= control.Maximum);
    }

    [StaTheory]
    [InlineData("123", 123)]
    [InlineData("-123", -123)]
    [InlineData("+123", 123)]
    [InlineData(" 123 ", 123)]
    [InlineData("0", 0)]
    [InlineData("-0", 0)]
    [InlineData("2147483647", Int32.MaxValue)]
    [InlineData("-2147483648", Int32.MinValue)]
    public void Text_ValidDecimal_UpdatesValue(string text, int expected)
    {
        using var control = new IntegerTextBox();

        control.Text = text;

        Assert.Equal(expected, control.Value);
    }

    [StaTheory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("12.5")]
    [InlineData("2147483648")]
    [InlineData("-2147483649")]
    public void Text_InvalidDecimal_DoesNotUpdateValue(string text)
    {
        using var control = new IntegerTextBox
        {
            Value = 100,
        };

        control.Text = text;

        Assert.Equal(100, control.Value);
        Assert.Equal(text, control.Text);
    }

    [StaFact]
    public void Value_Set_UpdatesText()
    {
        using var control = new IntegerTextBox();

        control.Value = 123;

        Assert.Equal(123, control.Value);
        Assert.Equal("123", control.Text);
    }

    [StaFact]
    public void Value_SetToSameValue_DoesNotRaiseValueChanged()
    {
        using var control = new IntegerTextBox();

        var eventCount = 0;
        control.ValueChanged += (_, _) => eventCount++;

        control.Value = 0;

        Assert.Equal(0, eventCount);
    }

    [StaFact]
    public void Value_SetToDifferentValue_RaisesValueChanged()
    {
        using var control = new IntegerTextBox();

        var eventCount = 0;
        control.ValueChanged += (_, _) => eventCount++;

        control.Value = 123;

        Assert.Equal(1, eventCount);
    }

    [StaFact]
    public void ValueChanged_EventRaisedAfterValueIsUpdated()
    {
        using var control = new IntegerTextBox();

        var observedValue = 0;
        control.ValueChanged += (_, _) => observedValue = control.Value;

        control.Value = 123;

        Assert.Equal(123, observedValue);
    }

    [StaFact]
    public void FormattedValue_ReturnsCurrentFormattedValue()
    {
        using var control = new IntegerTextBox
        {
            Value = 123,
            Suffix = "px",
        };

        Assert.Equal("123 px", control.FormattedValue);
    }

    [StaFact]
    public void FormatText_UpdatesText()
    {
        using var control = new IntegerTextBox
        {
            Value = 123,
        };

        control.Text = "invalid";

        control.FormatText();

        Assert.Equal("123", control.Text);
    }

    [StaFact]
    public void FormatText_DoesNotRaiseValueChanged()
    {
        using var control = new IntegerTextBox
        {
            Value = 123,
        };

        var eventCount = 0;
        control.ValueChanged += (_, _) => eventCount++;

        control.FormatText();

        Assert.Equal(0, eventCount);
    }

    [StaFact]
    public void Suffix_Set_FormatsText()
    {
        using var control = new IntegerTextBox
        {
            Value = 200,
        };

        control.Suffix = "px";

        Assert.Equal("200 px", control.Text);
    }

    [StaFact]
    public void Suffix_SetToEmpty_RemovesSuffix()
    {
        using var control = new IntegerTextBox
        {
            Value = 200,
            Suffix = "px",
        };

        control.Suffix = String.Empty;

        Assert.Equal("200", control.Text);
    }

    [StaFact]
    public void Suffix_SetToSameValue_DoesNotChangeText()
    {
        using var control = new IntegerTextBox
        {
            Value = 200,
            Suffix = "px",
        };

        control.Text = "custom";

        control.Suffix = "px";

        Assert.Equal("custom", control.Text);
    }

    [StaTheory]
    [InlineData("200 px", 200)]
    [InlineData("200px", 200)]
    [InlineData("200 PX", 200)]
    [InlineData("200", 200)]
    [InlineData(" 200 px ", 200)]
    public void Text_WithSuffix_UpdatesValue(string text, int expected)
    {
        using var control = new IntegerTextBox
        {
            Suffix = "px",
        };

        control.Text = text;

        Assert.Equal(expected, control.Value);
    }

    [StaFact]
    public void Text_SuffixOnly_DoesNotUpdateValue()
    {
        using var control = new IntegerTextBox
        {
            Value = 100,
            Suffix = "px",
        };

        control.Text = "px";

        Assert.Equal(100, control.Value);
    }

    [StaFact]
    public void SuffixChange_DoesNotRaiseValueChanged()
    {
        using var control = new IntegerTextBox();

        var eventCount = 0;
        control.ValueChanged += (_, _) => eventCount++;

        control.Suffix = "px";

        Assert.Equal(0, eventCount);
    }

    [StaTheory]
    [InlineData("FF", 0xFF)]
    [InlineData("ff", 0xFF)]
    [InlineData("ABCDEF", 0xABCDEF)]
    [InlineData("7FFFFFFF", 0x7FFFFFFF)]
    [InlineData("FFFFFFFF", -1)]
    [InlineData("80000000", Int32.MinValue)]
    public void Text_ValidHexadecimal_UpdatesValue(string text, int expected)
    {
        using var control = new IntegerTextBox
        {
            Hexadecimal = true,
        };

        control.Text = text;

        Assert.Equal(expected, control.Value);
    }

    [StaTheory]
    [InlineData("")]
    [InlineData("G")]
    [InlineData("-1")]
    [InlineData("0xFF")]
    [InlineData("100000000")]
    public void Text_InvalidHexadecimal_DoesNotUpdateValue(string text)
    {
        using var control = new IntegerTextBox
        {
            Value = 100,
            Hexadecimal = true,
        };

        control.Text = text;

        Assert.Equal(100, control.Value);
    }

    [StaFact]
    public void Hexadecimal_Set_FormatsText()
    {
        using var control = new IntegerTextBox
        {
            Value = 0xFF,
        };

        control.Hexadecimal = true;

        Assert.Equal("FF", control.Text);
    }

    [StaFact]
    public void Hexadecimal_SetToFalse_FormatsDecimalText()
    {
        using var control = new IntegerTextBox
        {
            Value = 0xFF,
            Hexadecimal = true,
        };

        control.Hexadecimal = false;

        Assert.Equal("255", control.Text);
    }

    [StaFact]
    public void Hexadecimal_NegativeValue_RoundTrips()
    {
        using var control = new IntegerTextBox
        {
            Hexadecimal = true,
            Value = -1,
        };

        Assert.Equal("FFFFFFFF", control.Text);

        control.Text = "FFFFFFFF";

        Assert.Equal(-1, control.Value);
    }

    [StaFact]
    public void Hexadecimal_WithSuffix_FormatsAndParses()
    {
        using var control = new IntegerTextBox
        {
            Hexadecimal = true,
            Suffix = "px",
            Value = 0xFF,
        };

        Assert.Equal("FF px", control.Text);

        control.Text = "FF px";

        Assert.Equal(255, control.Value);
    }

    [StaFact]
    public void Hexadecimal_SetToSameValue_DoesNotChangeText()
    {
        using var control = new IntegerTextBox
        {
            Hexadecimal = true,
            Value = 255,
        };

        control.Text = "custom";

        control.Hexadecimal = true;

        Assert.Equal("custom", control.Text);
    }

    [StaFact]
    public void Suffix_SetWithWhitespace_TrimsSuffix()
    {
        using var control = new IntegerTextBox
        {
            Value = 200,
        };

        control.Suffix = " px ";

        Assert.Equal("px", control.Suffix);
        Assert.Equal("200 px", control.Text);
    }

    [Fact]
    public void SetRange_WithMinimumGreaterThanMaximum_CollapsesRange()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WithMinimumGreaterThanMaximum_CollapsesRange(component);
    }

    [Fact]
    public void SetRange_WhenMinimumChanges_UpdatesMaximumWhenNecessary()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenMinimumChanges_UpdatesMaximumWhenNecessary(component);
    }

    [Fact]
    public void SetRange_WhenMaximumChanges_UpdatesMinimumWhenNecessary()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenMaximumChanges_UpdatesMinimumWhenNecessary(component);
    }

    [Fact]
    public void SetValue_ClampsValueToPermittedRange()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetValue_ClampsValueToPermittedRange(component);
    }

    [Fact]
    public void SetValue_WhenEffectiveValueDoesNotChange_DoesNotRaiseEvent()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetValue_WhenEffectiveValueDoesNotChange_DoesNotRaiseEvent(component);
    }

    [Fact]
    public void SetValue_WhenEffectiveValueChanges_RaisesEventOnce()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetValue_WhenEffectiveValueChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void SetRange_WhenEffectiveRangeDoesNotChange_DoesNotRaiseEvent()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenEffectiveRangeDoesNotChange_DoesNotRaiseEvent(component);
    }

    [Fact]
    public void SetRange_WhenEffectiveRangeChanges_RaisesEventOnce()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenEffectiveRangeChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void RangeChange_ThatClampsValue_RaisesBothEventsOnce()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .RangeChange_ThatClampsValue_RaisesBothEventsOnce(component);
    }

    [Fact]
    public void SetRange_RaisesEventAfterBothBoundariesAreUpdated()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_RaisesEventAfterBothBoundariesAreUpdated(component);
    }

    [Fact]
    public void SetRange_WhenOnlyMinimumChanges_RaisesEventOnce()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenOnlyMinimumChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void SetRange_WhenOnlyMaximumChanges_RaisesEventOnce()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenOnlyMaximumChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void SetRange_WhenValueRemainsWithinRange_DoesNotRaiseValueChanged()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetRange_WhenValueRemainsWithinRange_DoesNotRaiseValueChanged(component);
    }

    [Fact]
    public void SetValue_WhenClampedValueDoesNotChange_DoesNotRaiseEvent()
    {
        using var component = new IntegerTextBox();
        RangedValueSourceBaseTests
            .SetValue_WhenClampedValueDoesNotChange_DoesNotRaiseEvent(component);
    }

    [StaFact]
    public void LostFocus_InvalidText_RestoresPreviousValue()
    {
        WithFocusedControl((control, button) =>
        {
            control.Value = 100;
            control.Text = "invalid";

            Assert.Equal(100, control.Value);
            Assert.Equal("invalid", control.Text);

            _ = button.Focus();
            Application.DoEvents();

            Assert.Equal(100, control.Value);
            Assert.Equal("100", control.Text);
        });
    }

    [StaFact]
    public void LostFocus_OutOfRangeText_ClampsValue()
    {
        WithFocusedControl((control, button) =>
        {
            control.SetRange(0, 100);
            control.Value = 50;

            control.Text = "200";

            Assert.Equal(50, control.Value);

            _ = button.Focus();
            Application.DoEvents();

            Assert.Equal(100, control.Value);
            Assert.Equal("100", control.Text);
        });
    }

    [StaFact]
    public void LostFocus_ValidText_RetainsParsedValue()
    {
        WithFocusedControl((control, button) =>
        {
            control.Text = "200";

            Assert.Equal(200, control.Value);

            _ = button.Focus();
            Application.DoEvents();

            Assert.Equal(200, control.Value);
            Assert.Equal("200", control.Text);
        });
    }

    [StaFact]
    public void LostFocus_ValidTextWithSuffix_FormatsText()
    {
        WithFocusedControl((control, button) =>
        {
            control.Suffix = "px";
            control.Text = "200";

            Assert.Equal(200, control.Value);

            _ = button.Focus();
            Application.DoEvents();

            Assert.Equal(200, control.Value);
            Assert.Equal("200 px", control.Text);
        });
    }

    [StaFact]
    public void LostFocus_Hexadecimal_FormatsText()
    {
        WithFocusedControl((control, button) =>
        {
            control.Hexadecimal = true;
            control.Text = "FF";

            Assert.Equal(255, control.Value);

            _ = button.Focus();
            Application.DoEvents();

            Assert.Equal(255, control.Value);
            Assert.Equal("FF", control.Text);
        });
    }

    [StaFact]
    public void SuffixChanged_WhileFocused_DoesNotOverwriteText()
    {
        WithFocusedControl((control, _) =>
        {
            control.Text = "200";

            control.Suffix = "px";

            Assert.Equal(200, control.Value);
            Assert.Equal("200", control.Text);
        });
    }

    [StaFact]
    public void HexadecimalChanged_WhileFocused_OverwritesText()
    {
        WithFocusedControl((control, _) =>
        {
            control.Text = "200";

            control.Hexadecimal = true;

            Assert.Equal(200, control.Value);
            Assert.Equal("C8", control.Text);
        });
    }

    [StaFact]
    public void FormatText_WhileFocused_OverwritesEditingText()
    {
        WithFocusedControl((control, _) =>
        {
            control.Value = 100;
            control.Text = "200";

            Assert.Equal(200, control.Value);

            control.FormatText();

            Assert.Equal("200", control.Text);
        });
    }

    [StaFact]
    public void RangeChanged_WhileFocused_PreservesEditingText()
    {
        WithFocusedControl((control, button) =>
        {
            control.SetRange(0, 100);
            control.Value = 50;

            control.Text = "invalid";

            control.SetRange(60, 100);

            Assert.Equal(60, control.Value);
            Assert.Equal("invalid", control.Text);

            _ = button.Focus();
            Application.DoEvents();

            Assert.Equal(60, control.Value);
            Assert.Equal("60", control.Text);
        });
    }

    private static void WithFocusedControl(Action<IntegerTextBox, Button> action)
    {
        using var form = new Form
        {
            ShowInTaskbar = false,
            Width = 300,
            Height = 150,
        };

        using var control = new IntegerTextBox
        {
            Left = 10,
            Top = 10,
            TabIndex = 0,
        };

        using var button = new Button
        {
            Left = 10,
            Top = 50,
            TabIndex = 1,
            Text = "Other control",
        };

        form.Controls.Add(control);
        form.Controls.Add(button);

        form.Show();
        Application.DoEvents();

        form.Activate();

        Assert.True(control.Focus());
        Application.DoEvents();
        Assert.True(control.Focused);

        action(control, button);
    }
}
