﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Trailer_1 : MonoBehaviour
{
    [Header("Transitions timer")]
    public float timeBefore2ndLeak = 1;
    public float timeBefore1stZoomOut = 1;
    public float timeBefore3rdLeak = 1;
    public float timeBefore2ndZoomOut = 1;
    public float timeBeforeSS = 1;
    public float timeBefore3rdZoomOut = 1;
    public float timeBeforeZoomIn = 2;
    [Header("Trailer camera")]
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;
    [Header("First leak")]
    public LeakZone leakZone1;
    public List<LeakZone> pipeLeakZones1;
    public LeverScript associatedLever1;
    public int associatedPipe1;
    [Header("Second leak")]
    public LeakZone leakZone2;
    public List<LeakZone> pipeLeakZones2;
    public LeverScript associatedLever2;
    public int associatedPipe2;
    [Header("Third leak")]
    public LeakZone leakZone3;
    public List<LeakZone> pipeLeakZones3;
    public LeverScript associatedLever3;
    public int associatedPipe3;
    [Header("Secondary systems")]
    public SecondarySystem ss1;
    public SecondarySystem ss2;

    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Debug.started += ctx => StartCoroutine(LaunchCrisis());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LeaksManager.instance.StartSpecificLeak(leakZone1, pipeLeakZones1, associatedLever1, associatedPipe1);
            CharacterController2D.instance.animator.SetTrigger("Choc");
        }
    }

    IEnumerator LaunchCrisis()
    {
        CharacterController2D.instance.animator.SetTrigger("Sigh");
        yield return new WaitForSeconds(timeBefore2ndLeak);
        LeaksManager.instance.StartSpecificLeak(leakZone2, pipeLeakZones2, associatedLever2, associatedPipe2);
        CharacterController2D.instance.animator.SetTrigger("Choc");
        yield return new WaitForSeconds(timeBefore1stZoomOut);
        cam1.SetActive(true);
        CameraManager.instance.VCamZoom.GetComponent<Cinemachine.CinemachineVirtualCamera>().enabled = false;
        yield return new WaitForSeconds(timeBefore3rdLeak);
        LeaksManager.instance.StartSpecificLeak(leakZone3, pipeLeakZones3, associatedLever3, associatedPipe3);
        CharacterController2D.instance.animator.SetTrigger("Choc");
        yield return new WaitForSeconds(timeBefore2ndZoomOut);
        cam2.SetActive(true);
        cam1.SetActive(false);
        yield return new WaitForSeconds(timeBeforeSS);
        SecondarySystemsManager.instance.LaunchSpecificSS(ss1, null, LeverScript.RessourcesType.energy, false);
        SecondarySystemsManager.instance.LaunchSpecificSS(ss2, null, LeverScript.RessourcesType.oxygen, false);
        CharacterController2D.instance.animator.SetTrigger("Choc");
        yield return new WaitForSeconds(timeBefore3rdZoomOut);
        cam3.SetActive(true);
        cam2.SetActive(false);
        CharacterController2D.instance.animator.SetTrigger("Stress");
        yield return new WaitForSeconds(timeBeforeZoomIn);
        CameraManager.instance.VCamZoom.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 2;
        CameraManager.instance.VCamZoom.GetComponent<Cinemachine.CinemachineVirtualCamera>().enabled = true;
        cam1.SetActive(false);
        foreach (SecondarySystem item in HintSecondarySystemManager.instance.activeSecondarySystems)
            Destroy(item.associatedHint);
        HintSecondarySystemManager.instance.enabled = false;
        foreach (Leak item in HintLeakManager.instance.activesLeaks)
            Destroy(item.associatedHint);
        HintLeakManager.instance.enabled = false;
    }
}
