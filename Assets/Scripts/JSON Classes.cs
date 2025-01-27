[System.Serializable]
public class ClientRequest
{
    public string Action;
    public int AbilityIndex;
}

[System.Serializable]
public class ServerResponse
{
    public int PlayerHealth;
    public int EnemyHealth;
    public bool IsPlayerTurn;
}
