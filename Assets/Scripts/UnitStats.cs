using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    public int CurrentHealth = 100;
    public int MaxHealth = 100;

    private int Barrier = 0; // ���������� ���� (������)
    private List<Effect> ActiveEffects = new List<Effect>();

    /// <summary>
    /// ��������� ����, �������� ������. ���������� ���� ��������� � ��������.
    /// </summary>
    public void ApplyDamage(int damage)
    {
        int remainingDamage = Mathf.Max(0, damage - Barrier);
        Barrier = Mathf.Max(0, Barrier - damage);
        CurrentHealth -= remainingDamage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); // ������������ � ���������
        Debug.Log($"Unit took {damage} damage. Remaining health: {CurrentHealth}, Barrier: {Barrier}"); // ������ ����������� ��� ������������.
    }

    /// <summary>
    /// ��������� ������.
    /// </summary>
    public void AddBarrier(int value)
    {
        Barrier += value;
        Debug.Log($"Barrier increased by {value}. Current barrier: {Barrier}");
    }

    /// <summary>
    /// ��������� ��� �������� �������, ������ �� ������������.
    /// </summary>
    public void ApplyEffects()
    {
        for (int i = ActiveEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffects[i].Apply(this); // ��������� �������� �������
            ActiveEffects[i].Duration--; // ��������� ������������

            if (ActiveEffects[i].IsExpired()) // �������, ���� ������ ����
            {
                ActiveEffects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ��������� ��������������� ��������, ������� ������� � ���������� ������.
    /// </summary>
    public void Reset()
    {
        CurrentHealth = MaxHealth;
        Barrier = 0;
        ActiveEffects.Clear();
    }

    /// <summary>
    /// ��������� ����� ������.
    /// </summary>
    public void AddEffect(Effect effect)
    {
        ActiveEffects.Add(effect);
        Debug.Log($"Effect '{effect.Name}' added. Duration: {effect.Duration}");
    }

    /// <summary>
    /// ������� ������� � ��������� ���������.
    /// </summary>
    public void RemoveEffect(string effectName)
    {
        ActiveEffects.RemoveAll(effect => effect.Name == effectName);
    }

    /// <summary>
    /// ��������������� ��������.
    /// </summary>
    public void RestoreHealth(int value)
    {
        CurrentHealth += value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); // ������������ � �������� ���������
        Debug.Log($"Unit healed by {value}, current health {CurrentHealth}.");
    }
}

/// <summary>
/// �����, ����������� ������� �� ��������� (��������, "�������" ��� "�����������").
/// </summary>
public class Effect
{
    public string Name { get; private set; }
    public int Duration { get; set; } // ������������ �������
    public System.Action<UnitStats> ApplyAction; // �������� �������

    /// <summary>
    /// ����������� �������.
    /// </summary>
    public Effect(string name, int duration, System.Action<UnitStats> applyAction)
    {
        Name = name;
        Duration = duration;
        ApplyAction = applyAction;
    }

    /// <summary>
    /// ��������� ������ � ��������� ���������� �����.
    /// </summary>
    public void Apply(UnitStats target)
    {
        ApplyAction?.Invoke(target); // ��������� ��������
    }

    /// <summary>
    /// ���������, ���� �� ������.
    /// </summary>
    public bool IsExpired()
    {
        return Duration <= 0;
    }
}
