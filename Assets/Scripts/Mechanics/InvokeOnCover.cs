using System.Collections;
using System.Collections.Generic;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.Animations;
using Platformer.Core;
using UnityEngine.SceneManagement;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(Collider), typeof(Invokable))]
    public class InvokeOnCover : MonoBehaviour
    {
        public Invokable invokableObject;
        GameObject playerObj;
        Collider _collider;
        public Bounds Bounds => _collider.bounds;
        public const float ActiveWaitTime = 1f;
        public bool isReady = false;
        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<Collider>();
            invokableObject = GetComponent<Invokable>();
            isReady = false;
            StartCoroutine(SetActive());
        }

        // Update is called once per frame
        void Update()
        {
            if (playerObj == null) {
                playerObj = GameObject.Find("GhostPC");
            }
            else if (isReady && IsInteractable() && Input.GetButtonDown("Jump")) {
                // Save player position
                var gm = GameManager.Instance;
                gm.PlayerPos[gm.SceneName] = playerObj.transform.position;
                invokableObject.Invoke();
            }
        }
        
        bool IsInteractable() {
            var playerBounds = playerObj.GetComponent<GhostController>().Bounds;
            if (playerBounds.Intersects(Bounds)) {
                return true;
               
            }
            else {
                return false;            
            }
        }
        
        IEnumerator SetActive()
        {
            yield return new WaitForSeconds(ActiveWaitTime);
            isReady = true;

        }
    }
}
