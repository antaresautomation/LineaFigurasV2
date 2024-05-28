using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaComun.Clases
{
    public static class NamedPipeHelper
    {
        private const string PipeName = "miPipe";

        public static async Task EnviarVariableAsync(int variable)
        {
            using (var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.Out))
            {
                await pipeServer.WaitForConnectionAsync();
                using (var writer = new StreamWriter(pipeServer))
                {
                    await writer.WriteLineAsync(variable.ToString());
                }
            }
        }

        public static async Task<int> RecibirVariableAsync()
        {
            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.In))
            {
                await pipeClient.ConnectAsync();
                using (var reader = new StreamReader(pipeClient))
                {
                    var result = await reader.ReadLineAsync();
                    return int.Parse(result);
                }
            }
        }

        public static async Task EscucharCambiosAsync()
        {
            while (true)
            {
                int variable = await RecibirVariableAsync();
                OnVariableChanged(variable);
            }
        }

        public delegate void VariableChangedEventHandler(int newValue);
        public static event VariableChangedEventHandler VariableChanged;

        private static void OnVariableChanged(int newValue)
        {
            VariableChanged?.Invoke(newValue);
        }
    }
}
