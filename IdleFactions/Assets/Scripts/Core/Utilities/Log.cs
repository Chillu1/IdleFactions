using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFactions
{
	public enum LogLevel
	{
		None = 0,
		Error = 1,
		Warning = 2,
		Info = 3,
		Verbose = 4,
	}

	public static class Log
	{
		private static readonly Dictionary<string, LogLevel> categories = new Dictionary<string, LogLevel>()
		{
#if UNITY_EDITOR
			{ "general", LogLevel.Verbose },
			{ "unitlibrary", LogLevel.Verbose },
			{ "modifiers", LogLevel.Verbose },
			{ "networking", LogLevel.Verbose },
			{ "io", LogLevel.Verbose },
			{ "saveload", LogLevel.Verbose },
			{ "log", LogLevel.Verbose },
#else
            {"general", LogLevel.Warning},
            {"unitlibrary", LogLevel.Warning},
            {"modifiers", LogLevel.Warning},
            {"networking", LogLevel.Warning},
            {"io", LogLevel.Warning},
            {"saveload", LogLevel.Warning},
            {"log", LogLevel.Warning},
#endif
		};

		public static void AddCategory(string categoryName, LogLevel level = LogLevel.Warning)
		{
			if (categories.ContainsKey(categoryName))
			{
				Warning("Category " + categoryName + " already exists");
				categories[categoryName] = level;
				return;
			}

			categories.Add(categoryName, level);
		}

		public static LogLevel ChangeCategoryLogLevel(string categoryName, LogLevel level)
		{
			if (categories[categoryName] == level)
				return level;

			var oldLevel = categories[categoryName];
			Info($"Set log level to: {level}, from {oldLevel}", "log", true);
			categories[categoryName] = level;
			return oldLevel;
		}

		public static void Verbose(object message, string category = "general")
		{
			if (category != "log" && !CheckForCategory(category))
				return;
			if (categories[category] < LogLevel.Verbose)
				return;

			Debug.Log($"VERB: {message}");
		}

		/// <summary>
		///     Logs a message
		/// </summary>
		/// <remarks>
		///     Use <paramref name="strict"/> to log no matter the log level (specific command/terminal info/feedback)
		/// </remarks>
		public static void Info(object message, string category = "general", bool strict = false)
		{
			if (category != "log" && !CheckForCategory(category))
				return;
			if (categories[category] < LogLevel.Info && !strict)
				return;

			Debug.Log($"INFO: {message}");
		}

		public static void Info(params object[] messages)
		{
			Info(string.Join(", ", messages));
		}

		public static void Warning(object message, string category = "general")
		{
			if (category != "log" && !CheckForCategory(category))
				return;
			if (categories[category] < LogLevel.Warning)
				return;

			Debug.LogWarning($"WARN: {message}");
		}

		public static void Error(object message, string category = "general")
		{
			if (category != "log" && !CheckForCategory(category))
				return;
			if (categories[category] < LogLevel.Error)
				return;

			Debug.LogError($"ERROR: {message}");
		}

		public static void Exception(Exception exception, string category = "general")
		{
			// always log Exceptions
			Debug.LogException(exception);
			//Debug.LogError($"EXCE: {exception.GetType().Name}.\tMessage: {exception.Message}\nStackTrace: {exception.StackTrace}");
		}

		private static bool CheckForCategory(string category)
		{
			if (categories.ContainsKey(category))
			{
				return true;
			}
			else
			{
				Error(category + " is not setup", "log");
				return false;
			}
		}
	}
}