using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private Cube _cube;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _cubeHolder;

    [SerializeField] private float _repeatRate = 1f;
    [SerializeField] private int _spawnOffset = 4;
    [SerializeField] private int _poolCapacity = 7;
    [SerializeField] private int _poolMaxSize = 7;
    [SerializeField] private int _minSecondsUntilRelease = 2;
    [SerializeField] private int _maxSecondsUntilRelease = 5;

    private ObjectPool<Cube> _pool;

    private WaitForSeconds _timeUntilRelease;
    private System.Random _random = new System.Random();

    private void Awake()
    {
        int sleepTime = _random.Next(_minSecondsUntilRelease, _maxSecondsUntilRelease + 1);
        _timeUntilRelease = new WaitForSeconds(sleepTime);

        _pool = new ObjectPool<Cube>(
            createFunc: () => Init(),
            actionOnGet: (Cube) => SpawnCube(Cube),
            actionOnRelease: (Cube) => Cube.gameObject.SetActive(false),
            actionOnDestroy: (Cube) => Destroy(Cube),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
            );
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0.0f, _repeatRate);
    }

    private Cube Init()
    {
        Cube cube = Instantiate(_cube, _cubeHolder);
        cube.OnCubeCollision += ReleaseCube;

        return cube;
    }

    private void SpawnCube(Cube cube)
    {
        cube.IsReleased = false;

        cube.transform.position = new Vector3 (_spawnPoint.position.x + _random.Next(-_spawnOffset, _spawnOffset + 1),
                                                _spawnPoint.position.y,
                                                _spawnPoint.position.z + _random.Next(-_spawnOffset, _spawnOffset + 1));

        cube.transform.rotation = Quaternion.identity;

        if(cube.IsDefaultColor == false)
            cube.SetDefaultColor();

        if(cube.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        cube.gameObject.SetActive(true);
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private void ReleaseCube(Cube cube)
    {
        if(cube.IsReleased != true)
        {
            cube.IsReleased = true;
            StartCoroutine(WaitUntilRelease(cube));
        }
    }

    private IEnumerator WaitUntilRelease(Cube cube)
    {
        yield return _timeUntilRelease;

        _pool.Release(cube);
    }
}