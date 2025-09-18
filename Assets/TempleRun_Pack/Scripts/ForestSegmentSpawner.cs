using System.Collections.Generic;
using UnityEngine;

public class ForestSegmentSpawner : MonoBehaviour
{
    [Header("Prefabs to pick from")]
    [SerializeField] List<GameObject> prefabs = new();

    [Header("Spawn Count")]
    [SerializeField, Min(0)] int numberOfObjects = 20;
    [SerializeField, Min(1)] int maxObjects = 60;

    [Header("Area (leave empty to use this object's Renderer bounds)")]
    [SerializeField] Renderer areaRenderer;

    [Header("Edge Padding")]
    [Tooltip("World-units margin inside the plane on X/Z to keep clear.")]
    [SerializeField, Min(0f)] float edgePadding = 0.5f;

    [Header("Placement")]
    [SerializeField] LayerMask groundLayer = ~0;
    [SerializeField] float raycastHeight = 50f;
    [SerializeField] float maxRayDistance = 200f;
    [SerializeField] float yOffset = 0f;

    [Header("Avoid Player")]
    [SerializeField] bool avoidPlayer = true;
    [Tooltip("Assign your player Transform, or leave empty to auto-find tag 'Player'.")]
    [SerializeField] Transform player;
    [SerializeField, Min(0f)] float playerClearance = 2f;

    [Header("Parenting (optional)")]
    [Tooltip("Use an unscaled (1,1,1) empty here to be extra safe.")]
    [SerializeField] Transform spawnParent;

    bool _spawned;

    void Awake() => SpawnIfNeeded();
    void OnEnable() => SpawnIfNeeded();

    void OnValidate()
    {
        if (maxObjects < 1) maxObjects = 1;
        numberOfObjects = Mathf.Clamp(numberOfObjects, 0, maxObjects);
        if (raycastHeight < 0f) raycastHeight = 0f;
        if (maxRayDistance < 1f) maxRayDistance = 1f;
    }

    void SpawnIfNeeded()
    {
        if (_spawned) return;
        _spawned = true;

        if (!player && avoidPlayer)
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged) player = tagged.transform;
        }

        Spawn();
    }

    void Spawn()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning($"{name}: No prefabs assigned.");
            return;
        }

        var rend = areaRenderer ? areaRenderer : GetComponent<Renderer>();
        if (!rend)
        {
            Debug.LogError($"{name}: No Renderer found for spawn area. Assign 'areaRenderer' or place this on the plane.");
            return;
        }

        Bounds b = rend.bounds;

        // Compute padded rectangle (X/Z) inside the area
        float halfX = (b.size.x * 0.5f) - edgePadding;
        float halfZ = (b.size.z * 0.5f) - edgePadding;
        if (halfX <= 0f || halfZ <= 0f)
        {
            Debug.LogWarning($"{name}: edgePadding ({edgePadding}) too large for area; no spawn space.");
            return;
        }

        float minX = b.center.x - halfX;
        float maxX = b.center.x + halfX;
        float minZ = b.center.z - halfZ;
        float maxZ = b.center.z + halfZ;

        int targetCount = Mathf.Clamp(numberOfObjects, 0, maxObjects);
        Transform parent = spawnParent ? spawnParent : transform;

        int attempts = 0;
        int spawned = 0;
        int maxAttempts = targetCount * 6;

        while (spawned < targetCount && attempts < maxAttempts)
        {
            attempts++;

            // Random X/Z within padded rect, then raycast down to ground
            Vector3 rayStart = new Vector3(
                Random.Range(minX, maxX),
                b.max.y + raycastHeight,
                Random.Range(minZ, maxZ)
            );

            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, maxRayDistance, groundLayer))
                continue;

            Vector3 pos = hit.point;
            pos.y += yOffset;

            // Optional player clearance
            if (avoidPlayer && player)
            {
                Vector2 p2 = new Vector2(player.position.x, player.position.z);
                Vector2 h2 = new Vector2(pos.x, pos.z);
                if (Vector2.Distance(p2, h2) < playerClearance) continue;
            }

            var prefab = prefabs[Random.Range(0, prefabs.Count)];
            if (!prefab) continue;

            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // --- Stretch-proof spawn sequence ---
            // 1) Instantiate with no parent so it keeps prefab's world scale.
            GameObject go = Instantiate(prefab, pos, rot);

            // 2) Now parent it while preserving world transform, so parent scale won't stretch it.
            go.transform.SetParent(parent, true);

            // 3) Ensure localScale matches prefab's authoring scale (belt-and-braces).
            go.transform.localScale = prefab.transform.localScale;
            // ------------------------------------

            spawned++;
        }

        if (spawned < targetCount)
        {
            Debug.Log($"[{name}] Spawned {spawned}/{targetCount} (padding/player avoidance/ground checks limited placement).");
        }
    }
}
