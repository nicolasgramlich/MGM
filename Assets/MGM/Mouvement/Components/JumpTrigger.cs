﻿using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct JumpTrigger : IComponentData
{
    public bool Value;
}
