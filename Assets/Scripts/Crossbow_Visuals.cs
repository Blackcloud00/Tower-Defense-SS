using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow_Visuals : MonoBehaviour
{
    private Tower_Crossbow myTower;

    [SerializeField] private LineRenderer attackVisuals;
    [SerializeField] private float attackVisualDuration = .1f;

    private void Awake()
    {
        myTower = GetComponent<Tower_Crossbow>();
    }
    public void PlayAttackVFX(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(VFXCoroutine(startPoint, endPoint));
    }

    private IEnumerator VFXCoroutine(Vector3 startPoint, Vector3 endPoint)
    {
        myTower.EnableRotation(false);

        attackVisuals.enabled = true;

        attackVisuals.SetPosition(0, startPoint);
        attackVisuals.SetPosition(1, endPoint);

        yield return new WaitForSeconds(attackVisualDuration);

        attackVisuals.enabled = false;

        myTower.EnableRotation(true);
    }
}
