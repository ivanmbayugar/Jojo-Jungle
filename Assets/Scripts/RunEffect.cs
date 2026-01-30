using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunEffect : MonoBehaviour
{
    private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
    void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
