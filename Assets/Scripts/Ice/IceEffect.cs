using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class IceEffect : MonoBehaviour
{
    public float appearSpeed = 1;
    
    private new MeshRenderer renderer;
    private float appearance;
    private static readonly int SeedProperty = Shader.PropertyToID("_Seed");
    private static readonly int ScaleProperty = Shader.PropertyToID("_Scale");

    public float Appearance
    {
        get { return appearance; }
        set
        {
            appearance = value;
            Vector3 scale = Vector3.one * value;
            renderer.material.SetVector(ScaleProperty, scale);
        }
    }

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        FeedRandomSeed();
        StartCoroutine(AppearOverTime());
    }

    private IEnumerator AppearOverTime()
    {
        Appearance = 0;

        while (Appearance < 1)
        {
            Appearance += appearSpeed * Time.fixedDeltaTime;
            yield return null;
        }
        Appearance = 1f;
    }

    private void FeedRandomSeed()
    {
        renderer.material.SetVector(SeedProperty, new Vector2(Random.value, Random.value));
    }
}