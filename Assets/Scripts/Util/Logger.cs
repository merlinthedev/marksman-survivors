using UnityEngine;

namespace Util {
    public static class Logger {

        private static bool m_ShouldLog = true;

        public static void Log(object message, Color color, Object context) {
            if (m_ShouldLog) {
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