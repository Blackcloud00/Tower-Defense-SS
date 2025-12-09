using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSlot : MonoBehaviour
{
    public bool canBuild;

    public void ButtonCheck()
    {
        canBuild = !canBuild;
        Debug.Log("It worked hahahaha!");
    }
}
