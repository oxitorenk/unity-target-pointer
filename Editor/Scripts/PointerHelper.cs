using UnityEngine;

namespace Oxitorenk.TargetPointer.Editor.Scripts
{
    /// <summary>
    /// Static helper class to calculate target positions and indicators for UI pointers.
    /// </summary>
    public static class PointerHelper
    {
        /// <summary>
        /// Returns the screen position of a target based on its world position.
        /// </summary>
        /// <param name="mainCamera">Main camera used to project the world position to screen space.</param>
        /// <param name="targetPosition">World position of the target.</param>
        /// <returns>Vector3 representing the screen position of the target.</returns>
        public static Vector3 GetScreenPosition(Camera mainCamera, Vector3 targetPosition)
        {
            var screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
            return screenPosition;
        }

        /// <summary>
        /// Checks if the target is within the visible screen area.
        /// </summary>
        /// <param name="screenPosition">Screen position of the target.</param>
        /// <returns>True if the target is visible, otherwise false.</returns>
        public static bool IsTargetVisible(Vector3 screenPosition)
        {
            var isTargetVisible = screenPosition.z > 0 && 
                                  screenPosition.x > 0 && screenPosition.x < Screen.width && 
                                  screenPosition.y > 0 && screenPosition.y < Screen.height;
            
            return isTargetVisible;
        }

        /// <summary>
        /// Calculates the on-screen indicator position and angle when the target is off-screen.
        /// </summary>
        /// <param name="currentPosition">The current screen position of the target (world-to-screen point).</param>
        /// <param name="screenCentre">The center of the screen (typically Screen.width / 2, Screen.height / 2).</param>
        /// <param name="screenBounds">The bounds of the screen in width and height (typically half of Screen.width, Screen.height).</param>
        /// <returns>A tuple containing the updated screen position for the on-screen indicator and the calculated angle for the indicator's rotation.</returns>
        public static (Vector3 screenPosition, float angle) GetOnScreenPointerPositionAndAngle(Vector3 currentPosition, Vector3 screenCentre, Vector3 screenBounds)
        {
            // Adjust the target's position relative to the screen's center.
            var screenPosition = currentPosition - screenCentre;

            // Invert the position if the target is behind the camera to ensure correct on-screen placement.
            if (screenPosition.z < 0)
                screenPosition *= -1;

            // Calculate the angle between the x-axis (bottom of the screen) and the direction of the target.
            var angle = Mathf.Atan2(screenPosition.y, screenPosition.x);

            // Calculate the slope of the line that goes from (0,0) to the screenPosition (y = mx).
            var slope = Mathf.Tan(angle);

            // Determine if the target is on the right or left side of the screen and set the x position accordingly.
            if (screenPosition.x > 0)
            {
                // Constrain the x position to the maximum screen bounds, and calculate y using y = mx.
                screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
            }
            else
            {
                // Constrain the x position to the negative maximum screen bounds, and calculate y.
                screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
            }

            // If the calculated y position exceeds the screen bounds, adjust the x position accordingly.
            if (screenPosition.y > screenBounds.y)
            {
                // Constrain y to the screen bounds and adjust x based on the slope (x = y/m).
                screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
            }
            else if (screenPosition.y < -screenBounds.y)
            {
                // Constrain y to the negative bounds and adjust x accordingly.
                screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
            }

            // Restore the screen position relative to the original screen center.
            screenPosition += screenCentre;

            return (screenPosition, angle);
        }

    }
}
