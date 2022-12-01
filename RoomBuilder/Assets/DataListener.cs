using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataListener : MonoBehaviour
{

    private void Awake()
    {
        Debug.Log("DataListener is waking up");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies(int amount) {

        Debug.Log("LISTENING TO THIGNGS");
        Debug.Log(amount);

    }

   
}
