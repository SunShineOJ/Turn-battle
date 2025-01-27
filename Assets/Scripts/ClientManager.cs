using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public ServerManager Server;
    public HealthBar PlayerHealthBar;
    public HealthBar EnemyHealthBar;

    public void Start()
    {
        Server = new ServerManager(); // Локальный сервер
        UpdateUI(Server.GenerateResponse());
    }

    public void OnAbilityButtonClick(int abilityIndex)
    {
        ClientRequest request = new ClientRequest
        {
            Action = "USE_ABILITY",
            AbilityIndex = abilityIndex
        };

        string requestJson = JsonUtility.ToJson(request);
        string responseJson = Server.ProcessClientRequest(requestJson); // Запрос на сервер
        UpdateUI(responseJson);
    }

    public void OnRestartButtonClick()
    {
        ClientRequest request = new ClientRequest
        {
            Action = "RESTART"
        };
        string requestJson = JsonUtility.ToJson(request);
        string responseJson = Server.ProcessClientRequest(requestJson); // Запрос на сервер

        // Перезапуск боя
        Debug.Log("Перезапуск боя...");
        UpdateUI(responseJson);
    }

    private void UpdateUI(string responseJson)
    {
        ServerResponse response = JsonUtility.FromJson<ServerResponse>(responseJson);

        PlayerHealthBar.UpdateHealth(response.PlayerHealth, 100);
        EnemyHealthBar.UpdateHealth(response.EnemyHealth, 100);

        Debug.Log($"Player Health: {response.PlayerHealth}, Enemy Health: {response.EnemyHealth}");
    }
}
