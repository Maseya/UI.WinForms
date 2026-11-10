// <copyright file="RangedValueSynchronizerComponent.cs" organization="Maseya">
//     Copyright (c) 2026 spel werdz rite (SWR).
//     Licensed under the Mozilla Public License 2.0.
//     See LICENSE in the project root for license information, or visit
//     https://www.mozilla.org/MPL/2.0/.
// </copyright>

namespace Maseya.UI.WinForms;

using System;
using System.ComponentModel;

/// <summary>
/// Synchronizes the values and permitted ranges of two
/// <see cref="IRangedValueSource"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// This component maintains the authoritative value and permitted range.
/// When a source is assigned, it adopts the component's current range
/// and value.
/// </para>
/// <para>
/// Changes to the value or range of either source are propagated to
/// the component and the other source. Values are constrained to the
/// inclusive range defined by <see cref="Minimum"/> and
/// <see cref="Maximum"/>.
/// </para>
/// <para>
/// The component does not own or dispose its assigned sources.
/// It only manages its event subscriptions to them.
/// </para>
/// </remarks>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(ValueChanged))]
[DefaultBindingProperty(nameof(Value))]
public class RangedValueSynchronizerComponent : Component, IRangedValueSource
{
    private const int DefaultMinValue = Int32.MinValue;
    private const int DefaultMaxValue = Int32.MaxValue;
    private const int DefaultInitialValue = 0;

    private bool _disposed = false;

    private int _minimum = DefaultMinValue;
    private int _maximum = DefaultMaxValue;
    private int _value = DefaultInitialValue;

    private bool _isSettingRange = false;
    private bool _isSettingValue = false;
    private int _pendingValue = DefaultInitialValue;
    private int _dirtyValue = DefaultInitialValue;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="RangedValueSynchronizerComponent"/> class.
    /// </summary>
    public RangedValueSynchronizerComponent()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="RangedValueSynchronizerComponent"/> class and
    /// adds it to the specified container.
    /// </summary>
    /// <param name="container">
    /// The container that owns this component.
    /// </param>
    public RangedValueSynchronizerComponent(IContainer container)
    {
        container.Add(this);
    }

    /// <summary>
    /// Gets or sets the minimum permitted value.
    /// </summary>
    /// <value>
    /// The inclusive lower boundary of the permitted range.
    /// </value>
    /// <remarks>
    /// Setting this property above the current maximum also raises
    /// <see cref="Maximum"/> to the new minimum.
    /// If the current value falls below the resulting range, it is
    /// adjusted to the minimum.
    /// Changes are propagated to both assigned sources.
    /// </remarks>
    [Category("Behavior")]
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
    /// Gets or sets the maximum permitted value.
    /// </summary>
    /// <value>
    /// The inclusive upper boundary of the permitted range.
    /// </value>
    /// <remarks>
    /// Setting this property below the current minimum also lowers
    /// <see cref="Minimum"/> to the new maximum.
    /// If the current value exceeds the resulting range, it is
    /// adjusted to the maximum.
    /// Changes are propagated to both assigned sources.
    /// </remarks>
    [Category("Behavior")]
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
    /// Gets or sets the current synchronized value.
    /// </summary>
    /// <value>
    /// The current value, constrained to the inclusive permitted range.
    /// </value>
    /// <remarks>
    /// Values assigned outside the permitted range are clamped to
    /// the nearest permitted boundary.
    /// Changes are propagated to both assigned sources.
    /// </remarks>
    [Category("Behavior")]
    [Description("The current synchronized integer value.")]
    [DefaultValue(DefaultInitialValue)]
    [Bindable(true)]
    public int Value
    {
        get
        {
            return _isSettingRange ? _dirtyValue : _value;
        }

        set
        {
            var newValue = Math.Clamp(value, Minimum, Maximum);
            if (_isSettingRange)
            {
                _dirtyValue = newValue;
                //return;
            }

            SetValueInternal(newValue);
        }
    }

    /// <summary>
    /// Gets or sets the first value source to synchronize.
    /// </summary>
    /// <value>
    /// The first source, or <see langword="null"/> if no source is assigned.
    /// </value>
    /// <remarks>
    /// When a source is assigned, its range and value are replaced
    /// with the component's current range and value.
    /// The component subscribes to the source's
    /// <see cref="IRangedValueSource.ValueChanged"/> and
    /// <see cref="IRangedValueSource.RangeChanged"/> events.
    /// Replacing or removing the source releases those subscriptions.
    /// </remarks>
    [Category("Behavior")]
    [Description("The first value source to synchronize.")]
    [DefaultValue(null)]
    [TypeConverter(typeof(ReferenceConverter))]
    public IRangedValueSource? Source1
    {
        get;

        set
        {
            SetValueSourceInternal(ref field, value);
        }
    }

    /// <summary>
    /// Gets or sets the second value source to synchronize.
    /// </summary>
    /// <value>
    /// The second source, or <see langword="null"/> if no source is assigned.
    /// </value>
    /// <remarks>
    /// When a source is assigned, its range and value are replaced
    /// with the component's current range and value.
    /// The component subscribes to the source's
    /// <see cref="IRangedValueSource.ValueChanged"/> and
    /// <see cref="IRangedValueSource.RangeChanged"/> events.
    /// Replacing or removing the source releases those subscriptions.
    /// </remarks>
    [Category("Behavior")]
    [Description("The second value source to synchronize.")]
    [DefaultValue(null)]
    [TypeConverter(typeof(ReferenceConverter))]
    public IRangedValueSource? Source2
    {
        get;

        set
        {
            SetValueSourceInternal(ref field, value);
        }
    }

    /// <summary>
    /// Occurs when the synchronized value changes.
    /// </summary>
    /// <remarks>
    /// The event is raised after the component's value has changed.
    /// It is not raised when an operation leaves the effective value unchanged.
    /// </remarks>
    [Category("Behavior")]
    [Description("Occurs when the synchronized value changes.")]
    public event EventHandler? ValueChanged;

    /// <summary>
    /// Occurs when the permitted range changes.
    /// </summary>
    /// <remarks>
    /// The event is raised after both range boundaries have reached
    /// their final values.
    /// It is not raised when an operation leaves the effective range unchanged.
    /// </remarks>
    [Category("Behavior")]
    [Description("Occurs when the range of permitted values changes.")]
    public event EventHandler? RangeChanged;

    /// <summary>
    /// Sets the minimum and maximum permitted values.
    /// </summary>
    /// <param name="min">The new inclusive minimum value.</param>
    /// <param name="max">The new inclusive maximum value.</param>
    /// <remarks>
    /// Both boundaries are updated as a single logical operation.
    /// If <paramref name="max"/> is less than <paramref name="min"/>,
    /// the resulting range is collapsed to
    /// <c>[min, min]</c>.
    /// The current value is clamped to the resulting range.
    /// Changes are propagated to both assigned sources.
    /// </remarks>
    public void SetRange(int min, int max)
    {
        max = Math.Max(max, min);
        if (Minimum == min && Maximum == max)
        {
            return;
        }

        var _currentValue = _isSettingRange ? _dirtyValue : _value;

        _isSettingRange = true;
        _pendingValue = _dirtyValue = Math.Clamp(_currentValue, min, max);

        _minimum = min;
        _maximum = max;

        Source1?.SetRange(min, max);
        if (!_isSettingRange)
        {
            // A recursive SetRange call succeeded, making this invocation
            // out of date. Do not make further changes.
            goto _end;
        }

        Source2?.SetRange(min, max);
        if (!_isSettingRange)
        {
            goto _end;
        }

        SetValueInternal(Math.Clamp(_dirtyValue, Minimum, Maximum));
        if (!_isSettingRange)
        {
            goto _end;
        }

        _isSettingRange = false;

    _end:
        OnRangeChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="RangeChanged"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> containing the event data.
    /// </param>
    protected virtual void OnRangeChanged(EventArgs e)
    {
        RangeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> containing the event data.
    /// </param>
    protected virtual void OnValueChanged(EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Releases the event subscriptions maintained by this component.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> when disposing managed resources;
    /// otherwise, <see langword="false"/>.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Source1 is not null)
            {
                Source1.ValueChanged -= ValueSetter_ValueChanged;
                Source1.RangeChanged -= ValueSource_RangeChanged;
                Source1 = null;
            }

            if (Source2 is not null)
            {
                Source2.ValueChanged -= ValueSetter_ValueChanged;
                Source2.RangeChanged -= ValueSource_RangeChanged;
                Source2 = null;
            }
        }

        _disposed = true;

        base.Dispose(disposing);
    }

    /// <summary>
    /// Updates the component's value and propagates the change to both
    /// assigned value sources.
    /// </summary>
    /// <remarks>
    /// The value is expected to already be within the permitted range.
    /// Does nothing if the effective value has not changed.
    /// Raises <see cref="ValueChanged"/> after the value has been propagated.
    /// </remarks>
    private void SetValueInternal(int value)
    {
        if (_value == value)
        {
            return;
        }

        _value = value;
        _isSettingValue = true;
        Source1?.Value = value;
        if (!_isSettingValue)
        {
            goto _end;
        }

        Source2?.Value = value;

    _end:
        OnValueChanged(EventArgs.Empty);
        _isSettingValue = false;
    }

    /// <summary>
    /// Replaces an assigned value source and updates its event subscriptions.
    /// </summary>
    /// <param name="dest">
    /// A reference to the field holding the current value source.
    /// </param>
    /// <param name="src">
    /// The new value source, or <see langword="null"/> to remove the current source.
    /// </param>
    /// <remarks>
    /// Unsubscribes from the previous source before replacing it.
    /// When a new source is assigned, its range and value are synchronized
    /// with the component before event subscriptions are established.
    /// Throws <see cref="ObjectDisposedException"/> if attempting to assign
    /// a non-null source after the component has been disposed.
    /// </remarks>
    private void SetValueSourceInternal(
        ref IRangedValueSource? dest,
        IRangedValueSource? src)
    {
        ObjectDisposedException.ThrowIf(_disposed && src is not null, this);

        if (ReferenceEquals(dest, src))
        {
            return;
        }

        if (dest is not null)
        {
            dest.ValueChanged -= ValueSetter_ValueChanged;
            dest.RangeChanged -= ValueSource_RangeChanged;
        }

        dest = src;

        if (dest is not null)
        {
            dest.SetRange(Minimum, Maximum);
            dest.Value = Value;
            dest.ValueChanged += ValueSetter_ValueChanged;
            dest.RangeChanged += ValueSource_RangeChanged;
        }
    }

    private void ValueSetter_ValueChanged(object? sender, EventArgs e)
    {
        var rangeValueSource = (IRangedValueSource)sender!;
        if (_isSettingRange)
        {
            _dirtyValue = rangeValueSource.Value;
            return;
        }

        Value = rangeValueSource.Value;
    }

    private void ValueSource_RangeChanged(object? sender, EventArgs e)
    {
        var rangeValueSource = (IRangedValueSource)sender!;
        SetRange(rangeValueSource.Minimum, rangeValueSource.Maximum);
    }
}
