#if !DEVELOPMENT_BUILD && !FORCE_LOGGING

using System;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mine.Code.Framework.Util.Debug
{
    public static class Debug
    {
        public static ILogger s_Logger = UnityEngine.Debug.unityLogger;

        public static ILogger unityLogger
        {
            get { return UnityEngine.Debug.unityLogger; }
        }

        [Conditional("UNITY_EDITOR")]
        public static void Break() =>
            UnityEngine.Debug.Break();

        [Conditional("UNITY_EDITOR")]
        public static void ClearDeveloperConsole() =>
            UnityEngine.Debug.ClearDeveloperConsole();

        [Conditional("UNITY_EDITOR")]
        public static void DebugBreak() =>
            UnityEngine.Debug.DebugBreak();

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end) =>
            UnityEngine.Debug.DrawLine(start, end);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color) =>
            UnityEngine.Debug.DrawLine(start, end, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) =>
            UnityEngine.Debug.DrawLine(start, end, color, duration);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest) =>
            UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir) =>
            UnityEngine.Debug.DrawRay(start, dir);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color) =>
            UnityEngine.Debug.DrawRay(start, dir, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration) =>
            UnityEngine.Debug.DrawRay(start, dir, color, duration);

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest) =>
            UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);

        [Conditional("UNITY_EDITOR")]
        public static void Log(object message) =>
            UnityEngine.Debug.Log(message);

        [Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context) =>
            UnityEngine.Debug.Log(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message) =>
            UnityEngine.Debug.LogError(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message, Object context) =>
            UnityEngine.Debug.LogError(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception) =>
            UnityEngine.Debug.LogException(exception);

        [Conditional("UNITY_EDITOR")]
        public static void LogException(Exception exception, Object context) =>
            UnityEngine.Debug.LogException(exception, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message) =>
            UnityEngine.Debug.LogWarning(message);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message, Object context) =>
            UnityEngine.Debug.LogWarning(message, context);

        [Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(context, format, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogFormat(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
        {
            UnityEngine.Debug.Assert(condition);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, Object context)
        {
            UnityEngine.Debug.Assert(condition, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, object message, Object context)
        {
            UnityEngine.Debug.Assert(condition, message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string message, Object context)
        {
            UnityEngine.Debug.Assert(condition, message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            UnityEngine.Debug.AssertFormat(condition, format, args);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void AssertFormat(
            bool condition,
            Object context,
            string format,
            params object[] args)
        {
            UnityEngine.Debug.AssertFormat(condition, context, format, args);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message)
        {
            UnityEngine.Debug.LogAssertion(message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void LogAssertion(object message, Object context)
        {
            UnityEngine.Debug.LogAssertion(message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogAssertionFormat(format, args);
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogAssertionFormat(context, format, args);
        }

        public static bool isDebugBuild
        {
            get { return UnityEngine.Debug.isDebugBuild; }
        }

        [Conditional("UNITY_ASSERTIONS")]
        [Conditional("UNITY_EDITOR")]
        [Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
        public static void Assert(bool condition, string format, params object[] args)
        {
            UnityEngine.Debug.Assert(condition, format, args);
        }

        [Obsolete("Debug.logger is obsolete. Please use Debug.unityLogger instead (UnityUpgradable) -> unityLogger")]
        public static ILogger logger
        {
            get { return UnityEngine.Debug.logger; }
        }
    }
}

#endif