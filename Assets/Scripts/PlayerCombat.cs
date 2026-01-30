using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject weapon;
    [SerializeField] private float cooldown = 0.25f;

    private float lastShoot;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShootHandler()
    {
        if (Input.GetKey(KeyCode.Z) && Time.time > lastShoot + cooldown)
        {
            Shoot();
            lastShoot = Time.time;
        }
    }

    void Shoot()
    {
        animator.SetTrigger("shooting");

        Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;

        GameObject bulletObj = Instantiate(bullet, weapon.transform.position, Quaternion.identity);

        bulletObj.GetComponent<Bullet>().SetDirection(direction);
    }
}
