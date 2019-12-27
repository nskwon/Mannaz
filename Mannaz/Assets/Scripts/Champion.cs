﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Champion : MonoBehaviour
{
    public Camera mainCamera;
    public NavMeshAgent agent;
    private Vector3 recallPosition;
    public Transform track;
    private Transform cachedTransform;
    private Vector3 cachedPosition;
    public int recallCounter = 0;
   
    public GameObject recallVFXPrefab;

    public GameObject endRecallVFXPrefab;

    public GameObject idleVFXPrefab;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        cachedTransform = GetComponent<Transform>();
        if (track)
        {
            cachedPosition = track.position;
        }
        recallPosition = cachedPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (track && cachedPosition != track.position)
        {
            cachedPosition = track.position;
            transform.position = cachedPosition;
        }

        if (Input.GetKeyUp(KeyCode.B) && recallCounter == 0)
        {
            agent.SetDestination(cachedPosition);
            recallCounter++;
            StartCoroutine(recallCoroutine());
            StartCoroutine(homeCoroutine());
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

    }

    IEnumerator homeCoroutine()
    {

        const float waitTime = 8f;
        float counter = 0f;

        while (counter < waitTime)
        {

            if (Input.GetMouseButtonDown(1))
            {
                recallCounter--;
                yield break;
            }

            counter += Time.deltaTime;
            yield return null; //Don't freeze Unity
        }
        recallCounter--;
        agent.SetDestination(recallPosition);
    }

    IEnumerator recallCoroutine()
    {
        const float waitTime = 8f;
        float counter = 0f;

        GameObject recallEffect = Instantiate(recallVFXPrefab, transform.position, transform.rotation);
        animator.SetTrigger("Recall");


        while (counter < waitTime)
        {

            if (Input.GetMouseButtonDown(1))
            {
                Destroy(recallEffect.gameObject);
                recallCounter--;
                yield break;
            }

            counter += Time.deltaTime;
            yield return null; //Don't freeze Unity
        }
        transform.position = recallPosition;
        animator.SetTrigger("EndRecall");
        Instantiate(endRecallVFXPrefab, transform.position, transform.rotation);
        recallCounter--;

    }

}
