// <copyright file="NodeCacheService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Caching;

using System.Security.Cryptography;
using System.Text;

/// <summary>

/// TODO: Describe NodeCacheService.

/// </summary>

public class NodeCacheService
{
    private readonly string _cacheDir;

    public NodeCacheService()
    {
        _cacheDir = Path.Combine(Path.GetTempPath(), "ToscaWorkspaceGuardian", "NodeCache");
        Directory.CreateDirectory(_cacheDir);
    }

    private static string NormalizePathKey(string nodePath)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(nodePath ?? string.Empty);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private string GetFilePath(string nodePath)
    {
        var key = NormalizePathKey(nodePath);
        return Path.Combine(_cacheDir, $"node_{key}.txt");
    }

    public bool TryGetHash(string nodePath, out string? hash)
    {
        hash = null;
        try
        {
            var path = GetFilePath(nodePath);
            if (!File.Exists(path))
            {
                return false;
            }

            hash = File.ReadAllText(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void UpdateHash(string nodePath, string hash)
    {
        try
        {
            var path = GetFilePath(nodePath);
            File.WriteAllText(path, hash);
        }
        catch
        {
            // ignore
        }
    }
}

