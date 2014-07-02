using UnityEngine;
using System.Collections;

public static class Mathc {

	//returns -1 if value is negative, 1 if it is positive or 0
	public static int sign (float value) {
		if (value < 0) {
			return -1;
		}
		else return 1;
	}

	//returns 1 if clockwise, -1 if counterclockwise
	public static int clockwisePrefix (Vector3 axis, Vector3 direction, Vector3 targetDirection) {
		if (Vector3.Angle(Vector3.Cross(targetDirection, direction), axis) < 90) {
			return 1;
		}
		else {
			return -1;
		}
	}

}
