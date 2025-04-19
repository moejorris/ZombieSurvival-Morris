using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField]float _damage;
    [SerializeField]float _speed = 5f;
    [SerializeField]Vector3 _direction;
    [SerializeField]GameObject _impactParticle;
    [SerializeField]GameObject _bloodParticle;
    [SerializeField]GameObject _areaOfEffect;

    public void InitProjectile(Vector3 direction, float speed, float damage, GameObject impactParticle = null, GameObject bloodParticle = null, GameObject areaOfEffect = null)
    {
        _direction = direction;
        _speed = speed;
        _damage = damage;
        if(impactParticle) _impactParticle = impactParticle;
        if(areaOfEffect) _areaOfEffect = areaOfEffect;
        if(bloodParticle) _bloodParticle = bloodParticle;
        transform.forward = _direction;
        GetComponent<Rigidbody>().velocity = _direction * _speed;
        Destroy(gameObject, 30f);
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 point = Vector3.zero;
        Vector3 normal = Vector3.zero;
        Transform parent = null;
        bool health = false;

        bool isValidContact = false;

        foreach(ContactPoint contact in collision.contacts)
        {
            normal += contact.normal;
            point += contact.point;

            isValidContact = !(contact.otherCollider.GetComponent<PlayerHealth>() || contact.otherCollider.GetComponent<PlayerProjectile>());
        
            if(!parent) parent = contact.otherCollider.transform;
            if(contact.otherCollider.GetComponent<ZombieLimb>() && isValidContact) 
            {
                health = true;
                contact.otherCollider.GetComponent<ZombieLimb>().TakeDamage(_damage); 
                Instantiate(_bloodParticle, contact.point, Quaternion.identity);
            }
        }

        if(!isValidContact) return;

        normal.Normalize();
        point /= collision.contactCount;

        // health?.TakeDamage(_damage);
        if(_impactParticle)
        {
            Transform impact = Instantiate(_impactParticle, point, Quaternion.identity, parent).transform;
            impact.forward = normal;
        }

        if(_bloodParticle && health) Instantiate(_bloodParticle, point, Quaternion.identity);
        if(_areaOfEffect)
        {
            GameObject aoe = Instantiate(_areaOfEffect, point, Quaternion.identity);
            if(aoe.GetComponent<MissileAOE>()) aoe.GetComponent<MissileAOE>().damage = _damage /2f;
        }
        Debug.DrawRay(point, normal, Color.red, 10f);

        Destroy(gameObject);
    }
}
