using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lib:
//https://developer.android.com/reference/android/os/Vibrator
//https://developer.android.com/reference/android/os/VibrationEffect
//Implementierung:
//https://stackoverflow.com/questions/39651021/vibrate-with-duration-and-pattern
//https://stackoverflow.com/questions/13950338/how-to-make-an-android-device-vibrate-with-different-frequency
public class Vibration
{
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject vibrator;

    public AndroidJavaClass vibEffectClass;
    public List<AndroidJavaObject> vibEffect = new List<AndroidJavaObject>();

    public enum predefined { EFFECT_CLICK, EFFECT_DOUBLE_CLICK, EFFECT_TICK, EFFECT_HEAVY_CLICK }

    public Vibration()
    {
        //vibClass = new AndroidJavaClass("android.os.Vibrator");
        #if UNITY_ANDROID
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            vibEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            return;
        #endif
        Debug.Log("Vibration wird nicht unterstützt!");
    }

    public void DestroyVibration()
    {
        //Cancel();

        vibEffectClass.Dispose();
        vibrator.Dispose();
        currentActivity.Dispose();
        unityPlayer.Dispose();
    }

    /// <summary>
    /// Stops current vibration
    /// </summary>
    public void Cancel()
    {
        vibrator.Call("cancel");
    }

    public bool HasVibrator()
    {
        return vibrator.Call<bool>("hasVibrator");
    }

    public bool HasAmplitudeControl()
    {
        return vibrator.Call<bool>("hasAmplitudeControl");
    }

    /// <summary>
    /// Creates a OneShot
    /// </summary>
    /// <param name="milliseconds">in milliseconds</param>
    /// <param name="amplitude">must be a value between 0-255 or equal -1 (default-strength)</param>
    public int SetVibrationEffect(long duration, int amplitude = -1)
    {
        vibEffect.Add(vibEffectClass.CallStatic<AndroidJavaObject>("createOneShot", duration, amplitude));
        return vibEffect.Count - 1;
    }
    /// <summary>
    /// Predefined Effect
    /// </summary>
    /// <param name="effect">see Vibrator.predefined</param>
    public int SetVibrationEffect( predefined effect)
    {
        int effect_id = 0;
        switch (effect)
        {
            case predefined.EFFECT_CLICK: effect_id = 0; break;
            case predefined.EFFECT_DOUBLE_CLICK: effect_id = 1; break;
            case predefined.EFFECT_TICK: effect_id = 2; break;
            case predefined.EFFECT_HEAVY_CLICK: effect_id = 5; break;

        }
        vibEffect.Add(vibEffectClass.CallStatic<AndroidJavaObject>("createPredefined", effect_id));
        return vibEffect.Count - 1;
    }
    /// <summary>
    /// Creates an array of OneShots, which is played in sucession
    /// </summary>
    /// <param name="durations">in milliseconds</param>
    /// <param name="amplitudes">must be a value between 0-255 or equal -1 (default-strength)</param>
    /// <param name="repeat">index from which the array starts to loop. There is no loop when repeat = -1 </param>
    public int SetVibrationEffect(long[] durations, int[] amplitudes, int repeat = -1)
    {
        vibEffect.Add(vibEffectClass.CallStatic<AndroidJavaObject>("createWaveform", durations, amplitudes, repeat));
        return vibEffect.Count - 1;
    }
    /// <summary>
    /// Creates an array of OneShots, which is played in sucession. The amplitude alternates between 0 and default-strength in each step, starting with 0
    /// </summary>
    /// <param name="durations">in milliseconds</param>
    /// <param name="repeat">index from which the array starts to loop. There is no loop when repeat = -1 </param>
    public int SetVibrationEffect(long[] durations, int repeat = -1)
    {
        vibEffect.Add(vibEffectClass.CallStatic<AndroidJavaObject>("createWaveform", durations, repeat));
        return vibEffect.Count - 1;
    }


    public void Vibrate(int id)
    {
        if(vibEffect.Count <= id)
        {
            Debug.Log("Error: vibrationEffect not set");
            return;
        }
        vibrator.Call("vibrate", vibEffect[id]);
    }
}
