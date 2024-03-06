using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void ExecuteOnTriggEnter(GameObject collision) { }
    
    protected virtual void ExecuteOnTriggLeave(GameObject collision) { }
    protected virtual void ExecuteOnTriggLeave() { }
    protected virtual void ExecuteOnTriggEnter() { }
    
    protected void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.tag.Equals("Player")) return;
        ExecuteOnTriggEnter(collision.gameObject);
        ExecuteOnTriggEnter();
    }
    
    protected void OnTriggerExit(Collider collision)
    {
        if (!collision.gameObject.tag.Equals("Player")) return;
        ExecuteOnTriggLeave(collision.gameObject);
        ExecuteOnTriggLeave();
    }
}
