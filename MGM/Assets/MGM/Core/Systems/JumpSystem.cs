﻿using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Burst;
using Unity.Jobs;

namespace MGM
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public class JumpSystem : JobComponentSystem
    {  

        [BurstCompile]
        struct JumpJob : IJobForEachWithEntity<WorldRenderBounds,PhysicsVelocity, JumpCapabilityParameters >
        {
            [ReadOnly] public CollisionWorld World;
           
            public void Execute(Entity entity,int index,[ReadOnly]ref WorldRenderBounds renderBounds,ref PhysicsVelocity physics, ref JumpCapabilityParameters jcp)
            {
                // Do nothing if we don't try to jump
                if (jcp.JumpTrigerred)
                {

                    // if we already jumped at least once
                    if (jcp.CurrentJumpCount > 0)
                    {

                        var RaycastInput = new RaycastInput
                        {
                            Ray = new Ray { Origin = renderBounds.Value.Center, Direction =- math.up() /* (renderBounds.Value.Extents.y + 0.01f)*/ },
                            Filter = CollisionFilter.Default
                        };


                        ClosestHitWithIgnoreCollector collector = new ClosestHitWithIgnoreCollector(1f,World.Bodies,entity);

                        // NEED Unity Fix - Cast will return true even if the hit is the ignered entity.
                        if (World.CastRay(RaycastInput, ref collector))
                        {
                            // Work around to check that the hit is not the ignore entity
                            if (collector.Hit.ColliderKey.Value != 0)
                            {
                                jcp.CurrentJumpCount = 0;
                            }
                        }

                    }

                    // If we have not reache the max jump count
                    if (jcp.CurrentJumpCount < jcp.MaxJumpNumber)
                    {
                        // jump !
                        physics.Linear.y = jcp.Force;
                        jcp.CurrentJumpCount++;
                    }

                    // Reset the jump trigger to prevent auto jumping.
                    jcp.JumpTrigerred = false;
                }

            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new JumpJob() { World = World.Active.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld.CollisionWorld }.Schedule(this, inputDeps);
        }

        

    }
}