// <copyright file="IRangedValueSource.cs" organization="Maseya">
//     Copyright (c) 2026 spel werdz rite (SWR).
//     Licensed under the Mozilla Public License 2.0.
//     See LICENSE in the project root for license information, or visit
//     https://www.mozilla.org/MPL/2.0/.
// </copyright>

namespace Maseya.UI.WinForms;

using System;

/// <summary>
/// Represents a source of an integer value constrained by a minimum
/// and maximum permitted value.
/// </summary>
public interface IRangedValueSource
{
    /// <summary>
    /// Gets or sets the minimum permitted value.
    /// </summary>
    /// <remarks>
    /// Setting this property may also change <see cref="Maximum"/>
    /// to maintain a valid range. If the current <see cref="Value"/>
    /// falls below the new minimum, it is adjusted accordingly.
    /// <see cref="RangeChanged"/> is raised when the effective range changes.
    /// <see cref="ValueChanged"/> is raised if the value changes.
    /// </remarks>
    int Minimum
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum permitted value.
    /// </summary>
    /// <remarks>
    /// Setting this property may also change <see cref="Minimum"/>
    /// to maintain a valid range. If the current <see cref="Value"/>
    /// exceeds the new maximum, it is adjusted accordingly.
    /// <see cref="RangeChanged"/> is raised when the effective range changes.
    /// <see cref="ValueChanged"/> is raised if the value changes.
    /// </remarks>
    int Maximum
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the current integer value.
    /// </summary>
    /// <remarks>
    /// Values assigned outside the permitted range are clamped
    /// to the nearest permitted value. <see cref="ValueChanged"/>
    /// is raised only when the effective value changes.
    /// </remarks>
    int Value
    {
        get;
        set;
    }

    /// <summary>
    /// Occurs when the permitted range changes.
    /// </summary>
    /// <remarks>
    /// Raised after both range properties have reached their final values.
    /// The event is raised only when at least one boundary changes.
    /// </remarks>
    event EventHandler? RangeChanged;

    /// <summary>
    /// Occurs when the current integer value changes.
    /// </summary>
    /// <remarks>
    /// The event is raised after the new value has been applied.
    /// It is not raised when an assignment leaves the effective value unchanged.
    /// </remarks>
    event EventHandler? ValueChanged;

    /// <summary>
    /// Sets the minimum and maximum permitted values.
    /// </summary>
    /// <param name="min">The new minimum permitted value.</param>
    /// <param name="max">The new maximum permitted value.</param>
    /// <remarks>
    /// Updates both boundaries as a single logical operation.
    /// The current value is adjusted if it falls outside the new range.
    /// <see cref="RangeChanged"/> is raised once if the effective range changes.
    /// <see cref="ValueChanged"/> is raised if the current value changes.
    /// </remarks>
    void SetRange(int minimum, int maximum);
}
