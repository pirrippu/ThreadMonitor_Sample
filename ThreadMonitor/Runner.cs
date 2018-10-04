using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadMonitor
{
    public class Runner : IRunner
    {
        private int _stop;
        private string _name;

        public void Run(object obj)
        {
            string path = "D:\\_GIT\\ThreadMonitor_Sample\\FILES\\" + (string)obj + ".txt";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Create(path);
            }

            //Every loops lasts approximately about 30 seconds
            while (_stop.Equals(0))
            {
                //Do something for 5 seconds
                for (int i = 0; i < 10; i++)
                {
                    System.IO.File.AppendAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:ss:ffff") + Environment.NewLine);
                    Thread.Sleep(500);

                    //Break for-loop if stop is requested
                    if (_stop.Equals(1))
                    {
                        System.IO.File.AppendAllText(path, "=====STOP COMMAND DETECTED INSIDE FOR-LOOP=====" + Environment.NewLine);
                        break;
                    }
                }
                
                Thread.Sleep(25000);
            }
            System.IO.File.AppendAllText(path, "Received stop command" + Environment.NewLine);
        }
        public int Parse(string fileName)
        {
            throw new NotImplementedException();
        }


        public int Stop
        {
            get { return _stop; }
            set { _stop = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    public class RunnerEx : IRunner
    {
        private int _stop;
        private string _name;

        public void Run(object obj)
        {
            string path = "D:\\_GIT\\ThreadMonitor_Sample\\FILES\\" + (string)obj + ".txt";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Create(path);
            }

            //Every loops lasts approximately about 30 seconds
            while (_stop.Equals(0))
            {
                //Do something for 5 seconds
                for (int i = 0; i < 10; i++)
                {
                    System.IO.File.AppendAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:ss:ffff") + Environment.NewLine);
                    Thread.Sleep(500);

                    //Break for-loop if stop is requested
                    if (_stop.Equals(1))
                    {
                        System.IO.File.AppendAllText(path, "=====STOP COMMAND DETECTED INSIDE FOR-LOOP=====" + Environment.NewLine);
                        break;
                    }
                }

                System.IO.File.AppendAllText(path, "=====FORCED EXCEPTION=====" + Environment.NewLine);
                throw new Exception("FORCE EXCEPTION");
            }
            System.IO.File.AppendAllText(path, "Received stop command" + Environment.NewLine);
        }
        public int Parse(string fileName)
        {
            throw new NotImplementedException();
        }


        public int Stop
        {
            get { return _stop; }
            set { _stop = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    internal interface IRunner
    {
        void Run(object obj);

        int Parse(string fileName);

        int Stop { get; set; }

        string Name { get; set; }
    }
}
