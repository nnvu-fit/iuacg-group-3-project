using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

public class ParameterChanger : MonoBehaviour
{
    CubismModel model;
    CubismParameter param;
    //float _t
    int[] t = new int[60];
    int i;
    int r = 0;

    // Start is called before the first frame update
    void Start()
    {
        model = this.FindCubismModel();
        for (i = 0; i < 60; i++)
            t[i] = i - 30;
        i = 0;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (r == 0)
            i++;
        else
            i--;
        if (i == 59)
            r = 1;
        else if (i == 0)
            r = 0;

        param = model.Parameters[2];
        param.Value = t[i];
    }
}
