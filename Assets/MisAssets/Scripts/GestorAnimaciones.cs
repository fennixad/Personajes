using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// DESCRIPCION:
/// 
/// </summary>

[RequireComponent (typeof(CharacterController))]
public class GestorAnimaciones : MonoBehaviour
{
    // ***********************************************
    #region 1) Definicion de variables
    Animator anim;
    CharacterController cc;
    public CinemachineCamera[] cmApunta;

    float h, v;
    
    public float ejeH;
    public float ejeV;
    public bool apunta;
    public float velSuavizada, velMovimiento;

    Transform cam;
    Vector3 dir;

    #endregion
    // ***********************************************
    #region 2) Funciones de Unity
    private void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        cam = Camera.main.transform;

        cmApunta[0].Priority = 1;
        cmApunta[1].Priority = 0;
        if (velSuavizada <= 0f) velSuavizada = 4f;
        if (velMovimiento <= 0f) velMovimiento = 2f;
    }

    // Update is called once per frame
    void Update()
    {

        apunta = Input.GetMouseButton(1);

        if (apunta )
        {
            cmApunta[0].Priority = 1;
            cmApunta[1].Priority = 2;
        } else
        {
            cmApunta[0].Priority = 1;
            cmApunta[1].Priority = 0;
        }
            EjesVirtuales();
        DireccionRespectoCamara();
        Rotar();
        Mover();
        Actualizar_AnimMovimiento();
    }
    #endregion
// ***********************************************
    #region 3) Funciones originales
    void EjesVirtuales()
    {
        // ANTIGUO. VALORES DIRECTOS
        //ejeH = Input.GetAxisRaw("Horizontal");
        //ejeV = Input.GetAxisRaw("Vertical");

        // NUEVO. VALORES SUAVIZADOS
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        ejeH = Mathf.MoveTowards(ejeH, h, velSuavizada * Time.deltaTime);
        ejeV = Mathf.MoveTowards(ejeV, v, velSuavizada * Time.deltaTime);
    }

    void DireccionRespectoCamara()
    {
        dir = cam.right * h + cam.forward * v;
        dir.y = 0f;
        dir.Normalize();

        if (dir.magnitude > 0f) Debug.DrawRay(transform.position, dir, Color.yellow);
    }

    void Actualizar_AnimMovimiento()
    {
        if (apunta)
        {
            anim.SetFloat("X", ejeH);
            anim.SetFloat("Z", ejeV);
        }
        else
        {
            anim.SetFloat ("X", 0f);
            anim.SetFloat("Z", dir.magnitude);
        }
    }

    void Rotar()
    {
        if (apunta)
        {
                Vector3 dirFrontal = AccederDirFrontalCam();

                // rota el personaje segun eje frontal de la camara sin la inclinacion
                transform.rotation = Quaternion.LookRotation(dirFrontal);
        } else
        {
            if (dir.magnitude > 0f)
            {
                // rotacion segun input WASD
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    void Mover()
    {
        cc.Move(dir * velMovimiento *  Time.deltaTime);        
    }

    Vector3 AccederDirFrontalCam()
    {
        Vector3 dirFrontalCam = cam.forward;
        dirFrontalCam.y = 0f;
        dirFrontalCam.Normalize();

        return dirFrontalCam;
    }
    #endregion
// ***********************************************
}
