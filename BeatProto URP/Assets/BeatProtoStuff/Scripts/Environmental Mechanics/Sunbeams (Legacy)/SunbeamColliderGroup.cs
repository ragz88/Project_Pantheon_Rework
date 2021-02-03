using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunbeamColliderGroup : MonoBehaviour
{
    /// <summary>
    /// The expected isActive value of a potential shadow-casting object for this collection of colliders to be the correct one.
    /// </summary>
    [System.Serializable]
    public struct ExpectedShadowCasterState
    {
        public GameObject shadowCastingObject;
        public bool expectedState;
    }

    [Tooltip("The expected isActive value of each potential shadow-casting object for this collection of colliders to be the correct one.")]
    /// <summary>
    /// The expected isActive value of each potential shadow-casting object for this collection of colliders to be the correct one.
    /// </summary>
    public ExpectedShadowCasterState[] expectedCasterStates;



    public void CheckCurrentCasterStates()
    {
        bool incorrectStateFound = false;

        for (int i = 0; i < expectedCasterStates.Length; i++)
        {
            if (expectedCasterStates[i].expectedState != expectedCasterStates[i].shadowCastingObject.activeSelf)
            {
                incorrectStateFound = true;
                break;
            }
        }

        if (!incorrectStateFound)
        {
            gameObject.SetActive(true);       // Test if this actually runs when object not active
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
