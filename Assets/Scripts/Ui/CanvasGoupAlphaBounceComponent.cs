using System;
using UnityEngine;

public class CanvasGoupAlphaBounceComponent : MonoBehaviour
{
    public const float AlphaBounceSpeed = 1f;
    private bool isFading;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            throw new Exception("Component requires a CanvasGroup");
        }

        _canvasGroup.alpha = 0;
    }
    
    private void Update()
    {
        if (!isFading)
        {
            _canvasGroup.alpha = _canvasGroup.alpha + AlphaBounceSpeed*Time.deltaTime;    
            
            if (_canvasGroup.alpha >= 1)
            {
                isFading = true;
            }
        }
        else
        {
            _canvasGroup.alpha = _canvasGroup.alpha - AlphaBounceSpeed*Time.deltaTime;  
            
            if (_canvasGroup.alpha <= 0)
            {
                isFading = false;
            }
        }
        
        
    }
}