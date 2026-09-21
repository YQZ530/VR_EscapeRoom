using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DCXR.GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable] // Makes it visible in the Inspector (optional, but good practice for data classes)
public class CheckItem
{
    public GameObject item;
    public GameObject demoItem;
    public bool isFound;
    public GameEventType gameEventType;
    public string hint;
    public string name;

    public CheckItem(GameObject obj, GameObject demo, GameEventType eventType, string name, string hint)
    {
        this.item = obj;
        this.demoItem = demo;
        isFound = false;
        this.gameEventType = eventType;
        this.name = name;
        this.hint = hint;
    }

    // You could add a method here to set isFound, but we'll manage it externally for now
    public void SetFound(bool found)
    {
        isFound = found;
        if (found)
        {
            this.demoItem.SetActive(false);
        }
        else
        {
            this.demoItem.SetActive(true);
        }
            
    }
    
   
}


public class ChecklistManager
{
    // Our checklist. 
    public List<CheckItem> checklist = new List<CheckItem>();
// Event to notify when the current item or hint needs to be updated
    public static event Action<CheckItem> OnCurrentItemChanged;
    public static event Action OnAllItemsFound;
    private int _currentIndex;
    public GameEventType GetItemType(GameObject obj)
    {
        CheckItem itemToUpdate = checklist.FirstOrDefault(item => item.item == obj);
        if (itemToUpdate != null)
        {
            return itemToUpdate.gameEventType;
        }
        else
        {
            Debug.LogError("Cannot find gameobj with game type");
            return GameEventType.SearchBook;
        }
    }
    /// <summary>
    /// Adds a new GameObject to the checklist as a CheckItem.
    /// Returns true if added, false if the item already exists in the checklist.
    /// </summary>
    public bool AddItem(GameObject obj, GameObject demo, GameEventType eventType, string name, string hint)
    {
       
        if (checklist.Any(item => item.item == obj))
        {
            Debug.LogWarning($"Checklist already contains {obj.name}. Not adding again.");
            return false;
        }

        checklist.Add(new CheckItem(obj, demo, eventType, name, hint));
        //Debug.Log($"Added {obj.name} to the checklist.");
        return true;
    }

    public void ResetItemStatus()
    {
        _currentIndex = 0;
        foreach (CheckItem item in checklist)
        {
            item.SetFound(false); // Use the setter method from CheckItem
        }
        // Deactivate all demo items initially, only activate the first one
        foreach (CheckItem item in checklist)
        {
            item.demoItem.SetActive(false);
        }

        // Display the first item and hint if the list is not empty
        if (checklist.Count > 0)
        {
            checklist[_currentIndex].demoItem.SetActive(true);
            OnCurrentItemChanged?.Invoke(checklist[_currentIndex]);
        }
        
        Debug.Log("All items in the checklist have been reset to 'not found'.");
    }

    /// <summary>
    /// Sets the 'isFound' status for a specific GameObject in the checklist.
    /// </summary>
    /// <param name="obj">The GameObject to set.</param>
    /// <param name="foundStatus">The boolean status to set (true for found, false for not found).</param>
    /// <returns>True if the item was found and updated, false if the item was not in the checklist.</returns>
    public bool SetItem(GameObject obj, bool foundStatus)
    {
        // Only allow setting if the object is the current required object
        if (checklist.Count > 0 && _currentIndex < checklist.Count && checklist[_currentIndex].item == obj)
        {
            checklist[_currentIndex].SetFound(foundStatus);
            Debug.Log($"Set '{obj.name}' isFound to {foundStatus}.");

            if (foundStatus) // If the current item was found, advance to the next
            {
                checklist[_currentIndex].demoItem.SetActive(false); // Hide the demo of the found item
                _currentIndex++;
                if (_currentIndex < checklist.Count)
                {
                    checklist[_currentIndex].demoItem.SetActive(true); // Show the demo of the next item
                    OnCurrentItemChanged?.Invoke(checklist[_currentIndex]);
                }
                else
                {
                    OnAllItemsFound?.Invoke(); // All items found
                }
            }
            return true;
        }
        else if (checklist.Any(item => item.item == obj))
        {
            Debug.Log($"Item '{obj.name}' is not the current target. Please find '{GetCurrentItem()?.item.name}' first.");
            // Optionally, you might play a "wrong item" sound here.
            return false;
        }
        else
        {
            Debug.Log($"Item '{obj.name}' not found in the checklist. Cannot set status.");
            return false;
        }
    }
    public CheckItem GetCurrentItem()
    {
        if (checklist.Count > 0 && _currentIndex < checklist.Count)
        {
            return checklist[_currentIndex];
        }
        return null;
    }
    /// <summary>
    /// Checks if all items in the checklist are marked as 'isFound = true'.
    /// </summary>
    /// <returns>True if all items are found, false otherwise.</returns>
    public bool AreAllItemsFound()
    {
        if (checklist.Count == 0)
        {
            Debug.Log("Checklist is empty. Returning true (or false, depending on desired behavior for empty list).");
            return true; // Or false, depending on what "all found" means for an empty list.
                        // If an empty checklist means "nothing to find, so everything is technically found", return true.
                        // If it means "nothing to find, so it's not a complete checklist", return false.
        }

        return checklist.All(item => item.isFound);
    }

    /// <summary>
    /// Gets a list of all items that have not yet been found.
    /// </summary>
    /// <returns>A List of CheckItem objects that are not found.</returns>
    public List<CheckItem> GetUnfoundItems()
    {
        return checklist.Where(item => !item.isFound).ToList();
    }

    /// <summary>
    /// Gets a list of all items that have been found.
    /// </summary>
    /// <returns>A List of CheckItem objects that are found.</returns>
    public List<CheckItem> GetFoundItems()
    {
        return checklist.Where(item => item.isFound).ToList();
    }

    /// <summary>
    /// (For Debugging) Prints the current status of all items in the checklist.
    /// </summary>
    public void PrintChecklistStatus()
    {
        Debug.Log("--- Checklist Status ---");
        if (checklist.Count == 0)
        {
            Debug.Log("Checklist is empty.");
            return;
        }
        foreach (CheckItem item in checklist)
        {
            Debug.Log($"Item: {item.item.name}, Found: {item.isFound}");
        }
        Debug.Log("------------------------");
    }
}

public class VerifiedObjectMachine : MonoBehaviour
{
   
    public GameObject plantA;
    public GameObject BookB;
    public GameObject KeyC;
    public GameObject plantDemo;
    public GameObject BookDemo;
    public GameObject KeyDemo;

    public TextMeshProUGUI  demoNameText;
    public TextMeshProUGUI demoHintText;
    public Canvas demoCanvas;
    public ChecklistManager manager;
    private GameManager gameplayManager;
    private AudioSource audiosource;
    public AudioClip CorrectClip;
    public AudioClip WrongClip;

    public static event Action<GameEventType> ReceiveObjectEvent;
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        GameObject gameplayManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        if (gameplayManagerObj != null) gameplayManager = gameplayManagerObj.GetComponent<GameManager>();
        else Debug.LogError("cannot find GameManager at the finish chunk");
        
        OnCreation();
    }

    private void OnEnable()
    {
        GameManager.onGameStart += OnGameRestart;
        ChecklistManager.OnCurrentItemChanged += UpdateHintUI; // Subscribe to the event
        ChecklistManager.OnAllItemsFound += HandleAllItemsFound; // Subscribe to all found event
    }

    private void OnDisable()
    {
        GameManager.onGameStart -= OnGameRestart;
        ChecklistManager.OnCurrentItemChanged -= UpdateHintUI; // Unsubscribe
        ChecklistManager.OnAllItemsFound -= HandleAllItemsFound; // Unsubscribe
    }
    
    private void UpdateHintUI(CheckItem currentItem)
    {
        
        if (demoHintText != null)
        {
            demoHintText.text = currentItem.hint;
        }
        if (demoNameText != null)
        {
            demoNameText.text = currentItem.name; // Ensure hint panel is visible
        }
        currentItem.demoItem.SetActive(true);
        Debug.Log($"Current Objective: Find {currentItem.item.name}. Hint: {currentItem.hint}");
    }

    private void HandleAllItemsFound()
    {
        if (demoCanvas != null)
        {
            demoCanvas.enabled = false; // Hide hint panel when all items are found
        }
        Debug.Log("All items found in order!");
        gameplayManager.EndGame();
    }
    void OnGameRestart()
    {
        OnCreation();
    }
    void OnCreation()
    {
        if (manager == null)
        {
            manager = new ChecklistManager();
            
            manager.AddItem(KeyC, KeyDemo, GameEventType.SearchKey, "Pink Key" ,"Among the pages,\n truth resides,\nWhere key lies near and wisdom hides.\n");
            manager.AddItem(BookB, BookDemo, GameEventType.SearchBook, "Brown Book","High on the shelf,\n behind the row,\nA crucial book to help you go.");
            manager.AddItem(plantA,plantDemo, GameEventType.SearchPlant, "Orange Plant","A leafy friend, in shadows deep,\nWhere darkness hides,\n its secrets keep.");
        }
        
        manager.ResetItemStatus();
    }  

    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (other.gameObject.GetComponent<XRGrabInteractable>() != null)
        {
            if (manager.SetItem(other.gameObject, true))
            {

                audiosource.clip = CorrectClip;
                audiosource.Play();
                GameEventType eventType = manager.GetItemType(other.gameObject);
                ReceiveObjectEvent.Invoke(eventType);

            }
            else
            {
                audiosource.clip = WrongClip;
                audiosource.Play();
            }
            
        }
        else
        {
            Debug.Log("Target object does not have XR grab interactable");
        }



    }
}
