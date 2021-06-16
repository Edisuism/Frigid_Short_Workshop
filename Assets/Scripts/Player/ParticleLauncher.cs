using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleLauncher : MonoBehaviour
{
    [SerializeField]
    private float ammo = 100f;
    [SerializeField]
    private float ammoCap = 100f;
    public int particlesPerFrame;
    public Slider slider;
    public float spreadIncreaseSpeed;
    public float maxSpread;
    public float playerSafeDistance = 1f;
    public AudioClip[] audioClip;

    private float particlesStartAngle;
    private float currentSpread;
    [SerializeField]
    private float fillSpeed = 0.5f;
    private ParticleSystem ps;
    private AudioSource audioSource;
    
    public static ParticleLauncher Instance { get; private set; }
    public float Ammo
    {
        get { return ammo; }
        set { ammo = Mathf.Clamp(value, 0, ammoCap); }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        slider = LevelManager.Instance.GetComponentInChildren<Slider>();
        ps = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        particlesStartAngle = ps.shape.angle;
    }

    private void Update()
    {
        // Check if game is paused, i.e player winning or losing
        if (Time.timeScale > 0)
        {
            bool canLaunch = CanLaunch();

            if (slider.value < ammo / ammoCap)
            {
                slider.value += fillSpeed * Time.deltaTime;
            }

            if (slider.value > ammo / ammoCap)
            {
                slider.value -= fillSpeed * Time.deltaTime;
            }

            if (!canLaunch)
            {
                // Make jammed gun sound here.
            }

            if (Input.GetButton("Fire1") && canLaunch)
            {
                if (ammo > 0)
                {
                    audioSource.clip = audioClip[0];
                    audioSource.Play();
                    ps.Emit(particlesPerFrame);
                    SpreadParticlesOverTime();
                    ammo--;
                }
            }
            else
            {
                ResetSpread();
            }

            if (Input.GetButtonDown("Fire1") && canLaunch)
            {
                if (ammo == 0)
                {
                    audioSource.clip = audioClip[1];
                    audioSource.Play();
                }
            }

            if (Input.GetButtonDown("Fire2"))
            {
                IceStructureCreator creator = GetComponent<IceStructureCreator>();
                ps.Clear();

                if (creator.AllStructures.Count > 0)
                {
                    creator.DestroyIceStructures();
                    
                    // Play ice cracking sound effect
                    audioSource.clip = audioClip[2];
                    audioSource.Play();
                }
                ammo = ammoCap;
            }
        }
    }

    private bool CanLaunch()
    {
        Vector3 lookDirection = transform.forward.normalized;
        
        RaycastHit hit;
        int mask = LayerMask.GetMask("Structure");
        return !Physics.Raycast(transform.position, lookDirection, out hit, playerSafeDistance, mask);
    }

    private void SpreadParticlesOverTime()
    {
        if (currentSpread <= maxSpread)
        {
            currentSpread += spreadIncreaseSpeed * Time.deltaTime;
            var shape = ps.shape;
            shape.angle = particlesStartAngle + currentSpread;
        }
    }

    private void ResetSpread()
    {
        currentSpread = 0;
        var shape = ps.shape;
        shape.angle = particlesStartAngle;
    }
}
