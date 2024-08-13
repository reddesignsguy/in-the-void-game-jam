//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GravitySelector : MonoBehaviour
//{
//    private List<GameObject> _directionGameObjects;

//    private void Awake()
//    {

//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        foreach (Transform child in transform)
//        {
//            _directionGameObjects.Add(child.gameObject);
//        }

//        setVisible(false);
//    }

    
//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    public void setVisible(bool visible)
//    {
//        foreach (GameObject dir in _directionGameObjects)
//        {
//            SpriteRenderer _renderer = dir.GetComponent<SpriteRenderer>();
//            _renderer.color *= 0;
//        }
//    }
//}
