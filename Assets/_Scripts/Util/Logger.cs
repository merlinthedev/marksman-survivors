using System.Collections.Generic;
using UnityEngine;

namespace Util {
    public static class Logger {
        private static readonly bool shouldLog = true;
        public static List<Object> excludedContexts = new();

        public static void Log(object message, Color color, Object context) {
            if (shouldLog && !excludedContexts.Contains(context)) {
                Debug.Log("<color=" + color.ToString().ToLower() + ">" + message + "</color>" + context,
                    context);
            }
        }

        public static void Log(object message, Object context) {
            if (shouldLog) {
                Log(message, Color.WHITE, context);
            }
        }

        public static void LogError(object message, Object context) {
            if (shouldLog && !excludedContexts.Contains(context)) {
                Debug.LogError("<color=red>" + message + "</color>" + context, context);
            }
        }


        public enum Color {
            RED,
            YELLOW,
            GREEN,
            BLUE,
            WHITE,
            GREY,
            PINK
        }
    }
}