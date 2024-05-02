using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LandmarkModel
{
    [SerializeField] private int index;
    public int Index { get => index; set => index = value; }
    [SerializeField] private double score;
    public double Score { get => score; set => score = value; }
    [SerializeField] private string display_name;
    public string DisplayName { get => display_name; set => display_name = value; }
    [SerializeField] private string category_name;
    public string CategoryName { get => category_name; set => category_name = value; }
}

[Serializable]
public class LandmarkModelList
{
    [SerializeField] private List<LandmarkModel> face_blendshapes;
    public List<LandmarkModel> FaceBlendshapes { get => face_blendshapes; set => face_blendshapes = value; }
    [SerializeField] private string error;
    public string Error { get => error; set => error = value; }
}