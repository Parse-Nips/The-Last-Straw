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

    [SerializeField] 
    private bool m_Debug;
    
    private Coroutine mRoutineParadox;

    private Vector3 mPrevLocation = Vector3.zero;
    private Quaternion mPrevRotation = Quaternion.identity;
    private Vector3 mPrevScale = Vector3.zero;
    
    private void Start()
    {
        All.Add(this);
        InitializeOriginPoint();
    }
    
    private void FixedUpdate()
    {
        if (!SHIFTING)
        {
            RecordTransformChange();
        }
        else
        {
            PerformParadoxShift();
        }

        var t = transform;
        mPrevLocation = t.position;
        mPrevRotation = t.rotation;
        mPrevScale = t.localScale;
    }

    private void InitializeOriginPoint()
    {
        var t = transform;
        mPrevLocation = t.position;
        mPrevRotation = t.rotation;
        mPrevScale = t.localScale;
        History.Push( new TransformChange(Vector3.zero, Quaternion.identity, Vector3.zero));
    }

    private void RecordTransformChange()
    {
        var posDiff = transform.position - mPrevLocation;
        var rotDiff = mPrevRotation * Quaternion.Inverse(transform.rotation);
        var scaleDiff = mPrevScale - transform.localScale;

        var change = new TransformChange(posDiff, rotDiff, scaleDiff);

        if (m_Debug) Debug.Log("Change: " + change);

        History.Push(change);
    }

    private void PerformParadoxShift()
    {
        if (History.Count <= 0)
        {
            return;
        }
        
        var last = History.Last();
        var t = transform;
        t.position += last.PosChange;
        t.rotation = last.RotChange * t.rotation;
        t.localScale -= last.ScaleChange;
        if (m_Debug) Debug.Log("Last Shift: " + last);
        History.Pop();
    }

    private IEnumerator DoRoutineHistoryPoint()
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
                if (m_Debug)
                {
                    Debug.Log(transform);
                    Debug.Log("Adding " + posDiff);
                }
            }
            else
            {
                TransformChange last;
                if (History.Count > 0)
                {
                    last = History.Last();
                    transform.position -= last.PosChange;
                    transform.rotation = last.RotChange * transform.rotation;
                    transform.localScale -= last.ScaleChange;
                    if (m_Debug) Debug.Log("Moving " + last.PosChange);
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
            prevPos = lastPoint.PosChange;

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
    
    [System.Serializable]
    public struct TransformChange
    {
        public Vector3 PosChange;
        public Quaternion RotChange;
        public Vector3 ScaleChange;

        public TransformChange(Vector3 pPosDiff, Quaternion pRotDiff, Vector3 pScaleDiff)
        {
            PosChange = pPosDiff;
            RotChange = pRotDiff;
            ScaleChange = pScaleDiff;
        }

        public override string ToString()
        {
            return $"Pos Diff: {PosChange} | Rot Diff: {RotChange} | Scale Diff: {ScaleChange}";
        }
    }
}

