using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IcyRain.Grpc.Tools;

internal sealed class Service
{
    private readonly List<string> _usings = [];
    private readonly List<Operation> _operations = [];

    public Service(string data)
    {
        string[] lines = (data ?? string.Empty).Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        var state = ReadState.Using;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            switch (state)
            {
                case ReadState.Using:
                    ParseUsing(i, line, ref state);
                    break;
                case ReadState.Namespace:
                    ParseNamespace(i, line, ref state);
                    break;
                case ReadState.Service:
                    ParseService(i, line, ref state);
                    break;
                case ReadState.Operations:
                    ParseOperation(i, line, ref state);
                    break;
                case ReadState.End:
                    throw new InvalidOperationException("state is end");
            }
        }

        if (state != ReadState.End)
            throw new InvalidOperationException("state is not end");
    }

    public ReadOnlyCollection<string> Usings => _usings.AsReadOnly();

    public string Namespace { get; private set; }

    public string Name { get; private set; }

    public bool WithLZ4 { get; private set; }

    public bool WithProtectedMethods { get; private set; }

    public bool WithServer { get; private set; } = true;

    public bool WithClient { get; private set; } = true;

    public ReadOnlyCollection<Operation> Operations => _operations.AsReadOnly();

    private void ParseUsing(int i, string line, ref ReadState state)
    {
        if (line.StartsWith(Keys.Using))
        {
            string value = line.Substring(Keys.Using.Length).TrimStart().TrimEnd(' ', ';');
            CheckEmptyLine(i, line, value);
            _usings.Add(value);
        }
        else if (line.StartsWith(Keys.Namespace))
        {
            string value = line.Substring(Keys.Namespace.Length).TrimStart().TrimEnd(' ', ';');
            CheckEmptyLine(i, line, value);
            Namespace = value;
            state = ReadState.Namespace;
        }
    }

    private void ParseNamespace(int i, string line, ref ReadState state)
    {
        if (!line.StartsWith(Keys.Service))
            return;

        string value = line.Substring(Keys.Service.Length).TrimStart();
        CheckEmptyLine(i, line, value);
        state = ReadState.Service;

        if (value[value.Length - 1] == '{')
        {
            value = value.Substring(0, value.Length - 1).Trim();
            CheckEmptyLine(i, line, value);

            state = ReadState.Operations;
        }

        int index = value.IndexOf(Keys.With);

        if (index > -1)
            ParseNamespaceOptions(i, line, ref state, ref value, index);

        if (!IsName(value))
            throw GetLineException(i, line);

        Name = value;
    }

    private void ParseNamespaceOptions(int i, string line, ref ReadState state, ref string value, int index)
    {
        string with = value.Substring(index + Keys.With.Length);

        if (string.IsNullOrWhiteSpace(with))
            throw GetLineException(i, line);

        value = value.Substring(0, index).TrimEnd();
        bool withClient = false;
        bool withServer = false;

        foreach (string part in with.Split(','))
        {
            string withPart = part.Trim();

            if (withPart == Keys.LZ4)
                WithLZ4 = true;
            else if (withPart == Keys.ProtectedMethods)
                WithProtectedMethods = true;
            else if (withPart == Keys.Client)
                withClient = true;
            else if (withPart == Keys.Server)
                withServer = true;
            else
                throw GetLineException(i, line);
        }

        if (withClient || withServer)
        {
            WithClient = withClient;
            WithServer = withServer;
        }
    }

    private static void ParseService(int i, string line, ref ReadState state)
    {
        string value = line.Trim();
        CheckEmptyLine(i, line, value);

        if (value != "{")
            throw GetLineException(i, line);

        state = ReadState.Operations;
    }

    private void ParseOperation(int i, string line, ref ReadState state)
    {
        string value = line.Trim();
        CheckEmptyLine(i, line, value);

        if (value == "}")
            state = ReadState.End;
        else
        {
            var type = OperationType.Unary;
            var parameters = new List<Parameter>();

            if (value.StartsWith(Keys.Streaming))
            {
                value = value.Substring(Keys.Streaming.Length).TrimStart();
                type = OperationType.ServerStreaming;
            }

            int index = value.IndexOf(' ');

            if (index == -1)
                throw GetLineException(i, line);

            string responseType = value.Substring(0, index);
            value = value.Substring(index + 1).TrimStart();
            index = value.IndexOf('(');

            if (index == -1)
                throw GetLineException(i, line);

            string name = value.Substring(0, index);
            value = value.Substring(index + 1).TrimStart();

            if (value.Length < 1 || value[value.Length - 1] != ')')
                throw GetLineException(i, line);

            value = value.Substring(0, value.Length - 1).Trim();

            if (value.Length > 0)
                ParseParameters(i, line, value, ref type, parameters);

            _operations.Add(new Operation(type, name, parameters, responseType));
        }
    }

    private static void ParseParameters(int i, string line, string value, ref OperationType type, List<Parameter> parameters)
    {
        if (value.StartsWith(Keys.Streaming))
        {
            value = value.Substring(Keys.Streaming.Length).TrimStart();
            type = type == OperationType.ServerStreaming ? OperationType.DuplexStreaming : OperationType.ClientStreaming;

            if (!IsName(value))
                throw GetLineException(i, line);

            parameters.Add(new Parameter(value));
        }
        else
        {
            string[] parameterValues = value.Split(',');

            foreach (string parameterValue in parameterValues)
                ParseParameter(i, line, parameterValue, parameters);
        }
    }

    private static void ParseParameter(int i, string line, string parameterValue, List<Parameter> parameters)
    {
        string value = parameterValue.Trim();
        int index = value.IndexOf(' ');

        if (index == -1)
            throw GetLineException(i, line);

        string parameterType = parameterValue.Substring(0, index);
        string parameterName = parameterValue.Substring(index + 1).TrimStart();

        if (!IsName(parameterType) || !IsName(parameterName))
            throw GetLineException(i, line);

        parameters.Add(new Parameter(parameterType, parameterName));
    }

    private static bool IsName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        if (!char.IsLetter(name[0]))
            return false;

        for (int i = 1; i < name.Length; i++)
        {
            if (!char.IsLetterOrDigit(name[i]))
                return false;
        }

        return true;
    }

    private static void CheckEmptyLine(int i, string line, string value)
    {
        if (value.Length == 0)
            throw GetLineException(i, line);
    }

    private static InvalidOperationException GetLineException(int i, string line)
        => throw new InvalidOperationException($"line {i}: {line}");
}
