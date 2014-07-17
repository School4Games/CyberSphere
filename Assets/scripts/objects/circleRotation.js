#pragma strict

var targetCube : Transform;
 
function Update() {
 
    transform.RotateAround (targetCube.transform.position, Vector3.up, 10 * Time.deltaTime);

}