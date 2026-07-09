using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchMagic : MonoBehaviour
{
    [System.Serializable]
    public class AbilityData
    {
        public string abilityName;      // �������� ��� ����������� � UI
        public MonoBehaviour script;     // ������ �� ��������� (IceMagic, FireMagic � �.�.)
        public bool isUnlocked = false;  // �� ��������� ���������, ��������� �� ���� ����!
    }

    [Header("���������")]
    public TextMeshProUGUI magicText; // ������ �� TextMeshPro �� ������
    public TextMeshProUGUI notificationText;

    [Header("������ ������������")]
    public List<AbilityData> abilities = new List<AbilityData>(); 

    private List<AbilityData> activeAbilities = new List<AbilityData>(); 
    private int currentAbilityIndex = 0; 
    private int collectedCatsCount = 0;

    private int catsAtLevelStart = 0;

    void Start()
    {

        LoadProgress();
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool progressCorrected = false;


        if (currentSceneName == "DaniilA")
        {
            if (collectedCatsCount > 0)
            {
                collectedCatsCount = 0;
                progressCorrected = true;
            }
        }
        else if (currentSceneName == "Level2")
        {
            if (collectedCatsCount < 1)
            {
                collectedCatsCount = 1;
                progressCorrected = true;
            }
        }
        else if (currentSceneName == "Level3")
        {
            if (collectedCatsCount < 2)
            {
                collectedCatsCount = 2;
                progressCorrected = true;
            }
        }
        
        
        if (progressCorrected)
        {
          
            for (int i = 0; i < collectedCatsCount; i++)
            {
                if (i < abilities.Count)
                {
                    abilities[i].isUnlocked = true;
                }
            }

            
            SaveProgress();
        }
       

        // 2. Запоминаем проверенное количество котов на начало уровня
        catsAtLevelStart = collectedCatsCount;

        // 3. Обновляем доступные способности и UI
        RefreshAvailableAbilities();
    }

    void Update()
    {
        // ����������� ��������� ������� ������ 1, 2, 3 � ����������� �� ���������� ��������� �����
        for (int i = 0; i < activeAbilities.Count; i++) 
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) 
            {
                SelectAbility(i); 
                break; 
            }
        }
    }

    /// ���������� �� ����-�������� ��� ������������ � �������

    public void UnlockNextAbility()
    {
        if (collectedCatsCount < abilities.Count)
        {
            // ��������� ����������� �� �������� �������
            abilities[collectedCatsCount].isUnlocked = true;

            // ���������� ��� ����� ����������� ��� �����������
            string newlyUnlockedName = abilities[collectedCatsCount].abilityName;

            int newAbilityIndex = collectedCatsCount;
            collectedCatsCount++;

            // ��������� �������� � ������
            //SaveProgress();

            // ������������� ������ �������� ������������
            RefreshAvailableAbilities();

            // ����, ��� ����� �������� ���� ����� ����������� ��������� � ������ ��������
            for (int i = 0; i < activeAbilities.Count; i++)
            {
                if (activeAbilities[i].abilityName == abilities[newAbilityIndex].abilityName)
                {
                    SelectAbility(i);
                    break;
                }
            }

            // ��������� �����������: ���������� ������� �� ������
            ShowAbilityNotification(newlyUnlockedName);

            Debug.Log($"����������� ��������������! ����� �����: {collectedCatsCount}");
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("TotalUnlockedCats", collectedCatsCount);
        PlayerPrefs.Save();
    }

    public void LoadProgress(bool resetToLevelStart = false)
    {
        if (resetToLevelStart)
        {
            // ���������� ������� � ��������, ������� ���� ��� ������ �� �������
            collectedCatsCount = catsAtLevelStart;

            // �������������� ���������� ��������� � PlayerPrefs ����� �������/������������
            SaveProgress();
        }
        else
        {
            // ������� �������� ��� ������ �����
            collectedCatsCount = PlayerPrefs.GetInt("TotalUnlockedCats", 0);
        }

        // ������� ��������� ��� ����������� (���������� ������ ��������� � ������ �������)
        foreach (var ability in abilities)
        {
            ability.isUnlocked = false;
        }

        // ��������� ������ ��, ��� ������������ � ��� ������� �������
        for (int i = 0; i < collectedCatsCount; i++)
        {
            if (i < abilities.Count)
            {
                abilities[i].isUnlocked = true;
            }
        }
    }
    public void ResetToLevelStart()
    {
       
        LoadProgress(resetToLevelStart: true);
    }

    /// ��������� ������: ��������� ������ �� �����������, ������� ������ ��������������.
    public void RefreshAvailableAbilities() 
    {
        activeAbilities.Clear(); 

        foreach (var ability in abilities) 
        {
            if (ability.isUnlocked && ability.script != null) 
            {
                activeAbilities.Add(ability); 
            }
            else if (ability.script != null) 
            {
                // ���� ����� ������������� � ������������� ��������� � ������
                ability.script.enabled = false; 
            }
        }

        // ������������ ������, ���� ������� ��������� ����� ����� ����������
        if (currentAbilityIndex >= activeAbilities.Count) 
        {
            currentAbilityIndex = 0; 
        }

        // ���������� �����������
        if (activeAbilities.Count > 0) 
        {
            SelectAbility(currentAbilityIndex); 
        }
        else 
        {
            UpdateUI(); 
        }
    }

   
    private void SelectAbility(int index)
    {
        if (index < 0 || index >= activeAbilities.Count) return; 

        currentAbilityIndex = index;

       
        foreach (var ability in abilities)
        {
            if (ability.script != null)
            {
                ability.script.enabled = false;
            }
        }

       
        if (activeAbilities[currentAbilityIndex].script != null)
        {
            activeAbilities[currentAbilityIndex].script.enabled = true;
        }

        
        UpdateUI(); 
    }

    
    private void UpdateUI() 
    {
        if (magicText == null) return; 

        
        if (activeAbilities.Count <= 1) 
        {
            magicText.text = ""; 
            return; 
        }

        StringBuilder sb = new StringBuilder(); 

        for (int i = 0; i < activeAbilities.Count; i++) 
        {
            int slotNumber = i + 1; 
            string name = activeAbilities[i].abilityName; 

            if (i == currentAbilityIndex) 
            {
                sb.Append($"<color=#FFD700>[{slotNumber} {name}]</color>"); 
            }
            else 
            {
                sb.Append($"{slotNumber} {name}"); 
            }

            if (i < activeAbilities.Count - 1) 
            {
                sb.Append("   "); 
            }
        }

        magicText.text = sb.ToString(); 
    }

    private void ShowAbilityNotification(string abilityName)
    {
        if (notificationText != null)
        {
           
            CancelInvoke(nameof(HideNotification)); 

            
            notificationText.text = $"Получена способность:\n<color=#FFD700>{abilityName}</color>!\n<size=80%>Использовать: R|ЛКМ</size>";
            notificationText.gameObject.SetActive(true); 

           
            Invoke(nameof(HideNotification), 3f); 
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    // ���� ��� ��������� ������ "�������� ��������" ����� � ��������� ������� SwitchMagic
    [ContextMenu("�������� �������� �������")]
    public void ResetProgressEditor()
    {
        PlayerPrefs.DeleteKey("TotalUnlockedCats");
        collectedCatsCount = 0;
        foreach (var ability in abilities)
        {
            ability.isUnlocked = false;
        }
        Debug.Log("�������� ����� ������� ������� ��� ���������� �������!");
    }
#endif
}