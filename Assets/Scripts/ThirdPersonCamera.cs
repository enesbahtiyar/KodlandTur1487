using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour 
{        
    [SerializeField] GameObject player;
    [SerializeField][Range(0.5f, 2f)]
    float mouseSense = 1; 
    [SerializeField][Range(-20, -10)]
     int lookUp = -15;
    [SerializeField][Range(15, 25)]
    int lookDown = 20;
    Animator anim;

    public bool isSpectator;
    //serbest kamera hareket modu hızı
    [SerializeField] float speed = 50f;

    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }
    void Update()
    {
        float rotateX = Input.GetAxis("Mouse X") * mouseSense;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSense;
        if (!isSpectator)
        {

            Vector3 rotCamera = transform.rotation.eulerAngles;
            Vector3 rotPlayer = player.transform.rotation.eulerAngles;

            rotCamera.x = (rotCamera.x > 180) ? rotCamera.x - 360 : rotCamera.x;
            rotCamera.x = Mathf.Clamp(rotCamera.x, lookUp, lookDown);
            rotCamera.x -= rotateY;

            rotCamera.z = 0;
            rotPlayer.y += rotateX;

            transform.rotation = Quaternion.Euler(rotCamera);
            player.transform.rotation = Quaternion.Euler(rotPlayer);
        }
        else
        {
            //serbest kamera kodu 
            //mevcut kamera açısını alalım
            Vector3 rotCamera = transform.rotation.eulerAngles;

            //farenin hareketine bağlı olarak kameranın açısını değiştirecez
            rotCamera.x -= rotateY;
            rotCamera.z = 0;
            rotCamera.y += rotateX;

            transform.rotation = Quaternion.Euler(rotCamera);

            //wasd tuşlarını çekelim
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //wasd tuşlarına göre kameranın konumunu ayarlama yani position
            Vector3 dir = transform.right * x + transform.forward * z;

            transform.position += dir * speed * Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            //eğer imleç kitliyse
            if(Cursor.lockState == CursorLockMode.Locked) 
            {
                Cursor.lockState = CursorLockMode.None;
            }
            //eğer kitli değilse
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

 
    }
}
 
