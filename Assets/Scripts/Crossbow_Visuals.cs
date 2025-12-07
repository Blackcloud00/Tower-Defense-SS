using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow_Visuals : MonoBehaviour
{
    private Tower_Crossbow myTower;
    private Enemy myEnemy;

    [SerializeField] private LineRenderer attackVisuals;
    [SerializeField] private float attackVisualDuration = .1f;

    [Header("Glowing Visuals")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;

    [Space]
    private float currentIntensity;
    [SerializeField] private float maxIntensity = 150;
    [Space]
    [SerializeField] private Color startColor;

    [Header("Rotor Visuals")]
    [SerializeField] private Transform rotor;
    [SerializeField] private Transform rotorUnloaded;
    [SerializeField] private Transform rotorLoaded;


    [SerializeField] private Color endColor;

    [Header("Front Glow String")]
    [SerializeField] private LineRenderer frontString_L;
    [SerializeField] private LineRenderer frontString_R;

    [Space]

    [SerializeField] private Transform frontStartPoint_L;
    [SerializeField] private Transform frontStartPoint_R;
    [SerializeField] private Transform frontEndPoint_R;
    [SerializeField] private Transform frontEndPoint_L;

    [Header("Back Glow String")]
    [SerializeField] private LineRenderer backString_L;
    [SerializeField] private LineRenderer backString_R;
        
    [Space]

    [SerializeField] private Transform backStartPoint_L;
    [SerializeField] private Transform backStartPoint_R;
    [SerializeField] private Transform backEndPoint_R;
    [SerializeField] private Transform backEndPoint_L;

    [SerializeField] private LineRenderer[] lineRenderers;
    private void Awake()
    {
        myTower = GetComponent<Tower_Crossbow>();
        material = new Material(meshRenderer.material);
        meshRenderer.material = material;

        UpdateMaterialsOnLineRenderers();

        StartCoroutine(ChangeEmission(1));
    }

    private void UpdateMaterialsOnLineRenderers()
    {
        foreach (var lr in lineRenderers)
        {
            lr.material = material;
        }
    }

    private void Update()
    {
        UpdateEmissionColor();
        UpdateStrings();

        if (attackVisuals.enabled && myEnemy != null)
            attackVisuals.SetPosition(1, myEnemy.GetCenterPoint());
    }

    private void UpdateStrings()
    {
        UpdateStringVisual(frontString_R, frontStartPoint_R, frontEndPoint_R);
        UpdateStringVisual(frontString_L, frontStartPoint_L, frontEndPoint_L);
        UpdateStringVisual(backString_L, backStartPoint_L, backEndPoint_L);
        UpdateStringVisual(backString_R, backStartPoint_R, backEndPoint_R);
    }

    public void PlayReloadFX(float duration)
    {
        float newDuration = duration / 2;

        StartCoroutine(ChangeEmission(newDuration));
        StartCoroutine(UpdateRotorDuration(newDuration));
    }   
    private void UpdateEmissionColor()
    {
        Color emissionColor = Color.Lerp(startColor, endColor, currentIntensity / maxIntensity);

        emissionColor = emissionColor * Mathf.LinearToGammaSpace(currentIntensity);

        material.SetColor("_EmissionColor", emissionColor);
    }
    public void PlayAttackVFX(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(VFXCoroutine(startPoint, endPoint));
    }

    private IEnumerator VFXCoroutine(Vector3 startPoint, Vector3 endPoint)
    {
        //myTower.EnableRotation(false);

        myEnemy = myTower.currentEnemy;

        attackVisuals.enabled = true;

        attackVisuals.SetPosition(0, startPoint);
        attackVisuals.SetPosition(1, endPoint);

        yield return new WaitForSeconds(attackVisualDuration);

        attackVisuals.enabled = false;

        //myTower.EnableRotation(true);
    }

    private IEnumerator ChangeEmission(float duration)
    {
        float startTime = Time.time;
        float startIntensity = 0;

        //Do something repeadetly until the duration has passed
        while (Time.time - startTime < duration)
        {
            //Calculates the proportion of the duration that has elapsed since the start of the coroutine
            float tValue = (Time.time - startTime) / duration;
            currentIntensity = Mathf.Lerp(startIntensity, maxIntensity, tValue);
            yield return null;
        }

        currentIntensity = maxIntensity;
    }

    IEnumerator UpdateRotorDuration(float duration)
    {
        float startTime = Time.time;

        while(Time.time - startTime < duration)
        {
            float tValue = (Time.time - startTime) / duration;
            rotor.position = Vector3.Lerp(rotorUnloaded.position, rotorLoaded.position, tValue);
            yield return null;
        }

        rotor.position = rotorLoaded.position;  
    }

    private void UpdateStringVisual(LineRenderer lineRenderer, Transform startPoint, Transform endPoint)
    {
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
}
