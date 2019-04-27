using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BoardPosition : MonoBehaviour
{
    public IsoVector2 boardPos = new IsoVector2();

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = boardPos.ToWorldPos();
    }
}
