using UnityEngine;
using System.Collections.Generic;

public class GhostBoundary : MonoBehaviour
{
    [Header("Ghost Settings")]
    public float renderDistance = 350f;
    public int wrapLayers = 1;
    
    private Vector3 boxSize;
    private List<GhostData> ghosts = new List<GhostData>();

    private class GhostData
    {
        public GameObject obj;
        public Vector3Int offset;
    }

    void Start()
    {
        Boundary boundary = FindAnyObjectByType<Boundary>();
        if (boundary != null)
            boxSize = boundary.boxSize;
        else
            boxSize = new Vector3(400f, 400f, 400f);

        CreateGhosts();
    }

    void CreateGhosts()
    {
        Mesh asteroidMesh = null;
        Material asteroidMaterial = null;

        MeshFilter[] childMF = GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] childMR = GetComponentsInChildren<MeshRenderer>();
        
        foreach (var filter in childMF)
        {
            if (filter.sharedMesh != null)
            {
                asteroidMesh = filter.sharedMesh;
                break;
            }
        }
        
        foreach (var renderer in childMR)
        {
            if (renderer.sharedMaterial != null)
            {
                asteroidMaterial = renderer.sharedMaterial;
                break;
            }
        }

        for (int x = -wrapLayers; x <= wrapLayers; x++)
        {
            for (int y = -wrapLayers; y <= wrapLayers; y++)
            {
                for (int z = -wrapLayers; z <= wrapLayers; z++)
                {
                    if ( x == 0 && y == 0 && z == 0)
                        continue;

                    Vector3Int offset = new Vector3Int(x, y, z);
                    
                    GameObject ghost = new GameObject($"Ghost_{x}_{y}_{z}");
                    
                    MeshFilter ghostMF = ghost.AddComponent<MeshFilter>();
                    ghostMF.mesh = asteroidMesh;

                    MeshRenderer ghostMR = ghost.AddComponent<MeshRenderer>();
                    ghostMR.material = new Material(asteroidMaterial);
                    // ghostMR.material.color = Color.red;         // for testing ghosts

                    ghosts.Add(new GhostData { obj = ghost, offset = offset });
                }
            }
            
        }
    }

    void Update()
    {
        Vector3 halfSize = boxSize * 0.5f;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Vector3 scale = transform.localScale;

        Quaternion rotationOffset = Quaternion.Euler(270, 0, 0);

        // which boundary(s) is the asteroid close to
        bool nearRight = halfSize.x - pos.x < renderDistance;
        bool nearLeft = pos.x + halfSize.x < renderDistance;
        bool nearUp = halfSize.y - pos.y < renderDistance;
        bool nearDown = pos.y + halfSize.y < renderDistance;
        bool nearForward = halfSize.z - pos.z < renderDistance;
        bool nearBack = pos.z + halfSize.z < renderDistance;

        foreach (var ghost in ghosts)
        {
            ghost.obj.transform.rotation = rot * rotationOffset;
            ghost.obj.transform.localScale = scale;
            ghost.obj.transform.position = pos + Vector3.Scale(ghost.offset, boxSize);

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

            ghost.obj.SetActive(visible);
        }
    }

    void OnDestroy()
    {
        foreach (var ghost in ghosts)
            if (ghost.obj != null)
                Destroy(ghost.obj);
        ghosts.Clear();
    }
}