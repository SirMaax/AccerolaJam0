using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicSlime : Obstacle
{
    [SerializeField] private float timeToBecomeActive;
    [SerializeField] private float scaleFactor = 1.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = transform.position;
        pos.x = Random.Range(pos.x - 0.2f, pos.x + 0.2f);
        pos.y = Random.Range(pos.y - 0.2f, pos.y + 0.2f);
        pos.z = Random.Range(pos.z - 0.2f, pos.z + 0.2f);
        timeToBecomeActive = Random.Range(timeToBecomeActive - 0.5f, timeToBecomeActive + 1);
        StartCoroutine(ActivationTimer());
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    IEnumerator ActivationTimer()
    {
        float time = 0;
        while (time <= timeToBecomeActive)
        {
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
            Vector3 scale = transform.localScale;
            scale.x *= scaleFactor;
            scale.z *= scaleFactor;
            // scale.y *= scaleFactor / 2;
            scale.y *= 1.01f;
            transform.localScale = scale;
        }
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }
     
    protected override void ExecuteOnTriggEnter(GameObject collision)
    {
        Debug.LogError("Health not active");
        // collision.gameObject.transform.parent.GetComponentInChildren<Health>().TakeDamage();
    }
}
