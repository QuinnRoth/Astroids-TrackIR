using UnityEngine;
using System.Collections.Generic;

public class GhostBoundary : MonoBehaviour
{
    [Header("Ghost Settings")]
    public float renderDistance = 350f;
    public int wrapLayers = 1;
    
    private Boundary boundary;
    private Vector3 boxSize;
    private List<GhostData> ghosts = new List<GhostData>();

    private class GhostData
    {
        public GameObject root;
        public Vector3Int offset;
    }

    void Start()
    {
        boundary = GetComponent<Boundary>();
        if (boundary != null)
            boxSize = boundary.boxSize;
        else
            boxSize = new Vector3(600f, 600f, 600f);

        CreateGhosts();
    }

    void CreateGhosts()
    {
        for (int x = -wrapLayers; x <= wrapLayers; x++)
        {
            for (int y = -wrapLayers; y <= wrapLayers; y++)
            {
                for (int z = -wrapLayers; z <= wrapLayers; z++)
                {
                    if ( x == 0 && y == 0 && z == 0)
                        continue;

                    Vector3Int offset = new Vector3Int(x, y, z);
                    GameObject ghostRoot = new GameObject($"Ghost_{x}_{y}_{z}");
                    GhostData ghostData = new GhostData { root = ghostRoot, offset = offset };

                    CloneVisibleHierarchy(transform, ghostRoot.transform, ghostData);
                    ghosts.Add(ghostData);
                }
            } 
        }
    }

    private void CloneVisibleHierarchy(Transform source, Transform targetParent, GhostData ghostData)
    {
        foreach (Transform sourceChild in source)
        {
            GameObject targetChild = new GameObject(sourceChild.name);
            targetChild.transform.SetParent(targetParent, false);
            targetChild.transform.localPosition = sourceChild.localPosition;
            targetChild.transform.localRotation = sourceChild.localRotation;
            targetChild.transform.localScale = sourceChild.localScale;

            MeshFilter sourceMeshFilter = sourceChild.GetComponent<MeshFilter>();
            MeshRenderer sourceMeshRenderer = sourceChild.GetComponent<MeshRenderer>();
            if (sourceMeshFilter != null && sourceMeshFilter.sharedMesh != null &&
                sourceMeshRenderer != null && sourceMeshRenderer.sharedMaterial != null)
            {
                MeshFilter targetMeshFilter = targetChild.AddComponent<MeshFilter>();
                targetMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;

                MeshRenderer targetMeshRenderer = targetChild.AddComponent<MeshRenderer>();
                targetMeshRenderer.sharedMaterials = sourceMeshRenderer.sharedMaterials;

                ghostData.root.layer = sourceChild.gameObject.layer;
                targetChild.layer = sourceChild.gameObject.layer;
            }

            CloneVisibleHierarchy(sourceChild, targetChild.transform, ghostData);
        }
    }

    private bool IsGhostVisible(GhostData ghost)
    {
        Vector3 halfSize = boxSize * 0.5f;
        Vector3 pos = transform.position;

        // which boundary(s) is the asteroid close to
        bool nearRight = halfSize.x - pos.x < renderDistance;
        bool nearLeft = pos.x + halfSize.x < renderDistance;
        bool nearUp = halfSize.y - pos.y < renderDistance;
        bool nearDown = pos.y + halfSize.y < renderDistance;
        bool nearForward = halfSize.z - pos.z < renderDistance;
        bool nearBack = pos.z + halfSize.z < renderDistance;

        bool visible = true;

        if (ghost.offset.x > 0)
            visible &= nearRight;
        else if (ghost.offset.x < 0)
            visible &= nearLeft;

        if (ghost.offset.y > 0)
            visible &= nearUp;
        else if (ghost.offset.y < 0)
            visible &= nearDown;
    
        if (ghost.offset.z > 0)
            visible &= nearForward;
        else if (ghost.offset.z < 0)
            visible &= nearBack;

        return visible;
    }

    public Vector3 ClosestGhostToLaser(Camera cam, Vector2 screenPoint)
    {
        Vector3 bestPos = transform.position;
        float bestDistSq = float.MaxValue;
        float distToCamera = Vector3.Distance(transform.position, cam.transform.position);

        // If asteroid is extremely close to camera, just return main asteroid
        // (screen-space math is unreliable up close)
        if (distToCamera < 20f)
            return transform.position;

        Vector3 mainScreen = cam.WorldToScreenPoint(transform.position);
        if (mainScreen.z > 0f)
            bestDistSq = (new Vector2(mainScreen.x, mainScreen.y) - screenPoint).sqrMagnitude;

        foreach (var ghost in ghosts)
        {
            if (!IsGhostVisible(ghost))
                continue;

            Vector3 ghostCenter = transform.position + Vector3.Scale(ghost.offset, boxSize);
            Vector3 ghostScreen = cam.WorldToScreenPoint(ghostCenter);

            if (ghostScreen.z <= 0f)
                continue;   // behind camera

            float distSq = (new Vector2(ghostScreen.x, ghostScreen.y) - screenPoint).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                bestPos = ghostCenter;
            }
        }

        return bestPos;
    }

    void Update()
    {
        if (boundary != null)
            boxSize = boundary.boxSize;

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Vector3 scale = transform.localScale;

        foreach (var ghost in ghosts)
        {
            ghost.root.transform.rotation = rot;
            ghost.root.transform.localScale = scale;
            ghost.root.transform.position = pos + Vector3.Scale(ghost.offset, boxSize);

            bool visible = true;

            if (!IsGhostVisible(ghost))
                visible = false;

            ghost.root.SetActive(visible);
        }
    }

    void OnDestroy()
    {
        foreach (var ghost in ghosts)
            if (ghost.root != null)
                Destroy(ghost.root);
        ghosts.Clear();
    }
}