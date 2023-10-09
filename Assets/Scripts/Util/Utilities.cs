using System.Collections;
using UnityEngine;

namespace Util {
    public class Utilities : MonoBehaviour {
        private static float GetTriangleArea(Vector2 p1, Vector2 p2, Vector2 p3) {
            return Mathf.Abs((p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2.0f);
        }

        public static bool IsInsideTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 point) {
            float A = GetTriangleArea(p1, p2, p3);
            float A1 = GetTriangleArea(point, p2, p3);
            float A2 = GetTriangleArea(p1, point, p3);
            float A3 = GetTriangleArea(p1, p2, point);

            return Mathf.Abs(A - (A1 + A2 + A3)) < 0.01f;
        }

        public static Vector3 GetRandomPointInCircle(Vector3 origin, float radius) {
            Vector3 randomPoint = Random.insideUnitSphere * radius;
            randomPoint += origin;
            return randomPoint;
        }

        public static Vector3 GetRandomPointInTorus(Vector3 center, float minDistance) {
            Vector3 randomPoint = (Random.insideUnitSphere + Vector3.one) * minDistance;
            randomPoint += center;
            randomPoint.y = center.y;

            return randomPoint;
        }

        public static bool IsPointInsideCameraViewport(Camera camera, Vector3 point) {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);
            return viewportPoint.x is > 0 and < 1 && viewportPoint.y is > 0 and < 1;
        }

        public static void InvokeDelayed(System.Action action, float delay, MonoBehaviour context) {
            context.StartCoroutine(InvokeDelayedCoroutine(action, delay));
        }

        /// <summary>
        /// Clamp a Vector4 to a min and max value.
        /// </summary>
        /// <param name="input">Vector4 input</param>
        /// <param name="minInclusive">Minimum inclusive value</param>
        /// <param name="maxInclusive">Maximum inclusive value</param>
        /// <returns>The input value clamped</returns>
        public static Vector4 ClampVector4(Vector4 input, float minInclusive, float maxInclusive) {
            return new Vector4(
                Mathf.Clamp(input.x, minInclusive, maxInclusive),
                Mathf.Clamp(input.y, minInclusive, maxInclusive),
                Mathf.Clamp(input.z, minInclusive, maxInclusive),
                Mathf.Clamp(input.w, minInclusive, maxInclusive)
            );
        }

        public static bool MovementInfluenceValid(Vector4 movementInfluence) {
            float sum = 0;
            sum += movementInfluence.x + movementInfluence.y + movementInfluence.z + movementInfluence.w;
            return sum > 0.1f;
        }

        private static IEnumerator InvokeDelayedCoroutine(System.Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        public static void InvokeNextFrame(System.Action action, MonoBehaviour context) {
            context.StartCoroutine(InvokeNextFrameCoroutine(action));
        }

        private static IEnumerator InvokeNextFrameCoroutine(System.Action action) {
            yield return new WaitForEndOfFrame();
            action.Invoke();
        }
    }
}