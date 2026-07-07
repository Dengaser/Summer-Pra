using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SwitchMagic : MonoBehaviour
{
    [System.Serializable]
    public class AbilityData
    {
        public string abilityName;      // Название для отображения в UI
        public MonoBehaviour script;     // Ссылка на компонент (IceMagic, FireMagic и т.д.)
        public bool isUnlocked = false;  // По умолчанию выключено, открываем по ходу игры!
    }

    [Header("Интерфейс")]
    public TextMeshProUGUI magicText; // Ссылка на TextMeshPro на экране
    public TextMeshProUGUI notificationText;

    [Header("Список способностей")]
    public List<AbilityData> abilities = new List<AbilityData>(); 

    private List<AbilityData> activeAbilities = new List<AbilityData>(); 
    private int currentAbilityIndex = 0; 
    private int collectedCatsCount = 0;

    private int catsAtLevelStart = 0;

    void Start()
    {
        // 1. Загружаем сохраненный прогресс
        LoadProgress();

        // 2. ЗАПОМИНАЕМ, сколько у нас было котов на старте уровня
        catsAtLevelStart = collectedCatsCount;

        // 3. Обновляем доступные способности
        RefreshAvailableAbilities();
    }

    void Update()
    {
        // Динамически проверяем нажатия клавиш 1, 2, 3 в зависимости от количества доступных магий
        for (int i = 0; i < activeAbilities.Count; i++) 
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) 
            {
                SelectAbility(i); 
                break; 
            }
        }
    }

    /// Вызывается из кота-триггера при столкновении с игроком

    public void UnlockNextAbility()
    {
        if (collectedCatsCount < abilities.Count)
        {
            // Открываем способность по текущему индексу
            abilities[collectedCatsCount].isUnlocked = true;

            // Запоминаем имя новой способности для уведомления
            string newlyUnlockedName = abilities[collectedCatsCount].abilityName;

            int newAbilityIndex = collectedCatsCount;
            collectedCatsCount++;

            // Сохраняем прогресс в память
            SaveProgress();

            // Перестраиваем список активных способностей
            RefreshAvailableAbilities();

            // Ищем, под каким индексом наша НОВАЯ способность оказалась в списке активных
            for (int i = 0; i < activeAbilities.Count; i++)
            {
                if (activeAbilities[i].abilityName == abilities[newAbilityIndex].abilityName)
                {
                    SelectAbility(i);
                    break;
                }
            }

            // ЗАПУСКАЕМ УВЕДОМЛЕНИЕ: Показываем надпись на экране
            ShowAbilityNotification(newlyUnlockedName);

            Debug.Log($"Способность разблокирована! Всего котов: {collectedCatsCount}");
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("TotalUnlockedCats", collectedCatsCount);
        PlayerPrefs.Save();
    }

    public void LoadProgress(bool resetToLevelStart = false)
    {
        if (resetToLevelStart)
        {
            // Откатываем счётчик к значению, которое было при заходе на уровень
            collectedCatsCount = catsAtLevelStart;

            // Перезаписываем откаченный результат в PlayerPrefs перед смертью/перезапуском
            SaveProgress();
        }
        else
        {
            // Обычная загрузка при старте сцены
            collectedCatsCount = PlayerPrefs.GetInt("TotalUnlockedCats", 0);
        }

        // Сначала блокируем ВСЕ способности (сбрасываем старое состояние в памяти скрипта)
        foreach (var ability in abilities)
        {
            ability.isUnlocked = false;
        }

        // Открываем только те, что укладываются в наш текущий счётчик
        for (int i = 0; i < collectedCatsCount; i++)
        {
            if (i < abilities.Count)
            {
                abilities[i].isUnlocked = true;
            }
        }
    }

    /// Фильтрует список: оставляет только те способности, которые сейчас разблокированы.
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
                // Если магия заблокирована — принудительно выключаем её скрипт
                ability.script.enabled = false; 
            }
        }

        // Корректируем индекс, если текущая выбранная магия стала недоступна
        if (currentAbilityIndex >= activeAbilities.Count) 
        {
            currentAbilityIndex = 0; 
        }

        // Активируем способность
        if (activeAbilities.Count > 0) 
        {
            SelectAbility(currentAbilityIndex); 
        }
        else 
        {
            UpdateUI(); 
        }
    }

    // Активирует способность по индексу из списка доступных
    private void SelectAbility(int index)
    {
        if (index < 0 || index >= activeAbilities.Count) return; 

        currentAbilityIndex = index;

        // ШАГ 1: Сначала полностью выключаем ВСЕ скрипты магий на игроке
        foreach (var ability in abilities)
        {
            if (ability.script != null)
            {
                ability.script.enabled = false;
            }
        }

        // ШАГ 2: Включаем только ОДИН активный скрипт, выбранный в данный момент
        if (activeAbilities[currentAbilityIndex].script != null)
        {
            activeAbilities[currentAbilityIndex].script.enabled = true;
        }

        // Обновляем весь список способностей в UI[
        UpdateUI(); 
    }

    /// Генерирует динамическую строку для UI на основе количества открытых способностей
    private void UpdateUI() 
    {
        if (magicText == null) return; 

        // Если способностей 0 или ВСЕГО ОДНА — ничего не пишем на экране
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
            // Отменяем прошлые скрытия, если игрок умудрился собрать двух котов подряд
            CancelInvoke(nameof(HideNotification)); 

            
            notificationText.text = $"Получена способность:\n<color=#FFD700>{abilityName}</color>!\n<size=80%>Использовать: R|ЛКМ</size>";
            notificationText.gameObject.SetActive(true); 

            // Вызываем скрытие надписи ровно через 3 секунды
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
    // Этот код добавляет кнопку "Сбросить прогресс" прямо в инспектор скрипта SwitchMagic
    [ContextMenu("Сбросить прогресс кооотов")]
    public void ResetProgressEditor()
    {
        PlayerPrefs.DeleteKey("TotalUnlockedCats");
        collectedCatsCount = 0;
        foreach (var ability in abilities)
        {
            ability.isUnlocked = false;
        }
        Debug.Log("Прогресс котов успешно сброшен для следующего запуска!");
    }
#endif
}