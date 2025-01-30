// ------------------------------------------------------------------------
// Copyright 2021 The Dapr Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//     http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------

namespace Dapr.Client.Test
{
    using System.Linq;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Autogenerated = Dapr.Client.Autogen.Grpc.v1;

    public class DaprApiTokenTest
    {
        [Fact]
        public async Task DaprCall_WithApiTokenSet()
        {
            // Configure Client
            await using var client = TestClient.CreateForDaprClient(c => c.UseDaprApiToken("test_token"));

            var request = await client.CaptureGrpcRequestAsync(async daprClient =>
            {
                return await daprClient.GetSecretAsync("testStore", "test_key");
            });

            request.Dismiss();

            // Get Request and validate
            await request.GetRequestEnvelopeAsync<Autogenerated.GetSecretRequest>();

            request.Request.Headers.TryGetValues("dapr-api-token", out var headerValues);
            headerValues.Count().ShouldBe(1);
            headerValues.First().ShouldBe("test_token");
        }

        [Fact]
        public async Task DaprCall_WithoutApiToken()
        {
            // Configure Client
            await using var client = TestClient.CreateForDaprClient();

            var request = await client.CaptureGrpcRequestAsync(async daprClient =>
            {
                return await daprClient.GetSecretAsync("testStore", "test_key");
            });

            request.Dismiss();

            // Get Request and validate
            await request.GetRequestEnvelopeAsync<Autogenerated.GetSecretRequest>();

            request.Request.Headers.TryGetValues("dapr-api-token", out var headerValues);
            headerValues.ShouldBeNull();
        }
    }
}
