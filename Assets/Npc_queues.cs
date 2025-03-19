using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Npc_queues : MonoBehaviour
{
    public List<npc_wise> npc_queue = new List<npc_wise>();
    public Transform target1;
    public Transform target2;
    // Start is called before the first frame update
    void Start()
    {
        npc_queue.Add(transform.GetChild(0).GetComponent<npc_wise>());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateChild()
    {
        npc_queue = new List<npc_wise>(transform.GetComponentsInChildren<npc_wise>());

        Transform randomTarget = Random.Range(0, 2) == 0 ? target1 : target2;
        npc_queue.Last().target = randomTarget;
    }
}
