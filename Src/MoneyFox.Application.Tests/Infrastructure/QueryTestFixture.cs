﻿using System;
using System.Diagnostics.CodeAnalysis;
using MoneyFox.Persistence;
using Xunit;

namespace MoneyFox.Application.Tests.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class QueryTestFixture : IDisposable
    {
        public EfCoreContext Context { get; }

        public QueryTestFixture()
        {
            Context = InMemoryEfCoreContextFactory.Create();
        }

        public void Dispose()
        {
            InMemoryEfCoreContextFactory.Destroy(Context);
        }
    }

    [CollectionDefinition("QueryCollection")]
    public class QueryCollection : ICollectionFixture<QueryTestFixture>
    {
    }
}