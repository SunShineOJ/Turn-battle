using System.Collections.Generic;
using UnityEngine;

public class ServerManager
{
    private UnitStats PlayerStats = new UnitStats();
    private UnitStats EnemyStats = new UnitStats();
    private List<Ability> PlayerAbilities = new List<Ability>();
    private List<Ability> EnemyAbilities = new List<Ability>();
    private List<int> EnemyCooldowns = new List<int>(); //��� ������������ ��������� 
    private bool IsPlayerTurn = true;

    public ServerManager()
    {
        InitializeAbilities();
        ResetBattle();
    }

    private void InitializeAbilities()
    {
        PlayerAbilities.Add(new Ability("Attack", 0, 0, (caster, target) => target.ApplyDamage(8))); // ������ 0
        PlayerAbilities.Add(new Ability("Barrier", 2, 4, (caster, target) => caster.AddBarrier(5))); // ������ 1
        PlayerAbilities.Add(new Ability("Regeneration", 3, 5, (caster, target) => caster.AddEffect(new Effect("Regeneration", 3, stats => stats.RestoreHealth(2))))); // ������ 2
        PlayerAbilities.Add(new Ability("Fireball", 5, 6, (caster, target) => {
            target.ApplyDamage(5);
            target.AddEffect(new Effect("Burn", 5, stats => stats.ApplyDamage(1)));
        })); // ������ 3
        PlayerAbilities.Add(new Ability("Cleanse", 0, 5, (caster, target) => caster.RemoveEffect("Burn"))); // ������ 4
        EnemyAbilities.Add(new Ability("Attack", 0, 0, (caster, target) => target.ApplyDamage(6)));
        EnemyAbilities.Add(new Ability("Barrier", 2, 4, (caster, target) => caster.AddBarrier(5))); // ������ 1
        EnemyAbilities.Add(new Ability("Regeneration", 3, 5, (caster, target) => caster.AddEffect(new Effect("Regeneration", 3, stats => stats.RestoreHealth(2))))); // ������ 2
        EnemyAbilities.Add(new Ability("Fireball", 5, 6, (caster, target) => {
            target.ApplyDamage(5);
            target.AddEffect(new Effect("Burn", 5, stats => stats.ApplyDamage(1)));
        })); // ������ 3
        EnemyAbilities.Add(new Ability("Cleanse", 0, 5, (caster, target) => caster.RemoveEffect("Burn"))); // ������ 4
        // �������������� �������� ����� ��������� 0 (��� ����������� �������� � ������ ���)
        for (int i = 0; i < EnemyAbilities.Count; i++)
        {
            EnemyCooldowns.Add(0);
        }
    }



    public string ProcessClientRequest(string requestJson)
    {
        ClientRequest request = JsonUtility.FromJson<ClientRequest>(requestJson);

        if (request.Action == "USE_ABILITY")
        {
            UseAbility(request.AbilityIndex);
        }

        if (request.Action == "RESTART")
        {
            ResetBattle();
        }
        return GenerateResponse();
    }

    private void UseAbility(int abilityIndex)
    {
        if (IsPlayerTurn)
        {
            PlayerAbilities[abilityIndex].Execute(PlayerStats, EnemyStats);
        }
        else
        {
            EnemyUseRandomAbility();
        }

        EndTurn();
    }

    private void EnemyUseRandomAbility()
    {
        // �������� ��������� ����������� (�� �� ��������)
        List<int> availableAbilities = new List<int>();
        for (int i = 0; i < EnemyAbilities.Count; i++)
        {
            if (EnemyCooldowns[i] == 0)
            {
                availableAbilities.Add(i);
            }
        }

        if (availableAbilities.Count == 0)
        {
            Debug.Log("Enemy has no available abilities. Skipping turn.");
            return;
        }

        // �������� ��������� ����������� �� ���������
        int randomIndex = Random.Range(0, availableAbilities.Count);
        int abilityIndex = availableAbilities[randomIndex];

        // ���������� �����������
        EnemyAbilities[abilityIndex].Execute(EnemyStats, PlayerStats);

        // ������������� ������� ��� ���� �����������
        EnemyCooldowns[abilityIndex] = EnemyAbilities[abilityIndex].Cooldown;
        Debug.Log($"Enemy used '{EnemyAbilities[abilityIndex].Name}'.");
    }

    private void EndTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;

        // ��������� ������� � ����� ��������
        PlayerStats.ApplyEffects();
        EnemyStats.ApplyEffects();

        // ��������� �������� ��� ������������ �����
        for (int i = 0; i < EnemyCooldowns.Count; i++)
        {
            if (EnemyCooldowns[i] > 0)
            {
                EnemyCooldowns[i]--;
            }
        }

        // ��������� ����� ���
        if (PlayerStats.CurrentHealth <= 0 || EnemyStats.CurrentHealth <= 0)
        {
            ResetBattle();
        }
    }


    private void ResetBattle()
    {
        PlayerStats.Reset();
        EnemyStats.Reset();
    }

    public string GenerateResponse()
    {
        ServerResponse response = new ServerResponse
        {
            PlayerHealth = PlayerStats.CurrentHealth,
            EnemyHealth = EnemyStats.CurrentHealth,
            IsPlayerTurn = IsPlayerTurn
        };
        return JsonUtility.ToJson(response);
    }
}
