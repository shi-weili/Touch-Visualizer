using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingAverageFilter : BaseFilter
{
    // Public functions
    public override Vector2 Filter(Vector2[] samples)
    {
        int actualNumSamples = Math.Min(samples.Length, NumSamples);
        Vector2 sum = new Vector2(0, 0);

        for (int i = 0; i < actualNumSamples; i++)
        {
            sum += samples[samples.Length - 1 - i];
        }

        return sum / actualNumSamples;
    }
}
