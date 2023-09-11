using UnityEngine;

namespace Util {
    public static class Logger {
        public static void Log(object message, Color color, Object context) {
            Debug.Log("<color=" + color.ToString().ToLower() + ">" + message + "</color>", context);
        }

        public static void Log(object message, Object context) {
            Logger.Log(message, Color.WHITE, context);
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