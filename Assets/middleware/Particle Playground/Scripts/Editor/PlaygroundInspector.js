#pragma strict

import System;
import System.Collections.Generic;

@CustomEditor (Playground)
class PlaygroundInspector extends Editor {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static var playgroundScriptReference : Playground;
	static var playground : SerializedObject;						// Playground
	static var calculate : SerializedProperty;
	static var pixelFilterMode : SerializedProperty;
	static var garbageCollectOnResize : SerializedProperty;
	static var autoGroup : SerializedProperty;
	static var buildZeroAlphaPixels : SerializedProperty;
	static var drawGizmos : SerializedProperty;
	static var paintToolbox : SerializedProperty;
	static var showShuriken : SerializedProperty;
	
	static var particleSystems : SerializedProperty;
	static var manipulators : SerializedProperty;
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundInspector variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static var playgroundFoldout : boolean = true;
	static var particleSystemsFoldout : boolean = false;
	static var manipulatorsFoldout : boolean = false;
	static var advancedSettingsFoldout : boolean = false;
	static var manipulatorListFoldout : List.<boolean>;
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Internal variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	static var manipulatorHandlePosition : Vector3;
	static var maximumAllowedManipulatorSize : float = 100;
	static var maximumAllowedManipulatorStrength : float = 100;
	
	function OnEnable () {
		Initialize(target as Playground);
	}
	
	static function Initialize (targetRef : Playground) {
		if (playgroundScriptReference==null) return;
		playgroundScriptReference = targetRef;
		playground = new SerializedObject(playgroundScriptReference);
		particleSystems = playground.FindProperty("particleSystems");
		manipulators = playground.FindProperty("manipulators");
		calculate = playground.FindProperty("calculate");
		pixelFilterMode = playground.FindProperty("pixelFilterMode");
		garbageCollectOnResize = playground.FindProperty("garbageCollectOnResize");
		autoGroup = playground.FindProperty("autoGroup");
		buildZeroAlphaPixels = playground.FindProperty("buildZeroAlphaPixels");
		drawGizmos = playground.FindProperty("drawGizmos");
		paintToolbox = playground.FindProperty("paintToolbox");
		showShuriken = playground.FindProperty("showShuriken");
		
		manipulatorListFoldout = new List.<boolean>();
		manipulatorListFoldout.AddRange(new boolean[playgroundScriptReference.manipulators.Count]);
	}
	
	override function OnInspectorGUI () {
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Particle Playground v"+Playground.version.ToString(), EditorStyles.largeLabel, GUILayout.Height(20));
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Open Playground Wizard", EditorStyles.toolbarButton, GUILayout.Width(130))) {
			PlaygroundParticleWindow.ShowWindow();
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		RenderPlaygroundSettings();
		
		if (Event.current.type == EventType.ValidateCommand &&
			Event.current.commandName == "UndoRedoPerformed") {
			for (var p in playgroundScriptReference.particleSystems) {
				p.Start();
			}
		}
		
	}
	
	function OnSceneGUI () {
		if (playgroundScriptReference!=null && playgroundScriptReference.drawGizmos && manipulatorsFoldout)
			for (var i = 0; i<playgroundScriptReference.manipulators.Count; i++)
				RenderManipulatorInScene(playgroundScriptReference.manipulators[i], Color(.4,.6,1.0,1.0));
		if (GUI.changed)
            EditorUtility.SetDirty (target);
	}
	
	static function RenderManipulatorInScene (thisManipulator : ManipulatorObject, manipulatorColor : Color) {
		// Draw Manipulators in Scene View
		var id : int;
		if (thisManipulator.transform) {
			Handles.color = Color(manipulatorColor.r,manipulatorColor.g,manipulatorColor.b,Mathf.Clamp(Mathf.Abs(thisManipulator.strength),.25,1.0));
			Handles.color = thisManipulator.enabled? Handles.color : Color(manipulatorColor.r,manipulatorColor.g,manipulatorColor.b,.2);
			
			// Position
			if (Tools.current==Tool.Move)
				thisManipulator.transform.position = Handles.PositionHandle(thisManipulator.transform.position, Tools.pivotRotation==PivotRotation.Global? Quaternion.identity : thisManipulator.transform.rotation);
			// Rotation
			else if (Tools.current==Tool.Rotate)
				thisManipulator.transform.rotation = Handles.RotationHandle(thisManipulator.transform.rotation, thisManipulator.transform.position);
			// Scale
			else if (Tools.current==Tool.Scale)
				thisManipulator.transform.localScale = Handles.ScaleHandle(thisManipulator.transform.localScale, thisManipulator.transform.position, thisManipulator.transform.rotation, HandleUtility.GetHandleSize(thisManipulator.transform.position));
			
			id = GUIUtility.GetControlID(FocusType.Passive);
			
			// Sphere Size
			if (thisManipulator.shape==MANIPULATORSHAPE.Sphere) {
				thisManipulator.size = Handles.RadiusHandle (Quaternion.identity, thisManipulator.transform.position, thisManipulator.size);
				if (thisManipulator.enabled && GUIUtility.hotControl>0)
					Handles.Label(thisManipulator.transform.position+Vector3(thisManipulator.size+1,1,0), "Size: "+thisManipulator.size.ToString("f2"));
			
			// Box Bounds
			} else {
				DrawManipulatorBox(thisManipulator);
			}
			
			// Strength
			manipulatorHandlePosition = thisManipulator.transform.position+Vector3(0,thisManipulator.strength,0);
			
			Handles.DrawLine(thisManipulator.transform.position, manipulatorHandlePosition);
			id = GUIUtility.GetControlID(FocusType.Passive);
			thisManipulator.strength = Handles.ScaleValueHandle(thisManipulator.strength, manipulatorHandlePosition, Quaternion.identity, HandleUtility.GetHandleSize(manipulatorHandlePosition), Handles.SphereCap, 1);      
			if (thisManipulator.enabled && GUIUtility.hotControl>0)
				Handles.Label(manipulatorHandlePosition+Vector3(1,1,0), "Strength: "+thisManipulator.strength.ToString("f2"));
			
			Handles.color = Color(.4,.6,1.0,.025);
			Handles.DrawSolidDisc(thisManipulator.transform.position, Camera.current.transform.forward, thisManipulator.strength);
			Handles.color = Color(.4,.6,1.0,.5);
			Handles.DrawSolidDisc(thisManipulator.transform.position, Camera.current.transform.forward, HandleUtility.GetHandleSize(thisManipulator.transform.position)*.05);
		}
		
	}
	
	// Draws a Manipulator bounding box with handles in scene view
	static function DrawManipulatorBox (manipulator : ManipulatorObject) {
		var boxFrontTopLeft		: Vector3;
		var boxFrontTopRight	: Vector3;
		var boxFrontBottomLeft	: Vector3;
		var boxFrontBottomRight	: Vector3;
		var boxBackTopLeft		: Vector3;
		var boxBackTopRight		: Vector3;
		var boxBackBottomLeft	: Vector3;
		var boxBackBottomRight	: Vector3;
		var boxFrontDot : Vector3;
		var boxLeftDot : Vector3;
		var boxUpDot : Vector3;
		
		// Always set positive values of bounds
		manipulator.bounds.extents.x = Mathf.Abs(manipulator.bounds.extents.x);
		manipulator.bounds.extents.y = Mathf.Abs(manipulator.bounds.extents.y);
		manipulator.bounds.extents.z = Mathf.Abs(manipulator.bounds.extents.z);
		
		// Set positions from bounds
		boxFrontTopLeft 		= Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontTopRight 		= Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontBottomLeft 		= Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontBottomRight 	= Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxBackTopLeft 			= Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackTopRight 		= Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackBottomLeft 		= Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackBottomRight 		= Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		
		boxFrontDot				= Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y, manipulator.bounds.center.z);
		boxUpDot				= Vector3(manipulator.bounds.center.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z);
		boxLeftDot				= Vector3(manipulator.bounds.center.x, manipulator.bounds.center.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
				
		// Apply transform positioning
		boxFrontTopLeft			= manipulator.transform.TransformPoint(boxFrontTopLeft);
		boxFrontTopRight		= manipulator.transform.TransformPoint(boxFrontTopRight);
		boxFrontBottomLeft		= manipulator.transform.TransformPoint(boxFrontBottomLeft);
		boxFrontBottomRight		= manipulator.transform.TransformPoint(boxFrontBottomRight);
		boxBackTopLeft			= manipulator.transform.TransformPoint(boxBackTopLeft);
		boxBackTopRight			= manipulator.transform.TransformPoint(boxBackTopRight);
		boxBackBottomLeft		= manipulator.transform.TransformPoint(boxBackBottomLeft);
		boxBackBottomRight		= manipulator.transform.TransformPoint(boxBackBottomRight);
		
		boxFrontDot				= manipulator.transform.TransformPoint(boxFrontDot);
		boxLeftDot				= manipulator.transform.TransformPoint(boxLeftDot);
		boxUpDot				= manipulator.transform.TransformPoint(boxUpDot);
				
		// Draw front lines
		Handles.DrawLine(boxFrontTopLeft, boxFrontTopRight);
		Handles.DrawLine(boxFrontTopRight, boxFrontBottomRight);
		Handles.DrawLine(boxFrontBottomLeft, boxFrontTopLeft);
		Handles.DrawLine(boxFrontBottomRight, boxFrontBottomLeft);
		
		// Draw back lines
		Handles.DrawLine(boxBackTopLeft, boxBackTopRight);
		Handles.DrawLine(boxBackTopRight, boxBackBottomRight);
		Handles.DrawLine(boxBackBottomLeft, boxBackTopLeft);
		Handles.DrawLine(boxBackBottomRight, boxBackBottomLeft);
		
		// Draw front to back lines
		Handles.DrawLine(boxFrontTopLeft, boxBackTopLeft);
		Handles.DrawLine(boxFrontTopRight, boxBackTopRight);
		Handles.DrawLine(boxFrontBottomLeft, boxBackBottomLeft);
		Handles.DrawLine(boxFrontBottomRight, boxBackBottomRight);
		
		// Draw extents handles
		boxFrontDot = Handles.Slider(boxFrontDot, manipulator.transform.right, HandleUtility.GetHandleSize(boxFrontDot)*.03, Handles.DotCap, .0);
		boxUpDot = Handles.Slider(boxUpDot, manipulator.transform.up, HandleUtility.GetHandleSize(boxUpDot)*.03, Handles.DotCap, .0);
		boxLeftDot = Handles.Slider(boxLeftDot, manipulator.transform.forward, HandleUtility.GetHandleSize(boxLeftDot)*.03, Handles.DotCap, .0);
		
		manipulator.bounds.extents.x = manipulator.transform.InverseTransformPoint(boxFrontDot).x-manipulator.bounds.center.x;
		manipulator.bounds.extents.y = manipulator.transform.InverseTransformPoint(boxUpDot).y-manipulator.bounds.center.y;
		manipulator.bounds.extents.z = manipulator.transform.InverseTransformPoint(boxLeftDot).z-manipulator.bounds.center.z;		
	}
	
	static function RenderPlaygroundSettings () {
		EditorGUILayout.BeginVertical("box");
		if (GUILayout.Button((playgroundFoldout?"▼":"►")+" Playground Manager ", EditorStyles.label)) playgroundFoldout=!playgroundFoldout;
		if (playgroundFoldout) {
		
		EditorGUILayout.BeginVertical("box");
		if (playgroundScriptReference==null) {
			 playgroundScriptReference = GameObject.FindObjectOfType(Playground);
			if (playgroundScriptReference)
				Initialize(playgroundScriptReference);
		}
		
		if (playgroundFoldout && playgroundScriptReference!=null) {
			playground.Update();
			
			// Particle System List
			if (GUILayout.Button("Particle Systems ("+playgroundScriptReference.particleSystems.Count+")", EditorStyles.toolbarDropDown)) particleSystemsFoldout=!particleSystemsFoldout;
			if (particleSystemsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (playgroundScriptReference.particleSystems.Count>0) {
					for (var ps = 0; ps<playgroundScriptReference.particleSystems.Count; ps++) {
						
						EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(26));
						EditorGUILayout.BeginHorizontal();
						
						if (playgroundScriptReference.particleSystems[ps].particleSystemGameObject == Selection.activeGameObject) GUILayout.BeginHorizontal("box");
						
						GUILayout.Label(ps.ToString(), EditorStyles.miniLabel, [GUILayout.Width(18)]);
						if (GUILayout.Button(playgroundScriptReference.particleSystems[ps].particleSystemGameObject.name, EditorStyles.label)) {
							Selection.activeGameObject = playgroundScriptReference.particleSystems[ps].particleSystemGameObject;
						}
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundScriptReference.particleSystems.Count>1);
						if(GUILayout.Button("↑", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							particleSystems.MoveArrayElement(ps, ps==0?playgroundScriptReference.particleSystems.Count-1:ps-1);
						}
						if(GUILayout.Button("↓", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							particleSystems.MoveArrayElement(ps, ps<playgroundScriptReference.particleSystems.Count-1?ps+1:0);
						}
						GUI.enabled = true;
						
						// Clone
						if(GUILayout.Button("+", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							var ppsDuplicate : PlaygroundParticles = Instantiate(playgroundScriptReference.particleSystems[ps].particleSystemGameObject, playgroundScriptReference.particleSystems[ps].particleSystemTransform.position, playgroundScriptReference.particleSystems[ps].particleSystemTransform.rotation).GetComponent(PlaygroundParticles);
							
							// Cache state data
							for (var x = 0; x<ppsDuplicate.states.Count; x++)
								ppsDuplicate.states[x].Initialize();
							
							// Set particle count to initiate all arrays
							Playground.SetParticleCount(ppsDuplicate, ppsDuplicate.particleCount);
							
							// Add this to Manager
							if (Playground.reference!=null) {
								Playground.particlesQuantity++;
								Playground.reference.particleSystems.Add(ppsDuplicate);
								ppsDuplicate.particleSystemId = Playground.particlesQuantity;
								if (Playground.reference.autoGroup && ppsDuplicate.particleSystemTransform.parent==null)
									ppsDuplicate.particleSystemTransform.parent = Playground.referenceTransform;
							}
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							if (EditorUtility.DisplayDialog(
								"Remove "+playgroundScriptReference.particleSystems[ps].particleSystemGameObject.name+"?",
								"Are you sure you want to remove this Particle Playground System?", 
								"Yes", "No")) {
									if (Selection.activeGameObject==playgroundScriptReference.particleSystems[ps].particleSystemGameObject)
										Selection.activeGameObject = Playground.referenceTransform.gameObject;
									Playground.Destroy(playgroundScriptReference.particleSystems[ps]);
									playground.ApplyModifiedProperties();
									return;
								}
						}
						
						if (playgroundScriptReference.particleSystems[ps].particleSystemGameObject == Selection.activeGameObject) GUILayout.EndHorizontal();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
				} else {
					EditorGUILayout.HelpBox("No particle systems created.", MessageType.Info);
				}
				
				if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
					Playground.Particle();
				}
				
				EditorGUILayout.Separator();
			}
			
			// Manipulators
			if (GUILayout.Button("Global Manipulators ("+playgroundScriptReference.manipulators.Count+")", EditorStyles.toolbarDropDown)) manipulatorsFoldout=!manipulatorsFoldout;
			if (manipulatorsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (manipulators.arraySize>0) {
									
					
					for (var i = 0; i<manipulators.arraySize; i++) {
						var mName : String;
						if (playgroundScriptReference.manipulators[i].transform) {
							mName = playgroundScriptReference.manipulators[i].transform.name;
							if (mName.Length>24)
								mName = mName.Substring(0, 24)+"…";
						} else mName = "(Missing Transform!)";
						
						EditorGUILayout.BeginVertical("box");
						
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						if (GUILayout.Button(((manipulatorListFoldout[i]?"▼ ":"► ")+ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)), EditorStyles.label, GUILayout.Width(Screen.width/4))) manipulatorListFoldout[i]=!manipulatorListFoldout[i];
						if (playgroundScriptReference.manipulators[i].transform) {
							if (GUILayout.Button(" ("+mName+")", EditorStyles.label)) {
								Selection.activeGameObject = playgroundScriptReference.manipulators[i].transform.gameObject;
							}
						} else {
							GUILayout.Button(ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)+" (Missing Transform!)", EditorStyles.label);
						}
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundScriptReference.manipulators.Count>1);
						if(GUILayout.Button("↑", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							manipulators.MoveArrayElement(i, i==0?playgroundScriptReference.manipulators.Count-1:i-1);
						}
						if(GUILayout.Button("↓", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							manipulators.MoveArrayElement(i, i<playgroundScriptReference.manipulators.Count-1?i+1:0);
						}
						GUI.enabled = true;
						if(GUILayout.Button("+", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							Playground.reference.manipulators.Add(playgroundScriptReference.manipulators[i].Clone());
							manipulatorListFoldout.Add(manipulatorListFoldout[i]);
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, [GUILayout.Width(18), GUILayout.Height(16)])){
							
							if (EditorUtility.DisplayDialog(
								"Remove "+ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)+" Manipulator "+i+"?",
								"Are you sure you want to remove the Manipulator assigned to "+mName+"? (GameObject in Scene will remain intact)", 
								"Yes", "No")) {
									manipulators.DeleteArrayElementAtIndex(i);
									manipulatorListFoldout.RemoveAt(i);
									playground.ApplyModifiedProperties();
									return;
								}
						}
						
						EditorGUILayout.EndHorizontal();
						
						if (manipulatorListFoldout[i] && i<manipulators.arraySize) {
							RenderManipulatorSettings(playgroundScriptReference.manipulators[i], manipulators.GetArrayElementAtIndex(i), true);
						}
						GUI.enabled = true;
						EditorGUILayout.Separator();
						EditorGUILayout.EndVertical();
					}
					
				} else {
					EditorGUILayout.HelpBox("No manipulators created.", MessageType.Info);
				}
				
				if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
					if (Selection.gameObjects.Length>0 && Selection.activeGameObject.transform && Selection.activeTransform!=null)
						Playground.ManipulatorObject(Selection.activeGameObject.transform);
					else
						manipulators.InsertArrayElementAtIndex(manipulators.arraySize);
					manipulatorListFoldout.Add(true);
					SceneView.RepaintAll();
				}
					
				
				EditorGUILayout.Separator();
								
			}
			
			// Advanced Settings
			if (GUILayout.Button("Advanced", EditorStyles.toolbarDropDown)) advancedSettingsFoldout=!advancedSettingsFoldout;
			if (advancedSettingsFoldout) {
				
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(calculate, new GUIContent("Calculate Particles", "Calculate forces on PlaygroundParticles objects. Disabling this overrides independently set values and halts all PlaygroundParticles objects."));
				EditorGUILayout.PropertyField(garbageCollectOnResize, new GUIContent("Garbage Collection", "Issue a GC.Collect when resizing particle lists."));
				EditorGUILayout.PropertyField(autoGroup, new GUIContent("Group Automatically", "Automatically parent a PlaygroundParticles object to Playground Manager if it has no parent."));
				EditorGUILayout.PropertyField(buildZeroAlphaPixels, new GUIContent("Build Zero Alpha Pixels", "Turn this on if you want to build particles from 0 alpha pixels into states."));
				EditorGUILayout.PropertyField(drawGizmos, new GUIContent("Scene Gizmos", "Show gizmos in Scene View for Playground objects."));
				EditorGUILayout.PropertyField(paintToolbox, new GUIContent("Paint Toolbox", "Show Paint toolbox in Scene View when Source is set to Paint"));
				EditorGUILayout.PropertyField(showShuriken, new GUIContent("Show Shuriken", "Show the Shuriken component in Inspector."));
				EditorGUILayout.PropertyField(pixelFilterMode, new GUIContent("Pixel Filter Mode", "Color filtering mode when creating particles from pixels in an image."));
				EditorGUILayout.Separator();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Time Simulation");
				// Time reset
				if(GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(50))){
					Playground.TimeReset();
					for (var p = 0; p<playgroundScriptReference.particleSystems.Count; p++)
						playgroundScriptReference.particleSystems[p].Start();
				}
				
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Separator();
				
			}
			
			EditorGUI.indentLevel--;
			
			playground.ApplyModifiedProperties();
			EditorGUILayout.EndVertical();
		} else {
			EditorGUILayout.HelpBox("The Playground Manager runs all Particle Playground Systems in the scene, you need to create one in your scene to get started.", MessageType.Info);
			if(GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(50))){
				var managerInit : GameObject = Playground.ResourceInstantiate("Playground Manager");
				DestroyImmediate(managerInit);
				Playground.ResourceInstantiate("Playground Manager");
			}
			EditorGUILayout.EndVertical();
		}
		
	}
		EditorGUILayout.EndVertical();
	}
	
	static function RenderManipulatorSettings (thisManipulator : ManipulatorObject, serializedManipulator : SerializedProperty, isPlayground : boolean) {
		var serializedManipulatorAffects : SerializedProperty = serializedManipulator.FindPropertyRelative("affects");
		var serializedManipulatorSize : SerializedProperty;
		var serializedManipulatorStrength : SerializedProperty;
		thisManipulator.enabled = EditorGUILayout.ToggleLeft("Enabled", thisManipulator.enabled);
		EditorGUI.indentLevel++;
		GUI.enabled = thisManipulator.enabled;
		
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("transform"), new GUIContent("Transform"));
		if (thisManipulator.transform) {
			EditorGUI.indentLevel++;
			thisManipulator.transform.position = EditorGUILayout.Vector3Field("Position", thisManipulator.transform.position);
			thisManipulator.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", thisManipulator.transform.rotation.eulerAngles));
			thisManipulator.transform.localScale = EditorGUILayout.Vector3Field("Scale", thisManipulator.transform.localScale);
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("type"), new GUIContent("Type"));
		
		// Render properties
		if (thisManipulator.type==MANIPULATORTYPE.Property) {
			if (!thisManipulator.property) thisManipulator.property = new ManipulatorProperty();
			
			thisManipulator.property.type = System.Convert.ToInt32(EditorGUILayout.EnumPopup("Property Type", thisManipulator.property.type));
			
			// Velocity
			if (thisManipulator.property.type==MANIPULATORPROPERTYTYPE.Velocity || thisManipulator.property.type==MANIPULATORPROPERTYTYPE.AdditiveVelocity) {
				thisManipulator.property.velocity = EditorGUILayout.Vector3Field("Property Velocity", thisManipulator.property.velocity);
				thisManipulator.property.useLocalRotation = EditorGUILayout.Toggle("Local Rotation", thisManipulator.property.useLocalRotation);
			} else 
			// Color
			if (thisManipulator.property.type==MANIPULATORPROPERTYTYPE.Color) {
				thisManipulator.property.color = EditorGUILayout.ColorField("Property Color", thisManipulator.property.color);
				thisManipulator.property.onlyColorInRange = EditorGUILayout.Toggle("Only Color In Range", thisManipulator.property.onlyColorInRange);
				thisManipulator.property.keepColorAlphas = EditorGUILayout.Toggle("Keep Color Alphas", thisManipulator.property.keepColorAlphas);
			} else 
			if (thisManipulator.property.type==MANIPULATORPROPERTYTYPE.Size) {
				thisManipulator.property.size = EditorGUILayout.FloatField("Property Size", thisManipulator.property.size);
			}
			
			// Transition
			if (thisManipulator.property.type!=MANIPULATORPROPERTYTYPE.None) {
				thisManipulator.property.transition = System.Convert.ToInt32(EditorGUILayout.EnumPopup("Transition", thisManipulator.property.transition));
			}
		}
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("shape"), new GUIContent("Shape"));
		
		if (thisManipulator.shape==MANIPULATORSHAPE.Sphere) {
			serializedManipulatorSize = serializedManipulator.FindPropertyRelative("size");
			serializedManipulatorSize.floatValue = EditorGUILayout.Slider("Size", serializedManipulatorSize.floatValue, 0, maximumAllowedManipulatorSize);
		} else {
			EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("bounds"), new GUIContent("Bounds"));
		}
		EditorGUILayout.Separator();
		serializedManipulatorStrength = serializedManipulator.FindPropertyRelative("strength");
		serializedManipulatorStrength.floatValue = EditorGUILayout.Slider("Strength", serializedManipulatorStrength.floatValue, 0, maximumAllowedManipulatorStrength);
		
		if (isPlayground) {
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedManipulatorAffects, new GUIContent("Affects"));
		}
		
		EditorGUI.indentLevel--;
	}
	
	// Return name of a MANIPULATORTYPE
	static function ManipulatorTypeName (mType : MANIPULATORTYPE) : String {
		var returnString : String;
		switch (mType) {
			case MANIPULATORTYPE.None: returnString = "None"; break;
			case MANIPULATORTYPE.Attractor: returnString = "Attractor"; break;
			case MANIPULATORTYPE.AttractorGravitational: returnString = "Gravitational"; break;
			case MANIPULATORTYPE.Repellent: returnString = "Repellent"; break;
			case MANIPULATORTYPE.Property: returnString = "Property"; break;
			default: returnString = "Manipulator"; break;
		}
		return returnString;
	}
	
	// Return name of a MANIPULATORSHAPE
	static function ManipulatorTypeName (mShape : MANIPULATORSHAPE) : String {
		var returnString : String;
		switch (mShape) {
			case MANIPULATORSHAPE.Sphere: returnString = "Sphere"; break;
			case MANIPULATORSHAPE.Box: returnString = "Box"; break;
			default: returnString = "Null"; break;
		}
		return returnString;
	}
	
}