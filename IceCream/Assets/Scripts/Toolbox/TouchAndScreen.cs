using UnityEngine;

public static class TouchAndScreen
{
    /*
    //Spiel läuft immer in Landscape-Modus, daher ist die Bildschirmhöhe immer referenz
    public static Vector2 PixelToWorld(Vector3 pixelPos) => (Vector2)Camera.main.transform.position
        - Camera.main.orthographicSize * new Vector2(Camera.main.aspect, 1)
        + 2 * Camera.main.orthographicSize * new Vector2(Camera.main.aspect * pixelPos.x/Camera.main.pixelWidth, pixelPos.y/Camera.main.pixelHeight);
        */

    public static Vector2 PixelToWorld(Vector3 pixelPos)
    {
        float rot = Camera.main.transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 oPos = 2 * Camera.main.orthographicSize * new Vector2(Camera.main.aspect * (pixelPos.x / Camera.main.pixelWidth -.5f), pixelPos.y / Camera.main.pixelHeight -.5f);
        return (Vector2)Camera.main.transform.position + new Vector2(oPos.x * Mathf.Cos(rot) - oPos.y * Mathf.Sin(rot), 
                                                                     oPos.y * Mathf.Cos(rot) + oPos.x * Mathf.Sin(rot));
    }


    /// <summary>
    /// Veraltet, da man einfach rectT.position = world.position machen kann
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    public static Vector2 WorldToPixel(Vector3 worldPos, Vector2 center = new Vector2())
    {
        float rot = Camera.main.transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 diff = worldPos - Camera.main.transform.position;
        diff = new Vector2(diff.x * Mathf.Cos(rot) - diff.y * Mathf.Sin(rot),
                           diff.y * Mathf.Cos(rot) + diff.x * Mathf.Sin(rot));//Setze achsen gleich
        return ((diff / (new Vector2(Camera.main.aspect, 1) * Camera.main.orthographicSize)) + (Vector2.one * .5f - center))//von 0 bs 1
            * new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
    }
}
