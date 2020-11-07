using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class Sunbeams : MonoBehaviour
{
    /// <summary>
    /// Objects on these layers will block the sunbeam, creating a shadow.
    /// </summary>
    public LayerMask collisionLayers;

    /// <summary>
    /// The number of raycasts the beam uses to construct it's mesh and collider. More results in a more accurate and pretty-looking beam, at the 
    /// potential cost of performance.
    /// </summary>
    public int numRayChecks = 10;


    /// <summary>
    /// Transform at the top left corner of the beam.
    /// </summary>
    public Transform startingPointA;

    /// <summary>
    /// Transform at the top right corner of the beam.
    /// </summary>
    public Transform startingPointB;

    /// <summary>
    /// The collection of points our sunbeam should move between each time it's OnActiveBeat funtion is called.
    /// They should be assigned in cyclical order.
    /// </summary>
    public Transform[] shinePoints;


    /// <summary>
    /// Set this to true if you want to represent the sunBeam in game through a dynamic mesh and an assigned material/shader.
    /// Should be false if we're representing the beam using Unity Lighting.
    /// </summary>
    public bool visualiseMesh = false;


    /// <summary>
    /// The current index of the position our sunBeam should shine towards within our shinePoints array.
    /// </summary>
    private int currentShinePointIndex = 0;

    /// <summary>
    /// This value will always be lerped towards the currentShinePointIndex point stored in our shinePoints array.
    /// The beam will shine at this moving point, allowing the sun shine to move naturally from one position to the next.
    /// </summary>
    private Vector3 currentShinePosition;


    /// <summary>
    /// The speed at which the sunbeam moves from one point to another.
    /// </summary>
    public float beamMovementSpeed = 2f;


    /// <summary>
    /// Set to true each time the beam is told to move, then back to false once it's reached a very close proximity to the new shine position.
    /// Reduces the number of lerp calculations.
    /// </summary>
    private bool beamPositionLerping = false;


    /// <summary>
    /// How close the sunbeam ultimately gets to the current shine position when lerping. 
    /// </summary>
    private const float beamLerpAccuracy = 0.15f; 


    /// <summary>
    /// Stores all the vertices along the edges of our sunbeam.
    /// </summary>
    private  Vector2[] meshPoints;


    /// <summary>
    /// Our customisable mesh, used to render the sunbeam on screen
    /// </summary>
    private Mesh sunbeamMesh;


    /// <summary>
    /// We need this component to construct our collider around the sunbeam's edge
    /// </summary>
    private PolygonCollider2D beamCollider;

    /// <summary>
    /// This is used to tell Unity which mesh to draw. We'll be assigning our custom mesh into this.
    /// </summary>
    private MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        // We'll have a vertex in our mesh for every collision point from our ray checks and every starting point for our Raychecks
        // Hence, we'll have 2 verteces for every ray check
        meshPoints = new Vector2[2 * numRayChecks];

        // We want a reference to the PolyGonCollider on this object to create the collider around the edge of the beam
        beamCollider = GetComponent<PolygonCollider2D>();


        // These will be used to visualise the beam on screen
        sunbeamMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();

        // Finally, we tell the beam to point at the first shine point in the shinePoints array
        currentShinePosition = shinePoints[currentShinePointIndex].position;
    }

    // Update is called once per frame
    void Update()
    {
        // When our shinePointIndex has changed, our currentShinePosition must erp to a new transform. This bool is true when that process is still incomplete
        if (beamPositionLerping)
        {
            // Lerp towards new point
            currentShinePosition = Vector3.Lerp(currentShinePosition, shinePoints[currentShinePointIndex].position, beamMovementSpeed * Time.deltaTime);

            // Check if the current position of the beam is within the accuracy buffer we've defined. If so, we'll stop the lerping process.
            // CHANGE TO REMOVE SQUARE ROOT IF PERFORMANCE ISSUES ARISE
            if (Vector3.Distance(currentShinePosition, shinePoints[currentShinePointIndex].position) < beamLerpAccuracy)
            {
                beamPositionLerping = false;
            }
        }
        

        CalculateMeshPoints();

        // If we want the beam to be displayed through a shader and a mesh, this function will create said mesh.
        // (We could also represent it with Unity's lighting system)
        if (visualiseMesh)
        {
            ConstructMesh();
        }
        
        UpdateCollider();
    }


    /// <summary>
    /// Calls a raycasting function numRayChecks times, finding all the colliders the sunbeam will be touching.
    /// Then combines these with the starting points of the beam to form an array of Vector 3s, which we can use to 
    /// construct our sunbeam mesh and collider.
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
            
            // This collects the top points of our rectangle
            meshPoints[i] = transform.InverseTransformPoint(currentStartingPoint);

            // This collects the bottom points of our rectangle
            meshPoints[i + numRayChecks] = transform.InverseTransformPoint(FindEndPoint(currentStartingPoint));
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
        // We'll use this to get information about what our rays have interacted with
        RaycastHit hit;

        // Lets see if the ray has intersected with anything that creates shadows / blocks sunbeams
        // The direction is the angle between our top-left starting point and the current shine point. All rays run parallel to this initial ray.
        if (Physics.Raycast(startPoint, (currentShinePosition - startingPointA.position),
                out hit, Mathf.Infinity, collisionLayers))
        {
            // We've hit something! We can return the point of contact now.

            // Debug.DrawRay(startPoint, (currentShinePosition - startingPointA.position), Color.cyan);
            return hit.point;
        }
        else
        {
            // As we didn't hit anything, but aren't allowed to return null, we return a specific, unlikey value for our Vector3.
            // This should only ever be the case if something's gone wrong, and so should be used for debugging.

            // Debug.DrawRay(startPoint, (currentShinePosition - startingPointA.position) * 1000, Color.white);
            return new Vector3 (-500, -500, -500);
        }
    }


    /// <summary>
    /// Reorganises the vertices in our meshPoints array and feeds them into our polygon collider, creating a collider that aligns with the edges of the 
    /// sunbeam.
    /// </summary>
    private void UpdateCollider()
    {
        #region Explanaition
        // Our meshPoint array is organised in an order that makes meshConstruction simple - but assigning it directly into the collider's
        // points array results in a weird inversion - the collider is hourglass shaped rather than rectangular (Top left corner connected to bottom right,
        // rather than bottom left, and so forth)
        // See the SunbeamMeshGeneration document for details.
        #endregion

        // Because of this, we need to individually loop through the meshPoints array's top and bottom points, reorganising them into a cyclical order
        // for our collider to work with.

        // We start by making a temporary vector 2 array to contain each of the collider's vertices.
        Vector2[] colliderPoints = new Vector2[meshPoints.Length * 2];

        // Loop through the top points of our mesh array, adding each of it's points into our collider array. This order remains the same.
        for (int i = 0; i < numRayChecks; i++)
        {
            colliderPoints[i] = meshPoints[i];
        }

        // Loop through the bottom points of our mesh array, adding each of it's points into our collider array, but inverting their position
        // on that bottom edge
        for (int i = numRayChecks; i < meshPoints.Length; i++)
        {
            colliderPoints[i - (numRayChecks - 1)] = meshPoints[i];
        }

        // Now the order of points in our colliderPoints array is cyclical (clockwise direction), allowing our collider to make logical sense of it.
        // Finally, we just asign our temporary vector2 array as the current points array of our collider
        beamCollider.points = colliderPoints;
    }


    /// <summary>
    /// Takes the current meshPoints array and creates a mesh using those vertices. This mesh can be assigned a material, thus presenting one means 
    /// of representing the sunbeams to the player.
    /// </summary>
    private void ConstructMesh()
    {
        // While our collider needs Vector2's for it's vertices, our mesh requires Vector3's
        // Due to this, we need to cache our meshPoints array into a temporary Vector3 array to work with it effectively.
        Vector3[] verts = new Vector3[meshPoints.Length];
            
        for (int i =0; i < meshPoints.Length; i++)
        {
            verts[i] = new Vector3(meshPoints[i].x, meshPoints[i].y, 0);
        }

        // We can now freely assign this Vector3 array into our mesh's vertices array.
        sunbeamMesh.vertices = verts;

        // Meshes also require us to define the triangles they'll be expected to fill with colour. 
        // See this video for details https://www.youtube.com/watch?v=5c0MatF6G2M&t=304s

        // The number of triangles = (number of ray checks * 2) - 2     ==> two triangles for each vertical rectangle in our mesh.
        // The array stores a refernce to each of the vertices in these triangles - hence, it's 3 times this value in Length
        // See accompanying document entitled SunbeamMeshGeneration for details.

        // We need to make a temporary integer array, as we cannot edit individual elements in our mesh's trinagles array.
        int[] triangles = new int[((2 * numRayChecks) - 2) * 3];

        // Used to keep track of how many vertices we've stored, as we'll be storing several in each iteration of our for loop.
        int currentTriangleIndex = 0;

        // This loop needs to run once for ech vertical rectangle in our mesh (see SunbeamMeshGeneration document for details.)
        // This means it must run numTriangles/2 times, i.e. it must run    ((number of ray checks * 2) - 2)/2 times
        //                                                                = numRayChecks - 1  times
        for (int i = 0; i < numRayChecks - 1; i++)
        {
            // Now, we need to work with the 4 corners of each specific rectangle - constructing 2 triangles from each of them.
            #region Corner Index Value explanaitions
            // The indeces if these corners in our meshPoints array are as follows
            // Top Left Corner  = i
            // Top Right Corner = i + 1
            // Bot Left COrner  = i + numRayChecks
            // Bot Right Corner = i + numRaychecks + 1
            #endregion


            // First triangle of this reactangle
            // Top Left
            triangles[currentTriangleIndex] = i;
            currentTriangleIndex++;
            // Top Right
            triangles[currentTriangleIndex] = i + 1;
            currentTriangleIndex++;
            // Bottom Right
            triangles[currentTriangleIndex] = i + numRayChecks + 1;
            currentTriangleIndex++;

            // Second triangle of this reactangle
            // Top Left
            triangles[currentTriangleIndex] = i;
            currentTriangleIndex++;
            // Bottom Left
            triangles[currentTriangleIndex] = i + numRayChecks;
            currentTriangleIndex++;
            // Bottom Right
            triangles[currentTriangleIndex] = i + numRayChecks + 1;
            currentTriangleIndex++;

        }

        // Now that we've organised these vertices into specific groups of 3, we can assign our mesh's triangles array to the temporary one.
        sunbeamMesh.triangles = triangles;          

        GetComponent<MeshFilter>().mesh = sunbeamMesh;           // And here we finally assign it to the mesh filter component - telling Unity to draw it in the scene.
        //rend.material = meshMaterial;
        
    }


    /// <summary>
    /// Moves the sunbeam to the next shine point in the shinePoints array. If we reach the final shinePoint, the next increment will loop back to the
    /// first shinePoint in the array.
    /// </summary>
    public void OnActiveBeat()
    {
        // Modulous function (%) creates a simple way to cycle back to the beginning of the array once we've reached it's end
        currentShinePointIndex = (currentShinePointIndex + 1) % shinePoints.Length;

        // Tell our currentShinePosition to lerp to this new point.
        beamPositionLerping = true;
    }

    
}
