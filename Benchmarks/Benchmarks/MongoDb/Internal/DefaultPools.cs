// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.IO;

namespace Benchmarks.MongoDb.Internal
{
    public static class DefaultPools
    {
        public static readonly RecyclableMemoryStreamManager MemoryStream =
            new RecyclableMemoryStreamManager();
    }
}
