using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip explosionSound;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(explosionSound, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}
