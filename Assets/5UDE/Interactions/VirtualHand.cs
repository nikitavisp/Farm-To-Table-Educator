﻿/*
Copyright ©2017. The University of Texas at Dallas. All Rights Reserved. 

Permission to use, copy, modify, and distribute this software and its documentation for 
educational, research, and not-for-profit purposes, without fee and without a signed 
licensing agreement, is hereby granted, provided that the above copyright notice, this 
paragraph and the following two paragraphs appear in all copies, modifications, and 
distributions. 

Contact The Office of Technology Commercialization, The University of Texas at Dallas, 
800 W. Campbell Road (AD15), Richardson, Texas 75080-3021, (972) 883-4558, 
otc@utdallas.edu, https://research.utdallas.edu/otc for commercial licensing opportunities.

IN NO EVENT SHALL THE UNIVERSITY OF TEXAS AT DALLAS BE LIABLE TO ANY PARTY FOR DIRECT, 
INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING LOST PROFITS, ARISING 
OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN IF THE UNIVERSITY OF TEXAS AT 
DALLAS HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

THE UNIVERSITY OF TEXAS AT DALLAS SPECIFICALLY DISCLAIMS ANY WARRANTIES, INCLUDING, BUT 
NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE. THE SOFTWARE AND ACCOMPANYING DOCUMENTATION, IF ANY, PROVIDED HEREUNDER IS 
PROVIDED "AS IS". THE UNIVERSITY OF TEXAS AT DALLAS HAS NO OBLIGATION TO PROVIDE 
MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
*/

using UnityEngine;
using System.Collections;

public class VirtualHand : MonoBehaviour {
	
	// Enumerate states of virtual hand interactions
	public enum VirtualHandState {
		Open,
		Touching,
		Holding,
		Closed,
		RotationLR,
		ScaleUpDown			
	};

	// Inspector parameters
	[Tooltip("The tracking device used for tracking the real hand.")]
	public CommonTracker tracker;

	[Tooltip("The interactive used to represent the virtual hand.")]
	public Affect hand;

	[Tooltip("The button required to be pressed to grab objects.")]
	public CommonButton button;

	[Tooltip("The button required to be pressed to grab objects.")]
	public CommonButton button2;

	[Tooltip("The controller joystick used to determine relative direction (forward/backward) and speed.")]
	public CommonAxis joystick;



	[Tooltip("The speed amplifier for thrown objects. One unit is physically realistic.")]
	public float speed = 1.0f;

	// Private interaction variables
	VirtualHandState state;
	FixedJoint grasp;

	// Called at the end of the program initialization
	void Start () {

		// Set initial state to Open
		state = VirtualHandState.Open;

		// Ensure hand interactive is properly configured
		hand.type = AffectType.Virtual;
	}

	// FixedUpdate is not called every graphical frame but rather every physics frame
	void FixedUpdate ()
	{
		// If state is Open
		if (state == VirtualHandState.Open) {
			
			// If the hand is touching something
			if (hand.triggerOngoing) {

				// Change state to touching
				state = VirtualHandState.Touching;
			}

			else if(button.GetPress ()){
				state=VirtualHandState.Closed;
				hand.type= AffectType.Physical;	
			}

			// Process current Open state
			else {

				// Nothing to do for Open
			}
		}
		else if( state == VirtualHandState.Closed){

				if(!button.GetPress()){
					state=VirtualHandState.Open;
					hand.type=AffectType.Virtual;
				}
	else{

					//do nothing

				}
		}
		// If state is touching
		else if (state == VirtualHandState.Touching) {

			// If the hand is not touching something
			if (!hand.triggerOngoing) {

				// Change state to Open
				state = VirtualHandState.Open;
			}

			// If the hand is touching something and the button is pressed
			else if (hand.triggerOngoing && button.GetPress ()) {

				// Fetch touched target
				Collider target = hand.ongoingTriggers [0];
				// Create a fixed joint between the hand and the target
				grasp = target.gameObject.AddComponent<FixedJoint> ();
				// Set the connection
				grasp.connectedBody = hand.gameObject.GetComponent<Rigidbody> ();

				// Change state to holding
				state = VirtualHandState.Holding;
			}

			// Process current touching state
			else {

				// Nothing to do for touching
			}
		}

		

		// If state is holding
		else if (state == VirtualHandState.Holding) {

			// If grasp has been broken
			if (grasp == null) {
				
				// Update state to Open
				state = VirtualHandState.Open;
			}
				

			else if(button.GetPress() && button2.GetPress() && grasp!=null && joystick.GetAxis().y!=0){

				//grasp.gameObject.GetComponent<Rigidbody>().transform.localScale += new Vector3(25,25,25) ;
				state=VirtualHandState.ScaleUpDown;
			
			}
			// If button has been released and grasp still exists
			else if (!button.GetPress () && grasp != null) {
				// Get rigidbody of grasped target
				Rigidbody target = grasp.GetComponent<Rigidbody> ();
				// Break grasp
				DestroyImmediate (grasp);
				// Apply physics to target in the event of attempting to throw it
				target.velocity = hand.velocity * speed;
				target.angularVelocity = hand.angularVelocity * speed;
				// Update state to Open
				state = VirtualHandState.Open;
			}


			else if(button.GetPress() && button2.GetPress() && joystick.GetAxis().x!=0){
				state = VirtualHandState.RotationLR;	

			}

			// Process current holding state
			else {

				// Nothing to do for holding
			}
		}

		else if(state == VirtualHandState.RotationLR){

			if(button2.GetPress() && grasp!=null){
				if(joystick.GetAxis().x<0.2f && joystick.GetAxis().x>0f){
					hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, 30f, 0);
					}
				else if(joystick.GetAxis().x<0.6f && joystick.GetAxis().x>=0.2f){
					hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, 60f, 0);
			}
				else if(joystick.GetAxis().x<=1f && joystick.GetAxis().x>=0.6f){
					hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, 90f, 0);
			}

				else if(joystick.GetAxis().x>-0.2f && joystick.GetAxis().x<0f){
					hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, -30f, 0);
					}
				else if(joystick.GetAxis().x > -0.6f && joystick.GetAxis().x < -0.2f){
					hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, -60f, 0);
			}	else if(joystick.GetAxis().x >= -1f && joystick.GetAxis().x < -0.6f){
					hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, -90f, 0);
			}
 
		}

			else if (!button.GetPress () && grasp != null && !button2.GetPress()) {
				// Get rigidbody of grasped target
				Rigidbody target = grasp.GetComponent<Rigidbody> ();
				// Break grasp
				hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, 0f, 0);
				DestroyImmediate (grasp);
				// Apply physics to target in the event of attempting to throw it
				target.velocity = hand.velocity * speed;
				target.angularVelocity = hand.angularVelocity * speed;
				// Update state to Open
				state = VirtualHandState.Open;
			}


			else if(!button2.GetPress()){
				hand.gameObject.GetComponent<Rigidbody>().transform.eulerAngles = new Vector3(0, 0f, 0);	
				state=VirtualHandState.Holding;
			}
				else if(!button2.GetPress() && grasp == null){
					//grasp.GetComponent<Rigidbody>().transform.localScale -= new Vector3(25,25,25);
					state= VirtualHandState.Closed	;

				}
			else{


			}



		}

		else if(state == VirtualHandState.ScaleUpDown){
				if (grasp == null) {
				
				// Update state to Open
				state = VirtualHandState.Open;
			}


			else if(button2.GetPress() && grasp!=null && button.GetPress()){
				if(joystick.GetAxis().y<0f){
					grasp.gameObject.GetComponent<Rigidbody>().transform.localScale -= new Vector3(0.0001f,0.0001f,0.0001f) ;					}
				else if(joystick.GetAxis().y>0f){
					grasp.gameObject.GetComponent<Rigidbody>().transform.localScale += new Vector3(0.0001f,0.0001f,0.0001f) ;
			}
				
		}


				else if (!button.GetPress () && grasp != null && !button2.GetPress()) {

				// Get rigidbody of grasped target
				Rigidbody target = grasp.GetComponent<Rigidbody> ();
				// Break grasp
				//grasp.GetComponent<Rigidbody>().transform.localScale -= new Vector3(25,25,25);					

				DestroyImmediate (grasp);
				// Apply physics to target in the event of attempting to throw it
				
				target.velocity = hand.velocity * speed;
				target.angularVelocity = hand.angularVelocity * speed;
				// Update state to Open
				state = VirtualHandState.Open;
			}

				else if(!button2.GetPress()){
					//grasp.GetComponent<Rigidbody>().transform.localScale -= new Vector3(25,25,25);
					state= VirtualHandState.Holding	;

				}

				else if(!button2.GetPress() && grasp == null){
					//grasp.GetComponent<Rigidbody>().transform.localScale -= new Vector3(25,25,25);
					state= VirtualHandState.Closed	;

				}


			else {

				// Nothing to do for holding
			}	


		}
		//else if(state ==) 
	}
}