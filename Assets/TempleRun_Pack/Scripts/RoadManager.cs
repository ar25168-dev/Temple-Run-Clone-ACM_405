using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;                 // Auto-found if null
    [SerializeField] private Transform firstSegment;           // Starting road piece in scene
    [SerializeField] private GameObject segmentPrefab;         // Road segment prefab

    [Header("Road Settings")]
    [SerializeField] private float segmentLength = 2f;         // Length along local X (firstSegment.right)
    [SerializeField] private bool autoDetectLength = false;    // Try to infer from bounds if true
    [SerializeField] private int keepAhead = 5;                // Keep this many pieces ahead

    private readonly List<Transform> segments = new List<Transform>();
    private Vector3 dir; // world-space direction of road (local +X of segment)

    // --------- Setup helpers ---------
    private void GetPlayer()
    {
        if (player != null) return;
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) player = go.transform;
        else Debug.LogError("RoadManager: No GameObject tagged 'Player' found!");
    }

    private void DetectSegmentLengthIfRequested()
    {
        if (!autoDetectLength || firstSegment == null) return;

        // Use renderer bounds projected on road direction
        var rend = firstSegment.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Approx length by projecting bounds size onto dir
            var size = rend.bounds.size;
            // Build a vector aligned with dir using bounds extents along world axes
            // Fast approximation: dot product of size with |dir|
            var absDir = new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z));
            segmentLength = Vector3.Dot(absDir, size);
        }
        else
        {
            Debug.LogWarning("RoadManager: autoDetectLength enabled but no Renderer found; using assigned segmentLength.");
        }
    }

    // --------- Unity lifecycle ---------
    private void Start()
    {
        GetPlayer();

        if (!firstSegment || !segmentPrefab || !player)
        {
            Debug.LogError("RoadManager: Assign Player (or tag 'Player'), First Segment, and Segment Prefab.");
            enabled = false; return;
        }

        dir = firstSegment.right.normalized; // road runs along local X
        DetectSegmentLengthIfRequested();

        segments.Clear();
        segments.Add(firstSegment);
        EnsureBuffer();
    }

    private void Update()
    {
        if (!player) return;
        EnsureBuffer();
        CullBehind();
    }

    // --------- Core logic ---------
    private void EnsureBuffer()
    {
        while (segments.Count <= keepAhead)
        {
            Transform last = segments[segments.Count - 1];
            Vector3 nextPos = last.position + dir * segmentLength;
            Quaternion nextRot = last.rotation;

            var seg = Instantiate(segmentPrefab, nextPos, nextRot, transform);
            segments.Add(seg.transform);
        }
    }

    private void CullBehind()
    {
        // Remove earliest segments we’ve clearly passed along the road direction
        while (segments.Count > 0)
        {
            Transform first = segments[0];
            float progress = Vector3.Dot(player.position - first.position, dir);

            if (progress > segmentLength * 1.5f)
            {
                Destroy(first.gameObject);
                segments.RemoveAt(0);
            }
            else break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!firstSegment) return;
        Vector3 d = firstSegment.right.normalized;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(firstSegment.position,
            firstSegment.position + d * segmentLength * (keepAhead + 1));
    }
#endif
}
