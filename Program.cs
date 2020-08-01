using System;

namespace MemoryTest
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        System.Timers.Timer gameCheck;
        IntPtr foundAdress = IntPtr.Zero;
        // Process gameProcess;

        public static long ReadInt64(IntPtr process, IntPtr baseAddress)
        {
            var buffer = new byte[8];
            IntPtr byteRead;
            // ReadProcessMemory(process, baseAddress, buffer, 8, byteRead);
        }

        private void CheckProcess()
        {
            gameProcess = Process.GetProcessesByName("Crysis2").FirstOrDefault();

            if(gameProcess != null) {
                gameCheck.Stop();
                var gameModule = gameProcess.MainModule;
                var baseAddress = gameModule.BaseAddress.ToInt64() + 0x0152ED0C;
                var offsets = new [] { 0x2E8 };

                var realAddress = GetRealAddress(gameProcess.Handle, (IntPtr)baseAddress, offsets);
                foundAddress = (IntPtr)realAddress;

                string Address = realAddress.ToString("X");

                string debugInfo = "Address: " + ReadInt32(gameProcess.Handle, (IntPtr)realAddress).ToString();
            }
        }

        public static long GetRealAddress(IntPtr process, IntPtr baseAddress, int[] offsets)
        {
            var address = baseAddress.ToInt64();
            foreach (var offset in offsets)
            {
                address = ReadInt64(process, (IntPtr)baseAddress) + offset;
            }

            return address;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
