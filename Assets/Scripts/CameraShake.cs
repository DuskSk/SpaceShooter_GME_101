using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeAmount = 1f,
        _shakeDuration = 0f,
        _initialShakeDuration = 2f,
        _decreaseFactor;
    
    private Vector3 _initialCameraPosition;
    private float _camPosX, _camPosY;
    
    
    void Start()
    {
         _initialCameraPosition = transform.position;
        
    }    

    private void ShakeCamera()
    {
        _camPosX = Random.insideUnitCircle.x * _shakeAmount;
        _camPosY = Random.insideUnitCircle.y * _shakeAmount;
        transform.position = new Vector3(_camPosX, _camPosY, _initialCameraPosition.z);

        
    }

    private void StopShakeCamera()
    {
        transform.position = _initialCameraPosition;
    }


    private IEnumerator ShakeCameraRoutine()
    {
        while (true)
        {
            if (_shakeDuration > 0)
            {
                ShakeCamera();                
                _shakeDuration -= Time.deltaTime * _decreaseFactor;
                
            }
            else
            {
                _shakeDuration = _initialShakeDuration;
                StopShakeCamera();
                break;

            }
            yield return new WaitForEndOfFrame();
           
        }        

    }

    public void StartCameraShake()
    {
        StartCoroutine(ShakeCameraRoutine());
    }



}
