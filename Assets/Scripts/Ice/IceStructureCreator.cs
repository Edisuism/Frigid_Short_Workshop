using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class IceStructureCreator : MonoBehaviour
{
    public GameObject iceStructure;
    [SerializeField, InspectorName("Structure Instances")]
    private int structureInstancesCount;
    private Queue<GameObject> unusedStructures;
    private List<GameObject> allStructures;
    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public IReadOnlyCollection<GameObject> AllStructures => allStructures.AsReadOnly();

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        allStructures = new List<GameObject>();
        unusedStructures = new Queue<GameObject>(structureInstancesCount);
        InitializeStartingObjects();
    }

    private void InitializeStartingObjects()
    {
        for (int i = 0; i < structureInstancesCount; i++)
        {
            GameObject structureObject = InstantiateIceStructure(Vector3.zero, Quaternion.identity);
            structureObject.SetActive(false);
            unusedStructures.Append(structureObject);
        }
    }

    private void FixedUpdate()
    {
        // if (Input.GetButton("Fire1"))
        // {
        //     int mask = ~(1 << 8);
        //     RaycastHit hit;
        //     
        //     if (Physics.Raycast(transform.position, transform.forward, out hit, 1000,  mask))
        //     {
        //         Debug.DrawLine(transform.position, hit.point, Color.blue, 2);
        //         
        //         if (hit.collider.CompareTag("Structure"))
        //         {
        //             IceStructureModifer modifer = hit.collider.GetComponent<IceStructureModifer>();
        //             modifer.ExtendTo(hit.point, transform.position);
        //         }
        //         else
        //         {
        //             GameObject structure = Instantiate(iceStructure, hit.point, Quaternion.identity);
        //         }
        //     }
        // }
    }

    private void OnParticleCollision(GameObject other)
    {
        int collisionCount = ps.GetCollisionEvents(other, collisionEvents);
        
        for (int i = 0; i < collisionCount; i++)
        {
            ParticleCollisionEvent collisionEvent = collisionEvents[i];
            
            if (!collisionEvent.colliderComponent.CompareTag("Hot"))
            {
                CreateIceStructure(collisionEvent);
            }
        }
    }

    private void CreateIceStructure(ParticleCollisionEvent collisionEvent)
    {
        Vector3 position = collisionEvent.intersection;
        GameObject structureObject;
        
        if (unusedStructures.Count > 0)
        {
            structureObject = unusedStructures.Dequeue();
            structureObject.SetActive(true);
            structureObject.transform.position = position;
        }
        else
        {
            structureObject = InstantiateIceStructure(position, Quaternion.identity);
        }
        
        // Finally parent the object.
        structureObject.transform.parent = collisionEvent.colliderComponent.transform;
        
        // Change the scale to make sure any weird deformation doesn't happen. This should be investigated some day.
        structureObject.transform.localScale = iceStructure.transform.localScale;

        // Make the ice structure orient itself towards the player.
        Vector3 lookDirection = transform.position - position;
        structureObject.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        allStructures.Add(structureObject);
    }

    private GameObject InstantiateIceStructure(Vector3 position, Quaternion rotation)
    {
        return Instantiate(iceStructure, position, rotation);
    }

    public void DestroyIceStructures()
    {
        foreach (GameObject structureObject in allStructures)
        {
            structureObject.GetComponent<IceStructureBreak>()?.Break();
            DisableStructureObject(structureObject);
        }
        allStructures.Clear();
    }

    private void DisableStructureObject(GameObject structureObject)
    {
        structureObject.transform.parent = LevelManager.Instance.transform;
        structureObject.SetActive(false);
        
        structureObject.transform.rotation = Quaternion.identity;
        unusedStructures.Enqueue(structureObject);
    }
}