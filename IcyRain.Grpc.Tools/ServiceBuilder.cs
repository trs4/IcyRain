using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace IcyRain.Grpc.Tools;

internal static class ServiceBuilder
{
    private const string _indent = "    ";

    public static string Build(Service service)
    {
        if (service is null)
            throw new ArgumentNullException(nameof(service));

        var builder = new StringBuilder(2048);

        var usings = new HashSet<string>(service.Usings)
        {
            "System",
            "System.IO",
            "System.Threading.Tasks",
            "Grpc.Core",
            "IcyRain.Grpc.Service.Internal",
            "IcyRain.Grpc.Service.Wrappers",
            "IcyRain.Streams",
        };

        if (service.Usings.Count > 0)
        {
            foreach (string usingValue in usings.OrderByDescending(u => u.StartsWith("System")).ThenBy(u => u))
                builder.Append("using ").Append(usingValue).Append(";\r\n");

            builder.AppendLine();
        }

        builder.Append("namespace ").Append(service.Namespace).Append(";\r\n\r\n")
            .Append("public static class ").Append(service.Name).Append("\r\n{\r\n")
            .Append(_indent).Append("public static readonly string ServiceName = \"").Append(service.Name).Append("\";\r\n\r\n")
            .Append(_indent).Append("public static readonly bool WithLZ4 = ").Append(service.WithLZ4 ? "true" : "false").Append(";\r\n\r\n");

        var marshallers = new Dictionary<string, string>();

        var operations = new List<(Operation Operation, string MethodName,
            string RequestType, string RequestMarshaller, string ResponseType, string ResponseMarshaller)>();

        foreach (var operation in service.Operations)
        {
            string methodName = GetMethodName(operation);

            string requestType = GetTypeForMarshaller(operation.Parameters);
            string requestMarshaller = AddMarshaller(builder, service, marshallers, requestType);

            string responseType = GetTypeForMarshaller(operation.ResponseType);
            string responseMarshaller = AddMarshaller(builder, service, marshallers, responseType);

            operations.Add((operation, methodName, requestType, requestMarshaller, responseType, responseMarshaller));
        }

        foreach (var operation in operations)
        {
            builder.Append(_indent).Append("private static readonly Method<")
                .Append(operation.RequestType).Append(", ").Append(operation.ResponseType).Append("> ").Append(operation.MethodName)
                .Append(" = new(MethodType.").Append(operation.Operation.Type).Append(", ServiceName, \"").Append(operation.Operation.Name).Append("\", ")
                .Append(operation.RequestMarshaller).Append(", ").Append(operation.ResponseMarshaller).Append(");\r\n\r\n");
        }

        builder.Append(_indent).Append("public static ServerServiceDefinition BindService(Server serviceImpl)\r\n")
            .Append(_indent).Append(_indent).Append("=> ServerServiceDefinition.CreateBuilder()\r\n");

        foreach (var operation in operations)
        {
            builder.Append(_indent).Append(_indent).Append(".AddMethod(").Append(operation.MethodName)
                .Append(", serviceImpl.").Append(operation.Operation.Name).Append(")\r\n");
        }

        builder.Append(_indent).Append(_indent).Append(".Build();\r\n\r\n")
            .Append(_indent).Append("public static void BindService(ServiceBinderBase serviceBinder, Server serviceImpl)\r\n")
            .Append(_indent).Append("{\r\n");

        foreach (var operation in operations)
        {
            builder.Append(_indent).Append(_indent).Append("serviceBinder.AddMethod(").Append(operation.MethodName)
                .Append(", serviceImpl is null ? null : new ").Append(operation.Operation.Type).Append("ServerMethod<")
                .Append(operation.RequestType).Append(", ").Append(operation.ResponseType).Append(">(serviceImpl.")
                .Append(operation.Operation.Name).Append("));\r\n");
        }

        builder.Append(_indent).Append("}\r\n\r\n")
            .Append(_indent).Append("public class Client : ClientBase<Client>\r\n")
            .Append(_indent).Append("{\r\n")
            .Append(_indent).Append(_indent).Append("public Client(ChannelBase channel) : base(channel) { }\r\n\r\n")
            .Append(_indent).Append(_indent).Append("public Client(CallInvoker callInvoker) : base(callInvoker) { }\r\n\r\n")
            .Append(_indent).Append(_indent).Append("protected Client() { }\r\n\r\n")
            .Append(_indent).Append(_indent).Append("private Client(ClientBaseConfiguration configuration) : base(configuration) { }\r\n\r\n")
            .Append(_indent).Append(_indent).Append("protected override Client NewInstance(ClientBaseConfiguration configuration)\r\n")
            .Append(_indent).Append(_indent).Append(_indent).Append("=> new Client(configuration);\r\n");

        string modificator = service.ProtectedMethods ? "protected" : "public";

        foreach (var operation in operations)
        {
            var type = operation.Operation.Type;
            string name = operation.Operation.Name;
            bool hasClientStreamingPart = type is OperationType.ClientStreaming or OperationType.DuplexStreaming;

            builder.AppendLine().Append(_indent).Append(_indent).Append(modificator).Append(" Async").Append(type).Append("Call<");

            if (hasClientStreamingPart)
                builder.Append(operation.RequestType).Append(", ");

            builder.Append(operation.ResponseType).Append("> ").Append(name).Append('(');

            if (!hasClientStreamingPart)
                builder.Append(operation.RequestType).Append(" request, ");

            builder.Append("CallOptions options = default)\r\n")
                .Append(_indent).Append(_indent).Append(_indent).Append("=> CallInvoker.Async").Append(type)
                .Append("Call(").Append(operation.MethodName).Append(", null, options");

            if (!hasClientStreamingPart)
                builder.Append(", request");

            builder.Append(");\r\n");
        }

        builder.Append(_indent).Append("}\r\n\r\n")
            .Append(_indent).Append("[BindServiceMethod(typeof(").Append(service.Name).Append("), \"BindService\")]\r\n")
            .Append(_indent).Append("public abstract class Server\r\n")
            .Append(_indent).Append("{\r\n");

        bool isFirst = true;

        foreach (var operation in operations)
        {
            var type = operation.Operation.Type;
            string name = operation.Operation.Name;

            if (isFirst)
                isFirst = false;
            else
                builder.AppendLine();

            builder.Append(_indent).Append(_indent).Append("public abstract Task");

            if (type is OperationType.Unary or OperationType.ClientStreaming)
                builder.Append('<').Append(operation.ResponseType).Append('>');

            builder.Append(' ').Append(name).Append('(');

            switch (type)
            {
                case OperationType.Unary:
                    builder.Append(operation.RequestType).Append(" request");
                    break;
                case OperationType.ClientStreaming:
                    builder.Append("IAsyncStreamReader<").Append(operation.RequestType).Append("> request");
                    break;
                case OperationType.ServerStreaming:
                    builder.Append(operation.RequestType).Append(" request, IServerStreamWriter<").Append(operation.ResponseType).Append("> response");
                    break;
                case OperationType.DuplexStreaming:
                    builder.Append("IAsyncStreamReader<").Append(operation.RequestType).Append("> request, IServerStreamWriter<")
                        .Append(operation.ResponseType).Append("> response");
                    break;
            }

            builder.Append(", ServerCallContext context);\r\n");
        }

        builder.Append(_indent).Append("}\r\n\r\n")
            .Append("}\r\n");

        return builder.ToString();
    }

    private static string GetTypeForMarshaller(string type)
    {
        if (type == "void")
            return "EmptyWrapper";

        // %%TODO int string
        return type;
    }

    private static string GetTypeForMarshaller(ReadOnlyCollection<Parameter> parameters)
    {
        if (parameters.Count == 0)
            return "EmptyWrapper";
        else if (parameters.Count == 1)
            return parameters[0].Type;

        // %%TODO Wrapper<,,,,>
        return null;
    }

    private static string GetMarshallerName(string type)
        => $"_{char.ToLower(type[0])}{type.Substring(1)}Marshaller";

    private static string GetMethodName(Operation operation)
        => $"_{char.ToLower(operation.Name[0])}{operation.Name.Substring(1)}Method";

    private static string AddMarshaller(StringBuilder builder, Service service, Dictionary<string, string> marshallers, string type)
    {
        if (marshallers.TryGetValue(type, out string marshallerName))
            return marshallerName;

        marshallerName = GetMarshallerName(type);
        marshallers.Add(type, marshallerName);

        builder.Append(_indent).Append("private static readonly Marshaller<").Append(type).Append("> ").Append(marshallerName)
            .Append(" = new(GrpcSerialization.SerializeData");

        if (service.WithLZ4)
            builder.Append("WithLZ4");

        builder.Append(", GrpcSerialization.DeserializeData");

        if (service.WithLZ4)
            builder.Append("WithLZ4");

        builder.Append('<').Append(type).Append(">);\r\n\r\n");
        return marshallerName;
    }

}
