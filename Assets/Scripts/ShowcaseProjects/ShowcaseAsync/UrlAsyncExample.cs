using System;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcaseProjects.ShowcaseAsync
{
    public class UrlAsyncExample : MonoBehaviour
    {
        [SerializeField] private Button runButton;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TMP_InputField url;
        

        private void Awake()
        {
            runButton.onClick.AddListener(OnRunButton);
        }

        private async void OnRunButton()
        {
            try
            {
                // Установка текста "Processing..."
                resultText.text = "Processing...";

                // Используем using для автоматического закрытия HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Выполняем GET-запрос асинхронно
                    string result = await client.GetStringAsync(url.text);

                    // Ждем выполнения задержки в 2 секунды
                    await Task.Delay(2000);

                    // Отображаем первые 100 символов в resultText
                    resultText.text = result.Substring(0, Math.Min(result.Length, 100));
                }
            }
            catch (HttpRequestException ex)
            {
                // Обрабатываем исключения, которые могут возникнуть при выполнении запроса
                resultText.text = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Общий обработчик других исключений
                resultText.text = "An unexpected error occurred: " + ex.Message;
            }
        }
    }
}
