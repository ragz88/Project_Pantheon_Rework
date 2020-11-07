using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PolygonCollider2D))]
public class Sunbeams : MonoBehaviour
{
    public LayerMask collisionLayers;

    public int numRayChecks = 10;

    public Transform startingPointA;
    public Transform startingPointB;

    public Transform[] shinePoints;

    public bool visualiseMesh = false;

    private int currentShinePoint = 0;


    public  Vector2[] meshPoints;


    Mesh sunbeamMesh;                                           // Will reneder our beam on screen

    PolygonCollider2D beamCollider;

    public Material meshMaterial;

    MeshRenderer rend;

    public Transform midPointTrans;

    // Start is called before the first frame update
    void Start()
    {
        // We'll have a vertex in our mesh for every collision point from our ray checks, as well as for the 2 starting points
        meshPoints = new Vector2[numRayChecks + 2];

        // The first and last verteces of the mesh array will always be our two starting points. The raycast collisons points will 
        // fill in the middle positions as the beam moves around.
        meshPoints[0] = startingPointA.localPosition;
        meshPoints[meshPoints.Length - 1] = startingPointB.localPosition;

        // We want a reference to the PolyGonCollider on this object
        beamCollider = GetComponent<PolygonCollider2D>();



        sunbeamMesh = new Mesh();
        rend = GetComponent<MeshRenderer>();


        // Update polygon colour to a mix of primary colours based on the three category levels
        //polygonMat.color = new Color((offenseLevel / categoryMax), (mobilityLevel / categoryMax), (defenseLevel / categoryMax), 0.65f);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMeshPoints();
        ConstructMesh();
        ConstructCollider();
    }


    /// <summary>
    /// Calls a raycasting function numRayChecks times, finding all the colliders the sunbeam will be touching.
    /// Then combines these with the starting points of the beam to form an array of Vector 3s, which we can use to 
    /// construct our sunbeam mesh.
    /// </summary>
    private void CalculateMeshPoints()
    {
        // We want to fire numRayChecks rays from standardised intervals between our two starting points, 
        // checking what the beam would be colliding with in those parallel paths.
        for (int i = 0; i < numRayChecks; i++)
        {
            // Note: we subtract 1 from numRayChecks when looking for our lerped point to include both the start and end points in the 
            //       calculation, instead of just one of the two, while keeping our for loop in it's standard format.
            Vector3 currentStartingPoint = Vector3.Lerp(startingPointA.position, startingPointB.position, (i / ((float)numRayChecks - 1)));
            meshPoints[i + 1] = transform.InverseTransformPoint(FindEndPoint(currentStartingPoint));
        }
    }


    /// <summary>
    /// This function calculates the angle our sunbeam is currently pointing in, then fires off a raycast from the given start point in that angle.
    /// If it hits something, it returns the point of collision - if it does not, it will return (-500, -500, -500)
    /// That's not perfect, but I can't force it to return null.
    /// </summary>
    /// <param name="startPoint">The point the ray should start from. Direction calculated from StartingPointA and the current ShinePoint.</param>
    /// <returns>The point of collision between the ray and the first eligible object it encounters.</returns>
    private Vector3 FindEndPoint(Vector3 startPoint)
    {
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(startPoint, (shinePoints[currentShinePoint].position - startingPointA.position),
                out hit, Mathf.Infinity, collisionLayers))
        {
            Debug.DrawRay(startPoint, (shinePoints[currentShinePoint].position - startingPointA.position), Color.cyan);
            return hit.point;
        }
        else
        {
            //Debug.DrawRay(startPoint, (shinePoints[currentShinePoint].position - startingPointA.position) * 1000, Color.white);
            return new Vector3 (-500, -500, -500);
        }
    }

    private void ConstructCollider()
    {
        beamCollider.points = meshPoints;
    }

    private void ConstructMesh()
    {
        // Some Explanaition
        // The number of triangles in our mesh will always be equal to the number of ray points we work with. Please see attached image.
        // Thus, the number of vertices in our triangles array needs to be 3 times numRayChecks
        
        
        
        if (visualiseMesh)
        {
            Vector3[] verts = new Vector3[meshPoints.Length];
            
            for (int i =0; i < meshPoints.Length; i++)
            {
                verts[i] = new Vector3(meshPoints[i].x, meshPoints[i].y, 0);
            }

            sunbeamMesh.vertices = verts;

            int[] triangles = new int[3 * numRayChecks];

            int currentTriangleIndex = 3;

            triangles[0] = 0;
            triangles[1] = meshPoints.Length - 2;
            triangles[2] = meshPoints.Length - 1;

            for (int i = 2; i < meshPoints.Length - 1; i++)
            {
                triangles[currentTriangleIndex] = 0;
                currentTriangleIndex++;

                triangles[currentTriangleIndex] = i-1;
                currentTriangleIndex++;

                triangles[currentTriangleIndex] = i;
                currentTriangleIndex++;
            }

            sunbeamMesh.triangles = triangles;          // Tell Unity which indeces in the vertex array make up triangles (we only have 1 triangle).

            //GetComponent<MeshRenderer>().material = polygonMat;     // Here we give it a material so we can customise the colour and texture
            GetComponent<MeshFilter>().mesh = sunbeamMesh;          // And here we finally assign it to the mesh filter component - telling Unity to draw it in the scene.
            rend.material = meshMaterial;
        }
    }


    /// <summary>
    /// Moves the sunbeam to the next shine point in the shinePoints array. If we reach the final shinePoint, the next increment will loop back to the
    /// first shinePoint in the array.
    /// </summary>
    public void NextShinePoint()
    {
        // Modulous function (%) creates a simple way to cycle back to the beginning of the array once we've reached it's end
        currentShinePoint = (currentShinePoint + 1) % shinePoints.Length;
    }

    
}
