using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtension
{
    public static int GetNormali(this float math)
    {
        return (int)(Mathf.Abs(math)/math);
    }
}
