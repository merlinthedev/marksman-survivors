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
    
        public static bool IsPointInsideCameraViewport(Camera camera, Vector3 point) {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);
            return viewportPoint.x is > 0 and < 1 && viewportPoint.y is > 0 and < 1;
        }

        public static void InvokeDelayed(System.Action action, float delay, MonoBehaviour context) {
            context.StartCoroutine(InvokeDelayedCoroutine(action, delay));
        }
        
        private static IEnumerator InvokeDelayedCoroutine(System.Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    


    }
}