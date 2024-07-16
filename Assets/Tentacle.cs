using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public int length;
    public LineRenderer lineRend;
    public Vector3[] segmentPoses;
    private Vector3[] segmentV;

    public Transform targetDir;
    public float targetDist;
    public float maxDist;
    public float smoothSpeed;
    public float trailSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;



    private void Start()
    {
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
    }

    private void LateUpdate()
    {
        wiggleDir.localRotation = Quaternion.Euler(0f, 0f, (Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude) - Mathf.Sin(wiggleMagnitude) / 2);

        segmentPoses[0] = targetDir.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPosition = segmentPoses[i - 1] + targetDir.right * targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPosition, ref segmentV[i], smoothSpeed + i / trailSpeed);

            // Check distance and clamp to maxDist
            float distance = Vector3.Distance(segmentPoses[i], segmentPoses[i - 1]);
            if (distance > maxDist)
            {
                segmentPoses[i] = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * maxDist;
            }
        }

        lineRend.SetPositions(segmentPoses);
    }
}
