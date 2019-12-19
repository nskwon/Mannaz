﻿using System.Collections;
using UnityEngine;

public class Guards : MonoBehaviour
{

    public int health = 100;
    public float speed = 4f;
    public int damage = 15;
    public float attackRate = 1f;
    public float attackRange = 1f;
    public float spawnRate = 4f;
    public int attackDelay = 0;
    public int pullback = 0;
    public int forward = 0;
    public int backward = 0;
    public bool attacking = true;

    private Transform target;
    public string enemyTag = "Enemy";
    Quaternion initialRot;
    Vector3 initialPos;
    public GameObject WizardImpactEffect;

    void Start()
    {
        initialRot = transform.rotation;
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        StartCoroutine(CoUpdate());
    }

    void UpdateTarget()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }

    }

    void Update()
    {

        if (target == null)
        {
            transform.rotation = initialRot;
            attacking = false;
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if ( pullback == 0 )
        {
            initialPos = transform.position;
        }

        // attacking "animation"
        if (pullback < 20)
        {
            transform.position -= transform.forward * Time.deltaTime;
            pullback++;
        }
        else if (forward < 9)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 30);
            forward++;
        }
        else if ( forward == 9 )
        {
            HitTarget();
            forward++;
        }
        else if (backward < 35)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPos, Time.deltaTime * 13);
            backward++;
        }
        else
        {

            pullback = 0;
            forward = 0;
            backward = 0;
        }

    }

    IEnumerator CoUpdate()
    {
        while (true)
        {

            if (target == null)
            {
                yield return null;
            }
            else
            {
                attacking = true;
            }

            if (health <= 0)
            {
                Destroy(gameObject);
                yield return null;
            }

            // check to move forward
            if (!attacking)
            {
                transform.position += transform.forward * Time.deltaTime * speed;
            }

            yield return null;

        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(WizardImpactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1.5f);
    }

}
