using System;
using CippSharp.Reorderable;
using UnityEngine;
using UnityEngine.Events;

public class Example4 : MonoBehaviour
{
	[Serializable]
	public class ReorderableUnityEventReferences : ReorderableArray<UnityEventBaseReferences>
	{
		
	}

	[Reorderable]
	public ReorderableUnityEventReferences references = new ReorderableUnityEventReferences();

	[ReorderableUnityEvent(order = 1)]
	public UnityEvent amerika;

	public UnityEvent amrika1;
}
