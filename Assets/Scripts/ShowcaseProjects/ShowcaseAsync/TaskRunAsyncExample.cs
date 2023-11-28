using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShowcaseProjects.ShowcaseAsync
{
    public class TaskRunAsyncExample : MonoBehaviour
    {
        [SerializeField] private Button runButton;
        [SerializeField] private TextMeshProUGUI resultText;

        private void Awake()
        {
            runButton.onClick.AddListener(OnRunButton);
        }

        private async void OnRunButton()
        {
            Debug.Log($"Thread id{Thread.CurrentThread.ManagedThreadId}");  // основной поток id = 1
            resultText.text = "Processing...";

            //Методы Task.Run также используют пул потоков для выполнения асинхронных операций.
            //Когда вы используете Task.Run, вы отправляете вашу задачу в пул потоков, который выполняет ее асинхронно.
            //Это удобно, потому что создание нового потока может быть дорогостоящей операцией
            
            Task<string> task1 = Task.Run(() => SomeMethod1()); // берет фоновый поток из ThreadPool
            Task<string> task2 = Task.Run(() => SomeMethod2()); 

            await Task.WhenAll(task1, task2);
            string result = $"{task1.Result}, {task2.Result}";

            Debug.Log($"Thread id{Thread.CurrentThread.ManagedThreadId}");  // основной поток id = 1
            // Обратите внимание, что изменения объектов Unity (например, изменение положения объекта или обновление текстуры)
            // должны осуществляться в основном потоке (main thread) для избежания проблем с синхронизацией.
            resultText.text = result; 
        }

        private async Task<string> SomeMethod1()
        {
            Debug.Log($"Thread id{Thread.CurrentThread.ManagedThreadId}"); // фоновый поток 
            await Task.Delay(500);
            Debug.Log($"Thread id{Thread.CurrentThread.ManagedThreadId}");

            return "Result from Method 1";
        }

        private async Task<string> SomeMethod2()
        {
            Debug.Log($"Thread id{Thread.CurrentThread.ManagedThreadId}"); // фоновый поток 
            await Task.Delay(1000);
            Debug.Log($"Thread id{Thread.CurrentThread.ManagedThreadId}");

            return "Result from Method 2";
        }

    }
}