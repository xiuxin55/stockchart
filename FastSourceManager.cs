using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class FastSourceManager
    {
        private static FastSourceManager _instance;
        public static FastSourceManager Instance
        {
            get
            {
                if(_instance ==null)
                {
                    _instance = new FastSourceManager();
                }
                return _instance;
            }
        }

        Dictionary<string, List<IFastSource>> FastSourceDic = new Dictionary<string, List<IFastSource>>();
        public void RegisterSource(string name, IFastSource source)
        {
            List<IFastSource> temp;
            if(FastSourceDic.TryGetValue(name,out temp))
            {
                FastSourceDic[name].Add(source);
            }
            else
            {
                FastSourceDic.Add(name,new List<IFastSource>());
                FastSourceDic[name].Add(source);
            }
        }

        public void DragZoomIn(int start, int end)
        {
            List<IFastSource> temp;
            if (FastSourceDic.TryGetValue("", out temp))
            {
                foreach (var item in temp)
                {
                    item.DragZoomIn(start,end);
                }
            }
        }

        public void MouseWheelZoomIn()
        {
            List<IFastSource> temp;
            if (FastSourceDic.TryGetValue("", out temp))
            {
                foreach (var item in temp)
                {
                    item.MouseWheelZoomIn();
                }
            }
        }

        public void MouseWheelZoomOut()
        {
            List<IFastSource> temp;
            if (FastSourceDic.TryGetValue("", out temp))
            {
                foreach (var item in temp)
                {
                    item.MouseWheelZoomOut();
                }
            }
        }
    }
}
