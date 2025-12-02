using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class CameraPersister : MonoBehaviour
{
    private static CameraPersister instance;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera virtualCamera;

    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        GetCameraReferences();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindAndAssignConfiner();
    }

    private void GetCameraReferences()
    {
        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();

        if (virtualCamera == null)
            virtualCamera = GetComponentInChildren<CinemachineCamera>();

        if (confiner == null && virtualCamera != null)
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Invoke(nameof(ReassignConfiner), 0.1f);
    }

    private void ReassignConfiner()
    {
        FindAndAssignConfiner();
        ReassignFollowTarget();
    }

    private void FindAndAssignConfiner()
    {
        GameObject confinerObject = GameObject.FindWithTag("CameraConfiner");

        if (confinerObject != null)
        {
            Collider2D col = confinerObject.GetComponent<Collider2D>();

            if (col != null && confiner != null)
                confiner.BoundingShape2D = col;
        }
    }

    private void ReassignFollowTarget()
    {
        if (virtualCamera == null) return;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            virtualCamera.Follow = player.transform;
    }
}
