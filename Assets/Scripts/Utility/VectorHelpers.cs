using UnityEngine;

public static class VectorHelpers
{
    public static Vector3 ClampRotation(Vector3 rotation, Vector3 referenceRotation, float horizontalFreedom, float verticalFreedom)
    {
        var angleDiffY = Mathf.DeltaAngle(referenceRotation.y, rotation.y);
        var angleDiffX = Mathf.DeltaAngle(referenceRotation.x, rotation.x);
        var absAngleY = Mathf.Abs(angleDiffY);
        var absAngleX = Mathf.Abs(angleDiffX);
        
        if (absAngleY > horizontalFreedom / 2)
        {
            var degreesToRotateBack = (absAngleY - horizontalFreedom / 2) * Mathf.Sign(angleDiffY);
            rotation.y -= degreesToRotateBack;
        }

        if (absAngleX > verticalFreedom / 2)
        {
            var degreesToRotateBack = (absAngleX - verticalFreedom / 2) * Mathf.Sign(angleDiffX);
            rotation.x -= degreesToRotateBack;
        }

        return rotation;
    }
}