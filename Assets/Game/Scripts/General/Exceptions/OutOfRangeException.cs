using System;
using System.Runtime.ExceptionServices;

public class OutOfRangeException {

	public static void ThrowIfEqual<T>(T target, T value, string fieldName) where T : IComparable<T> {
		if (target.CompareTo(value) == 0) {
			ArgumentOutOfRangeException exception = new(
				$"'{fieldName}' must be not equal with {value}. " +
				$"But '{fieldName}' is {target}."
			);

			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}

	public static void ThrowIfNotEqual<T>(T target, T value, string fieldName) where T : IComparable<T> {
		if (target.CompareTo(value) != 0) {
			ArgumentOutOfRangeException exception = new(
				$"'{fieldName}' must be equal with {value}. " +
				$"But '{fieldName}' is {target}."
			);

			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}

	public static void ThrowIfLess<T>(T target, T value, string fieldName) where T : IComparable<T> {
		if (target.CompareTo(value) < 0) {
			ArgumentOutOfRangeException exception = new(
				$"'{fieldName}' must be equal or greater than {value}. " +
				$"But '{fieldName}' is {target}."
			);

			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}

	public static void ThrowIfGreater<T>(T target, T value, string fieldName) where T : IComparable<T> {
		if (target.CompareTo(value) > 0) {
			ArgumentOutOfRangeException exception = new(
				$"'{fieldName}' must be equal or less than {value}. " +
				$"But '{fieldName}' is {target}."
			);

			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}

	public static void ThrowIfEqualOrLess<T>(T target, T value, string fieldName) where T : IComparable {
		if (target.CompareTo(value) <= 0) {
			ArgumentOutOfRangeException exception = new(
				$"'{fieldName}' must be greater than {value}. " +
				$"But '{fieldName}' is {target}."
			);

			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}

	public static void ThrowIfEqualOrGreater<T>(T target, T value, string fieldName) where T : IComparable<T> {
		if (target.CompareTo(value) >= 0) {
			ArgumentOutOfRangeException exception = new(
				$"'{fieldName}' must be less than {value}. " +
				$"But '{fieldName}' is {target}."
			);

			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}

}