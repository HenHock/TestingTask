using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtension
{
    public static float ToPercent(this float value, float maxValue)
    {
        return value / maxValue;
    }
}
