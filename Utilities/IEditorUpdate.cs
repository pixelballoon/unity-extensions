public interface IEditorUpdate
{

#if UNITY_EDITOR
	void OnEditorInit();
	void OnEditorUpdate();
#endif

}
