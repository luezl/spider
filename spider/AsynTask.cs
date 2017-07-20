using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Forms;

namespace spider
{

	public class AsynTask 
	{

		private Action _completed;

		private Action<object> _threadStart;

		private static AsynTask _asynTask;
		public static AsynTask Factory {
			get {
				if (_asynTask == null) _asynTask = new AsynTask();				
				return _asynTask;
			}
		}

		private SynchronizationContext _context;
		
		public AsynTask()
		{
			_context = SynchronizationContext.Current;
		}

		public AsynTask Task(Action<AsynTask> doWork)
		{
			return Task(doWork,null,null);
		}

		public AsynTask Task(Action<AsynTask> doWork, Action successFunc)
		{
			return Task(doWork, successFunc, null);
		}

		public AsynTask Task(Action<AsynTask> doWork, Action successFunc, Action<Exception> failureFunc)
		{
			_threadStart= o =>
			{
				try
				{
					doWork(this);

					if (successFunc != null) Post(successFunc);
				}
				catch (Exception e)
				{
					if (failureFunc != null) Post(() => { failureFunc(e); });
				}
				finally
				{
					Final();
				}
			};			
			return this;
		}
		public AsynTask RunTask(Action<AsynTask> doWork)
		{
			return Task(doWork).Start(); 
		}

		public AsynTask RunTask(Action<AsynTask> doWork, Action successFunc)
		{
			return Task(doWork, successFunc).Start();
		}

		public AsynTask RunTask(Action<AsynTask> doWork, Action successFunc, Action<Exception> failureFunc)
		{
			return Task(doWork, successFunc, failureFunc).Start();
		}

		public AsynTask Start()
		{
			if (_threadStart == null) return this;
			ThreadPool.QueueUserWorkItem(new WaitCallback(_threadStart));

			return this;
		}

		private void Final()
		{
			if (_completed != null)
				Post(_completed);				
		}

		public AsynTask Completed(Action complete)
		{
			_completed = complete;
			return this;
		}

		public void Post(Action action)
		{
			Post(o => { action(); });
		}

		public void Post(Action<object> action, object param = null)
		{ 
			if (_context !=null)
				_context.Post(o => { action(o); }, param);
		}

	}
}
