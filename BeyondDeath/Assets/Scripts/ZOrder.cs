using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ZOrde se ajusta segun la coordenada para que no se solape con el entorno como columnas , etc
 * Usa en bottomAnchor en los pies
 * */
public class ZOrder : MonoBehaviour
{

    [SerializeField] private Transform anchor;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (anchor)
        {
            spriteRenderer.sortingOrder = (int)(anchor.position.y * -10);
        }
        else
        {
            spriteRenderer.sortingOrder = (int)(transform.position.y * -10);//se puede cambiar por -100 para la precision
        }

    }
}

