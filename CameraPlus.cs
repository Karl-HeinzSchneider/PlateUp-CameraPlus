using System.Reflection;

using Cinemachine;
using Kitchen;
using KitchenMods;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlateUp_CameraPlus
{
    public class CameraPlus : GenericSystemBase, IModSystem
    {
        private static readonly FieldInfo PlayerViewData =
            typeof(PlayerView).GetField("Data", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly Vector3 CameraOffset = new Vector3(0f, 35.82595f, -30f);

        private InputAction toggleAction;
        private InputAction positionAction;
        private InputAction followAction;
        private InputAction stickZoomAction;
        private InputAction wheelZoomAction;

        private bool cameraEnabled;
        private bool followEnabled;
        private bool hasAnchor;
        private Vector3 anchor;
        private Vector3 velocity;
        private float fieldOfView = 10f;

        private Camera controlledCamera;
        private CinemachineBrain controlledBrain;
        private float originalFieldOfView;
        private PlayerView playerView;
        private int? trackedPlayerId;

        protected override void Initialise()
        {
            base.Initialise();

            toggleAction = new InputAction("Toggle CameraPlus", InputActionType.Button, "<Keyboard>/q");
            toggleAction.AddBinding("<Gamepad>/rightStickPress");

            positionAction = new InputAction("Set Camera Position", InputActionType.Button, "<Keyboard>/e");
            positionAction.AddBinding("<Gamepad>/rightShoulder");

            followAction = new InputAction("Toggle Follow Mode", InputActionType.Button, "<Keyboard>/f");
            stickZoomAction = new InputAction("Stick Zoom", InputActionType.Value, "<Gamepad>/rightStick/y");
            wheelZoomAction = new InputAction("Wheel Zoom", InputActionType.Value, "<Mouse>/scroll/y");

            toggleAction.Enable();
            positionAction.Enable();
            followAction.Enable();
            stickZoomAction.Enable();
            wheelZoomAction.Enable();
        }

        protected override void OnUpdate()
        {
            if (toggleAction.WasPressedThisFrame())
            {
                cameraEnabled = !cameraEnabled;
            }

            if (!cameraEnabled)
            {
                ReleaseCamera();
                return;
            }

            if (followAction.WasPressedThisFrame())
            {
                followEnabled = !followEnabled;
            }

            Camera camera = Camera.main;
            if (controlledCamera != camera)
            {
                ReleaseCamera();
            }

            PlayerView target = FindLocalPlayer();
            if (!camera || !target)
            {
                ReleaseCamera();
                return;
            }

            if (!controlledBrain)
            {
                if (!camera.TryGetComponent(out CinemachineBrain brain) || !brain.isActiveAndEnabled)
                {
                    return;
                }

                controlledCamera = camera;
                controlledBrain = brain;
                originalFieldOfView = camera.fieldOfView;
                brain.enabled = false;
            }

            if (!hasAnchor || followEnabled || positionAction.IsPressed() || positionAction.WasPressedThisFrame())
            {
                Vector3 position = target.transform.position;
                anchor = new Vector3(position.x, 0f, position.z) + CameraOffset;
                hasAnchor = true;
            }

            UpdateZoom();
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, anchor, ref velocity, 0.5f);
            camera.fieldOfView = fieldOfView;
        }

        private PlayerView FindLocalPlayer()
        {
            int? localPlayerId = null;
            if (Players.Main != null)
            {
                foreach (PlayerInfo player in Players.Main.All())
                {
                    if (player.IsLocalUser)
                    {
                        localPlayerId = player.ID;
                        break;
                    }
                }
            }

            if (playerView && playerView.isActiveAndEnabled && trackedPlayerId == localPlayerId)
            {
                return playerView;
            }

            playerView = null;
            trackedPlayerId = localPlayerId;
            if (!localPlayerId.HasValue || PlayerViewData == null)
            {
                return null;
            }

            foreach (PlayerView view in Object.FindObjectsOfType<PlayerView>())
            {
                if (view.isActiveAndEnabled &&
                    PlayerViewData.GetValue(view) is PlayerView.ViewData data && data.PlayerID == localPlayerId.Value)
                {
                    playerView = view;
                    break;
                }
            }

            return playerView;
        }

        private void UpdateZoom()
        {
            float wheel = wheelZoomAction.ReadValue<float>();
            float stick = stickZoomAction.ReadValue<float>();
            float zoom = wheel == 0f ? 0f : Mathf.Sign(wheel) * 8f;
            if (Mathf.Abs(stick) > 0.15f)
            {
                zoom += stick * UnityEngine.Time.unscaledDeltaTime * 60f;
            }

            float scale = 0.110037f * Mathf.Exp(0.00888164f * fieldOfView);
            fieldOfView = Mathf.Clamp(fieldOfView - zoom * scale, 3f, 69f);
        }

        private void ReleaseCamera()
        {
            if (controlledCamera)
            {
                controlledCamera.fieldOfView = originalFieldOfView;
            }

            if (controlledBrain)
            {
                controlledBrain.enabled = true;
            }

            controlledCamera = null;
            controlledBrain = null;
            hasAnchor = false;
            velocity = Vector3.zero;
        }

        protected override void OnStopRunning()
        {
            ReleaseCamera();
            cameraEnabled = false;
            playerView = null;
            base.OnStopRunning();
        }

        protected override void OnDestroy()
        {
            ReleaseCamera();
            toggleAction?.Dispose();
            positionAction?.Dispose();
            followAction?.Dispose();
            stickZoomAction?.Dispose();
            wheelZoomAction?.Dispose();
            base.OnDestroy();
        }
    }
}
