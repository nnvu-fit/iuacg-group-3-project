using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

public class ModelController : MonoBehaviour
{
    // register the cubism model
    public CubismModel cubismModel;

    // register landmark models
    public List<LandmarkModel> landmarkModels;

    // Start is called before the first frame update
    void Start()
    {
        cubismModel = this.FindCubismModel();
    }


    // LateUpdate is called after Update once per frame
    void LateUpdate()
    {     
        //Init all face model landmark
        LandmarkModel eyeleft = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeBlinkLeft");
        LandmarkModel eyeright = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeBlinkRight");
        LandmarkModel browleft = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "browOuterUpLeft");
        LandmarkModel browright = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "browOuterUpRight");
        LandmarkModel mouthpucker = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "mouthPucker");
        LandmarkModel mouthopen = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "jawOpen");
        LandmarkModel eyelookleft = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeLookInLeft");
        LandmarkModel eyelookright = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeLookInRight");
        LandmarkModel eyelookdown = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeLookDownLeft");
        LandmarkModel eyelookup = landmarkModels.Find(landmarkModels => landmarkModels.CategoryName == "eyeLookUpLeft");

        //Adjust model left eye
        cubismModel.Parameters[4].Value = (float)(1 - 2*eyeright.Score);

        //Adjust model right eye
        cubismModel.Parameters[6].Value = (float)(1 - 2*eyeleft.Score);

        //Adjust model eye to look left or right
        cubismModel.Parameters[8].Value = (float)(-1* eyelookright.Score + eyelookleft.Score);

        //Adjust model eye to look down or up
        cubismModel.Parameters[9].Value = (float)(-2*eyelookdown.Score + 2*eyelookup.Score);

        //Adjust model left brow
        cubismModel.Parameters[10].Value = (float)(2*browright.Score - 1);

        //Adjust model right brow
        cubismModel.Parameters[11].Value = (float)(2*browleft.Score - 1);

        //Adjust model mouth form
        cubismModel.Parameters[12].Value = (float)(3*mouthpucker.Score - 2);

        //Adjust model mouth open
        cubismModel.Parameters[13].Value = (float)(mouthopen.Score);

    }
}
