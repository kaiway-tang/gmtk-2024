using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISmartEnemy
{
    public void SetTarget(Vector2 position); 
    public int EvaluatePosition(Vector2 position);  
}

public class Enemy : HPEntity
{

    // Update is called once per frame
    void Update()
    {
        
    }

}
