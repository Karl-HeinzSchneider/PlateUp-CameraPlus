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


namespace PlateUp_StaticCamera
{
    public class StaticCamera : GenericSystemBase, IModSystem
    {   

        protected override void Initialise()
        {
            base.Initialise();            
        }

        protected override void OnUpdate()
        {            
        }
    }
}
