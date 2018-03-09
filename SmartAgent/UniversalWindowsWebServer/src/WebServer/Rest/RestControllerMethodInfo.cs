﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Restup.HttpMessage.Models.Schemas;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;

namespace Restup.Webserver.Rest
{
    internal class RestControllerMethodInfo
    {
        internal enum TypeWrapper
        {
            None,
            Task,
            AsyncOperation
        }

        private readonly IEnumerable<Type> _validParameterTypes;

        private readonly UriParser _uriParser;
        private readonly ParsedUri _matchUri;
        private readonly IEnumerable<ParameterValueGetter> _parameterGetters;

        internal MethodInfo MethodInfo { get; }
        internal HttpMethod Verb { get; }
        internal bool HasContentParameter { get; }
        internal Type ContentParameterType { get; }
        internal TypeWrapper ReturnTypeWrapper { get; }
        internal Func<object[]> ControllerConstructorArgs { get; }

        internal RestControllerMethodInfo(
            MethodInfo methodInfo,
            Func<object[]> constructorArgs,
            TypeWrapper typeWrapper)
        {
            constructorArgs.GuardNull(nameof(constructorArgs));
            _uriParser = new UriParser();
            _matchUri = GetUriFromMethod(methodInfo);

            ReturnTypeWrapper = typeWrapper;
            ControllerConstructorArgs = constructorArgs;
            MethodInfo = methodInfo;

            _validParameterTypes = GetValidParameterTypes();
            _parameterGetters = GetParameterGetters(methodInfo);
            Verb = GetVerb();

            Type contentParameterType;
            HasContentParameter = TryGetContentParameterType(methodInfo, out contentParameterType);
            ContentParameterType = contentParameterType;
        }

        private ParsedUri GetUriFromMethod(MethodInfo methodInfo)
        {
            var uriFormatter = methodInfo.GetCustomAttribute<UriFormatAttribute>();
            ParsedUri parsedUri;
            if (!_uriParser.TryParse(uriFormatter.UriFormat, out parsedUri))
                throw new Exception($"Could not parse uri: {uriFormatter.UriFormat}");

            return parsedUri;
        }

        private Type[] GetValidParameterTypes()
        {
            return new[] {
                typeof(IEnumerable<byte>),
                typeof(IEnumerable<sbyte>),
                typeof(IEnumerable<short>),
                typeof(IEnumerable<ushort>),
                typeof(IEnumerable<int>),
                typeof(IEnumerable<uint>),
                typeof(IEnumerable<long>),
                typeof(IEnumerable<ulong>),
                typeof(IEnumerable<decimal>),
                typeof(IEnumerable<double>),
                typeof(IEnumerable<float>),
                typeof(IEnumerable<string>),
                typeof(IEnumerable<bool>),
                typeof(IEnumerable<char>),
                typeof(string),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(byte),
                typeof(bool),
                typeof(DateTime),
                typeof(char),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
            };
        }

        private bool TryGetContentParameterType(MethodInfo methodInfo, out Type content)
        {
            var fromContentParameter = methodInfo.GetParameters().FirstOrDefault(p => p.GetCustomAttribute<FromContentAttribute>() != null);
            if (fromContentParameter != null)
            {
                content = fromContentParameter.ParameterType;
                return true;
            }

            content = null;
            return false;
        }

        private ParameterValueGetter[] GetParameterGetters(MethodInfo methodInfo)
        {
            var fromUriParams = (from p in methodInfo.GetParameters()
                                 where p.GetCustomAttribute<FromContentAttribute>() == null
                                 select p).ToList();

            if (!ParametersHaveValidType(fromUriParams.Select(p => p.ParameterType)))
            {
                throw new InvalidOperationException("Can't use method parameters with a custom type.");
            }

           return fromUriParams.Select(x => GetParameterGetter(x, _matchUri)).ToArray();
        }

        private static ParameterValueGetter GetParameterGetter(ParameterInfo parameterInfo, ParsedUri matchUri)
        {
            var methodName = parameterInfo.Name;
            var firstPathPartMatch = matchUri.PathParts
                .Select((x, i) => new { Part = x, Index = i })
                .Where(x => x.Part.PartType == PathPart.PathPartType.Argument)
                .FirstOrDefault(x => methodName.Equals(x.Part.Value, StringComparison.OrdinalIgnoreCase));

            var parameterType = parameterInfo.ParameterType;
            if (firstPathPartMatch != null)
                return new PathParameterValueGetter(methodName, parameterType, firstPathPartMatch.Index);

            var firstQueryParameterMatch = matchUri.Parameters
                .FirstOrDefault(x => methodName.Equals(x.Value, StringComparison.OrdinalIgnoreCase));

            if (firstQueryParameterMatch != null)
                return new QueryParameterValueGetter(methodName, parameterType, firstQueryParameterMatch);

            throw new Exception($"Method {methodName} not found in rest controller method uri {matchUri}.");
        }

        private bool ParametersHaveValidType(IEnumerable<Type> parameters)
        {
            return !parameters.Except(_validParameterTypes).Any();
        }

        private HttpMethod GetVerb()
        {
            TypeInfo returnType;

            if (ReturnTypeWrapper == TypeWrapper.None)
                returnType = MethodInfo.ReturnType.GetTypeInfo();
            else
                returnType = MethodInfo.ReturnType.GetGenericArguments()[0].GetTypeInfo();

            return GetVerb(returnType);
        }

        private HttpMethod GetVerb(TypeInfo returnType)
        {
            if (IsRestResponseOfType<IGetResponse>(returnType))
                return HttpMethod.GET;
            if (IsRestResponseOfType<IPostResponse>(returnType))
                return HttpMethod.POST;
            if (IsRestResponseOfType<IPutResponse>(returnType))
                return HttpMethod.PUT;
            if (IsRestResponseOfType<IDeleteResponse>(returnType))
                return HttpMethod.DELETE;

            throw new ArgumentException($"Verb for return type {returnType} not know. Please use only {typeof(IGetResponse)}, {typeof(IPostResponse)}, {typeof(IPutResponse)}, {typeof(IDeleteResponse)} as return types.");
        }

        private static bool IsRestResponseOfType<T>(TypeInfo returnType)
        {
            return returnType.ImplementedInterfaces.Contains(typeof(T)) || returnType.AsType() == typeof(T);
        }

        internal bool Match(ParsedUri uri)
        {
            if (_matchUri.PathParts.Count != uri.PathParts.Count)
                return false;

            for (var i = 0; i < _matchUri.PathParts.Count; i++)
            {
                var fromPart = _matchUri.PathParts[i];
                var toPart = uri.PathParts[i];
                if (fromPart.PartType == PathPart.PathPartType.Argument)
                    continue;

                if (!fromPart.Value.Equals(toPart.Value, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            if (uri.Parameters.Count < _matchUri.Parameters.Count)
                return false;

            return _matchUri.Parameters.All(x => uri.Parameters.Any(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        }

        internal IEnumerable<object> GetParametersFromUri(ParsedUri uri)
        {
            return _parameterGetters.Select(x => x.GetParameterValue(uri)).ToArray();
        }

        public override string ToString()
        {
            return $"Hosting {Verb} method on {_matchUri}";
        }
    }
}
