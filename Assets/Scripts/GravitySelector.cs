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
        //GravitySelectorUI.SetActive(true);

        // Set up selection FX
        _selectionSprite.SetActive(true);

        // Annoying edge case: Just ignore
        objectPos.y += halfOfObjHeight;
        GravitySelectorUI.transform.position = objectPos;
        objectPos.y -= halfOfObjHeight;

        ResetSelectionSpritePosition(objectBounds);

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
        _selectionSprite.SetActive(false);

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
            //setSelectionAnimation(direction);
        }
            

        _selectedGravityDirection = direction;

    }

    private void SetSelectionAnimation(GravityDirection direction, Bounds objectBounds)
    {
        Transform transform = _selectionSprite.transform;
        transform.rotation = Quaternion.identity;

        ResetSelectionSpritePosition(objectBounds);

        // Rotations are done counter clock wise
        switch (direction)
        {
            case GravityDirection.NORTH:
                transform.RotateAround(objectBounds.center, Vector3.forward, 0);
                return;
            case GravityDirection.SOUTH:
                transform.RotateAround(objectBounds.center, Vector3.forward, 180);
                return;
            case GravityDirection.EAST:
                transform.RotateAround(objectBounds.center, Vector3.forward, 270);
                return;
            case GravityDirection.WEST:
                transform.RotateAround(objectBounds.center, Vector3.forward, 90);
                return;
            default:
                return;
        }
    }


    /* Moves the selection sprite to the center of the provided bounds of the reference sprite
     @param objectbounds  Bounds  Bounds of the reference sprite
    */
    private void ResetSelectionSpritePosition(Bounds objectBounds)
    {
        Transform transform = _selectionSprite.transform;
        Bounds selectionSpriteBounds = _selectionSprite.GetComponent<SpriteRenderer>().bounds;

        // Get adjustments
        Vector2 offset = GetSelectionSpriteOffset(objectBounds, selectionSpriteBounds);
        Vector3 scaleMultiplier = GetSelectionSpriteScaleMultiplier(objectBounds, selectionSpriteBounds);

        // Apply adjustments
        transform.localScale = Vector3.Scale(transform.localScale, scaleMultiplier);
        transform.position = (Vector2)objectBounds.center + offset;
    }


    /* Gets the offset vector needed to place the selection sprite just above the reference sprite
     * 
     @param objectBounds  Bounds   Bounds of the reference sprite
     @params selectionSpriteBounds  Bounds  Bounds of the selection sprite

     Returns the offset as a Vector2
     */
    private Vector2 GetSelectionSpriteOffset(Bounds objectBounds, Bounds selectionSpriteBounds)
    {
        float selectionSpriteHeight = selectionSpriteBounds.size.y;
        float objectHeight = objectBounds.size.y;

        Vector2 offset = new Vector2(0, selectionSpriteHeight / 2 + objectHeight / 2);
        return offset;
    }


    /* Gets the scale multiplier that scales the selection sprite's WIDTH to the reference sprite's WIDTH
     * 
     @param objectBounds  Bounds   Bounds of the reference sprite
     @params selectionSpriteBounds  Bounds  Bounds of the selection sprite

     Returns the multiplier as a Vector3
     */
    private Vector3 GetSelectionSpriteScaleMultiplier(Bounds objectBounds, Bounds selectionSpriteBounds)
    {
        // Get WIDTHS, not heights
        float selectionSpriteWidth = selectionSpriteBounds.size.x;
        float objectWidth = objectBounds.size.x;

        Vector3 scaleMultiplier = new Vector3(objectWidth / selectionSpriteWidth, 1, 1);
        return scaleMultiplier;
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
