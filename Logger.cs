/*
 *   Copyright (c) 2023 Joseph Early (@Joseph-Early on GitHub)
 *   All rights reserved.

 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

using System;
using System.IO;

namespace L200;

public static class Logger
{
    #region Logging Levels
    public enum Levels
    {
        FATAL,
        ERROR,
        WARN,
        INFO,
        DEBUG,
        TRACE
    }
    #endregion

    private static Levels _DefaultLevel;
    private static bool _logToFile;
    private static bool _logTime;
    private static string? _logPath;
    private static string? _logName;
    private static bool _initialized = false;

    /// <summary> Setup the logger. </summary>
    /// <param name="logToFile"> Log to a file </param>
    /// <param name="logToConsole"> Log to the console </param>
    /// <param name="logTime"> Include the time in the log </param>
    /// <param name="logPath"> Destination for log files (not file) </param>
    /// <param name="DefaultLevel"> Default log level </param>
    public static void Setup(bool logToFile = false, bool logTime = false, string logPath = "log/", Levels DefaultLevel = Levels.DEBUG)
    {
        // Set the values for the logger
        _logToFile = logToFile;
        _logTime = logTime;
        _logPath = logPath;
        _DefaultLevel = DefaultLevel;

        // Generate the log file name based on the current date and time (to milliseconds)
        _logName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".log";

        // Set initialized to true
        _initialized = true;
    }

    #region Specific Level Log Methods
    /// <summary> Log an fatal error message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string LogFatal<T>(T value) => Log(value, Levels.FATAL);

    /// <summary> Log an error message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string LogError<T>(T value) => Log(value, Levels.ERROR);

    /// <summary> Log a warning message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string LogWarn<T>(T value) => Log(value, Levels.WARN);

    /// <summary> Log an info message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string LogInfo<T>(T value) => Log(value, Levels.INFO);

    /// <summary> Log a debug message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string LogDebug<T>(T value) => Log(value, Levels.DEBUG);

    /// <summary> Log a trace message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string LogTrace<T>(T value) => Log(value, Levels.TRACE);

    /// <summary> Log a message to the console. </summary>
    /// <param name="message"> The message to log. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Log<T>(T value) => Log(value, _DefaultLevel);
    #endregion

    /// <summary> Log a message to the console. </summary>
    /// <param name"logs"> The logger instance </param>
    /// <param name="message"> The message to log. </param>
    /// <param name="level"> The level of the message. </param>
    /// <returns> The message that was logged represented as a string. </returns>
    public static string Log<T>(T value, Levels level = Levels.DEBUG)
    {
        // Check if Instance is not null
        if (!_initialized) throw new Exception($"LogIt is not initialized. Please call Logger.Setup() before using the logger.");

        // Get the current colour
        var color = Console.ForegroundColor;

        // Set the prefix based on the level
        var prefix = level switch
        {
            Levels.FATAL => "[FATAL]",
            Levels.ERROR => "[ERROR]",
            Levels.WARN => "[WARN]",
            Levels.INFO => "[INFO]",
            Levels.DEBUG => "[DEBUG]",
            Levels.TRACE => "[TRACE]",
            _ => "[INFO]"
        };

        // Set the colour based on the level
        Console.ForegroundColor = level switch
        {
            Levels.FATAL => ConsoleColor.Red,
            Levels.ERROR => ConsoleColor.DarkRed,
            Levels.WARN => ConsoleColor.Yellow,
            Levels.INFO => ConsoleColor.White,
            Levels.DEBUG => ConsoleColor.DarkGray,
            Levels.TRACE => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        // The message to log formatted with the prefix and time
        var message = $"{prefix} {(_logTime ? DateTime.Now.ToString("HH:mm:ss.fff") : "")} {value}";

        // Log the message
        Console.WriteLine(message);

        // Reset the colour
        Console.ForegroundColor = color;

        // Log the message to a file
        if (_logToFile)
        {
            // Check the path exists
            if (!Directory.Exists(_logPath))
            {
                // Create the directory
                Directory.CreateDirectory(_logPath!);
            }

            // Create the file if it doesn't exist
            if (!File.Exists(_logPath + _logName))
            {
                // Store the previous path
                var previousPath = Directory.GetCurrentDirectory();

                // Change directory to the log path
                Directory.SetCurrentDirectory(_logPath!);

                // Create the file
                File.Create(_logName!).Dispose();

                // Change directory back to the original
                Directory.SetCurrentDirectory(previousPath);
            }

            // Write the message to the file
            using (var writer = new StreamWriter(_logPath + _logName, true))
            {
                writer.WriteLine(message);
            }
        }

        // Reset the colour
        return message;
    }
}