using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICone
{
    Rigidbody2D Get_rb();
    int GetID();
    void SetID(int _id);
    IceAttribute Get_attribute();
    void ResetAttributes();
    Vector2 Get_posInCone();
    void Set_posInCone(Vector2 position);
    void Set_prevIce(Rigidbody2D _prev);
    string Get_name();
    Transform Get_transform();
    IEnumerator FillSpace(Vector2 endPos);
    IEnumerator ShootIce(ICone prev);
}
