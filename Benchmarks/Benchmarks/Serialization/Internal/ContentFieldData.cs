// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Benchmarks.Serialization.Internal;

public class ContentFieldData : Dictionary<string, string>
{
    public ContentFieldData()
    {
    }

    public ContentFieldData(int capacity)
        : base(capacity)
    {
    }
}
