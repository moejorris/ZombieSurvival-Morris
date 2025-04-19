using UnityEngine;

public class ChargeBeamStandIn : MonoBehaviour
{
    [SerializeField] float peakSize = 0.5f;
    [SerializeField] Vector3 rotationSpeed = new Vector3(0, -900f, 900f);

    void Awake()
    {
        peakSize = transform.localScale.x;
    }

    void Update()
    {
        transform.localEulerAngles += rotationSpeed * Time.deltaTime;
    }

    public void UpdateChargeAmount(float chargeAmount)
    {
        transform.localScale = Vector3.one * chargeAmount * peakSize;
        if(chargeAmount >= 1)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
