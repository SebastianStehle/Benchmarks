// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Benchmarks.Serialization.Internal
{
    public sealed class ContentFieldDataSurrogate : Dictionary<string, string>, ISurrogate<ContentFieldData>
    {
        public void FromSource(ContentFieldData source)
        {
            EnsureCapacity(source.Count);

            foreach (var (key, value) in source)
            {
                this[key] = value;
            }
        }

        public ContentFieldData ToSource()
        {
            var result = new ContentFieldData(Count);

            foreach (var (key, value) in this)
            {
                var actualKey = key;

                if (actualKey == "iv")
                {
                    actualKey = string.Intern(key);
                }

                result.Add(actualKey, value);
            }

            return result;
        }
    }
}
