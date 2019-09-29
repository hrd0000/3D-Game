﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

namespace Com.Mygame {
	
	public class Director : System.Object {
		private static Director _instance;
		public SceneController currentSceneController { get; set; }

		public static Director getInstance() {
			if (_instance == null) {
				_instance = new Director ();
			}
			return _instance;
		}
	}
	public class Judger : MonoBehaviour {
		public CoastController fromCoast;
		public CoastController toCoast;
		public BoatController boat;

		public Judger(CoastController from,CoastController to, BoatController b) {
			fromCoast = from;
			toCoast = to;
			boat = b;
		}

		public int check() {   // 0->not finish, 1->lose, 2->win
			int from_priest = 0;
			int from_devil = 0;
			int to_priest = 0;
			int to_devil = 0;

			int[] fromCount = fromCoast.getCharacterNum();
			from_priest += fromCount[0];
			from_devil += fromCount[1];

			int[] toCount = toCoast.getCharacterNum();
			to_priest += toCount[0];
			to_devil += toCount[1];

			if (to_priest + to_devil == 6)      // win
				return 2;

			int[] boatCount = boat.getCharacterNum();
			if (boat.get_to_or_from() == -1) {   // boat at toCoast
				to_priest += boatCount[0];
				to_devil += boatCount[1];
			} else {   // boat at fromCoast
				from_priest += boatCount[0];
				from_devil += boatCount[1];
			}
			if (from_priest < from_devil && from_priest > 0) {       // lose
				return 1;
			}
			if (to_priest < to_devil && to_priest > 0) {
				return 1;
			}
			return 0;           // not finish
		}
	}

	public interface SceneController {
		void loadResources ();
	}

	public interface UserAction {
		void moveBoat();
		void characterIsClicked(MyCharacterController characterCtrl);
		void restart();
	}

	/*-----------------------------------Moveable------------------------------------------*/
	/* old way to move
	public class Moveable: MonoBehaviour {
		
		readonly float move_speed = 20;

		// change frequently
		int moving_status;	// 0->not moving, 1->moving to middle, 2->moving to dest
		Vector3 dest;
		Vector3 middle;

		void Update() {
			if (moving_status == 1) {
				transform.position = Vector3.MoveTowards (transform.position, middle, move_speed * Time.deltaTime);
				if (transform.position == middle) {
					moving_status = 2;
				}
			} else if (moving_status == 2) {
				transform.position = Vector3.MoveTowards (transform.position, dest, move_speed * Time.deltaTime);
				if (transform.position == dest) {
					moving_status = 0;
				}
			}
		}
		public void setDestination(Vector3 _dest) {
			dest = _dest;
			middle = _dest;
			if (_dest.y == transform.position.y) {	// boat moving
				moving_status = 2;
			}
			else if (_dest.y < transform.position.y) {	// character from coast to boat
				middle.y = transform.position.y;
			} else {								// character from boat to coast
				middle.x = transform.position.x;
			}
			moving_status = 1;
		}

		public void reset() {
			moving_status = 0;
		}
	}
	 */

	/*-----------------------------------MyCharacterController------------------------------------------*/
	public class MyCharacterController {
		readonly GameObject character;
		//readonly Moveable moveableScript;
		readonly ClickGUI clickGUI;
		readonly int characterType;	// 0->priest, 1->devil
		public readonly float movingSpeed = 20;

		// change frequently
		bool _isOnBoat;
		CoastController coastController;


		public MyCharacterController(string which_character) {
			
			if (which_character == "priest") {
				character = Object.Instantiate (Resources.Load ("Perfabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
				characterType = 0;
				character.GetComponent<Renderer>().material.color = Color.white;
			} else {
				character = Object.Instantiate (Resources.Load ("Perfabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
				characterType = 1;
				character.GetComponent<Renderer>().material.color = Color.red;
			}
			//moveableScript = character.AddComponent (typeof(Moveable)) as Moveable;

			clickGUI = character.AddComponent (typeof(ClickGUI)) as ClickGUI;
			clickGUI.setController (this);
		}

		public void setName(string name) {
			character.name = name;
		}

		public void setPosition(Vector3 pos) {
			character.transform.position = pos;
		}

		/*
		public void moveToPosition(Vector3 destination) {
			moveableScript.setDestination(destination);
		}
		*/

		public int getType() {	// 0->priest, 1->devil
			return characterType;
		}

		public string getName() {
			return character.name;
		}

		public void getOnBoat(BoatController boatCtrl) {
			coastController = null;
			character.transform.parent = boatCtrl.getGameobj().transform;
			_isOnBoat = true;
		}

		public void getOnCoast(CoastController coastCtrl) {
			coastController = coastCtrl;
			character.transform.parent = null;
			_isOnBoat = false;
		}

		public bool isOnBoat() {
			return _isOnBoat;
		}

		public CoastController getCoastController() {
			return coastController;
		}

		public void reset() {
			// moveableScript.reset ();
			coastController = (Director.getInstance ().currentSceneController as FirstController).fromCoast;
			getOnCoast (coastController);
			setPosition (coastController.getEmptyPosition ());
			coastController.getOnCoast (this);
		}

		public Vector3 getPos() {
			return this.character.transform.position;
		}

		public GameObject getGameobj() {
			return this.character;
		}
	}

	/*-----------------------------------CoastController------------------------------------------*/
	public class CoastController {
		readonly GameObject coast;
		readonly Vector3 from_pos = new Vector3(9,1,0);
		readonly Vector3 to_pos = new Vector3(-9,1,0);
		readonly Vector3[] positions;
		readonly int to_or_from;	// to->-1, from->1

		// change frequently
		MyCharacterController[] passengerPlaner;

		public CoastController(string _to_or_from) {
			positions = new Vector3[] {new Vector3(6.5F,2.25F,0), new Vector3(7.5F,2.25F,0), new Vector3(8.5F,2.25F,0), 
				new Vector3(9.5F,2.25F,0), new Vector3(10.5F,2.25F,0), new Vector3(11.5F,2.25F,0)};

			passengerPlaner = new MyCharacterController[6];

			if (_to_or_from == "from") {
				coast = Object.Instantiate (Resources.Load ("Perfabs/Stone", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
				coast.name = "from";
				to_or_from = 1;
			} else {
				coast = Object.Instantiate (Resources.Load ("Perfabs/Stone", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
				coast.name = "to";
				to_or_from = -1;
			}
		}

		public int getEmptyIndex() {
			for (int i = 0; i < passengerPlaner.Length; i++) {
				if (passengerPlaner [i] == null) {
					return i;
				}
			}
			return -1;
		}

		public Vector3 getEmptyPosition() {
			Vector3 pos = positions [getEmptyIndex ()];
			pos.x *= to_or_from;
			return pos;
		}

		public void getOnCoast(MyCharacterController characterCtrl) {
			int index = getEmptyIndex ();
			passengerPlaner [index] = characterCtrl;
		}

		public MyCharacterController getOffCoast(string passenger_name) {	// 0->priest, 1->devil
			for (int i = 0; i < passengerPlaner.Length; i++) {
				if (passengerPlaner [i] != null && passengerPlaner [i].getName () == passenger_name) {
					MyCharacterController charactorCtrl = passengerPlaner [i];
					passengerPlaner [i] = null;
					return charactorCtrl;
				}
			}
			Debug.Log ("cant find passenger on coast: " + passenger_name);
			return null;
		}

		public int get_to_or_from() {
			return to_or_from;
		}

		public int[] getCharacterNum() {
			int[] count = {0, 0};
			for (int i = 0; i < passengerPlaner.Length; i++) {
				if (passengerPlaner [i] == null)
					continue;
				if (passengerPlaner [i].getType () == 0) {	// 0->priest, 1->devil
					count[0]++;
				} else {
					count[1]++;
				}
			}
			return count;
		}

		public void reset() {
			passengerPlaner = new MyCharacterController[6];
		}
	}

	/*-----------------------------------BoatController------------------------------------------*/
	public class BoatController {
		readonly GameObject boat;
		//readonly Moveable moveableScript;
		readonly Vector3 fromPosition = new Vector3 (5, 0.75F, 0);
		readonly Vector3 toPosition = new Vector3 (-5, 0.75F, 0);
		readonly Vector3[] from_positions;
		readonly Vector3[] to_positions;
		public readonly float movingSpeed = 20;

		GameObject cameraObj;
		GameObject lightObj;

		// change frequently
		int to_or_from; // to->-1; from->1
		MyCharacterController[] passenger = new MyCharacterController[2];

		public BoatController() {
			to_or_from = 1;

			from_positions = new Vector3[] { new Vector3 (4.5F, 1.25F, 0), new Vector3 (5.5F, 1.25F, 0) };
			to_positions = new Vector3[] { new Vector3 (-5.5F, 1.25F, 0), new Vector3 (-4.5F, 1.25F, 0) };

			boat = Object.Instantiate (Resources.Load ("Perfabs/Boat", typeof(GameObject)), fromPosition, Quaternion.identity, null) as GameObject;
			boat.name = "boat";

			//moveableScript = boat.AddComponent (typeof(Moveable)) as Moveable;
			boat.AddComponent (typeof(ClickGUI));

			boat.GetComponent<Renderer>().material.color = new Color(0.7569F, 0.6039F, 0.4196F, 1);

			attachCamera();	// add a camera moving with this boat
			attachLight();
		}

		private void attachCamera() {
			cameraObj = new GameObject("Camera_follow");	// 新建一个空对象叫做Camera_follow
			cameraObj.transform.parent = boat.transform;	// 先让这个对象成为boat的子对象
			// 调整一下Camera_follow与boat的相对方位
			cameraObj.transform.localPosition = new Vector3(0, 7, -10);
			cameraObj.transform.localRotation = Quaternion.Euler(10, 0, 0);

			cameraObj.AddComponent<Camera>();	// 添加Camera组件，让这个空对象成为一个摄像机
			Camera cameraComp = cameraObj.GetComponent<Camera>();	// 获取摄像机组件
			cameraComp.fieldOfView = 40;

			// 将Resources/skybox添加为这个摄像机的天空盒
			cameraObj.AddComponent<Skybox>().material = Resources.Load("skybox") as Material;
		}

		private void attachLight() {
			lightObj = new GameObject("Light_follow");	
			lightObj.transform.parent = boat.transform;
			lightObj.transform.localPosition = new Vector3(0, 5, -2);
			lightObj.transform.localRotation = Quaternion.Euler(60, 0, 0);

			lightObj.AddComponent<Light>();
			Light lightComp =  lightObj.GetComponent<Light>();
			lightComp.type = LightType.Spot;		// 调整光源的类型为聚光灯
			lightComp.intensity = 4;		// 调整光源的强度
			lightComp.spotAngle = 170;		// 调整光源的照射张角
		}

		/*
		public void Move() {
			if (to_or_from == -1) {
				moveableScript.setDestination(fromPosition);
				to_or_from = 1;
			} else {
				moveableScript.setDestination(toPosition);
				to_or_from = -1;
			}
		}
		*/

		public Vector3 getDestination() {
			if (to_or_from == -1) {
				return fromPosition;
			} else {
				return toPosition;
			}
		}

		public void move() {
			if (to_or_from == -1) {
				to_or_from = 1;
			} else {
				to_or_from = -1;
			}
		}
		public int getEmptyIndex() {
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] == null) {
					return i;
				}
			}
			return -1;
		}

		public bool isEmpty() {
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] != null) {
					return false;
				}
			}
			return true;
		}

		public Vector3 getEmptyPosition() {
			Vector3 pos;
			int emptyIndex = getEmptyIndex ();
			if (to_or_from == -1) {
				pos = to_positions[emptyIndex];
			} else {
				pos = from_positions[emptyIndex];
			}
			return pos;
		}

		public void GetOnBoat(MyCharacterController characterCtrl) {
			int index = getEmptyIndex ();
			passenger [index] = characterCtrl;
		}

		public MyCharacterController GetOffBoat(string passenger_name) {
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] != null && passenger [i].getName () == passenger_name) {
					MyCharacterController charactorCtrl = passenger [i];
					passenger [i] = null;
					return charactorCtrl;
				}
			}
			Debug.Log ("Cant find passenger in boat: " + passenger_name);
			return null;
		}

		public GameObject getGameobj() {
			return boat;
		}

		public int get_to_or_from() { // to->-1; from->1
			return to_or_from;
		}

		public int[] getCharacterNum() {
			int[] count = {0, 0};
			for (int i = 0; i < passenger.Length; i++) {
				if (passenger [i] == null)
					continue;
				if (passenger [i].getType () == 0) {	// 0->priest, 1->devil
					count[0]++;
				} else {
					count[1]++;
				}
			}
			return count;
		}

		public void reset() {
			//moveableScript.reset ();
			if (to_or_from == -1) {
				move ();
			}
			boat.transform.position = fromPosition;
			passenger = new MyCharacterController[2];
		}
	}
}