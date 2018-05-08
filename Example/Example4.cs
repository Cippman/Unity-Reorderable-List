using CippSharp.Reorderable;
using UnityEngine;
using UnityEngine.Events;

public class Example4 : MonoBehaviour
{
	[ReorderableUnityEvent(order = 1)]
	public UnityEvent amerika;

	public UnityEvent amrika1;
}
