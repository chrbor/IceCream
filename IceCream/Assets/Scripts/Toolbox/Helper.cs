using UnityEngine;

public static class Helper
{
    /// <summary>
    /// Transforms a rotation to a vector in wspace
    /// </summary>
    /// <param name="angle">in radians</param>
    /// <returns></returns>
    public static Vector2 RotToVec(float angle) => new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

    /// <summary>
    /// Matrix, which multiplied with localPosition returns localPosition displaced by diff_rot degrees
    /// </summary>
    /// <param name="diff_rot">displacement in radians</param>
    /// <returns></returns>
    public static Vector4 GetRotationMatrix(float diff_rot) => new Vector4(Mathf.Cos(diff_rot), -Mathf.Sin(diff_rot), Mathf.Sin(diff_rot), Mathf.Cos(diff_rot));
}
