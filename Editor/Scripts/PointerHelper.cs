using UnityEngine;

namespace Oxitorenk.TargetPointer
{
    public static class PointerHelper
    {
        /// <summary>
        /// Converts a world position to a screen position based on the given camera
        /// </summary>
        /// <param name="camera">Camera to use</param>
        /// <param name="targetPosition">Target's position</param>
        /// <returns></returns>
        public static Vector3 GetScreenPosition(Camera camera, Vector3 targetPosition)
        {
            return camera.WorldToScreenPoint(targetPosition);
        }
        
        // Checks if the target is visible on the screen
        public static bool IsTargetVisible(Vector3 screenPosition)
        {
            return screenPosition.z > 0 &&
                   screenPosition.x >= 0 && screenPosition.x <= Screen.width &&
                   screenPosition.y >= 0 && screenPosition.y <= Screen.height;
        }
        
        /// <summary>
        /// Calculates the pointer's screen position and angle for off-screen targets
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="angle"></param>
        /// <param name="screenCenter"></param>
        /// <param name="screenBounds"></param>
        public static void CalculatePointerPositionAndAngle(ref Vector3 screenPosition, ref float angle, Vector3 screenCenter, Vector3 screenBounds)
        {
            screenPosition -= screenCenter;

            // Flip screenPosition if the target is behind the camera
            if (screenPosition.z < 0)
                screenPosition *= -1;

            // Calculate angle and slope based on screenPosition
            angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
            var slope = Mathf.Tan(angle);

            // Adjust position to fit within screen bounds
            if (screenPosition.x > 0)
            {
                screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
            }
            else
            {
                screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
            }

            if (screenPosition.y > screenBounds.y)
            {
                screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
            }
            else if (screenPosition.y < -screenBounds.y)
            {
                screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
            }

            screenPosition += screenCenter;
        }
    }
}
