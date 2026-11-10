
namespace Maseya.UI.WinForms.Tests;

using System;
using System.ComponentModel;

using Maseya.UI.WinForms.Tests.Fakes;

using Xunit;

public class RangedValueSynchronizerComponentTests
{
    [Fact]
    public void Constructor_InitializesValueWithinRange()
    {
        using var component = new RangedValueSynchronizerComponent();

        Assert.True(component.Minimum <= component.Value);
        Assert.True(component.Value <= component.Maximum);
        Assert.Null(component.Source1);
        Assert.Null(component.Source2);
    }

    [Fact]
    public void Constructor_WithContainer_AddsComponentToContainer()
    {
        using var container = new Container();
        using var component = new RangedValueSynchronizerComponent(container);

        Assert.Contains(component, container.Components.Cast<IComponent>());
    }

    [Fact]
    public void SetRange_WithMinimumGreaterThanMaximum_CollapsesRange()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WithMinimumGreaterThanMaximum_CollapsesRange(component);
    }

    [Fact]
    public void SetRange_WhenMinimumChanges_UpdatesMaximumWhenNecessary()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenMinimumChanges_UpdatesMaximumWhenNecessary(component);
    }

    [Fact]
    public void SetRange_WhenMaximumChanges_UpdatesMinimumWhenNecessary()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenMaximumChanges_UpdatesMinimumWhenNecessary(component);
    }

    [Fact]
    public void SetValue_ClampsValueToPermittedRange()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetValue_ClampsValueToPermittedRange(component);
    }

    [Fact]
    public void SetValue_WhenEffectiveValueDoesNotChange_DoesNotRaiseEvent()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetValue_WhenEffectiveValueDoesNotChange_DoesNotRaiseEvent(component);
    }

    [Fact]
    public void SetValue_WhenEffectiveValueChanges_RaisesEventOnce()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetValue_WhenEffectiveValueChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void SetRange_WhenEffectiveRangeDoesNotChange_DoesNotRaiseEvent()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenEffectiveRangeDoesNotChange_DoesNotRaiseEvent(component);
    }

    [Fact]
    public void SetRange_WhenEffectiveRangeChanges_RaisesEventOnce()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenEffectiveRangeChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void RangeChange_ThatClampsValue_RaisesBothEventsOnce()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .RangeChange_ThatClampsValue_RaisesBothEventsOnce(component);
    }

    [Fact]
    public void SetRange_RaisesEventAfterBothBoundariesAreUpdated()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_RaisesEventAfterBothBoundariesAreUpdated(component);
    }

    [Fact]
    public void SetRange_WhenOnlyMinimumChanges_RaisesEventOnce()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenOnlyMinimumChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void SetRange_WhenOnlyMaximumChanges_RaisesEventOnce()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenOnlyMaximumChanges_RaisesEventOnce(component);
    }

    [Fact]
    public void SetRange_WhenValueRemainsWithinRange_DoesNotRaiseValueChanged()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetRange_WhenValueRemainsWithinRange_DoesNotRaiseValueChanged(component);
    }

    [Fact]
    public void SetValue_WhenClampedValueDoesNotChange_DoesNotRaiseEvent()
    {
        using var component = new RangedValueSynchronizerComponent();
        RangedValueSourceBaseTests
            .SetValue_WhenClampedValueDoesNotChange_DoesNotRaiseEvent(component);
    }

    [Fact]
    public void AssignSource_AdoptsComponentRangeAndValue()
    {
        using var component = new RangedValueSynchronizerComponent();

        component.SetRange(10, 100);
        component.Value = 50;

        var source = new FakeRangedValueSource();

        component.Source1 = source;

        Assert.Equal(10, source.Minimum);
        Assert.Equal(100, source.Maximum);
        Assert.Equal(50, source.Value);
    }

    [Fact]
    public void AssigningSelfAsSource_DoesNotInvalidateState()
    {
        using var component = new RangedValueSynchronizerComponent();

        component.SetRange(10, 100);
        component.Value = 50;

        component.Source1 = component;

        // Assigning itself must preserve the existing state.
        Assert.Same(component, component.Source1);
        Assert.Equal(10, component.Minimum);
        Assert.Equal(100, component.Maximum);
        Assert.Equal(50, component.Value);

        // Value changes must not cause recursive updates.
        component.Value = 75;

        Assert.Equal(75, component.Value);

        // Range changes must not cause recursive updates.
        component.SetRange(20, 80);

        Assert.Equal(20, component.Minimum);
        Assert.Equal(80, component.Maximum);
        Assert.Equal(75, component.Value);

        // Clamping must also work with the self-reference.
        component.SetRange(30, 60);

        Assert.Equal(30, component.Minimum);
        Assert.Equal(60, component.Maximum);
        Assert.Equal(60, component.Value);

        Assert.Same(component, component.Source1);
    }

    [Fact]
    public void SourceReplacement_PreservesComponentState()
    {
        using var component = new RangedValueSynchronizerComponent();

        component.SetRange(10, 100);
        component.Value = 60;

        var originalSource = new FakeRangedValueSource();
        var replacementSource = new FakeRangedValueSource();

        component.Source1 = originalSource;

        component.Source1 = replacementSource;

        // Replacing a source must not change the component's state.
        Assert.Equal(10, component.Minimum);
        Assert.Equal(100, component.Maximum);
        Assert.Equal(60, component.Value);

        // The replacement adopts the component's state.
        Assert.Equal(10, replacementSource.Minimum);
        Assert.Equal(100, replacementSource.Maximum);
        Assert.Equal(60, replacementSource.Value);

        // The previous source is no longer synchronized.
        component.SetRange(20, 80);
        component.Value = 40;

        Assert.Equal(10, originalSource.Minimum);
        Assert.Equal(100, originalSource.Maximum);
        Assert.Equal(60, originalSource.Value);

        Assert.Equal(20, replacementSource.Minimum);
        Assert.Equal(80, replacementSource.Maximum);
        Assert.Equal(40, replacementSource.Value);
    }

    [Fact]
    public void ComponentValueChange_UpdatesBothSources()
    {
        using var component = new RangedValueSynchronizerComponent();

        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        component.Value = 42;

        Assert.Equal(42, source1.Value);
        Assert.Equal(42, source2.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void SourceValueChange_UpdatesComponentAndOtherSource(int sourceIndex)
    {
        using var component = new RangedValueSynchronizerComponent();

        component.SetRange(0, 100);

        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        var changedSource =
            sourceIndex == 1 ? source1 : source2;

        var otherSource =
            sourceIndex == 1 ? source2 : source1;

        changedSource.Value = 42;

        Assert.Equal(42, component.Value);
        Assert.Equal(42, otherSource.Value);
    }

    [Fact]
    public void ComponentRangeChange_UpdatesBothSources()
    {
        using var component = new RangedValueSynchronizerComponent();

        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        component.SetRange(10, 20);

        Assert.Equal(10, source1.Minimum);
        Assert.Equal(20, source1.Maximum);
        Assert.Equal(10, source2.Minimum);
        Assert.Equal(20, source2.Maximum);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void SourceRangeChange_UpdatesComponentAndOtherSource(int sourceIndex)
    {
        using var component = new RangedValueSynchronizerComponent();

        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        var changedSource =
            sourceIndex == 1 ? source1 : source2;

        var otherSource =
            sourceIndex == 1 ? source2 : source1;

        changedSource.SetRange(10, 20);

        Assert.Equal(10, component.Minimum);
        Assert.Equal(20, component.Maximum);

        Assert.Equal(10, otherSource.Minimum);
        Assert.Equal(20, otherSource.Maximum);
    }

    [Fact]
    public void ReplacingSource_DetachesPreviousSource()
    {
        using var component = new RangedValueSynchronizerComponent()
        {
            Value = 0,
        };

        var oldSource = new FakeRangedValueSource();
        var newSource = new FakeRangedValueSource();

        component.Source1 = oldSource;
        component.Source1 = newSource;

        oldSource.Value = 10;

        Assert.Equal(0, component.Value);

        newSource.Value = 20;

        Assert.Equal(20, component.Value);
    }

    [Fact]
    public void RemovingSource_DetachesPreviousSource()
    {
        using var component = new RangedValueSynchronizerComponent()
        {
            Value = 0,
        };

        var source = new FakeRangedValueSource();

        component.Source1 = source;
        component.Source1 = null;

        source.Value = 10;

        Assert.Equal(0, component.Value);
    }

    [Fact]
    public void AssigningSameSourceToBothProperties_SynchronizesCorrectly()
    {
        using var component = new RangedValueSynchronizerComponent();

        var source = new FakeRangedValueSource();

        component.Source1 = source;
        component.Source2 = source;

        var eventCount = 0;
        component.ValueChanged += (_, _) => eventCount++;

        source.Value = 10;

        Assert.Equal(10, component.Value);
        Assert.Equal(1, eventCount);
    }

    [Fact]
    public void SetRange_UpdatesSourcesBeforeRaisingEvents()
    {
        using var component = new RangedValueSynchronizerComponent();

        component.SetRange(0, 100);
        component.Value = 75;

        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        var raisedEvents = new List<string>();

        void AssertSynchronizedState()
        {
            Assert.Equal(20, component.Minimum);
            Assert.Equal(50, component.Maximum);
            Assert.Equal(50, component.Value);

            Assert.Equal(20, source1.Minimum);
            Assert.Equal(50, source1.Maximum);
            Assert.Equal(50, source1.Value);

            Assert.Equal(20, source2.Minimum);
            Assert.Equal(50, source2.Maximum);
            Assert.Equal(50, source2.Value);
        }

        component.RangeChanged += (_, _) =>
        {
            AssertSynchronizedState();
            raisedEvents.Add(nameof(component.RangeChanged));
        };

        component.ValueChanged += (_, _) =>
        {
            AssertSynchronizedState();
            raisedEvents.Add(nameof(component.ValueChanged));
        };

        component.SetRange(20, 50);

        Assert.Equal(
            new[]
            {
            nameof(component.ValueChanged),
            nameof(component.RangeChanged),
            },
            raisedEvents);
    }

    [Fact]
    public void Dispose_DetachesBothSources()
    {
        var component = new RangedValueSynchronizerComponent();

        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        component.Dispose();

        source1.Value = 10;
        source2.Value = 20;

        Assert.Equal(0, component.Value);
        Assert.Null(component.Source1);
        Assert.Null(component.Source2);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var component = new RangedValueSynchronizerComponent();
        var source1 = new FakeRangedValueSource();
        var source2 = new FakeRangedValueSource();

        component.Source1 = source1;
        component.Source2 = source2;

        component.Dispose();

        var exception = Record.Exception(component.Dispose);

        Assert.Null(exception);
        Assert.Null(component.Source1);
        Assert.Null(component.Source2);
    }

    [Fact]
    public void AssigningSourceAfterDispose_Throws()
    {
        var component = new RangedValueSynchronizerComponent();
        component.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(
            () => component.Source1 = new FakeRangedValueSource());
    }
}
