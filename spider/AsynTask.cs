using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Forms;
/*****************************************************************
 —————————————————————————————————————————————————————————————————
  使用方法：
  AsynTask.Task(()=>{
      异步处理。。。 
       AsynTask.UpdateUI(() =>
       {
            UI控件更新。。。
       });
  })
  .OnSuccess(() =>
  {
      成功时处理。。。
  })
  .OnError(ex =>
  {
      失败时处理。。。
  })
  .Start(); 
 —————————————————————————————————————————————————————————————————— 
 ******************************************************************/
namespace spider
{
    /// <summary>
    /// 异步处理共通类
    /// </summary>
	public class AsynTask 
	{
        //异步执行的处理Action
        private Action _doWork {get;set;}

        //处理成功的Action
        private Action _successFunc { get; set; }

        //处理失败的Action
        private Action<Exception> _failureFunc { get; set; }

        //处理结束的Acion
        private Action _completed { get; set; }

        //异步更新UI
        private static SynchronizationContext _context { get; set; }

        //异步池
        private static List<AsynTask> _asynPool { get; set; }

        //处理状态
        public bool IsBusy = false;

        private static int POOLCOUNT = 3;

        public static AsynTask Task(Action doWork)
        {
            var t = AsynPool();
            t._doWork = doWork;
            return t;
        }

        /// <summary>
        /// 异步处理构造方法
        /// </summary>
        /// <returns></returns>
        public AsynTask()
        {
            _context = SynchronizationContext.Current;
        }

        /// <summary>
        /// 异步处理构造方法
        /// </summary>
        /// <param name="doWork"></param>
        /// <returns></returns>
        public AsynTask(Action doWork)
		{
            _context = SynchronizationContext.Current;
            this._doWork = doWork;
		}

        /// <summary>
        /// 处理成功时
        /// </summary>
        /// <param name="successFunc"></param>
        /// <returns></returns>
        public AsynTask OnSuccess(Action successFunc)
        {
            this._successFunc = successFunc;
            return this;
        }

        /// <summary>
        /// 处理失败时
        /// </summary>
        /// <param name="failureFunc"></param>
        /// <returns></returns>
        public AsynTask OnError(Action<Exception> failureFunc)
        {
            this._failureFunc = failureFunc;
            return this;
        }
	
        /// <summary>
        /// 启动异步处理
        /// </summary>
		public void Start()
		{
            IsBusy = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(o => {
                try
                {
                    _doWork();

                    if (_successFunc != null) UpdateUI(_successFunc);
                }
                catch (Exception e)
                {
                    if (_failureFunc != null) UpdateUI(() => { _failureFunc(e); });
                }
                finally
                {                    
                    if (_completed != null) UpdateUI(_completed);
                    Clear();
                }
            }));            
		}

        /// <summary>
        /// 清除线程
        /// </summary>
        private void Clear()
        {
            IsBusy = false;
            this._doWork = null;
            this._failureFunc = null;
            this._successFunc = null;
            this._completed = null;
        }


        /// <summary>
        /// UI更新
        /// </summary>
        /// <param name="action"></param>
        public static void UpdateUI(Action action)
		{
			UpdateUI(o => { action(); });
		}

        /// <summary>
        /// UI更新
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
		public static void UpdateUI(Action<object> action, object param = null)
		{
            if (_context != null)
                _context.Post(o => { action(o); }, param);
		}

        /// <summary>
        /// 设置线程池内线程个数
        /// </summary>
        /// <param name="cnt"></param>
        public static void SetPoolCount(int cnt)
        {
            POOLCOUNT = cnt;
        }

        /// <summary>
        /// 异步线程池
        /// </summary>
        /// <returns></returns>
        private static AsynTask AsynPool()
        {
            if (_asynPool == null || _asynPool.Count() != POOLCOUNT)
            {
                _asynPool = new List<AsynTask>();
                for (int ii = 0; ii < POOLCOUNT; ii++)
                {
                    _asynPool.Add(new AsynTask());
                }
            }

            while(true)
            {
                foreach (AsynTask t in _asynPool)
                {
                    if (!t.IsBusy) return t;
                }
                Thread.Sleep(50);
            }
        }

	}
}
