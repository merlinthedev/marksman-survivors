using System;
using System.Collections;
using System.Numerics;
using Champions;
using UnityEditor;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

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

        public static Vector3 GetPointToMouseDirection(Vector3 point) {
            Vector3 mousePos = GetMouseWorldPosition();
            return (mousePos - point).normalized;
        }


        /// <summary>
        /// Gets the mouse position in world coordinates. For now the Y level is hardcoded to 1.15 because currently
        /// this is the only Y level the champion can be on. For the future we might have to refactor this to take
        /// in the current Y level of the champion.
        /// </summary>
        /// <returns>The mouse position in world coordinates with the same Y level as the champion.</returns>
        /// <exception cref="Exception">If the raycast somehow misses the level, this will throw an exception</exception>
        public static Vector3 GetMouseWorldPosition() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("ExcludeFromMovementClicks");
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                var mousePos = hit.point;
                mousePos.y = 1.15f;

                return mousePos;
            }

            throw new Exception("Our raycast did not find something... check where we are hovering [Utilities]");
        }

        public static bool IsPointInsideCameraViewport(Camera camera, Vector3 point) {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);
            return viewportPoint.x is > 0 and < 1 && viewportPoint.y is > 0 and < 1;
        }

        public static void InvokeDelayed(System.Action action, float delay, MonoBehaviour context) {
            context.StartCoroutine(InvokeDelayedCoroutine(action, delay));
        }

        /// <summary>
        /// Gets the signed angle between the current champion moving direction and the mouse position.
        /// </summary>
        /// <param name="champion">Champion to get the current moving direction from.</param>
        /// <returns>Gets the signed angle between the current champion moving direction and the mouse position.</returns>
        public static float GetGlobalAngleFromDirection(Champion champion) {
            float angle = 0;
            Vector3 direction = (GetMouseWorldPosition() - champion.transform.position);

            // Debug.Log("MouseToWorldPosition: " + GetMouseWorldPosition());
            // Debug.Log("Champion Position: " + champion.transform.position);
            // Debug.Log("Direction: " + direction);
            //
            Debug.DrawLine(GetMouseWorldPosition(), champion.transform.position, Color.red, 0.5f);
            Debug.DrawLine(champion.transform.position,
                champion.transform.position + champion.GetCurrentMovementDirection(), Color.yellow, 0.5f);

            angle = Vector3.SignedAngle(direction, champion.GetCurrentMovementDirection(), Vector3.up);
            // Debug.Log("Champion current movement direction: " + champion.GetCurrentMovementDirection());
            //
            // Logger.Log("Angle from direction to mouse: " + angle, champion);
            return angle;
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

        /// <summary>
        /// Checks whether or not we should use the influence for enemy spawns or not.
        /// </summary>
        /// <param name="movementInfluence"></param>
        /// <returns>Should the movement data be used to influence spawns.</returns>
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