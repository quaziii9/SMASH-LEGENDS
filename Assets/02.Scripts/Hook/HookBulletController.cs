using UnityEngine;
using UnityEngine.Pool;

public class HookBulletController : MonoBehaviour
{
    private enum BulletType
    {
        Default,
        FinishDefault,
        Heavy,
        FinishHeavy,
        Skill,
        HeavySkill
    }
    private enum FirePosition
    {
        Left,
        Right
    }
    private enum FireDeleteType
    {
        Default,
        Heavy,
        FinishHeavy
    }

    private GameObject[] _bulletContainers = new GameObject[6];

    private ObjectPool<HookBullet>[] _bulletPool = new ObjectPool<HookBullet>[6];
    private HookBullet[] _bullets = new HookBullet[6];

    private ObjectPool<FireDelete>[] _bulletCreateEffectPool = new ObjectPool<FireDelete>[3];
    private FireDelete[] _bulletCreateEffects = new FireDelete[3];

    private Transform _bulletSpawnPositionLeft;
    private Transform _bulletSpawnPositionRight;
    private ParticleSystem _shotEffect;
    [SerializeField]
    private GameObject _parrot;

    private int _rootIndex = 7;
    private int _boneIndex = 0;
    private int _leftWeaponIndex = 2;
    private int _rightWeaponIndex = 3;
    private int _cylinderIndex = 0;

    private float _jumpRotationValue = 45f;

    private Vector3 _defaultSkillBulletRotation = new Vector3(0, -7f, 0);
    private Vector3 _heavySkillBulletRotation = new Vector3(0, -5f, 0);
    private Vector3 _groundPosition;
    private Vector3[] _effectEulerAnglesOnJump = {
        new Vector3(-90, 0, 0),
        new Vector3(-90, 45, 0),
        new Vector3(0, 0, 90),
        new Vector3(90, -45, 0),
        new Vector3(90, 0, 0),
        new Vector3(90, 45, 0),
        new Vector3(0, 0, -90),
        new Vector3(0, 45, -90)
    };
    private Vector3[] _effectEulerAngles = {
        new Vector3(-1, 0, 0),
        new Vector3(-1, 45, 0),
        new Vector3(0, 0, 45),
        new Vector3(1, -45, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 45, 0),
        new Vector3(0, 0, -1),
        new Vector3(0, 45, -1)
    };

    private void Start()
    {
        _bulletSpawnPositionLeft = transform.GetChild(_rootIndex).GetChild(_boneIndex).GetChild(_leftWeaponIndex).GetChild(_cylinderIndex).transform;
        _bulletSpawnPositionRight = transform.GetChild(_rootIndex).GetChild(_boneIndex).GetChild(_rightWeaponIndex).GetChild(_cylinderIndex).transform;
        for (int i = 0; i < _bulletContainers.Length; ++i)
        {
            _bulletContainers[i] = transform.GetChild(i).gameObject;
            _bullets[i] = _bulletContainers[i].transform.GetChild(0).GetComponent<HookBullet>();
        }
        _bulletCreateEffects[(int)FireDeleteType.Default] = _bulletContainers[(int)BulletType.Default].transform.GetChild(1).GetComponent<FireDelete>();
        _bulletCreateEffects[(int)FireDeleteType.Heavy] = _bulletContainers[(int)BulletType.Heavy].transform.GetChild(1).GetComponent<FireDelete>();
        _bulletCreateEffects[(int)FireDeleteType.FinishHeavy] = _bulletContainers[(int)BulletType.FinishHeavy].transform.GetChild(1).GetComponent<FireDelete>();
        CreateObjectPool();
        _shotEffect = _parrot.transform.GetChild(0).GetComponent<ParticleSystem>();

        Vector3 calibratePosition = new Vector3(0, 0.05f, 0);
        _groundPosition = transform.position + calibratePosition;
    }

    private void OnDisable()
    {
        if (_parrot.activeSelf)
        {
            _parrot.SetActive(false);
        }
    }

    private void SetParrotOnAnimationEvent()
    {
        if (_parrot.activeSelf)
        {
            _parrot.SetActive(false);
        }

        _parrot.SetActive(true);
    }

    public void SetParrotOff()
    {
        Debug.Log("!");
        if (_parrot.activeSelf)
        {
            _parrot.SetActive(false);
        }
    }

    private Vector3 GetFirePosition(FirePosition firePosition)
    {
        switch (firePosition)
        {
            case FirePosition.Left:
                return _bulletSpawnPositionLeft.position;
            case FirePosition.Right:
                return _bulletSpawnPositionRight.position;
            default:
                return Vector3.zero;
        }
    }
    private void FireFromLeftHandOnAnimationEvent(BulletType bulletType)
    {
        GetCreateBullet(GetFirePosition(FirePosition.Left), (int)bulletType);
        CreateBulletEffect(GetFirePosition(FirePosition.Left), (int)bulletType);
    }
    private void FireFromRightHandOnAnimationEvent(BulletType bulletType)
    {
        GetCreateBullet(GetFirePosition(FirePosition.Right), (int)bulletType);
        CreateBulletEffect(GetFirePosition(FirePosition.Right), (int)bulletType);
    }

    private void FinishHeavyAttackOnAnimationEvent()
    {
        Vector3 finishBulletPosition = _bulletSpawnPositionLeft.position - _bulletSpawnPositionRight.position;
        Vector3 spawnPosition = _bulletSpawnPositionRight.position + (finishBulletPosition / 2);
        GetCreateBullet(spawnPosition, (int)BulletType.FinishHeavy);
        Vector3 startEffectPosition = spawnPosition + transform.forward;
        CreateBulletEffect(startEffectPosition, (int)BulletType.FinishHeavy);
    }

    private void FireWithSkillOnAnimationEvent(BulletType bulletType)
    {
        if (_parrot.activeSelf)
        {
            CreateSkillBullet(bulletType);
            _shotEffect.Play();
        }
    }
    private HookBullet GetCreateBullet(Vector3 spawnPosition, int bulletIndex)
    {
        if (IsGroundAttack(_groundPosition))
        {
            spawnPosition = spawnPosition + transform.forward;
        }

        HookBullet bullet = _bulletPool[bulletIndex].Get();
        bullet.transform.SetParent(transform, false);
        bullet.transform.position = spawnPosition;
        bullet.transform.forward = transform.forward;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정

        if (IsGroundAttack(_groundPosition) == false)
        {
            bullet.transform.Rotate(JumpBulletRotate(_jumpRotationValue, bulletIndex));
        }

        bullet.gameObject.SetActive(true);
        return bullet;
    }
    private void CreateSkillBullet(BulletType bulletType)
    {
        Vector3 bulletRotation;

        if (bulletType == BulletType.Skill)
        {
            bulletRotation = _defaultSkillBulletRotation;
        }
        else
        {
            bulletRotation = _heavySkillBulletRotation;
        }

        GetCreateBullet(_parrot.transform.position, (int)bulletType).transform.Rotate(bulletRotation);
    }
    private void CreateBulletEffect(Vector3 spawnPosition, int bulletType)
    {
        FireDelete effect = _bulletCreateEffectPool[GetFireDeleteIndex(bulletType)].Get();
        effect.transform.position = spawnPosition;
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정

        if (bulletType == (int)BulletType.Heavy || bulletType == (int)BulletType.FinishHeavy)
        {
            effect.transform.forward = transform.forward;
        }
        else
        {
            DefaultBulletEffectRotate(effect.gameObject, _jumpRotationValue);
        }

        effect.gameObject.SetActive(true);

        int GetFireDeleteIndex(int bulletType)
        {
            if (bulletType == (int)BulletType.Default)
            {
                return bulletType;
            }
            else
            {
                return bulletType - 1;
            }
        }
    }

    private Vector3 JumpBulletRotate(float value, int bulletType)
    {
        if (transform.forward.x < 0)
        {
            return transform.forward * -value;
        }
        else if (transform.forward.z != 0)
        {
            return new Vector3(value, 0, 0);
        }
        else
        {
            return transform.forward * value;
        }
    }
    private void DefaultBulletEffectRotate(GameObject effect, float value)
    {
        if (IsDiagonalAttack())
        {
            int direction = (int)transform.rotation.eulerAngles.y / 45;

            if (IsGroundAttack(_groundPosition))
            {
                effect.transform.rotation = Quaternion.Euler(_effectEulerAnglesOnJump[direction]);
            }
            else
            {
                Vector3 eulerAngle = CalculateEffectEulerAngle(direction, value);
                effect.transform.rotation = Quaternion.Euler(eulerAngle);
            }
        }
    }
    private Vector3 CalculateEffectEulerAngle(int direction, float value)
    {
        Vector3 currentEulerAngle = _effectEulerAngles[direction];

        currentEulerAngle.x *= value;
        if (currentEulerAngle.z != 45)
        {
            currentEulerAngle.z *= value;
        }

        return currentEulerAngle;
    }
    private bool IsDiagonalAttack() => transform.rotation.eulerAngles.y % 45 == 0;
    private bool IsGroundAttack(Vector3 position) => transform.position.y < position.y;
    private void CreateObjectPool()
    {
        _bulletPool[(int)BulletType.Default] = new ObjectPool<HookBullet>(CreateBulletOnPool, GetPoolBullet, ReturnBulletToPool, (bullet) => Destroy(bullet.gameObject), true, 50, 500);
        _bulletPool[(int)BulletType.FinishDefault] = new ObjectPool<HookBullet>(CreateFinishComboBulletOnPool, GetPoolBullet, ReturnBulletToPool, (bullet) => Destroy(bullet.gameObject), true, 50, 500);
        _bulletPool[(int)BulletType.Heavy] = new ObjectPool<HookBullet>(CreateHeavyBulletOnPool, GetPoolBullet, ReturnBulletToPool, (bullet) => Destroy(bullet.gameObject), true, 50, 500);
        _bulletPool[(int)BulletType.FinishHeavy] = new ObjectPool<HookBullet>(CreateLastHeavyBulletOnPool, GetPoolBullet, ReturnBulletToPool, (bullet) => Destroy(bullet.gameObject), true, 50, 500);
        _bulletPool[(int)BulletType.Skill] = new ObjectPool<HookBullet>(CreateSkillBulletOnPool, GetPoolBullet, ReturnBulletToPool, (bullet) => Destroy(bullet.gameObject), true, 50, 500);
        _bulletPool[(int)BulletType.HeavySkill] = new ObjectPool<HookBullet>(CreateSkillHeavyBulletOnPool, GetPoolBullet, ReturnBulletToPool, (bullet) => Destroy(bullet.gameObject), true, 50, 500);

        _bulletCreateEffectPool[(int)FireDeleteType.Default] = new ObjectPool<FireDelete>(CreateDefaultBulletFireDeleteOnPool, GetPoolBulletFireDelete, ReturnBulletFireDelete, (effect) => Destroy(effect.gameObject), true, 50, 500);
        _bulletCreateEffectPool[(int)FireDeleteType.Heavy] = new ObjectPool<FireDelete>(CreateHeavyBulletFireDeleteOnPool, GetPoolBulletFireDelete, ReturnBulletFireDelete, (effect) => Destroy(effect.gameObject), true, 50, 500);
        _bulletCreateEffectPool[(int)FireDeleteType.FinishHeavy] = new ObjectPool<FireDelete>(CreateLastHeavyBulletFireDeleteOnPool, GetPoolBulletFireDelete, ReturnBulletFireDelete, (effect) => Destroy(effect.gameObject), true, 50, 500);
    }
    private HookBullet CreateBulletOnPool()
    {
        HookBullet bullet = Instantiate(_bullets[(int)BulletType.Default], transform);
        bullet.Pool = _bulletPool[(int)BulletType.Default];
        bullet.constructor = gameObject;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return bullet;
    }
    private HookBullet CreateFinishComboBulletOnPool()
    {
        HookBullet bullet = Instantiate(_bullets[(int)BulletType.FinishDefault], transform);
        bullet.Pool = _bulletPool[(int)BulletType.FinishDefault];
        bullet.constructor = gameObject;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return bullet;
    }
    private HookBullet CreateHeavyBulletOnPool()
    {
        HookBullet bullet = Instantiate(_bullets[(int)BulletType.Heavy], transform);
        bullet.Pool = _bulletPool[(int)BulletType.Heavy];
        bullet.constructor = gameObject;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return bullet;
    }
    private HookBullet CreateLastHeavyBulletOnPool()
    {
        HookBullet bullet = Instantiate(_bullets[(int)BulletType.FinishHeavy], transform);
        bullet.Pool = _bulletPool[(int)BulletType.FinishHeavy];
        bullet.constructor = gameObject;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return bullet;
    }
    private HookBullet CreateSkillBulletOnPool()
    {
        HookBullet bullet = Instantiate(_bullets[(int)BulletType.Skill], transform);
        bullet.Pool = _bulletPool[(int)BulletType.Skill];
        bullet.constructor = gameObject;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return bullet;
    }
    private HookBullet CreateSkillHeavyBulletOnPool()
    {
        HookBullet bullet = Instantiate(_bullets[(int)BulletType.HeavySkill], transform);
        bullet.Pool = _bulletPool[(int)BulletType.HeavySkill];
        bullet.constructor = gameObject;
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return bullet;
    }
    private void GetPoolBullet(HookBullet bullet)
    {
        bullet.gameObject.SetActive(true);
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
    }
    private void ReturnBulletToPool(HookBullet bullet) => bullet.gameObject.SetActive(false);
    private FireDelete CreateDefaultBulletFireDeleteOnPool()
    {
        FireDelete effect = Instantiate(_bulletCreateEffects[(int)FireDeleteType.Default], transform);
        effect.pool = _bulletCreateEffectPool[(int)FireDeleteType.Default];
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return effect;
    }
    private FireDelete CreateHeavyBulletFireDeleteOnPool()
    {
        FireDelete effect = Instantiate(_bulletCreateEffects[(int)FireDeleteType.Heavy], transform);
        effect.pool = _bulletCreateEffectPool[(int)FireDeleteType.Heavy];
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return effect;
    }
    private FireDelete CreateLastHeavyBulletFireDeleteOnPool()
    {
        FireDelete effect = Instantiate(_bulletCreateEffects[(int)FireDeleteType.FinishHeavy], transform);
        effect.pool = _bulletCreateEffectPool[(int)FireDeleteType.FinishHeavy];
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
        return effect;
    }
    private void GetPoolBulletFireDelete(FireDelete effect)
    {
        effect.gameObject.SetActive(true);
        effect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f); // 크기 설정
    }
    private void ReturnBulletFireDelete(FireDelete effect) => effect.gameObject.SetActive(false);
}
