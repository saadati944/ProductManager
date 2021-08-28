using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Framework
{
    public class LoggingInterceptor : IInterceptor
    {
        private const string LogPath = "Logs.txt";
        private static StreamWriter _writer;
        private static int _dept = 0;
        static LoggingInterceptor()
        {
            //if (!Directory.Exists(LogPath))
            //    Directory.CreateDirectory(LogPath);
            //int i = 0;
            //string fullpath;
            //do {
            //    fullpath = Path.Combine(LogPath , string.Format("LogFile_{0:D5}.txt", i++));
            //} while (File.Exists(fullpath));
            try
            {
                _writer = new StreamWriter(File.Open(LogPath, FileMode.Append));
                _writer.AutoFlush = true;
                _writer.WriteLine(DateTime.Now.ToString());
            }
            catch { _writer = null; }
        }
        public void Intercept(IInvocation invocation)
        {
            string address = invocation.Method.DeclaringType.FullName + "." + invocation.Method.Name;
            if (invocation.Method.IsGenericMethod)
                address += "<" + string.Join(", ", invocation.Method.GetGenericArguments().Select(x=>x.Name)) + ">";
            int starttime = Environment.TickCount;
            try
            {
                Log("Calling_" + address);
                _dept++;

                invocation.Proceed();

                _dept--;
                Log("Succeed_" + address, starttime);
            }
            catch (Exception ex)
            {
                _dept--;
                Console.WriteLine("Error_"+address, starttime);
                throw;
            }
            finally
            {
                // Console.WriteLine("Exit_"+address, starttime);
            }
        }
        private void Log(string mes, int starttime=0)
        {
            if (_writer == null)
                return;
            StringBuilder sb = new StringBuilder();
            sb.Append(Duration(starttime) + ":");
            for (int i = 0; i < _dept; i++)
                sb.Append("|");
            sb.Append(mes);
            _writer.WriteLine(sb.ToString());
        }
        private string Duration(int startTime=0)
        {
            return string.Format("{0:D7}", startTime == 0 ? 0 : Environment.TickCount - startTime);
        }
    }
}
