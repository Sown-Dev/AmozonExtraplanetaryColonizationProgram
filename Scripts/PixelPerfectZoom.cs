using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class PixelPerfectCameraZoom : MonoBehaviour{
    public PixelPerfectCamera ppCam;


    public float aspectRatio = 16f / 9f; // Common aspect ratio (16:9)

    int baseX;
    int baseY;
    private int targetX;


    private const int LERP_RATE = 24;

    private void Awake(){
        baseX = ppCam.refResolutionX;
        baseY = ppCam.refResolutionY;
        zoomLevel = 1;
        targetX = baseX;
    }

    private float prevZoom;

    public float zoomLevel = 1; //log 2 scale
    public int maxZoom = 2;
    public int minZoom = 1;


    private void FixedUpdate(){
       transform.localPosition= Vector3Int.RoundToInt( transform.localPosition/16)*16;

    }

    void Update(){
        if(Time.timeScale <= 0) return;
        
        
        
        if (Input.mouseScrollDelta.y != 0){
            // Determine the direction to zoom based on mouse scroll direction
            bool zoomIn = Input.mouseScrollDelta.y < 0;

            // Calculate the new reference resolution
            if (zoomIn){
                zoomLevel++;
            }
            else{
                zoomLevel--;
            }

            zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
            if (zoomLevel != prevZoom){
                prevZoom = zoomLevel;
                targetX = Mathf.RoundToInt(baseX * Mathf.Pow(2, zoomLevel));
            }
        }
        //don't lerp if values are close, instead just reset to avoid rounding errors

        if (Mathf.RoundToInt(Mathf.Lerp(ppCam.refResolutionX, targetX, Time.unscaledDeltaTime * LERP_RATE) / 2) * 2 == ppCam.refResolutionX){
            ppCam.refResolutionX = targetX;
        }
        else
            ppCam.refResolutionX =
                Mathf.RoundToInt(Mathf.Lerp(ppCam.refResolutionX, targetX, Time.unscaledDeltaTime * LERP_RATE) / 2) * 2;


        ppCam.refResolutionY = Mathf.RoundToInt((ppCam.refResolutionX / aspectRatio)/2)*2;
    }
}