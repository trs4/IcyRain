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
                    break;
                case ReadState.Namespace:
                    if (line.StartsWith(Keys.Service))
                    {
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
                        {
                            string with = value.Substring(index + Keys.With.Length);

                            if (string.IsNullOrWhiteSpace(with))
                                throw GetLineException(i, line);

                            value = value.Substring(0, index).TrimEnd();

                            foreach (string part in with.Split(','))
                            {
                                string withPart = part.Trim();

                                if (withPart == Keys.LZ4)
                                    WithLZ4 = true;
                                else if (withPart == Keys.ProtectedMethods)
                                    ProtectedMethods = true;
                                else
                                    throw GetLineException(i, line);
                            }
                        }

                        if (!IsName(value))
                            throw GetLineException(i, line);

                        Name = value;
                    }
                    break;
                case ReadState.Service:
                    {
                        string value = line.Trim();
                        CheckEmptyLine(i, line, value);

                        if (value != "{")
                            throw GetLineException(i, line);

                        state = ReadState.Operations;
                    }
                    break;
                case ReadState.Operations:
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
                                    {
                                        value = parameterValue.Trim();
                                        index = value.IndexOf(' ');

                                        if (index == -1)
                                            throw GetLineException(i, line);

                                        string parameterType = parameterValue.Substring(0, index);
                                        string parameterName = parameterValue.Substring(index + 1).TrimStart();

                                        if (!IsName(parameterType) || !IsName(parameterName))
                                            throw GetLineException(i, line);

                                        parameters.Add(new Parameter(parameterType, parameterName));
                                    }
                                }
                            }

                            _operations.Add(new Operation(type, name, parameters, responseType));
                        }
                    }
                    break;
                case ReadState.End:
                    throw new InvalidOperationException("state is end");
            }
        }

        if (state != ReadState.End)
            throw new InvalidOperationException("state is not end");
    }

    public ReadOnlyCollection<string> Usings => _usings.AsReadOnly();

    public string Namespace { get; }

    public string Name { get; }

    public bool WithLZ4 { get; }

    public bool ProtectedMethods { get; }

    public ReadOnlyCollection<Operation> Operations => _operations.AsReadOnly();

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
