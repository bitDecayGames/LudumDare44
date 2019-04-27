using UnityEngine;

public class CustomMath
{
    public static bool AreVectorsEqual(Vector2 a, Vector2 b){
        return Vector2.SqrMagnitude(a - b) < 0.0001;
    }
    
    public static bool AreVectorsWithinRangeOfEachOther(Vector2 a, Vector2 b, float range){
        return Vector2.SqrMagnitude(a - b) < range;
    }

    public static bool IsAPointWithinBounds2D(Vector3 point, Bounds bounds)
    {
        return bounds.Contains(new Vector3(point.x, point.y, bounds.min.z));
    }
    
    public static bool IsASpriteWithinBounds2D(Vector3 spritePosition, Bounds spriteBounds, Bounds containingBoundaries)
    {
        var topLeft = spriteBounds.min;
        var bottomRight = spriteBounds.max;
            
        return containingBoundaries.Contains(new Vector3(spritePosition.x + topLeft.x, spritePosition.y + topLeft.y, containingBoundaries.min.z)) &&
                                              containingBoundaries.Contains(new Vector3(spritePosition.x + bottomRight.x, spritePosition.y + bottomRight.y, containingBoundaries.min.z));
    }
}
