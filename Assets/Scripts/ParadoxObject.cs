using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ParadoxObject : MonoBehaviour
{
    public static List<ParadoxObject> All = new List<ParadoxObject>();
    public static bool SHIFTING = false;

    public Stack<TransformChange> History = new Stack<TransformChange>();

    private Coroutine mRoutineParadox;

    private Vector3 mPrevLocation = Vector3.zero;
    private Quaternion mPrevRotation = Quaternion.identity;
    private Vector3 mPrevScale = Vector3.zero;
    // Start is called before the first frame update
    private void Start()
    {
        All.Add(this);
        mPrevLocation = transform.position;
        mPrevRotation = transform.rotation;
        mPrevScale = transform.localScale;
        History.Push( new TransformChange(Vector3.zero, Quaternion.identity, Vector3.zero));

        //History.Add(new HistoryPoint(transform));
        //mRoutineParadox = StartCoroutine(DoRoutineHistoryPoint());
    }

    private void FixedUpdate()
    {
        Vector3 posDiff;
        Quaternion rotDiff;
        Vector3 scaleDiff;

        if (!SHIFTING)
        {
            posDiff = transform.position - mPrevLocation;
            rotDiff = mPrevRotation * Quaternion.Inverse(transform.rotation);
            scaleDiff = mPrevScale - transform.localScale;

            TransformChange change = new TransformChange(posDiff, rotDiff, scaleDiff);

            Debug.Log("Change: " + change);

            History.Push(change);
        }
        else
        {
            if (History.Count > 0)
            {
                TransformChange last = History.Last();
                transform.position += last.VelChange;
                transform.rotation = last.RotChange * transform.rotation;
                transform.localScale -= last.ScaleChange;
                Debug.Log("Last Shift: " + last);
                History.Pop();
            }
        }

        mPrevLocation = transform.position;
        mPrevRotation = transform.rotation;
        mPrevScale = transform.localScale;
    }

    IEnumerator DoRoutineHistoryPoint()
    {
        Vector3 posDiff;
        Quaternion rotDiff;
        Vector3 scaleDiff;

        Transform lastTransform = transform;
        while (true)
        {
            if(!SHIFTING)
            {
                posDiff = lastTransform.position - transform.position;
                rotDiff = lastTransform.rotation * Quaternion.Inverse(transform.rotation);
                scaleDiff = lastTransform.localScale - transform.localScale;

                History.Push(new TransformChange(posDiff,rotDiff,scaleDiff));

                lastTransform = transform;
                Debug.Log(transform);
                Debug.Log("Adding " + posDiff);
            }
            else
            {
                TransformChange last;
                if (History.Count > 0)
                {
                    last = History.Last();
                    transform.position -= last.VelChange;
                    transform.rotation = last.RotChange * transform.rotation;
                    transform.localScale -= last.ScaleChange;
                    Debug.Log("Moving " + last.VelChange);
                    History.Pop();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator DoParadoxShift()
    {
        TransformChange lastPoint;

        Vector3 currPos;
        Vector3 prevPos;

        Quaternion currRot;
        Quaternion prevRot;

        Vector3 currScale;
        Vector3 prevScale;

        bool shift = true;

        while (true)
        {
            if (History.Count > 0) lastPoint = History.Last();
            else break;

            currPos = transform.position;
            prevPos = lastPoint.VelChange;

            currRot = transform.rotation;
            prevRot = lastPoint.RotChange;

            currScale = transform.localScale;
            prevScale = lastPoint.ScaleChange;

            while (shift)
            {
                transform.position = Vector3.Lerp(currPos, prevPos, Time.deltaTime * 2);
                transform.rotation = Quaternion.Lerp(currRot, prevRot, Time.deltaTime * 2);
                transform.localScale = Vector3.Lerp(currScale, prevScale, Time.deltaTime * 2);
                shift = (currPos != prevPos && currRot != prevRot && currScale != prevScale);
                yield return null;
            }
            History.Pop();
        }
    }
}

[System.Serializable]
public struct TransformChange
{
    public Vector3 VelChange;
    public Quaternion RotChange;
    public Vector3 ScaleChange;

    public TransformChange(Vector3 pPosDiff, Quaternion pRotDiff, Vector3 pScaleDiff)
    {
        VelChange = pPosDiff;
        RotChange = pRotDiff;
        ScaleChange = pScaleDiff;
    }

    public override string ToString()
    {
        return string.Format("Pos Diff: {0} | Rot Diff: {1} | Scale Diff: {2}", VelChange, RotChange, ScaleChange);
    }
}