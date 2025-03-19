using System.Collections;
using System.Collections.Generic;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using NavMeshPlus.Extensions;

public class npc_wise : MonoBehaviour
{
    // static public members
    public static npc_wise instance;

    // -----------------------------------------------------------------------------------------
    // public members
    public Transform tf;
    public Vector2 movement;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // The name of the sprite sheet to use
    public bool spriteRandomisation = true;

    [Tooltip("The name of the sprite sheet to use if randomisation is on")]
    // The name of the sprite sheet to use   
    public string SpriteSheetName;
    // -----------------------------------------------------------------------------------------
    // private members
    private Vector2 previousPosition;

    // The name of the currently loaded sprite sheet
    private string LoadedSpriteSheetName;

    // The dictionary containing all the sliced up sprites in the sprite sheet
    private Dictionary<string, Sprite> spriteSheet;


    public Transform target;
    NavMeshAgent agent;

    void Awake()
    {
        instance = this;
        previousPosition = tf.position;
        //velocity = rb.velocity;
        this.LoadSpriteSheet();
        animator.SetFloat("speed", 0);
        animator.SetInteger("orientation", 4);
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        agent.SetDestination(target.position);
    }

    private void FixedUpdate()
    {
        movement.x = agent.desiredVelocity.x;
        movement.y = agent.desiredVelocity.y;

        animationUpdate();
    }

    private void LateUpdate()
    {
        // Check if the sprite sheet name has changed (possibly manually in the inspector)
        if (this.LoadedSpriteSheetName != this.SpriteSheetName)
        {
            // Load the new sprite sheet
            this.LoadSpriteSheet();
        }

        // Swap out the sprite to be rendered by its name
        // Important: The name of the sprite must be the same!
        this.spriteRenderer.sprite = this.spriteSheet[this.spriteRenderer.sprite.name];
    }

    public void animationUpdate()
    {
        animator.SetFloat("speed", Mathf.Abs(movement.x) + Mathf.Abs(movement.y));
        if (movement.x > 1)
            animator.SetInteger("orientation", 6);
        if (movement.x < -1)
            animator.SetInteger("orientation", 2);
        if (movement.y > 1)
            animator.SetInteger("orientation", 0);
        if (movement.y < -1)
            animator.SetInteger("orientation", 4);
    }

    private void LoadSpriteSheet()
    {

        // Load the sprites from a sprite sheet file (png). 
        // Note: The file specified must exist in a folder named Resources
        string spritesheetfolder = "Characters/";
        string selectedNames;
        if (spriteRandomisation)
        {
            List<string> names = new List<string> { "chara_01", "chara_03", "chara_04" };
            selectedNames = names[Random.Range(0, names.Count)];
        }
        else
        {
            selectedNames = SpriteSheetName;
        }
        string spritesheetfilepath = spritesheetfolder + selectedNames + "/spritesheet";
        var sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);
        if (sprites.Count() == 0)
        {
            spritesheetfilepath = spritesheetfolder + "chara_01/spritesheet";
            sprites = Resources.LoadAll<Sprite>(spritesheetfilepath);
        }

        this.spriteSheet = sprites.ToDictionary(x => x.name, x => x);

        // Remember the name of the sprite sheet in case it is changed later
        this.LoadedSpriteSheetName = this.SpriteSheetName;
    }

    public void change_target(Transform new_target)
    {
        target = new_target;
    }
}
