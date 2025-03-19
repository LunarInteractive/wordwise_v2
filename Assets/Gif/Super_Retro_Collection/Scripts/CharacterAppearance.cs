// -----------------------------------------------------------------------------------------
// using classes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// -----------------------------------------------------------------------------------------
// player movement class
public class CharacterAppearance : MonoBehaviour
{
    // static public members
    public static CharacterAppearance instance;

    // -----------------------------------------------------------------------------------------
    // public members
    public Transform tf;
    public Vector2 movement;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // The name of the sprite sheet to use
    public string SpriteSheetName;

    // -----------------------------------------------------------------------------------------
    // private members
    private Vector2 previousPosition;

    // The name of the currently loaded sprite sheet
    private string LoadedSpriteSheetName;

    // The dictionary containing all the sliced up sprites in the sprite sheet
    private Dictionary<string, Sprite> spriteSheet;

    // -----------------------------------------------------------------------------------------
    // This method is called when the game object is first initialized.
    // It is responsible for initializing the object's instance variable, setting the previous position of the object,
    // loading the sprite sheet, and setting the initial speed and orientation of the animator.
    // 
    // This method is called once when the game object is first created.
    void Awake()
    {
        // Set the instance variable of the CharacterAppearance class to the current instance of the class.
        // This allows other scripts to access this instance of the class.
        instance = this;

        // Set the previousPosition variable to the current position of the object's transform.
        // This is used to calculate the object's velocity in the FixedUpdate method.
        previousPosition = tf.position;

        // Load the sprite sheet using the LoadSpriteSheet method.
        // The sprite sheet is loaded from a file named "spritesheet" in a folder named "Characters" within the "Resources" folder.
        // If the sprite sheet cannot be found, the default sprite sheet is loaded.
        this.LoadSpriteSheet();

        // Set the initial speed of the animator to 0.
        // This is done by calling the SetFloat method of the animator and passing in the name of the parameter ("speed") and the value (0).
        animator.SetFloat("speed", 0);

        // Set the initial orientation of the animator to 4.
        // This is done by calling the SetInteger method of the animator and passing in the name of the parameter ("orientation") and the value (4).
        animator.SetInteger("orientation", 4);
    }
    // -----------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {

    }
    // -----------------------------------------------------------------------------------------
    // FixedUpdate is called every fixed frame-rate frame, if the MonoBehaviour is enabled. 
    // It is used for physics-related calculations. 
    // 
    // This method updates the movement vector by calculating the difference in x and y coordinates
    // between the current position of the object and the previous position of the object. 
    // The difference is used to calculate the velocity of the object. 
    // The velocity is then stored in the movement vector. 
    // 
    // The previous position is then updated to the current position of the object. 
    // 
    // Finally, the animationUpdate method is called to update the animation parameters based on the movement vector.
    void FixedUpdate()
    {
        // Calculate the difference in x coordinates between the current position of the object
        // and the previous position of the object.
        movement.x = tf.position.x - previousPosition.x;

        // Calculate the difference in y coordinates between the current position of the object
        // and the previous position of the object.
        movement.y = tf.position.y - previousPosition.y;

        // Update the previousPosition variable to the current position of the object.
        previousPosition = tf.position;

        // Call the animationUpdate method to update the animation parameters based on the movement vector.
        animationUpdate();
    }

    // Runs after the animation has done its work
    private void LateUpdate()
    {
        // Check if the name of the sprite sheet has changed
        // The name may have been changed manually in the inspector
        if (this.LoadedSpriteSheetName != this.SpriteSheetName)
        {
            // If the name of the sprite sheet has changed, reload the new sprite sheet
            // This is done by calling the LoadSpriteSheet method
            // The LoadSpriteSheet method loads the sprites from a sprite sheet file (png)
            // The file specified must exist in a folder named Resources
            // The sprite sheet is loaded from a file named "spritesheet" in a folder named "Characters"
            // within the "Resources" folder.
            // If the sprite sheet cannot be found, the default sprite sheet is loaded.
            this.LoadSpriteSheet();
        }

        // Swap out the sprite to be rendered by its name
        // The name of the sprite must be the same!
        // The name of the sprite is obtained from the spriteRenderer component's current sprite
        // The spriteSheet dictionary is searched for a sprite with the same name
        // The spriteRenderer component's sprite is then set to the sprite found in the dictionary
        this.spriteRenderer.sprite = this.spriteSheet[this.spriteRenderer.sprite.name];
    }

    // -----------------------------------------------------------------------------------------
    // This method updates the animation parameters of the character based on its movement.
    // It takes into account the absolute values of the x and y components of the movement vector.
    // The speed of the character is set based on the sum of the absolute values of the x and y components.
    // The orientation of the character is set based on the direction of the x component of the movement vector.
    // If the x component is greater than 0.01, the orientation is set to 6 (right).
    // If the x component is less than -0.01, the orientation is set to 2 (left).
    // If the y component is greater than 0.01, the orientation is set to 0 (up).
    // If the y component is less than -0.01, the orientation is set to 4 (down).
    // The animator component is used to set the animation parameters.
    public void animationUpdate()
    {
        // Set the speed of the character based on the sum of the absolute values of the x and y components of the movement vector.
        animator.SetFloat("speed", Mathf.Abs(movement.x) + Mathf.Abs(movement.y));

        // Set the orientation of the character based on the direction of the x component of the movement vector.
        if (movement.x > 0.01f)
            animator.SetInteger("orientation", 6); // If the x component is greater than 0.01, set the orientation to 6 (right).
        if (movement.x < -0.01f)
            animator.SetInteger("orientation", 2); // If the x component is less than -0.01, set the orientation to 2 (left).
        if (movement.y > 0.01f)
            animator.SetInteger("orientation", 0); // If the y component is greater than 0.01, set the orientation to 0 (up).
        if (movement.y < -0.01f)
            animator.SetInteger("orientation", 4); // If the y component is less than -0.01, set the orientation to 4 (down).

    }
    // -----------------------------------------------------------------------------------------
    // This method loads the sprites from a sprite sheet file (png). 
    // It is responsible for loading the sprites from a sprite sheet file. 
    // The sprite sheet file is located in a folder named "Characters", within the "Resources" folder.
    // The name of the sprite sheet file is "spritesheet".
    // 
    // The method first constructs the file path of the sprite sheet file by concatenating the 
    // folder name, the name of the sprite sheet, and the file name.
    // 
    // It then loads all the sprites from the sprite sheet file. The method uses the Resources.LoadAll
    // method to load all the sprites from the file. The method specifies the type of object to load
    // as Sprite.
    // 
    // If no sprites are loaded, it assumes that the sprite sheet file specified in the inspector
    // does not exist. In this case, it falls back to loading the sprites from the default sprite
    // sheet file.
    // 
    // Finally, it converts the loaded sprites into a dictionary where the key is the name of the sprite
    // and the value is the sprite itself. The method also stores the name of the sprite sheet that was
    // loaded so that it can be used later to check if the sprite sheet has been changed.
    private void LoadSpriteSheet()
    {
        // Construct the file path of the sprite sheet file
        string spritesheetfolder = "Characters/";
        string spritesheetfilepath = spritesheetfolder + this.SpriteSheetName + "/spritesheet";

        // Load all the sprites from the sprite sheet file
        var sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);

        // If no sprites are loaded, assume that the sprite sheet file specified in the inspector
        // does not exist. Fall back to loading the sprites from the default sprite sheet file.
        if (sprites.Count() == 0)
        {
            spritesheetfilepath = spritesheetfolder + "chara_01/spritesheet";
            sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);
        }

        // Convert the loaded sprites into a dictionary where the key is the name of the sprite
        // and the value is the sprite itself
        this.spriteSheet = sprites.ToDictionary(x => x.name, x => x);

        // Store the name of the sprite sheet that was loaded so that it can be used later to check
        // if the sprite sheet has been changed
        this.LoadedSpriteSheetName = this.SpriteSheetName;
    }
}
