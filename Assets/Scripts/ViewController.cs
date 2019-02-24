using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [Header("View configurations")]
    [Range(0, 360)]
    [SerializeField] float viewAngle;
    [SerializeField] float viewDistance;

    [Header("Mesh configurations")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Transform meshTransform;
    [SerializeField] int meshPartitionsCount;
    [SerializeField] int meshEdgeResolution;
    [SerializeField] bool debugRay;
    private Mesh mesh;

    int objectsLayerMask = 1 << 9;


    public float GetViewDistance() {
        return viewDistance;
    }

    public float GetViewAngle(){
        return viewAngle;
    }

    public Vector3 AngleToDir(float angle) {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    private struct RayInfo {
        public Vector3 rayDirection;
        public float rayAngle;
        public float rayLenght;
        public Collider collider;

        public RayInfo(Vector3 _rayDirection, float _rayAngle, float _rayLenght, Collider _collider) {
            rayDirection = _rayDirection;
            rayAngle = _rayAngle;
            rayLenght = _rayLenght;
            collider = _collider;
        }
    };

    // Update is called once per frame
    void LateUpdate() {
        float startingAngle = transform.eulerAngles.y - viewAngle / 2;
        float angleIncrement = viewAngle / meshPartitionsCount;
        List<RayInfo> rays = new List<RayInfo>();
        Collider prevCollider = null;

        for (int i = 0; i < meshPartitionsCount + 1; ++i) {
            Vector3 rayDirection = AngleToDir(startingAngle + i * angleIncrement);
            float rayLenght = viewDistance;


            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, viewDistance, objectsLayerMask)) {
                rayLenght = hit.distance;
            }

            RayInfo ray = new RayInfo(rayDirection, startingAngle + i * angleIncrement, rayLenght, hit.collider);


            bool hitOneObstacle = prevCollider == null && hit.collider != null || prevCollider != null && hit.collider == null;
            bool hitTwoObstacles = prevCollider != hit.collider && prevCollider != null && hit.collider != null;
            if (i > 0 && meshEdgeResolution > 0 && (hitOneObstacle || hitTwoObstacles)) {
                KeyValuePair<RayInfo, RayInfo> twoRays = FindCloseEdgeRays(rays[i - 1], ray);
                rays.Add(twoRays.Key);
                rays.Add(twoRays.Value);
            }

            rays.Add(ray);
            prevCollider = hit.collider;
            
        }

        if (debugRay) {
            foreach (RayInfo ri in rays){
                Debug.DrawRay(transform.position, ri.rayDirection * ri.rayLenght, Color.red);
            }
        }
        
        GenerateMesh(rays);
        //meshTransform.localRotation = transform.localRotation;
    }

    KeyValuePair<RayInfo, RayInfo> FindCloseEdgeRays(RayInfo min, RayInfo max) {
        for (int j = 0; j < meshEdgeResolution; ++j) {
            float newAngle = (min.rayAngle + max.rayAngle) / 2;
            Vector3 newRayDir = AngleToDir(newAngle);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, newRayDir, out hit, viewDistance, objectsLayerMask)) {
                if (min.collider != null && min.collider == hit.collider) {
                    min = new RayInfo(newRayDir, newAngle, hit.distance, hit.collider);
                }

                if (min.collider == null || min.collider != null && min.collider != hit.collider) {
                    max = new RayInfo(newRayDir, newAngle, hit.distance, hit.collider);
                }

            } else {
                if (min.collider != null) {
                    max = new RayInfo(newRayDir, newAngle, viewDistance, null);
                } else {
                    min = new RayInfo(newRayDir, newAngle, viewDistance, null);
                }

            }
        }

        return new KeyValuePair<RayInfo, RayInfo>(min, max);
    }

    void GenerateMesh(List<RayInfo> rays) {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(Vector3.zero);
        foreach (RayInfo ri in rays) {
            float globalAngle = ri.rayAngle - transform.eulerAngles.y; ;
            Vector3 globalDir = AngleToDir(globalAngle);
            vertices.Add(globalDir * ri.rayLenght);
        }

        mesh.SetVertices(vertices);

        List<int> triangles = new List<int>();

        for (int i = 1; i < vertices.Count - 1; ++i) {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();
    }
}
