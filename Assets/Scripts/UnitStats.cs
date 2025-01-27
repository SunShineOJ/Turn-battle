using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    public int CurrentHealth = 100;
    public int MaxHealth = 100;

    private int Barrier = 0; // Количество щита (Барьер)
    private List<Effect> ActiveEffects = new List<Effect>();

    /// <summary>
    /// Применяет урон, учитывая барьер. Оставшийся урон снимается с здоровья.
    /// </summary>
    public void ApplyDamage(int damage)
    {
        int remainingDamage = Mathf.Max(0, damage - Barrier);
        Barrier = Mathf.Max(0, Barrier - damage);
        CurrentHealth -= remainingDamage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); // Ограничиваем в диапазоне
        Debug.Log($"Unit took {damage} damage. Remaining health: {CurrentHealth}, Barrier: {Barrier}"); // Пример логирования для тестирования.
    }

    /// <summary>
    /// Добавляет барьер.
    /// </summary>
    public void AddBarrier(int value)
    {
        Barrier += value;
        Debug.Log($"Barrier increased by {value}. Current barrier: {Barrier}");
    }

    /// <summary>
    /// Применяет все активные эффекты, снижая их длительность.
    /// </summary>
    public void ApplyEffects()
    {
        for (int i = ActiveEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffects[i].Apply(this); // Применяем действие эффекта
            ActiveEffects[i].Duration--; // Уменьшаем длительность

            if (ActiveEffects[i].IsExpired()) // Удаляем, если эффект истёк
            {
                ActiveEffects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Полностью восстанавливает здоровье, очищает эффекты и сбрасывает барьер.
    /// </summary>
    public void Reset()
    {
        CurrentHealth = MaxHealth;
        Barrier = 0;
        ActiveEffects.Clear();
    }

    /// <summary>
    /// Добавляет новый эффект.
    /// </summary>
    public void AddEffect(Effect effect)
    {
        ActiveEffects.Add(effect);
        Debug.Log($"Effect '{effect.Name}' added. Duration: {effect.Duration}");
    }

    /// <summary>
    /// Удаляет эффекты с указанным названием.
    /// </summary>
    public void RemoveEffect(string effectName)
    {
        ActiveEffects.RemoveAll(effect => effect.Name == effectName);
    }

    /// <summary>
    /// Восстанавливает здоровье.
    /// </summary>
    public void RestoreHealth(int value)
    {
        CurrentHealth += value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); // Ограничиваем в пределах максимума
        Debug.Log($"Unit healed by {value}, current health {CurrentHealth}.");
    }
}

/// <summary>
/// Класс, описывающий эффекты на персонажа (например, "Горение" или "Регенерация").
/// </summary>
public class Effect
{
    public string Name { get; private set; }
    public int Duration { get; set; } // Длительность эффекта
    public System.Action<UnitStats> ApplyAction; // Действие эффекта

    /// <summary>
    /// Конструктор эффекта.
    /// </summary>
    public Effect(string name, int duration, System.Action<UnitStats> applyAction)
    {
        Name = name;
        Duration = duration;
        ApplyAction = applyAction;
    }

    /// <summary>
    /// Применяет эффект к указанным параметрам юнита.
    /// </summary>
    public void Apply(UnitStats target)
    {
        ApplyAction?.Invoke(target); // Выполняем действие
    }

    /// <summary>
    /// Проверяет, истёк ли эффект.
    /// </summary>
    public bool IsExpired()
    {
        return Duration <= 0;
    }
}
