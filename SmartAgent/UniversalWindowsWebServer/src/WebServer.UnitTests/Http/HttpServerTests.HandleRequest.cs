﻿using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Restup.HttpMessage.Models.Schemas;
using Restup.Webserver.Http;
using Restup.Webserver.UnitTests.TestHelpers;

namespace Restup.Webserver.UnitTests.Http
{
    [TestClass]
    public class HttpServerTests_HandleRequest
    {
        [TestMethod]
        public void HandleRequest_RegisteredOnDefaultRoute_RoutesSuccesfully()
        {
            new FluentHttpServerTests()
                .Given
                    .ListeningOnDefaultRoute()
                .When
                    .RequestHasArrived("/Get")
                .Then
                    .AssertRouteHandlerReceivedRequest()
                    .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.OK);
        }

        [TestMethod]
        [DataRow("api")]
        [DataRow("/api")]
        [DataRow("api/")]
        public void HandleRequest_RegisteredOnPrefixedRoute_RoutesSuccesfully(string registeredPrefix)
        {
            new FluentHttpServerTests()
               .Given
                   .ListeningOnRoute(registeredPrefix)
               .When
                   .RequestHasArrived("/api/Get")
               .Then
                   .AssertRouteHandlerReceivedRequest()
                   .AssertRouteHandlerRequest(x => x.Uri, new Uri("/Get", UriKind.Relative))
                   .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.OK);
        }

        [TestMethod]
        public void HandleRequest_OnNonRegisteredRoute_ReturnsBadRequest()
        {
            new FluentHttpServerTests()
               .When
                   .RequestHasArrived("/api/Get")
               .Then
                   .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.BadRequest);
        }

        [TestMethod]
        public void GivenMultipleRouteHandlersAreAddedInSequentialOrder_WhenRequestIsReceivedOnApiRoute_ThenRequestIsSuccesfullyReceived()
        {
            new FluentHttpServerTests()
               .Given
                   .ListeningOnDefaultRoute()
                   .ListeningOnRoute("/api")
               .When
                   .RequestHasArrived("/api/Get")
               .Then
                   .AssertRouteHandlerReceivedNoRequests(string.Empty)
                   .AssertRouteHandlerReceivedRequest("/api")
                   .AssertRouteHandlerRequest("/api", x => x.Uri, new Uri("/Get", UriKind.Relative))
                   .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.OK);
        }

        [TestMethod]
        public void GivenMultipleRouteHandlersAreAddedInSequentialOrder_WhenRequestIsReceivedOnAnyRoute_ThenRequestIsSuccesfullyReceived()
        {
            new FluentHttpServerTests()
               .Given
                   .ListeningOnDefaultRoute()
                   .ListeningOnRoute("/api")
               .When
                   .RequestHasArrived("/index.html")
               .Then
                   .AssertRouteHandlerReceivedNoRequests("/api")
                   .AssertRouteHandlerReceivedRequest(string.Empty)
                   .AssertRouteHandlerRequest(string.Empty, x => x.Uri, new Uri("/index.html", UriKind.Relative))
                   .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.OK);
        }

        [TestMethod]
        public void GivenMultipleRouteHandlersAreAddedInReverseOrder_WhenRequestIsReceivedOnApiRoute_ThenRequestIsSuccesfullyReceived()
        {
            new FluentHttpServerTests()
               .Given
                   .ListeningOnRoute("/api")
                   .ListeningOnDefaultRoute()
               .When
                   .RequestHasArrived("/api/Get")
               .Then
                   .AssertRouteHandlerReceivedNoRequests(string.Empty)
                   .AssertRouteHandlerReceivedRequest("/api")
                   .AssertRouteHandlerRequest("/api", x => x.Uri, new Uri("/Get", UriKind.Relative))
                   .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.OK);
        }

        [TestMethod]
        public void GivenMultipleRouteHandlersAreAddedInReverseOrder_WhenRequestIsReceivedOnAnyRoute_ThenRequestIsSuccesfullyReceived()
        {
            new FluentHttpServerTests()
               .Given
                   .ListeningOnRoute("/api")
                   .ListeningOnDefaultRoute()
               .When
                   .RequestHasArrived("/index.html")
               .Then
                   .AssertRouteHandlerReceivedNoRequests("/api")
                   .AssertRouteHandlerReceivedRequest(string.Empty)
                   .AssertRouteHandlerRequest(string.Empty, x => x.Uri, new Uri("/index.html", UriKind.Relative))
                   .AssertLastResponse(x => x.ResponseStatus, HttpResponseStatus.OK);
        }

        [TestMethod]
        public void GivenMultipleRouteHandlersAreBeingAddedWithTheSamePrefix_ThenAnExceptionShouldBeThrown()
        {
            var configuration = new HttpServerConfiguration()
                .RegisterRoute("api", new EchoRouteHandler());

            Assert.ThrowsException<Exception>(() => configuration.RegisterRoute("api", new EchoRouteHandler()));
        }

        [TestMethod]
        public void GivenMultipleRouteHandlersAreBeingAddedOnTheCatchAllRoute_ThenAnExceptionShouldBeThrown()
        {
            var configuration = new HttpServerConfiguration();
            configuration.RegisterRoute(new EchoRouteHandler());

            Assert.ThrowsException<Exception>(() => configuration.RegisterRoute(new EchoRouteHandler()));
        }
    }
}
