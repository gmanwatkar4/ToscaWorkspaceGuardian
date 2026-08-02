// <copyright file="OutputParser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using System.Text.RegularExpressions;
using ToscaWorkspaceGuardian.Core.Models;

public class OutputParser
{
    public OutputDocument Parse(string output)
    {
        var document = new OutputDocument();

        if (string.IsNullOrWhiteSpace(output))
        {
            return document;
        }

        OutputObject? currentObject = null;
        using var reader = new System.IO.StringReader(output);
        string? rawLine;
        while ((rawLine = reader.ReadLine()) != null)
        {
            var line = rawLine.Trim();

            //----------------------------------------
            // Object
            //----------------------------------------
            if (line.StartsWith("'") && line.Contains('['))
            {
                var match = Regex.Match(
                    line,
                    @"'(.+?)'\s+\[(.+?)\]");

                if (match.Success)
                {
                    currentObject = new OutputObject
                    {
                        Name = match.Groups[1].Value,
                        ObjectType = match.Groups[2].Value,
                        RawText = line,
                    };

                    document.Objects.Add(currentObject);
                }

                continue;
            }

            if (currentObject == null)
            {
                continue;
            }

            //----------------------------------------
            // Collection Start
            //----------------------------------------
            //----------------------------------------
            // Single-line Collection
            //----------------------------------------
            if (line.Contains(" : {"))
            {
                int colon = line.IndexOf(':');

                string collectionName = line[..colon];

                if (collectionName.Contains('['))
                {
                    collectionName = collectionName[..collectionName.IndexOf('[')];
                }

                collectionName = collectionName.Trim();

                currentObject.Collections[collectionName] = new();

                string values = line[(colon + 1)..].Trim();

                values = values.Trim('{', '}');

                if (!string.IsNullOrWhiteSpace(values))
                {
                    foreach (var value in values.Split(','))
                    {
                        currentObject.Collections[collectionName]
                            .Add(value.Trim(' ', '\''));
                    }
                }

                continue;
            }

            //----------------------------------------
            // Property
            //----------------------------------------
            if (line.Contains('='))
            {
                bool readOnly = line.StartsWith("(R)");

                string property = line;

                if (readOnly)
                {
                    property = property.Substring(3);
                }

                int index = property.IndexOf('=');

                if (index < 0)
                {
                    continue;
                }

                currentObject.Properties.Add(
                    new OutputProperty
                    {
                        Name = property[..index].Trim(),
                        Value = property[(index + 1)..].Trim('\''),
                        IsReadOnly = readOnly,
                    });

                continue;
            }
        }

        return document;
    }
}
