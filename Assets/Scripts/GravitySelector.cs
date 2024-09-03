using System.Collections;
using UnityEngine;

public class GravitySelector : MonoBehaviour
{
    // SINGLETON
    public static GravitySelector _instance { get; private set; }

    // Gravity selector UI and components
    [SerializeField] private GameObject GravitySelectorUI;
    [SerializeField] private GravitySelectionSoundManager _soundManager;
    [SerializeField] private GravitySelectionPostProcessingManager _postProcessingManager;
    [SerializeField] private GameObject _selectionSprite;
    private Animator _gravitySelectorAnimator;

    // Fields
    public GravityDirection _selectedGravityDirection { get; private set; }
    public int _interactableInstanceID { get; private set; } // The ID of the interactable object whose gravity is being selected for

    private Coroutine SelectGravityCoroutine; // Gets run when gravity is being manipulated

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Destroy(gameObject);
        }
        
        _gravitySelectorAnimator = GravitySelectorUI.GetComponent<Animator>();

        GravitySelectorUI.SetActive(false);
    }

    /* Initiates the process of selecting a gravity direction
     * EDGE: halfOfObjHeight is used to handle a small edge case relating to the coordinates of the obj
     */
    public void StartSelection(int objID, Vector2 objectPos, float halfOfObjHeight, Bounds objectBounds)
    {
        _interactableInstanceID = objID;

        // Initialize gravity selector
        GravitySelectorUI.SetActive(true);

        // Annoying edge case: Just ignore
        objectPos.y += halfOfObjHeight;
        GravitySelectorUI.transform.position = objectPos;
        objectPos.y -= halfOfObjHeight;

        // Handle gravity selection separately
        SelectGravityCoroutine = StartCoroutine(SelectGravity(objectPos, objectBounds));

        // SFX
        _soundManager.DrownSounds();

        _postProcessingManager.Desaturate();

        // Pause physics system
        pauseTime(true);
    }


    /*
     * For every frame, this function evaluates the gravity direction that the player is selecting 
     * and updates this object's state accordingly
     */
    private IEnumerator SelectGravity(Vector2 centerOfSelector, Bounds objectBounds)
    {

        while (true)
        {
            Vector3 mousePos = MouseHelper._instance.GetMouseWorldPosition();

            // Check which cardinal direction the player wants to select
            Vector2 directionVector = (Vector2)mousePos - centerOfSelector;
            GravityDirection direction = vectorToSelectedDirection(directionVector);
            setSelectedGravity(direction, objectBounds);

            // Pass unscaled time variable to shader
            SpriteRenderer renderer = _selectionSprite.GetComponent<SpriteRenderer>();
            renderer.material.SetFloat("_UnscaledTime", Time.unscaledTime);

            yield return new WaitForEndOfFrame();
        }
    }

    public void EndSelection()
    {
        if (SelectGravityCoroutine == null)
            return;

        // Coroutine logic
        StopCoroutine(SelectGravityCoroutine);
        SelectGravityCoroutine = null;

        // Gravity selector logic
        GravitySelectorUI.SetActive(false);

        // Time logic
        pauseTime(false);

        // Remove reference
        _interactableInstanceID = -1;

        // SFX
        _soundManager.Reset();

        _postProcessingManager.ResetSaturation();
    }

    /* 
        Algorithm: The gravity direction that the player wants to select for this object is calculated by:
        1. Getting the vector from the center of the gravity selector to the current mouse position.
        2. If the magnitude of the X component is greater than the Y component, then the player is trying to select either a west or east direction
            i.e: a horizontal one... and the true is vice versa.
        3. The sign of the corresponding vector component indicates which of the two directions-- in the case of the above, west or east-- is the selected direction
    */
    private GravityDirection vectorToSelectedDirection(Vector2 vector)
    {
        // Edge: Vector not large enough
        if (Mathf.Abs(vector.x) < 0.45 && Mathf.Abs(vector.y) < 0.45)
            return GravityDirection.NONE;

        // 2. Magnitudes of the X and Y vector components
        float absoluteX = Mathf.Abs(vector.x);
        float absoluteY = Mathf.Abs(vector.y);

        bool isAHorizontalDirection = absoluteX > absoluteY;
        if (isAHorizontalDirection)
            return vector.x > 0 ? GravityDirection.EAST : GravityDirection.WEST; // Determine which horizontal dir
        else
            return vector.y > 0 ? GravityDirection.NORTH : GravityDirection.SOUTH; // Determine which vertical dir
    }

    /* Updates gravity selection UI and the temporary gravity variable*/
    private void setSelectedGravity(GravityDirection direction, Bounds bounds)
    {
        // If a new dir is selected, play appropriate anims
        if (direction != _selectedGravityDirection)
        {
            SetSelectionAnimation(direction, bounds);
            setSelectionAnimation(direction);
        }
            

        _selectedGravityDirection = direction;

    }

    private void SetSelectionAnimation(GravityDirection direction, Bounds bounds)
    {
        Transform transform = _selectionSprite.transform;
        switch (direction)
        {
            case GravityDirection.NORTH:
                transform.position = bounds.center;
                return;
            case GravityDirection.SOUTH:
                return;
            case GravityDirection.EAST:
                return;
            case GravityDirection.WEST:
                return;
            default:
                return;
        }
    }

    private void setSelectionAnimation(GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.NORTH:
                _gravitySelectorAnimator.SetTrigger("North");
                return;
            case GravityDirection.SOUTH:
                _gravitySelectorAnimator.SetTrigger("South");
                return;
            case GravityDirection.EAST:
                _gravitySelectorAnimator.SetTrigger("East");
                return;
            case GravityDirection.WEST:
                _gravitySelectorAnimator.SetTrigger("West");
                return;
            default:
                _gravitySelectorAnimator.SetTrigger("None");
                return;
        }
    }

    private void pauseTime(bool pause)
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

}
