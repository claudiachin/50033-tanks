using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;   

    private bool m_IsInvincible;  
    private static bool m_collected;
    private GameObject PlayerTank;
    private Color newCol;

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    void Start() 
    {
        PlayerTank = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (GameObject.Find("ShieldPowerUp") == null && !m_collected) 
        {
            m_IsInvincible = true;
            m_collected = true;
            
            // change material
            foreach (Transform eachChild in PlayerTank.transform.GetChild(0))
            {
                ColorUtility.TryParseHtmlString("#FFAB16", out newCol);
                eachChild.GetComponent<Renderer>().materials[0].SetColor("_Color", newCol);
            }
            
            StartCoroutine(removeInvincible());
        }
    }

    IEnumerator removeInvincible()
    {
        yield return new WaitForSeconds(15.0f);

        // change material back
        foreach (Transform eachChild in PlayerTank.transform.GetChild(0))
        {
            ColorUtility.TryParseHtmlString("#2A64B2", out newCol);
            eachChild.GetComponent<Renderer>().materials[0].SetColor("_Color", newCol);
        }

        m_IsInvincible = false;
    }

    public void TakeDamage(float amount)
    {
        if (!(this == PlayerTank.GetComponent<TankHealth>() && m_IsInvincible)) {
            // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
            m_CurrentHealth -= amount;

            SetHealthUI();
            if (m_CurrentHealth <= 0f && !m_Dead) OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        gameObject.SetActive(false);

        m_IsInvincible = false;
    }
}