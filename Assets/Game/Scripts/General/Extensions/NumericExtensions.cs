using UnityEngine;

public static class NumericExtensions {

	public static int Sign(this float self) => self.Approximately(0f) ? 0 : (int)Mathf.Sign(self);
	public static bool Approximately(this float self, float other) => Mathf.Approximately(self, other);
	public static float Round(this float self) => Mathf.Round(self);
	public static int RoundToInt(this float self) => Mathf.RoundToInt(self);
	public static float Ceil(this float self) => Mathf.Ceil(self);
	public static int CeilToInt(this float self) => Mathf.CeilToInt(self);
	public static float Floor(this float self) => Mathf.Floor(self);
	public static int FloorToInt(this float self) => Mathf.FloorToInt(self);

}