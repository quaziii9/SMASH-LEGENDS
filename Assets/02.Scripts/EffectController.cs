using Cysharp.Threading.Tasks;
using UnityEngine;
public class EffectController : MonoBehaviour
{
    [SerializeField] private GameObject[] _effectPrefabs;
    [SerializeField] private ParticleSystem _dieEffect;
    protected GameObject[] _effects;
    private Rigidbody _rigidbody;
    private float _scaleOffset;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _scaleOffset = 1 / transform.localScale.x;
        // 이펙트를 모아서 관리하기 위해 중간 단계의 오브젝트 생성
        GameObject EffectController = Instantiate(new GameObject(), transform);
        EffectController.name = "Effect Controller";
        _effects = new GameObject[_effectPrefabs.Length];
        for (int i = 0; i < _effectPrefabs.Length; ++i)
        {
            GameObject effect = Instantiate(_effectPrefabs[i], EffectController.transform);
            _effects[i] = effect;
            _effects[i].transform.localScale =
                new Vector3(_effects[i].transform.localScale.x * _scaleOffset,
                _effects[i].transform.localScale.y * _scaleOffset,
                _effects[i].transform.localScale.z * _scaleOffset);
            _effects[i].transform.position =
                new Vector3(transform.position.x + (_effects[i].transform.position.x * _scaleOffset),
                transform.position.y + (_effects[i].transform.position.y * _scaleOffset),
                transform.position.z + (_effects[i].transform.position.z * _scaleOffset));
            _effects[i].SetActive(false);
        }

        CreateDieEffect();
    }

    private void CreateDieEffect()
    {
        _dieEffect = Instantiate(_dieEffect, transform.position, Quaternion.identity);
        _dieEffect.gameObject.SetActive(false);
    }
    public void SetDieEffect()
    {
        _dieEffect.gameObject.SetActive(true);
        _dieEffect.transform.position = transform.position;
        _dieEffect.transform.forward = _rigidbody.velocity;
        _dieEffect.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
