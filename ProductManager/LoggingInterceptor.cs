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
        private const string LogPath = "calllog.txt";
        private static StreamWriter _writer;
        private static int _dept = 0;
        static LoggingInterceptor()
        {
            _writer = new StreamWriter(File.Open(LogPath, FileMode.Append));
            _writer.AutoFlush = true;
            _writer.WriteLine(DateTime.Now.ToString());
        }
        public void Intercept(IInvocation invocation)
        {
            string address = invocation.Method.DeclaringType.FullName + "." + invocation.Method.Name;
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
