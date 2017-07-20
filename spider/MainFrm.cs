using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Ivony.Html;
using Ivony.Html.Parser;
using System.Threading;

namespace spider
{
    public partial class MainFrm : Form
    {

        public MainFrm()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = new DbMapper();

            this.progressBar1.Value = 0;

            string config = File.ReadAllText(@".\config.json");
            var json = JsonConvert.DeserializeObject<List<Config>>(config);
            var len = json.Count();
            log("开始获取数据！");
            new AsynTask().RunTask(t =>
            {
                for (Int32 i = 0; i < len; i++)
                {
                    var it = json[i];
                    var d = new edata();
                    d.name = it.name;
                    d.url = it.url;

                    t.Post(() =>
                    {
                        log(d.name);
                        log(d.url);
                    });

                    var dom = new JumonyParser().LoadDocument(it.url);
                    if (!string.IsNullOrWhiteSpace(it.selector1))
                    {
                        d.value1 = dom.FindSingle(it.selector1).InnerText();

                    }
                    if (!string.IsNullOrWhiteSpace(it.selector2))
                    {
                        d.value2 = dom.FindSingle(it.selector2).InnerText();

                    }
                    if (!string.IsNullOrWhiteSpace(it.selector3))
                    {
                        d.value3 = dom.FindSingle(it.selector3).InnerText();

                    }
                    if (!string.IsNullOrWhiteSpace(it.selector4))
                    {
                        d.value4 = dom.FindSingle(it.selector4).InnerText();

                    }
                    if (!string.IsNullOrWhiteSpace(it.selector5))
                    {
                        d.value5 = dom.FindSingle(it.selector5).InnerText();

                    }

                    //DB
                    db.insert(d);

                    t.Post(() =>
                    {
                        this.progressBar1.Value = i / len * 100;
                    });
                }

            }, () =>
            {
                log("处理完毕！");
            }, ex => {
                log("处理失败了！" + ex.Message);
            });

           
        }

        public void updateUI(Action<object> action, object param=null)
        {
            var context = SynchronizationContext.Current;
            if (context != null) 
            {
                context.Post(o => { action(o); }, param);
            }       
        }

        public void log(string msg)
        {
            int i = this.MsgBox.Items.Add(msg);
            this.MsgBox.SelectedIndex = i;
        
        }
    }
}
