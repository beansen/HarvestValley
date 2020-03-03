using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class CollectableCrops
{
    private float timer;
    private List<ObjectData> objects;
    private ItemName seed;
    private CollectableState state = CollectableState.Enabled;
    private DateTime collectionTime;
    private Vector3 worldPosition;

    public ItemName Seed
    {
        get { return seed; }
    }

    public bool Collectable
    {
        get { return state == CollectableState.Collectable; }
    }

    public bool Deactivated
    {
        get { return state == CollectableState.Deactivated; }
    }

    public Vector3 WorldPosition
    {
        get { return worldPosition; }
    }

    public CollectableCrops(ItemName seed, Vector3 worldPosition)
    {
        this.seed = seed;
        this.worldPosition = worldPosition;
    }
    

    public void Update(Vector3 playerPosition)
    {
        if (state == CollectableState.Enabled)
        {
            timer += Time.deltaTime * 4;
            float sin = Mathf.Sin(timer);

            for (int i = 0; i < objects.Count; i++)
            {
                ObjectData objectData = objects[i];
                Vector3 newPos = objectData.WorldPosition;
                newPos.x += objects[i].Direction * 0.01f;
                newPos.y = Mathf.Max(0f, 2.5f * sin);
                objectData.WorldPosition = newPos;
                objects[i] = objectData;
                objects[i].UiObject.transform.position = Camera.main.WorldToScreenPoint(newPos);
            }

            if (sin <= 0)
            {
                state = CollectableState.Collectable;
                collectionTime = DateTime.Now;
            }
        }
        else if (state == CollectableState.Collectable)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].UiObject.transform.position = Camera.main.WorldToScreenPoint(objects[i].WorldPosition);
            }

            if (DateTime.UtcNow.Subtract(collectionTime).TotalSeconds >= 60)
            {
                DeactivateCollectables();
            }
        }
        else if (state == CollectableState.Collected)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].WorldPosition =
                    Vector3.MoveTowards(objects[i].WorldPosition, playerPosition, Time.deltaTime * 10);
                objects[i].UiObject.transform.position = Camera.main.WorldToScreenPoint(objects[i].WorldPosition);

                if (Vector3.Distance(objects[i].WorldPosition, playerPosition) < 0.1f)
                {
                    DeactivateCollectables();
                    break;
                }
            }
        }
    }

    public void AddItem(GameObject gameObject, Vector3 worldPosition)
    {
        if (objects == null)
        {
            objects = new List<ObjectData>();
        }

        int direction = Random.Range(0, 2) == 0 ? -1 : 1;
        objects.Add(new ObjectData(direction, worldPosition, gameObject));
    }

    public void SetCollected()
    {
        state = CollectableState.Collected;
    }

    private void DeactivateCollectables()
    {
        for (int i = 0; i < objects.Count; i++)
        {
             objects[i].UiObject.SetActive(false);
        }

        state = CollectableState.Deactivated;
    }

    private class ObjectData
    {
        public Vector3 WorldPosition { get; set; }

        public int Direction { get; private set; }

        public GameObject UiObject { get; private set; }
        

        public ObjectData(int direction, Vector3 worldPosition, GameObject uiObject)
        {
            this.Direction = direction;
            this.WorldPosition = worldPosition;
            this.UiObject = uiObject;
        }
    }
    
    public enum CollectableState
    {
        Deactivated,
        Collectable,
        Collected,
        Enabled
    }
}