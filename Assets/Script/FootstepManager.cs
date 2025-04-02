using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : NetworkBehaviour
{
    public static List<Footstep> deactivated_footsteps = new List<Footstep>();

    [SerializeField] int footstep_interval;
    [SerializeField] int footstep_lifetime;
    int footstep_time;
    GameObject footstep_holder;

    // variables for checking if the character is walking
    bool is_walking;
    Vector3 last_position;
    Vector3 movement_direction;

    [SerializeField] GameObject footstep_prefab;
    GameObject new_footstep;
    float adj = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        footstep_time = footstep_interval;
        last_position = transform.position;
        footstep_holder = GameObject.Find("Footsteps");
    }

    private void Update()
    {
        is_walking = (last_position != transform.position);
        movement_direction = (transform.position - last_position).normalized;
        last_position = transform.position;
        
        Debug.Log(is_walking);

        if (is_walking)
        {
            FootstepCreationCheck();
        }
    }

    GameObject NewFootstep()
    {
        GameObject footstep;

        //footstep = Instantiate(footstep_prefab, transform.position, Quaternion.identity, footstep_holder.transform);

        //                                        TO DO- footstep opitmization
        if (deactivated_footsteps.Count == 0)
        {
            footstep = Instantiate(footstep_prefab, transform.position, Quaternion.identity, footstep_holder.transform);
        }
        else
        {
            deactivated_footsteps[0].gameObject.SetActive(true);
            footstep = deactivated_footsteps[0].gameObject;
            deactivated_footsteps.Remove(deactivated_footsteps[0]);
        }

        return footstep;
    }
    public void FootstepCreationCheck()
    {
        footstep_time--;
        if (footstep_time < 1)
        {
            new_footstep = NewFootstep();
            if (new_footstep.TryGetComponent(out Footstep footstep))
            {
                footstep.transform.position = transform.position;
                footstep.left_or_right = adj;
                footstep.lifetime = footstep_lifetime;
                footstep.direction = movement_direction;
                footstep.Initialize();
            }
           
            footstep_time = footstep_interval;
            adj = adj * -1;
        }
    }   
}
