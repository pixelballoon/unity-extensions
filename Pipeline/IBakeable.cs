public interface IBakeable
{
	#if UNITY_EDITOR
	void Bake();
	#endif
}
