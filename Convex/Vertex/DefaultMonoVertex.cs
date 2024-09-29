using UnityEngine;
using System;

namespace MvConvex
{
    [Serializable]
    public class DefaultMonoVertex : MonoBehaviour, IVertex
    { 
        public Vector3 Position { get => transform.position; } 
        [SerializeField] public int Index { get; set; }
    }
}
