using System.Collections.Generic;
using UnityEngine;

namespace Util {
    public static class Logger {
        private static bool m_ShouldLog = true;
        public static List<Object> excludedContexts = new();

        public static void Log(object message, Color color, Object context) {
            if (m_ShouldLog && !excludedContexts.Contains(context)) {
                Debug.Log("<color=" + color.ToString().ToLower() + ">" + message + "</color>", context);
            }
        }

        public static void Log(object message, Object context) {
            if (m_ShouldLog) {
                Log(message, Color.WHITE, context);
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