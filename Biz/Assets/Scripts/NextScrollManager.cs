using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NextScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;

    const int SIZE = 9;
    const float NEXT = 0.168f;
    public float Next
    {
        get
        {
            return NEXT;
        }
    }
    float[] pos = new float[SIZE];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SIZE; i++)
        {
            pos[i] = NEXT * i;
        }
    }
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }
    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        
    }
    // 드래그 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        for (int i = 0; i < SIZE; i++)
        {
            if ((scrollbar.value < pos[i] + NEXT * 0.5f) &&
                (scrollbar.value > pos[i] - NEXT * 0.5f))
            {
                scrollbar.value = pos[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
