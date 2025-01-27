public class Ability
{
    public string Name;
    public int Duration;
    public int Cooldown;
    private System.Action<UnitStats, UnitStats> Effect;

    public Ability(string name, int duration, int cooldown, System.Action<UnitStats, UnitStats> effect)
    {
        Name = name;
        Duration = duration;
        Cooldown = cooldown;
        Effect = effect;
    }

    public void Execute(UnitStats caster, UnitStats target)
    {
        Effect?.Invoke(caster, target);
    }
}
