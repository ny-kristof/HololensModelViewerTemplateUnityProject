using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchVersion : MonoBehaviour
{
    public void Version()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}
