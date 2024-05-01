using System.Collections.Generic;
using Live2D.Cubism.Core;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    // register the cubism model
    [SerializeField] private CubismModel cubismModel;
    
    // register landmark models
    public ICollection<LandmarkModel> landmarkModels;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
