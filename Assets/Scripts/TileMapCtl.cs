using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileTerrain.Component;

public class TileMapCtl : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }
    RaycastHit hit;
    Ray ray;
    private int currentMaterial=1;
    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Map.instance.InitinalMap();
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit)) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            Map.instance.mapAction = MapAction.Raise;
            Map.instance.ChangeGroundHeight(hit.point,5f,2f);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Map.instance.mapAction = MapAction.Down;
            Map.instance.ChangeGroundHeight(hit.point, 5f, -2f);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            currentMaterial -= 1;
            if (currentMaterial==0) {
                currentMaterial = 5;
            }
            Map.instance.ChangeMaterial(currentMaterial);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            currentMaterial += 1;
            if (currentMaterial == 6) {
                currentMaterial = 1;
            }
            Map.instance.ChangeMaterial(currentMaterial);
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            Map.instance.mapAction = MapAction.Spread;
            Map.instance.UpdateMaterial(hit.point, 5f, currentMaterial);
        }
    }
}
