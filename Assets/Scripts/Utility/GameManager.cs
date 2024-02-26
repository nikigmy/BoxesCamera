using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    public ServiceProvider ServiceProvider;

    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private CameraController cameraController;
    private void Awake()
    {
        instance = this;
        ServiceProvider = new ServiceProvider();
        ServiceProvider.RegisterService<IInput>(mouseInput);
        ServiceProvider.RegisterService(cameraController);
    }
}