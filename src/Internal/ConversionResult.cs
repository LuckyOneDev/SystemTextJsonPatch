// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class ConversionResult
{
    public ConversionResult(bool canBeConverted, object convertedInstance)
    {
        CanBeConverted = canBeConverted;
        ConvertedInstance = convertedInstance;
    }

    public bool CanBeConverted { get; }
    public object ConvertedInstance { get; }
}