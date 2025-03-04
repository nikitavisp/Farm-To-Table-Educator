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

public class Steering : MonoBehaviour {

	// Enumerate the states of steering
	public enum SteeringState {
		NotSteering,
		WalkingForward,
		WalkingBackward,
		RunningForward,
		RunningBackward
		//TurnLeft???
		//TurnRight???
	};
	
    // Inspector parameters
    [Tooltip("The tracking device used to determine absolute direction for steering.")]
    public CommonTracker tracker;

    [Tooltip("The controller joystick used to determine relative direction (forward/backward) and speed.")]
	public CommonAxis joystick;

	[Tooltip("A button required to be pressed to activate steering.")]
	public CommonButton button;  

    [Tooltip("The space that is translated by this interaction. Usually set to the physical tracking space.")]
    public CommonSpace space;

    [Tooltip("The median speed for movement expressed in meters per second.")]
    public float speed = 1.0f;

	// Private interaction variables
	private SteeringState state;

	// Called at the end of the program initialization
	void Start () {

		// Set initial steering state to not steering
		state = SteeringState.NotSteering;
	}
		
    // FixedUpdate is not called every graphical frame but rather every physics frame
	void FixedUpdate () {

		// If state is not steering
		if (state == SteeringState.NotSteering) {
		
			// If the joystick is pressed forward and the button is pressed
			if (joystick.GetAxis ().y > 0.0f && joystick.GetAxis().y<0.5f && button.GetPress ()) {

				// Change state to steering forward
				state = SteeringState.WalkingForward;				
			}
			
			else if(joystick.GetAxis().y>=0.5f && button.GetPress()){
					//changing the state to Running Forward
					state = SteeringState.RunningForward;
				}
	
			// If the joystick is pressed backward and the button is pressed
			else if (joystick.GetAxis ().y < 0.0f && joystick.GetAxis().y>-0.5f && button.GetPress ()) {

				// Change state to steering backward
				state = SteeringState.WalkingBackward;
			}
			else if(joystick.GetAxis().y<=-0.5f && button.GetPress()){
					//changing the state to Running Backward
					state = SteeringState.RunningBackward;
				}


			// Process current not steering state
			else {

				// Nothing to do for not steering
			}
		}

		// If state is steering forward
		else if (state == SteeringState.WalkingForward) {

			//if speen is increased, The user will go into running state from walking state. 
			if(joystick.GetAxis().y>=0.5f && button.GetPress()){
					//changing the state to Running Forward
					state = SteeringState.RunningForward;
				}

			// If the button is not pressed
			else if (!button.GetPress ()) {

				// Change state to not steering 
				state = SteeringState.NotSteering;
			}

			// If the joystick is pressed backward and the button is pressed
			else if (joystick.GetAxis ().y < 0.0f && joystick.GetAxis().y>-0.5f && button.GetPress ()) {

				// Change state to steering backward
				state = SteeringState.WalkingBackward;
			}

			else if(joystick.GetAxis().y<=-0.5f && button.GetPress()){
					//changing the state to Running Forward
					state = SteeringState.RunningBackward;
				}


			// Process current steering forward state
			else {

				// Translate the space based on the tracker's absolute forward direction and the joystick's forward value
				Vector3 direction= tracker.transform.forward;
				direction.y =0.0f; 
				space.transform.position += joystick.GetAxis ().y * direction * speed * Time.deltaTime;
			}
		}

		// If state is steering backward
		else if (state == SteeringState.WalkingBackward) {

			if(joystick.GetAxis().y<=-0.5f && button.GetPress()){
					//changing the state to Running backward
					state = SteeringState.RunningBackward;
				}

			// If the button is not pressed
			else if (!button.GetPress ()) {

				// Change state to not steering 
				state = SteeringState.NotSteering;
			}

			// If the joystick is pressed forward and the button is pressed
			else if (joystick.GetAxis ().y > 0.0f && joystick.GetAxis().y<0.5f && button.GetPress ()) {

				// Change state to steering forward
				state = SteeringState.WalkingForward;
			}

			else if(joystick.GetAxis().y>=0.5f && button.GetPress()){
					//changing the state to Running Forward
					state = SteeringState.RunningForward;
				}

			// Process current steering backward state
			else {

				// Translate the space based on the tracker's absolute forward direction and the joystick's backward value
				Vector3 direction= tracker.transform.forward;
				direction.y =0.0f; 
				space.transform.position += joystick.GetAxis ().y * direction * speed * Time.deltaTime;
			}
		} 
		else if(state == SteeringState.RunningForward) {

			if(joystick.GetAxis().y>0.0f && joystick.GetAxis().y<0.5f && button.GetPress()){
				state=SteeringState.WalkingForward;
			}
			else if(joystick.GetAxis().y<0.0f && joystick.GetAxis().y>-0.5f && button.GetPress()){
				state=SteeringState.WalkingBackward;
			}
			else if(joystick.GetAxis().y<=-0.5f && button.GetPress()){
				state=SteeringState.RunningBackward;
			}
			else if(!button.GetPress()){
				state=SteeringState.NotSteering;
			}
			else{
				Vector3 direction= tracker.transform.forward;
				direction.y =0.0f; 
				space.transform.position += joystick.GetAxis ().y * direction * speed * Time.deltaTime;

			}



		}
		else if(state == SteeringState.RunningBackward){

			if(joystick.GetAxis().y>0.0f && joystick.GetAxis().y<0.5f && button.GetPress()){
				state=SteeringState.WalkingForward;
			}
			else if(joystick.GetAxis().y<0.0f && joystick.GetAxis().y>-0.5f && button.GetPress()){
				state=SteeringState.WalkingBackward;
			}
			else if(joystick.GetAxis().y>=0.5f && button.GetPress()){
				state=SteeringState.RunningForward;
			}
			else if(!button.GetPress()){
				state = SteeringState.NotSteering;
			}
			else{
				Vector3 direction= tracker.transform.forward;
				direction.y =0.0f; 
				space.transform.position += joystick.GetAxis ().y * direction * speed * Time.deltaTime;

			}

		}	

	}
}