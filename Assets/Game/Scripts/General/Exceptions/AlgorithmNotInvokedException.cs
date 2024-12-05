using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

[Serializable]
public class AlgorithmNotInvokedException : InvalidOperationException {

	public AlgorithmNotInvokedException(IAlgorithm self, string fieldName) : base(
		$"Cannot access '{fieldName}' because {self.GetType().Name} is not generated. " +
		$"Please call Invoke() before using it."
	) {}

	public static T ThrowIfNull<T>(IAlgorithm self, T value, [CallerMemberName] string callerName = "") {
		
		if (value is null) {
			AlgorithmNotInvokedException exception = new(self, callerName);
			ExceptionDispatchInfo.Capture(exception).Throw();
		}

		return value;
	}
}