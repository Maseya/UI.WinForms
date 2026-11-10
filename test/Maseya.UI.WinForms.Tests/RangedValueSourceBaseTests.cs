namespace Maseya.UI.WinForms.Tests;

internal static class RangedValueSourceBaseTests
{
    internal static void SetRange_WithMinimumGreaterThanMaximum_CollapsesRange(
        IRangedValueSource source)
    {
        source.SetRange(20, 10);

        Assert.Equal(20, source.Minimum);
        Assert.Equal(20, source.Maximum);
        Assert.Equal(20, source.Value);
    }

    internal static void SetRange_WhenMinimumChanges_UpdatesMaximumWhenNecessary(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);
        source.Minimum = 150;

        Assert.Equal(150, source.Minimum);
        Assert.Equal(150, source.Maximum);
        Assert.Equal(150, source.Value);
    }

    internal static void SetRange_WhenMaximumChanges_UpdatesMinimumWhenNecessary(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);
        source.Value = 50;
        source.Maximum = -10;

        Assert.Equal(-10, source.Minimum);
        Assert.Equal(-10, source.Maximum);
        Assert.Equal(-10, source.Value);
    }

    internal static void SetValue_ClampsValueToPermittedRange(IRangedValueSource source)
    {
        source.SetRange(10, 20);

        source.Value = 5;
        Assert.Equal(10, source.Value);

        source.Value = 25;
        Assert.Equal(20, source.Value);
    }

    internal static void SetValue_WhenEffectiveValueDoesNotChange_DoesNotRaiseEvent(
        IRangedValueSource source)
    {
        source.SetRange(-100, 100);
        source.Value = 0;

        var eventCount = 0;
        source.ValueChanged += (_, _) => eventCount++;

        source.Value = 0;

        Assert.Equal(0, eventCount);
    }

    internal static void SetValue_WhenEffectiveValueChanges_RaisesEventOnce(
        IRangedValueSource source)
    {
        source.SetRange(-100, 100);
        source.Value = 0;

        var eventCount = 0;
        source.ValueChanged += (_, _) => eventCount++;

        source.Value = 10;

        Assert.Equal(1, eventCount);
    }

    internal static void SetRange_WhenEffectiveRangeDoesNotChange_DoesNotRaiseEvent(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);

        var eventCount = 0;
        source.RangeChanged += (_, _) => eventCount++;

        source.SetRange(0, 100);

        Assert.Equal(0, eventCount);
    }

    internal static void SetRange_WhenEffectiveRangeChanges_RaisesEventOnce(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);

        var eventCount = 0;
        source.RangeChanged += (_, _) => eventCount++;

        source.SetRange(10, 20);

        Assert.Equal(1, eventCount);
    }

    internal static void RangeChange_ThatClampsValue_RaisesBothEventsOnce(
        IRangedValueSource source)
    {
        source.SetRange(-100, 100);
        source.Value = 50;

        var rangeEventCount = 0;
        var valueEventCount = 0;

        source.RangeChanged += (_, _) => rangeEventCount++;
        source.ValueChanged += (_, _) => valueEventCount++;

        source.SetRange(0, 25);

        Assert.Equal(25, source.Value);
        Assert.Equal(1, rangeEventCount);
        Assert.Equal(1, valueEventCount);
    }

    internal static void SetRange_RaisesEventAfterBothBoundariesAreUpdated(
        IRangedValueSource source)
    {
        var eventCount = 0;

        source.RangeChanged += (_, _) =>
        {
            eventCount++;

            Assert.Equal(10, source.Minimum);
            Assert.Equal(20, source.Maximum);
        };

        source.SetRange(10, 20);

        Assert.Equal(1, eventCount);
    }

    internal static void SetRange_WhenOnlyMinimumChanges_RaisesEventOnce(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);

        var eventCount = 0;
        source.RangeChanged += (_, _) => eventCount++;

        source.SetRange(10, 100);

        Assert.Equal(10, source.Minimum);
        Assert.Equal(100, source.Maximum);
        Assert.Equal(1, eventCount);
    }

    internal static void SetRange_WhenOnlyMaximumChanges_RaisesEventOnce(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);

        var eventCount = 0;
        source.RangeChanged += (_, _) => eventCount++;

        source.SetRange(0, 90);

        Assert.Equal(0, source.Minimum);
        Assert.Equal(90, source.Maximum);
        Assert.Equal(1, eventCount);
    }

    internal static void SetRange_WhenValueRemainsWithinRange_DoesNotRaiseValueChanged(
        IRangedValueSource source)
    {
        source.SetRange(0, 100);
        source.Value = 50;

        var eventCount = 0;
        source.ValueChanged += (_, _) => eventCount++;

        source.SetRange(10, 90);

        Assert.Equal(50, source.Value);
        Assert.Equal(0, eventCount);
    }

    internal static void SetValue_WhenClampedValueDoesNotChange_DoesNotRaiseEvent(
        IRangedValueSource source)
    {
        source.SetRange(10, 20);
        source.Value = 10;

        var eventCount = 0;
        source.ValueChanged += (_, _) => eventCount++;

        source.Value = 5;

        Assert.Equal(10, source.Value);
        Assert.Equal(0, eventCount);
    }
}
