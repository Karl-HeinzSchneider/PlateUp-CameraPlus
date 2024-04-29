using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cinemachine;
using Kitchen;
using KitchenMods;

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Collections;
using Random = System.Random;
using System.Reflection;


namespace PlateUp_StaticCamera
{
    public class StaticCamera : GenericSystemBase, IModSystem       
    {     
        private InputAction CameraAction;
        private bool StaticCameraEnabled = false;

        private InputAction EditAction;
        private bool EditModeEnabled = false;

        private InputAction ScrollAction;
        private bool IsScrolling = false;

        private Vector3 CameraPosition = new Vector3(0, 0, 0);
        private Vector3 CameraVelocity = new Vector3(0, 0, 0);

        protected override void Initialise()
        {
            base.Initialise();
         
            this.InitKeybindings();
        }

        protected override void OnUpdate()
        {
            Component MainCamera = Camera.main;

            if (this.StaticCameraEnabled)
            {
                ((Behaviour)MainCamera.GetComponent<CinemachineBrain>()).enabled = false;
                MainCamera.transform.position = Vector3.SmoothDamp(MainCamera.transform.position, this.CameraPosition, ref this.CameraVelocity, 0.5f);
            }
            else
            {
                ((Behaviour)MainCamera.GetComponent<CinemachineBrain>()).enabled = true;
            }
        }
     
        private void InitKeybindings()
        {
            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Gamepad.html#properties

            // CameraAction
            this.CameraAction = new InputAction("ToggleStaticCamera", (InputActionType) 0,"<Keyboard>/Q",(string) null, (string) null, (string) null);
            InputActionSetupExtensions.AddBinding(this.CameraAction,"<Gamepad>/rightStickPress/", (string) null, (string)null, (string)null);

            this.CameraAction.performed += (Action<InputAction.CallbackContext>)(ctx =>
            {
               this.ToggleStaticCamera();   
            });
            this.CameraAction.Enable();

            //
            this.EditAction = new InputAction("SetCameraPosition", (InputActionType)0, "<Keyboard>/E", (string)null, (string)null, (string)null);
            InputActionSetupExtensions.AddBinding(this.EditAction, "<Gamepad>/selectButton/", (string)null, (string)null, (string)null);

            this.EditAction.performed += (Action<InputAction.CallbackContext>)(ctx =>
            {
                this.SetCameraPosition();
            });
            this.EditAction.Enable();
        }

        private void ToggleStaticCamera()
        {
            if (this.StaticCameraEnabled)
            {
                this.DisableStaticCamera();
            }
            else
            {
                this.EnableStaticCamera();
            }
        }

        private void EnableStaticCamera()
        {
            this.StaticCameraEnabled = true;
        }

        private void DisableStaticCamera()
        {
            this.StaticCameraEnabled = false;
        }

        private void SetCameraPosition()
        {     
            Debug.Log("KHS LOG - SetCameraPosition");

            // Find local player ID
            int LocalID = 0;

            foreach(PlayerInfo info in Players.Main.All())
            {
                if (info.IsLocalUser)
                {
                    this.LogObject(info);
                    LocalID = info.ID;
                    break;
                }
            }

            Debug.Log($"Local ID: {LocalID}");

            //
            PlayerView[] PlayerViewArray = GameObject.FindObjectsOfType<PlayerView>();

            foreach (PlayerView view in PlayerViewArray)
            {
                int ID = this.GetPlayerID(view);

                Debug.Log($"check ID: {ID}");

                if(ID != LocalID)
                {
                    continue;
                }

                Vector3 ViewPosition = view.transform.position;

                // default 35.82595
                float Height = 35.82595f;
                ViewPosition.y = Height;

                float DeltaZ = 30f;
 
                this.CameraPosition = new Vector3(ViewPosition.x, Height, ViewPosition.z -DeltaZ);

                //Component MainCamera = Camera.main;
                //Debug.Log("MainCamera XYZ");
                //Debug.Log(MainCamera.transform.position.x);
                //Debug.Log(MainCamera.transform.position.y);
                //Debug.Log(MainCamera.transform.position.z);
            }
        }

        private int GetPlayerID(PlayerView view)
        {
            return ((PlayerView.ViewData)typeof(PlayerView).GetField("Data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)view)).PlayerID;
        }

        private void LogObject(object obj)
        {
            Debug.Log("--KHS LOG obj:");
            // Get the type of the object
            Type objType = obj.GetType();

            // Get all properties of the object
            PropertyInfo[] properties = objType.GetProperties();

            // Log each property and its value
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);
                Debug.Log(property.Name + ": " + value);
            }
        }
    }
}
