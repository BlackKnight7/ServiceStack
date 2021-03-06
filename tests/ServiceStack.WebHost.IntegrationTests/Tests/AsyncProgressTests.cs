﻿// Copyright (c) Service Stack LLC. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceStack.Text;
using ServiceStack.WebHost.IntegrationTests.Services;

namespace ServiceStack.WebHost.IntegrationTests.Tests
{
    public class AsyncProgressTests
    {
        [Test]
        public async Task Can_report_progress_when_downloading_async()
        {
            var hold = AsyncServiceClient.BufferSize;
            AsyncServiceClient.BufferSize = 100; //Default BufferSize = 8192

            try
            {
                var asyncClient = new JsonServiceClient(Config.ServiceStackBaseUri);

                var progress = new List<string>();

                //Note: total = -1 when 'Transfer-Encoding: chunked'
                //Available in ASP.NET or in HttpListener when downloading responses with known lengths: 
                //E.g: Strings, Files, etc.
                asyncClient.OnDownloadProgress = (done, total) =>
                    progress.Add("{0}/{1} bytes downloaded".Fmt(done, total));

                List<Movie> response = await asyncClient.GetAsync(new TestProgress());

                progress.Each(x => x.Print());

                Assert.That(response.Count, Is.GreaterThan(0));
                Assert.That(progress.Count, Is.GreaterThan(0));
                Assert.That(progress.First(), Is.EqualTo("100/1160 bytes downloaded"));
                Assert.That(progress.Last(), Is.EqualTo("1160/1160 bytes downloaded"));
            }
            finally
            {
                AsyncServiceClient.BufferSize = hold;
            }
        }         
    }
}