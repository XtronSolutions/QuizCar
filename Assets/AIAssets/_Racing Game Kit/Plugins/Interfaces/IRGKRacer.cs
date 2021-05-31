using System;
using UnityEngine;
using RacingGameKit;
using System.Collections.Generic;
using SmartAssembly.Attributes;



namespace RacingGameKit.Interfaces
{
    [DoNotObfuscate()]
    public interface IRGKRacer
    {
        float Speed { get; set; }
        Vector3 Position { get; }
        Transform CachedTransform { get; }
        GameObject CachedGameObject { get; }
        eAIRoadPosition CurrentRoadPosition { get; }
        Vector3 Velocity { get; }
    }

}
