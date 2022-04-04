using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

// Vectical: (-1, 1), Horizontal: (1, 1)


/// <summary>
/// Another way: use two axis mapping.
/// </summary>

public class MapTest : MonoBehaviour, IDragHandler, IBeginDragHandler {
    private Vector2 current;
    private Vector2 previouse;
    private Vector2 delta;
    private Vector2 target;

    //void Update() {
        //   Transform cubes = GameObject.Find("Cubes").transform;
        //   cubes.Translate(this.delta);
       // StartCoroutine(this.SetCamera());
    //}
    // 502, 492
    public void OnBeginDrag(PointerEventData eventData) {
        this.previouse = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        // Debug.LogError("OnDrag");
        Vector2 currentPoint = Vector2.zero;
        Vector2 previousePoint = Vector2.zero;
        this.current = eventData.position;
        Ray ray = Camera.main.ScreenPointToRay(current);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            currentPoint = (Vector2)hit.point;
        }
        ray = Camera.main.ScreenPointToRay(previouse);
        if (Physics.Raycast(ray, out hit, 100)) {
            previousePoint = (Vector2)hit.point;
        }
        this.delta += currentPoint - previousePoint;
        this.previouse = this.current;
    }

    void LateUpdate() {
        Camera.main.transform.Translate(-this.delta);
        if (this.delta != Vector2.zero) {
            Debug.LogError(this.delta);
        }
        this.delta = Vector2.zero;
    }
}