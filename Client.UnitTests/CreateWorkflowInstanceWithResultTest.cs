using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GatewayProtocol;
using Grpc.Core;
using NUnit.Framework;
using Type = Google.Protobuf.WellKnownTypes.Type;

namespace Zeebe.Client
{
    [TestFixture]
    public class CreateWorkflowInstanceWithResultTest : BaseZeebeTest
    {
        [Test]
        public async Task ShouldSendRequestAsExpected()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    BpmnProcessId = "process",
                    Version = -1
                },
                RequestTimeout = 20 * 1000
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .WithResult()
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public void ShouldTimeoutRequest()
        {
            // given
            TestService.AddRequestHandler(typeof(CreateWorkflowInstanceWithResultRequest),
                request =>
                {
                    new EventWaitHandle(false, EventResetMode.AutoReset).WaitOne();
                    return null;
                });

            // when
            var task = ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .WithResult()
                .Send(TimeSpan.Zero);
            var aggregateException = Assert.Throws<AggregateException>(() => task.Wait(TimeSpan.FromSeconds(15)));
            var rpcException = (RpcException) aggregateException.InnerExceptions[0];

            // then
            Assert.AreEqual(Grpc.Core.StatusCode.DeadlineExceeded, rpcException.Status.StatusCode);
        }

        [Test]
        public void ShouldCancelRequest()
        {
            // given
            TestService.AddRequestHandler(typeof(CreateWorkflowInstanceWithResultRequest),
                request =>
                {
                    new EventWaitHandle(false, EventResetMode.AutoReset).WaitOne();
                    return null;
                });

            // when
            var task = ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .WithResult()
                .Send(new CancellationTokenSource(TimeSpan.Zero).Token);
            var aggregateException = Assert.Throws<AggregateException>(() => task.Wait(TimeSpan.FromSeconds(15)));
            var rpcException = (RpcException)aggregateException.InnerExceptions[0];

            // then
            Assert.AreEqual(Grpc.Core.StatusCode.Cancelled, rpcException.Status.StatusCode);
        }

        [Test]
        public async Task ShouldSendRequestWithVersionAsExpected()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    BpmnProcessId = "process",
                    Version = 1
                },
                RequestTimeout = 20 * 1000
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .Version(1)
                .WithResult()
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldSendRequestWithWorkflowKeyAsExpected()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    WorkflowKey = 1
                },
                RequestTimeout = 20 * 1000
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .WorkflowKey(1)
                .WithResult()
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldSendRequestWithRequestTimeoutAsExpected()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    WorkflowKey = 1
                },
                RequestTimeout = 123 * 1000
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .WorkflowKey(1)
                .WithResult()
                .Send(TimeSpan.FromSeconds(123));

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldSendRequestWithVariablesAsExpected()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    WorkflowKey = 1,
                    Variables = "{\"foo\":1}"
                },
                RequestTimeout = 20 * 1000
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .WorkflowKey(1)
                .Variables("{\"foo\":1}")
                .WithResult()
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldSendRequestWithVariablesAndProcessIdAsExpected()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    BpmnProcessId = "process",
                    Version = -1,
                    Variables = "{\"foo\":1}"
                },
                RequestTimeout = 20 * 1000
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .Variables("{\"foo\":1}")
                .WithResult()
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldSendRequestWithFetchVariables()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    BpmnProcessId = "process",
                    Version = -1,
                    Variables = "{\"foo\":1}"
                },
                RequestTimeout = 20 * 1000,
                FetchVariables = { "foo", "bar" }
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .Variables("{\"foo\":1}")
                .WithResult()
                .FetchVariables("foo", "bar")
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldSendRequestWithFetchVariablesAsList()
        {
            // given
            var expectedRequest = new CreateWorkflowInstanceWithResultRequest
            {
                Request = new CreateWorkflowInstanceRequest
                {
                    BpmnProcessId = "process",
                    Version = -1,
                    Variables = "{\"foo\":1}"
                },
                RequestTimeout = 20 * 1000,
                FetchVariables = { "foo", "bar" }
            };

            // when
            await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .Variables("{\"foo\":1}")
                .WithResult()
                .FetchVariables(new List<string> { "foo", "bar" })
                .Send();

            // then
            var request = TestService.Requests[typeof(CreateWorkflowInstanceWithResultRequest)][0];
            Assert.AreEqual(expectedRequest, request);
        }

        [Test]
        public async Task ShouldReceiveResponseAsExpected()
        {
            // given
            var expectedResponse = new CreateWorkflowInstanceWithResultResponse
            {
                BpmnProcessId = "process",
                Version = 1,
                WorkflowKey = 2,
                WorkflowInstanceKey = 121,
                Variables = "{\"foo\":\"bar\"}",
            };

            TestService.AddRequestHandler(typeof(CreateWorkflowInstanceWithResultRequest), request => expectedResponse);

            // when
            var workflowInstanceResponse = await ZeebeClient.NewCreateWorkflowInstanceCommand()
                .BpmnProcessId("process")
                .LatestVersion()
                .WithResult()
                .Send();

            // then
            Assert.AreEqual(2, workflowInstanceResponse.WorkflowKey);
            Assert.AreEqual(1, workflowInstanceResponse.Version);
            Assert.AreEqual(121, workflowInstanceResponse.WorkflowInstanceKey);
            Assert.AreEqual("process", workflowInstanceResponse.BpmnProcessId);
            Assert.AreEqual("{\"foo\":\"bar\"}", workflowInstanceResponse.Variables);
        }
    }
}