﻿using System.Text.RegularExpressions;

namespace Gamelib.Util;

public class PathUtil
{
    private const string ExeName = ".exe";

    private static readonly string replaceDirSep = Path.DirectorySeparatorChar == '\\' ? new string(Path.DirectorySeparatorChar, 2) : Path.DirectorySeparatorChar.ToString();
    private static readonly Regex multiDirSepChar = new(@$"(?<=({replaceDirSep}))\1+", RegexOptions.Compiled);


    /// <summary>
    /// Sanitize the passed Path<br/>
    /// 1.) Correct path with OS relevant directory seperator<br/>
    /// 2.) Remove leading comma ','<br/>
    /// 3.) Remove leading and trailing quotation marks '"'<br/>
    /// 4.) Remove leading directory seperator if no UNC path<br/>
    /// 5.) Remove trailing directory seperator<br/>
    /// </summary>
    /// <returns>The sanitized path <see langword="string"/></returns>
    public static string? Sanitize(string? path)
    {
        if (path is null)
            return path;

        // 1, 2 and 3
        path = path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .TrimStart(',')
            .TrimStart('\"')
            .TrimEnd('\"');

        // 4.) e.g. "\Data\game.exe" will be "Data\game.exe" but UNC path e.g. "\\pcname\test" will not be stripped
        if (path.Length > 2 && path[0] == Path.DirectorySeparatorChar && path[1] != Path.DirectorySeparatorChar)
            path = path.TrimStart(Path.DirectorySeparatorChar);

        // 5.) e.g. "D:\Games\\Game1\\\Bin\\ will be "D:\Games\Game1\Bin\"
        path = multiDirSepChar.Replace(path, "");

        // 6.) e.g. "D:\Games\Game1\Bin\" will be "D:\Games\Game1\Bin"
        path = path.TrimEnd(Path.DirectorySeparatorChar);

        return path;
    }


    /// <summary>
    /// Remove arguments after the executable if any
    /// </summary>
    /// <example>
    /// D:\Games\Game1\Bin\game.exe -withcheats -fullscreen 
    /// will be D:\Games\Game1\Bin\game.exe
    /// </example>
    /// <returns>The path <see langword="string"/> with removed parameters</returns>
    public static string? RemoveArgsFromExecutable(string? path)
    {
        int? pos = path?.IndexOf(ExeName);
        if (pos > 0)
            return Sanitize(path![..((int)pos + ExeName.Length)]) ?? string.Empty;

        return path;
    }


    /// <summary>
    /// Return whether passed in path is an executable
    /// </summary>
    /// <param name="path"></param>
    /// <returns>True if passed in Path is an executable; otherwise false</returns>
    public static bool IsExecutable(string? path) =>
        Path.GetExtension(path ?? string.Empty).ToLower() == ExeName;

}