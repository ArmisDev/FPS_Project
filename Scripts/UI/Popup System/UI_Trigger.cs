using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Project.Player;
using Project.Weapon;

[RequireComponent(typeof(AudioSource))]
public class UI_Trigger : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject UI;
    [SerializeField] private Mouse_LockState mouseLockState;
    [SerializeField] private bool onlyDisplayOnce;
    [SerializeField][Tooltip("Is set to true, the object will be destroyed when the player leaves")] private bool shouldStopPlayer;
    private Collider collider;
    private AudioSource audioSource;
    private Player_Movement playerMovement;
    private Weapon_Main currentWeaponMainLogic;
    private Weapon_Inventory weapon_Inventory;

    [Header("Audio Parameters")]
    [SerializeField] private AudioClip menuOpen;
    [SerializeField] private AudioClip menuClose;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        TryGetComponent(out Collider currentCollider);
        if(currentCollider == null)
        {
            Debug.LogError("Pleae add collider to " + this.name + "!!");
            return;
        }
        collider = currentCollider;
        audioSource = GetComponent<AudioSource>();
        UI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Component caching
        other.TryGetComponent(out Player_Movement currentplayerMovement);
        weapon_Inventory = FindObjectOfType<Weapon_Inventory>();

        if (currentplayerMovement == null) return;
        if (weapon_Inventory == null) return;
        if(mouseLockState == null)
        {
            Debug.LogWarning("Please make sure that the script can access the mouse lock state");
        }

        playerMovement = currentplayerMovement;

        //First set the velocity to zero
        //This ensures that components dependent on the magnitude of the players velocity will not play
        //I.e footstep sounds, animations, etc.
        playerMovement.playerVelocity = Vector3.zero;
        playerMovement.stopMovement = shouldStopPlayer;

        //Stop player look rotation
        playerMovement.stopLookRotation = true;

        //This must be called after we have grabbed the player logic
        FindandStopCurrentWeapon();

        //Turn off weapon switching
        weapon_Inventory.stopWeaponSwitch = true;

        //Hide the cursor
        mouseLockState.RequestControl(Mouse_LockState.ControlPriority.Popup, true, CursorLockMode.Confined);

        //Set UI Visability to true
        UI.SetActive(true);

        //Play Audioclip
        audioSource.PlayOneShot(menuOpen);

        //Turn off weapon switching
        weapon_Inventory.stopWeaponSwitch = true;
    }

    //Check for currentWeapon
    void FindandStopCurrentWeapon()
    {
        if (playerMovement == null) return;
        GameObject playerGO = playerMovement.gameObject;

        Weapon_Main[] weapon_Mains = FindObjectsOfType<Weapon_Main>();
        if(weapon_Mains.Length > 0)
        {
            //Weapon_Main[] weapon_Mains = playerGO.GetComponentsInChildren<Weapon_Main>();
            foreach(Weapon_Main weapon_Main in weapon_Mains)
            {
                if (weapon_Main.gameObject.activeSelf)
                {
                    currentWeaponMainLogic = weapon_Main;
                    currentWeaponMainLogic.stopWeaponLogic = true;
                    Debug.Log(currentWeaponMainLogic.gameObject.name);
                }
            }
        }
    }

    //Called via button
    public void ResumeGameplay()
    {
        //Allow the player to move again
        playerMovement.stopMovement = false;

        //Resume player look rotation
        playerMovement.stopLookRotation = false;

        //Bring the mouse back
        mouseLockState.RequestControl(Mouse_LockState.ControlPriority.Popup, false, CursorLockMode.Locked);
        mouseLockState.ReleaseControl(Mouse_LockState.ControlPriority.Popup);

        weapon_Inventory.stopWeaponSwitch = false;

        StartCoroutine(DelayLogic());
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    IEnumerator DelayLogic()
    {
        yield return new WaitForSeconds(0.15f);
        //Play Audioclip
        audioSource.PlayOneShot(menuClose);

        //Hide the UI
        UI.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        //This allows us to use the coroutine even if we have no weaponmain
        //This protects against instances where player walks through trigger with no weapons
        if(currentWeaponMainLogic != null)
        {
            currentWeaponMainLogic.stopWeaponLogic = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(onlyDisplayOnce)
        {
            Destroy(gameObject);
        }
    }
}