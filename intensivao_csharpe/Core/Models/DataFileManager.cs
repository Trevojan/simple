using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DataFileManager
    {
        private readonly string outputDir = Path.Combine("..", "Output");

        public async Task StartMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Simple Data ---");
                Console.WriteLine("1 > Exportar novo item");
                Console.WriteLine("2 > Editar arquivo existente");
                Console.WriteLine("0 > Sair");
                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out int opt)) continue;
                if (await MenuOptionAsync(opt)) break;
            }
        }

        private async Task<bool> MenuOptionAsync(int opt)
        {
            Console.Clear();
            switch (opt)
            {
                case 0:
                    return true;
                case 1:
                    await ExportDataItemAsync();
                    break;
                case 2:
                    await EditFileAsync();
                    break;
                default:
                    Console.WriteLine("Opção inválida. Pressione Enter.");
                    Console.ReadLine();
                    break;
            }
            return false;
        }

        private async Task ExportDataItemAsync()
        {
            Console.Write("Nome do arquivo alvo (sem extensão): ");
            string fileName = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(fileName) || !ValidateFileName(fileName)) return;

            Directory.CreateDirectory(outputDir);
            string path = Path.Combine(outputDir, $"{fileName}.json");

            var dataList = File.Exists(path)
                ? JsonSerializer.Deserialize<List<DataFile>>(File.ReadAllText(path)) ?? new List<DataFile>()
                : new List<DataFile>();

            DataFile item = PromptDataFile();
            dataList.Add(item);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(dataList, options));

            Console.WriteLine("Item adicionado. Pressione Enter.");
            Console.ReadLine();
        }

        private async Task EditFileAsync()
        {
            Directory.CreateDirectory(outputDir);
            string[] files = Directory.GetFiles(outputDir, "*.json");
            if (files.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo encontrado. Pressione Enter.");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < files.Length; i++)
                Console.WriteLine($"{i + 1} > {Path.GetFileName(files[i])}");

            Console.Write("Escolha o número do arquivo: ");
            if (!int.TryParse(Console.ReadLine(), out int fileChoice) || fileChoice < 1 || fileChoice > files.Length) return;

            string path = files[fileChoice - 1];
            var dataList = JsonSerializer.Deserialize<List<DataFile>>(File.ReadAllText(path)) ?? new List<DataFile>();

            Console.Clear();
            Console.WriteLine("Itens no arquivo:");
            for (int i = 0; i < dataList.Count; i++)
                Console.WriteLine($"{i + 1} > {dataList[i].Name} [{dataList[i].Prioridade}] ({dataList[i].Status})");

            Console.Write("Escolha o item para editar: ");
            if (!int.TryParse(Console.ReadLine(), out int itemChoice) || itemChoice < 1 || itemChoice > dataList.Count) return;

            dataList[itemChoice - 1] = PromptDataFile();

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(dataList, options));

            Console.WriteLine("Item atualizado. Pressione Enter.");
            Console.ReadLine();
        }

        private DataFile PromptDataFile()
        {
            Console.Write("Nome: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Descrição: ");
            string desc = Console.ReadLine() ?? "";

            Console.Write("Prioridade (Baixa, Media, Alta): ");
            Enum.TryParse(Console.ReadLine(), true, out Prioridade prioridade);

            Console.Write("Status (Pendente, EmAndamento, Concluido): ");
            Enum.TryParse(Console.ReadLine(), true, out Status status);

            return new DataFile
            {
                Name = name,
                Descricao = desc,
                Prioridade = prioridade,
                Status = status
            };
        }

        private bool ValidateFileName(string input)
        {
            string pattern = @"^[^<>:\""/\\|?*]+$";
            return Regex.IsMatch(input, pattern);
        }
    }
}
