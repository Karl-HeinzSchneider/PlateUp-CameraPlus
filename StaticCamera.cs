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

        private EntityQuery Appliances;

        struct CHasBeenSetOnFire : IModComponent
        {

        }

        protected override void Initialise()
        {
            base.Initialise();

            Appliances = GetEntityQuery(new QueryHelper()
                   .All(typeof(CAppliance))
                   .None(
                       typeof(CFire),
                       typeof(CIsOnFire),
                       typeof(CFireImmune),
                       typeof(CHasBeenSetOnFire)
                   ));
        }

        protected override void OnUpdate()
        {
            var appliances = Appliances.ToEntityArray(Allocator.TempJob);
            foreach (var appliance in appliances)
            {
                EntityManager.AddComponent<CIsOnFire>(appliance);
                EntityManager.AddComponent<CHasBeenSetOnFire>(appliance);
            }
            appliances.Dispose();
        }
    }
}
