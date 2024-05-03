using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

public class ModelController : MonoBehaviour
{
    // register the cubism model
    public CubismModel cubismModel;

    private SocketController socketController;

    // register landmark models
    public List<LandmarkModel> landmarkModels;

    LandmarkModel eyeright;

    CubismParameter param;
    int[] t = new int[60];
    int i;
    int r = 0;

    // Start is called before the first frame update
    void Start()
    {
        cubismModel = this.FindCubismModel();
        for (i = 0; i < 60; i++)
            t[i] = i - 30;
        i = 0;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        eyeright = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeBlinkRight");
        if (eyeright == null)
        {
            if (r == 0)
                i++;
            else
                i--;
            if (i == 59)
                r = 1;
            else if (i == 0)
                r = 0;
        }
        else
            i = -30;

        //eyeright.Score = (double)t[i];

        param = cubismModel.Parameters[2];
        param.Value = t[i];

        //if (socketController.landmarkModelList != null && socketController.landmarkModelList.FaceBlendshapes != null && socketController.landmarkModelList.FaceBlendshapes.Count > 0)
        //{
        //    landmarkModels = socketController.landmarkModelList.FaceBlendshapes;
        //}
        //LandmarkModel eyeleft = landmarkModels[9];
        eyeright = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeBlinkRight");
        //Console.WriteLine(eyeleft.Score);
        //int temp = landmarkModels.Count;
        //if (temp > 30)
        //    temp = 30;
        //cubismModel.Parameters[2].Value = (float)temp;
        //cubismModel.Parameters[1].Value = (float)eyeleft.Score;
        //cubismModel.Parameters[0].Value = 30;

        //if (r == 0)
        //    i++;
        //else
        //    i--;
        //if (i == 59)
        //    r = 1;
        //else if (i == 0)
        //    r = 0;
        //int g;
        ////eyeright.Score = (double)t[i];
        //if (eyeright.Score == null)
        //    g = -30;
        //else
        //    g = 30;

        //param = cubismModel.Parameters[0];
        //param.Value = (float)g;
    }
}


//{
//    CubismModel model;
//    CubismParameter param;
////float _t
//    int[] t = new int[60];
//    int i;
//    int r = 0;

//// Start is called before the first frame update
//    void Start(){
//        model = this.FindCubismModel();
//        for (i = 0; i < 60; i++)
//            t[i] = i - 30;
//        i = 0;
//    }


//// Update is called once per frame
//    void LateUpdate(){
//        if (r == 0)
//            i++;
//        else
//            i--;
//        if (i == 59)
//            r = 1;
//        else if (i == 0)
//            r = 0;

//        param = model.Parameters[2];
//        param.Value = t[i];
//    }
//}