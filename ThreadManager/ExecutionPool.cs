using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutionPool
{
    public class ExecutionPool : IExecutionPool
    {
        List<StrategyThread> strategyWorkersList = new List<StrategyThread>();

        private void QueueStrategy(IRunner runner, string strategyPath, CancellationToken cancellationToken)
        {
            var ts = TaskScheduler.Default;
            var tco = TaskCreationOptions.LongRunning;
            Task.Factory.StartNew(() => runner.Run(strategyPath), cancellationToken, tco, ts);
        }

        public List<StrategyThread> GetAllStrategyThreads()
        {
            return strategyWorkersList;
        }

        public int StartStrategy(string strategyName, string strategyPath, int type)
        {
            int ret = 0;

            IRunner runner = null;

            if (type == 1)
            {
                runner = new StrategyRunner("Escal");
            }
            else
            {
                runner = new StrategyRunnerEx("Escal");
            }
            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            runner.Name = strategyName;
            runner.CancellationToken = cancellationTokenSource.Token;

            try
            {
                
                strategyWorkersList.Add(new StrategyThread()
                {
                    Name = strategyName,
                    Runner = runner,
                    CancellationTokenSource = cancellationTokenSource
                });
                QueueStrategy(runner, strategyPath, cancellationTokenSource.Token);
                ret = 1;
            }
            catch (AggregateException ae)
            {
                ae.Flatten();
                throw new Exception("One thread exception", ae);
            }
            catch (Exception ex)
            {
                throw new Exception("One thread exception", ex);
            }

            return ret;
        }

        public int StopStrategy(string strategyName)
        {
            int ret = 0;

            try
            {
                StrategyThread strategyThread = strategyWorkersList.FirstOrDefault(s => s.Name.Equals(strategyName));

                if (strategyThread == null)
                {
                    return -1;
                }
                strategyThread.Runner.Stop = 1;
                //((CancellationTokenSource)strategyThread.CancellationTokenSource).Cancel();
                //((CancellationTokenSource)strategyThread.CancellationTokenSource).Dispose();
                strategyWorkersList.Remove(strategyThread);
                ret = 1;
            }
            catch (Exception)
            {

                throw;
            }

            return ret;
        }

        public int StopAllStrategy()
        {
            int ret = 0;

            try
            {
                foreach (var strategyThread in strategyWorkersList.ToList())
                {
                    strategyThread.Runner.Stop = 1;
                    //((CancellationTokenSource)strategyThread.CancellationTokenSource).Cancel();
                    //((CancellationTokenSource)strategyThread.CancellationTokenSource).Dispose();
                    strategyWorkersList.Remove(strategyThread);
                }
                ret = 1;
            }
            catch (Exception)
            {
                
                throw;
            }

            return ret;
        }

        public int StartStrategyGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public int StopStrategyGroup(string groupName)
        {
            throw new NotImplementedException();
        }
    }

    public class StrategyRunner : IRunner
    {
        public StrategyRunner(string executionType)
        {
            _executionType = executionType;
        }
        private string _executionType;

        public void Run(string strategyDirectory)
        {
            StreamWriter file = new StreamWriter(Path.Combine(strategyDirectory, _name + ".txt"), true);

            try
            {
                while (_stop.Equals(0))
                {
                    file.WriteLine(DateTime.Now.ToString("YYYY MMMM DD - HH:mm:ss"));
                    file.Flush();

                    Thread.Sleep(2000); //DO SHIT

                    if (((CancellationToken)_ct).IsCancellationRequested)
                    {
                        file.WriteLine("Cancellation requested.");
                        file.Flush();
                        break;
                    }

                    //Thread.Sleep(1000); //DO SHIT
                }
                if (_stop.Equals(1))
                {
                    file.WriteLine("Stop requested");
                    file.Flush();
                }
            }
            catch (AggregateException ae)
            {
                file.WriteLine("AGGREGATE EXCEPTION");
                file.Flush();
            }
            catch (Exception ex)
            {
                file.WriteLine("EXCEPTION");
                file.Flush();
            }
           
        }

        private object _ct;

        public object CancellationToken
        {
            get { return _ct; }
            set { _ct = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _stop;
        public int Stop
        {
            get { return _stop; }
            set { _stop = value; }
        }

    }

    public class StrategyRunnerEx : IRunner
    {
        public StrategyRunnerEx(string executionType)
        {
            _executionType = executionType;
        }
        private string _executionType;

        public void Run(string strategyDirectory)
        {
            StreamWriter file = new StreamWriter(Path.Combine(strategyDirectory, _name + ".txt"), true);

            try
            {
                int counter = 0;
                while (_stop.Equals(0))
                {
                    file.WriteLine(DateTime.Now.ToString("YYYY MMMM DD - HH:mm:ss"));
                    file.Flush();

                    Thread.Sleep(2000); //DO SHIT

                    if (((CancellationToken)_ct).IsCancellationRequested)
                    {
                        file.WriteLine("Cancellation requested.");
                        file.Flush();
                        break;
                    }
                    counter++;

                    if (counter > 10)
                    {
                        file.WriteLine("Exception forced");
                        
                        throw new Exception("Exception Forced", new Exception("INNER EXCEPTION"));
                    }

                    //Thread.Sleep(1000); //DO SHIT
                }
                if (_stop.Equals(1))
                {
                    file.WriteLine("Stop requested");
                    file.Flush();
                }
            }
            catch (AggregateException ae)
            {
                file.WriteLine("AGGREGATE EXCEPTION");
                file.Flush();
                throw new Exception("EXCEPTION FORCED", ae);
            }
            catch (Exception ex)
            {
                file.WriteLine("EXCEPTION");
                file.Flush();
                throw new Exception("EXCEPTION FORCED", ex);
            }

        }

        private object _ct;

        public object CancellationToken
        {
            get { return _ct; }
            set { _ct = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _stop;
        public int Stop
        {
            get { return _stop; }
            set { _stop = value; }
        }

    }

    public interface IRunner
    {
        void Run(string strategyDirectory);

        string Name { get; set; }
        object CancellationToken { get; set; }
        int Stop { get; set; }

    }

    public class StrategyThread
    {
        public string Name { get; set; }
        public IRunner Runner { get; set; }
        public object CancellationTokenSource { get; set; }
    }
}
