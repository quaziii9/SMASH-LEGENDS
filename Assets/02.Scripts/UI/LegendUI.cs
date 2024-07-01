using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendUI : MonoBehaviour
{
    private void LateUpdate()
    {
        // 캔버스의 회전을 고정
        transform.rotation = Quaternion.identity;
    }
}
