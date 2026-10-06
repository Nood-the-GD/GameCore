using UnityEngine;

public static class VectorUtil
{
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public static Vector3 GetPointAroundCircle(Vector3 origin, float radius, float angleInDegrees)
    {
        float radian = angleInDegrees * Mathf.Deg2Rad;
        float x = origin.x + radius * Mathf.Cos(radian);
        float y = origin.y + radian * Mathf.Sin(radian);
        return new Vector3(x, y);
    }

    public static Vector3 GetRandomPointAroundCircle(Vector3 origin, float radius)
    {
        float randomAngle = Random.Range(0f, 360f);
        return GetPointAroundCircle(origin, radius, randomAngle);
    }
}