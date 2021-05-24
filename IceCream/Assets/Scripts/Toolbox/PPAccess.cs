using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPAccess : MonoBehaviour
{
    public static PPAccess postprocess;

    Volume volume;
    [HideInInspector]
    public Bloom bloom;
    [HideInInspector]
    public DepthOfField doF;
    [HideInInspector]
    public ColorAdjustments colorAdj;

    void Awake()
    {
        postprocess = this;
        volume = GetComponent<Volume>();

        volume.sharedProfile.TryGet(out bloom);
        volume.sharedProfile.TryGet(out doF);
        volume.sharedProfile.TryGet(out colorAdj);
    }

    public static void ResetPostProcess()
    {
        postprocess.doF.focalLength.value = 1;
        postprocess.colorAdj.colorFilter.value = Color.white;
    }
}