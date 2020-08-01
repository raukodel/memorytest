using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MemoryTest
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr process, IntPtr baseAddress, [Out] byte[] buffer, int size, out IntPtr bytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr process, IntPtr baseAddress, byte[] buffer, int size, ref int bytesWritten);

        public static IntPtr foundAddress = IntPtr.Zero;
        public static Process gameProcess;

        public static long ReadInt64(IntPtr process, IntPtr baseAddress)
        {
            var buffer = new byte[8];
            IntPtr byteRead;
            ReadProcessMemory(process, baseAddress, buffer, 8, out byteRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static void CheckProcess()
        {
            gameProcess = Process.GetProcessesByName("Spotify").FirstOrDefault();
            
            if(gameProcess != null) {
                var gameModule = gameProcess.MainModule;
                var baseAddress = gameModule.BaseAddress.ToInt64() + 0x0152ED0C;
                var offsets = new [] { 0x2E8 };

                var realAddress = Program.GetRealAddress(gameProcess.Handle, (IntPtr)baseAddress, offsets);
                Program.foundAddress = (IntPtr)realAddress;

                var array = BitConverter.GetBytes(100);
                int bytesWritten;
                //WriteProcessMemory(gameProcess.Handle, Program.foundAddress, array, (uint)array.Length, out bytesWritten);

                string Address = realAddress.ToString("X");

                Console.WriteLine(gameProcess.ToString());
                Console.WriteLine(gameModule.ToString());
                Console.WriteLine(baseAddress.ToString());
                Console.WriteLine(foundAddress.ToString());
                Console.WriteLine(Address);
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
            Program.CheckProcess();
        }
    }
}
