using System.Runtime.ExceptionServices;
using UnityEngine;

public static class ComponentExtensions {

	public static T GetComponentChecked<T>(this MonoBehaviour monoBehaviour) where T : Component {
		
		if (monoBehaviour.TryGetComponent(out T result)) return result;

		string exceptionMessage = $"Missing component in {monoBehaviour.name}: {typeof(T).Name}";
		MissingComponentException exception = new(exceptionMessage);

		ExceptionDispatchInfo.Capture(exception).Throw();
		return null;

	}

}