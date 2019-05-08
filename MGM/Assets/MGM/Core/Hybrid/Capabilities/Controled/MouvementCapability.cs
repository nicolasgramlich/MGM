﻿using Unity.Entities;
using UnityEngine;

namespace MGM
{
    public class MouvementCapability : ControledCapability<MouvementInputResponse>
    {


        [SerializeField] private bool MovesIn3D = true;
        [SerializeField] private float MouvementSpeed = 5;
        [SerializeField] [Range(0, 1)] private float MovementInertia = 1;
        [SerializeField] private bool ShouldFaceForward = true;
        
        protected override void SetUpCapabilityParameters(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var mcp = new MouvementCapabilityParameters
            {
                    Speed = MouvementSpeed,
                    ShouldFaceForward = ShouldFaceForward,
                    MovementInertia = MovementInertia
            };
         
            dstManager.AddComponentData(entity, mcp);


            var heading = new Heading
            {

            };

            dstManager.AddComponentData(entity, heading);

            if (MovesIn3D)
            {
                dstManager.AddComponentData(entity, new Mouvement3DSystemTarget());
            }
            else{
                dstManager.AddComponentData(entity, new Mouvement2DSystemTarget());
            }
        }
    }
}
