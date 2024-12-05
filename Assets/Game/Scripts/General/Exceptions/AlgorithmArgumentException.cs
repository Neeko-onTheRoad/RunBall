using System;
using System.Runtime.ExceptionServices;

[Serializable]
public class AlgorithmArgumentException : InvalidOperationException {

	public AlgorithmArgumentException(IAlgorithm self, string fieldName) : base(
		$"Cannot generate {self.GetType().Name} before '{fieldName}' are not assigned or empty. " +
		$"Please assign valid value to '{fieldName}' first"
	) {}

	public static void ThrowIfNull(IAlgorithm self, string fieldName, object value) {

		if (value is null) {
			AlgorithmArgumentException exception = new(self, fieldName);
			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}
}