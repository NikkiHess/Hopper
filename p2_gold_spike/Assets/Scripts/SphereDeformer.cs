using UnityEngine;

public class SphereDeformer : MonoBehaviour
{
    [SerializeField] float deformationStrength = 1f; // how much to deform around the sphere

    MeshFilter meshFilter;
    Vector3[] originalVertices, deformedVertices;
    GameObject sphere; // sphere to wrap around

    private void Start()
    {
        // this should always be parented to a sphere, I can always change this later tho
        sphere = transform.parent.gameObject;

        // get the meshfilter to mess with
        meshFilter = GetComponent<MeshFilter>();

        // store original vertices
        originalVertices = meshFilter.mesh.vertices;
        deformedVertices = new Vector3[originalVertices.Length];
    }

    private void Update()
    {
        // deform around the sphere
        DeformPlane();
    }

    private void DeformPlane()
    {
        // get the center and radius of the sphere
        Vector3 sphereCenter = sphere.transform.position;
        float sphereRadius = sphere.transform.localScale.x * 0.5f; // assumes uniform scaling

        // loop over all vertices
        for (int i = 0; i < originalVertices.Length; i++)
        {
            // get original vertex position in world space
            Vector3 worldVertex = transform.TransformPoint(originalVertices[i]);

            // calculate direction of sphere center to vertex
            Vector3 direction = (worldVertex - sphereCenter).normalized;

            // move the vertex out to wrap around the sphere, using the sphere radius and deformation strength
            deformedVertices[i] = sphereCenter + direction * (sphereRadius + deformationStrength);
        }

        // update the mesh with deformed vertices
        meshFilter.mesh.vertices = deformedVertices;
        meshFilter.mesh.RecalculateNormals(); // recalculate the normals so the lighting updates correctly
        meshFilter.mesh.RecalculateBounds();
    }
}
