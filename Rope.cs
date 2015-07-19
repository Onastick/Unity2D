using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class Rope : MonoBehaviour {

	Rigidbody2D[] rigidbodies;
	JointMotor2D motor;
	JointAngleLimits2D limits;
	Vector2 anchorDistance;


	public Sprite SpriteImage;
	public Material Material;
	public int RopeLength;
	public string SortingLayer = "Default";

	[Header("Rigidbody2D")]
	public bool DecrementMassValue;
	public double DecrementValue;
	public double Mass = 1;
	public float LinearDrag = 0;
	public float AngularDrag = 0.005F;
	public float GravityScale = 1;
	public bool FixedAngle;
	public bool isKinematic;
	public RigidbodyInterpolation2D Interpolate = RigidbodyInterpolation2D.None;
	public RigidbodySleepMode2D SleepingMode = RigidbodySleepMode2D.StartAsleep;
	public CollisionDetectionMode2D CollisionDetection = CollisionDetectionMode2D.None;

	[Header("Hinge2D")]
	public bool EnableCollision;
	public Rigidbody2D BaseRigidBody;
	public bool UseAutoAnchor = true;
	public float anchorOffset;
	public Vector2 Anchor;
	public Vector2 ConnectedAnchor;
	public bool UseMotor;
	public float MotorSpeed;
	public float MaximumMotorForce;
	public bool UseLimit = true;
	public float LowerAngle = 45;
	public float UpperAngle = -45;

	public void GenerateRope()
	{
		if(this.gameObject.GetComponent<HingeJoint2D>() == null){
			this.gameObject.AddComponent<HingeJoint2D>();
		}
		BaseRigidBody = this.GetComponent<Rigidbody2D>();

		Vector3 distance = new Vector3(0,0,0);
		for(int n = 0; n < RopeLength; n++)
		{

			GameObject RopeObject = new GameObject (SpriteImage.name, typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(HingeJoint2D)) as GameObject;
			SpriteRenderer Rope = RopeObject.GetComponent<SpriteRenderer>();
			Rope.sprite = SpriteImage;
			Rope.sortingLayerName = SortingLayer;
			Rope.material = Material;
			if(n==0){
				RopeObject.transform.SetParent (this.transform);
				RopeObject.transform.localPosition = Vector3.zero;
			}
			else
			{
				//Use this code if you wish to have ladder parenting for all new objects. Anchors may not set properly.
				//RopeObject.transform.SetParent (this.gameObject.GetComponentsInChildren<SpriteRenderer>().ElementAt(n).transform);
				//distance.y = -DistanceBetweenRopePiece;

				RopeObject.transform.SetParent (this.transform);
				distance.y = -(Rope.bounds.size.y * n);

				RopeObject.transform.localPosition = distance;
			}
			RopeObject.name = SpriteImage.name + " " + n;

		}
		Debug.Log ("Rope generated!");
	}

	public void ApplyRigidbody2D() 
	{
		int n = 0;
		var bodies = gameObject.GetComponentsInChildren<Rigidbody2D>(true);
		foreach(var rigidbody in bodies)
		{
			if(n!=0){

				#region "Update Rigidbody2D Components"
				rigidbody.mass = (float)Mass;
				rigidbody.drag = LinearDrag;
				rigidbody.angularDrag = AngularDrag;
				rigidbody.gravityScale = GravityScale;
				rigidbody.fixedAngle = FixedAngle;
				rigidbody.isKinematic = isKinematic;
				rigidbody.interpolation = Interpolate;
				rigidbody.sleepMode = SleepingMode;
				rigidbody.collisionDetectionMode = CollisionDetection;
				#endregion

				if(DecrementMassValue){
					Mass -= DecrementValue;
				}
			}
			else{
				rigidbody.fixedAngle = true;
				rigidbody.isKinematic = true;
			}
			n++;
		}
		Debug.Log ("Rigidbody components updated.");
	}
	public void ApplyHinge2D()
	{
		rigidbodies = this.gameObject.GetComponentsInChildren<Rigidbody2D>(true);
		int n = 0;
		var hinges = this.gameObject.GetComponentsInChildren<HingeJoint2D>(true);

		foreach(var hinge in hinges)
		{
			hinge.enableCollision = EnableCollision;
			if(!UseAutoAnchor){
				hinge.anchor = Anchor;
				hinge.connectedAnchor = ConnectedAnchor;
			}

			#region Update Hinge2D Components"
			motor.motorSpeed = MotorSpeed;
			motor.maxMotorTorque = MaximumMotorForce;
			hinge.motor = motor;
			hinge.useMotor = UseMotor;

			limits.min = LowerAngle;
			limits.max = UpperAngle;
			hinge.limits = limits;
			hinge.useLimits = UseLimit;
			#endregion
			if(n!=0){
				hinge.connectedBody = rigidbodies[n-1];
			}
			else
				hinge.connectedBody = null;
			n++;
		}

		if(UseAutoAnchor){
			AutoAnchor();
		}
		Debug.Log ("Hinge2D components updated.");
	}

	private void AutoAnchor(){
		int n = 0;

		var hinges = this.gameObject.GetComponentsInChildren<HingeJoint2D>(true);
		foreach(var hinge in hinges)
		{
			anchorDistance.y = this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.y/2 - anchorOffset;
			if(n==0){
				Debug.Log ("Base anchor skipped.");
			}
			if(n==1){
				hinge.anchor = anchorDistance;
				hinge.connectedAnchor = anchorDistance;
			}
			if(n>=2){
				hinge.anchor = anchorDistance;
				anchorDistance.y = -anchorDistance.y;
				hinge.connectedAnchor = anchorDistance;
			}
			n++;
		}
	}
}
