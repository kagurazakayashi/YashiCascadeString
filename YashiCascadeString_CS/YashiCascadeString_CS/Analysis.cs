﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace YashiCascadeString_CS
{
    class Analysis
    {
        /*
        YashiCascadeString_CS --analysis YCSDF10000[]:@[2::rootArray][2-1]1[3-1:A]AA[3:B]BB[3:C]CC[4::arr2][4]1[4]2[4]3[4]4[3:D]DD[2]2[3::][3]1[3]2[3]3[3]4[3::][3]1[3]2[3]3[3]4@

YCSDF10000[]:@
-[2]rootArray(Array):
--[2]1
---[3]A:AA
---[3]B:BB
---[3]C:CC
----[4]arr2(Array):
----[4]1
----[4]2
----[4]3
----[4]4
---[3]D:DD
--[2]2
--[3](Array):
---[3]1
---[3]2
---[3]3
---[3]4
---[3](Array):
---[3]1
---[3]2
---[3]3
---[3]4@
        */

        private Dictionary<string, Object> returnvalue;
        public string instring;
        public string indentstart = "[";
        public string indentend = "]";
        public string indentdic = ":";

        private ArrayList nowObjs = new ArrayList();


        public Dictionary<string, Object> start()
        {
            //Console.WriteLine("String=" + instring);
            //str.Substring(start-1, length)
            int bchk = check();
            if (bchk < 0)
            {
                Console.WriteLine("YCSDF ERROR " + bchk + ".");
            }
            else
            {
                convert(instring.Substring(14, instring.Length - 15));
            }
            return returnvalue;
        }

        private void convert(string datastr)
        {
            Dictionary<string, Object> rootdic = new Dictionary<string, Object>();
            Console.WriteLine("convert=");
            string[] units = datastr.Split(indentstart.ToCharArray()[0]);
            int oldindent = 0;
            for (int i = 1; i < units.Length; i++)
            {
                string nowunit = units[i]; //当前数据条目
                int nowtype = 0; //0数组 1字典 2字符串
                int nowindent = 0; //缩进
                int cindent = 0; //缩进差异

                string nowkey = ""; //key
                string nowValue = ""; //value
                string[] colon = nowunit.Split(indentdic.ToCharArray()[0]);

                
                if (colon.Length == 2) //字典
                {
                    nowtype = 1;
                    nowindent = int.Parse(colon[0]); //缩进
                    string keyvaluestr = colon[1]; //.Split(indentend.ToCharArray()[0])
                    string[] keyvalue = keyvaluestr.Split(indentend.ToCharArray()[0]);
                    nowkey = keyvalue[0];
                    nowValue = keyvalue[1];
                    //Console.WriteLine("字典 " + nowkey + " " + nowValue);
                }
                else if(colon.Length == 3) //数组声明
                {
                    nowtype = 0;
                    nowindent = int.Parse(colon[0]); //缩进
                    string v = colon[2];
                    nowkey = v.Substring(0, v.Length - 1);
                    //Console.WriteLine("数组 " + nowkey);
                }
                else if (colon.Length == 1) //字符串
                {
                    nowtype = 2;
                    string[] indentvalue = nowunit.Split(indentend.ToCharArray()[0]);
                    nowindent = int.Parse(indentvalue[0]);
                    nowValue = indentvalue[1];
                    //Console.WriteLine("字符串 " + nowValue);
                }
                else
                {
                    Console.WriteLine("YCSDF ERROR -7 (" + colon.Length + ")."); //数据条目拆分失败
                }

                cindent = nowindent - oldindent;
                oldindent = nowindent;
                Console.WriteLine("当前数据=" + nowunit + ", 当前缩进=" + nowindent + ", 缩进差异=" + cindent + ", 数据类型=" + nowtype + ", 键=" + nowkey + ", 值=" + nowValue);
                convertthis(nowunit, nowindent, cindent, nowtype, nowkey, nowValue);

                /*

            012
            123

                */

            }
        }
        
        private void convertthis(string nowunit, int nowindent, int cindent, int nowtype, string nowkey, string nowValue)
        {
            int nowindentc = nowindent - nowObjs.Count; //计算要填充未知的量
            if (nowindentc > 0) //如果请求位数数组长度不够
            {
                //Console.WriteLine("填充未知级别 (" + nowindentc + ").");
                for (int nowindentci = 0; nowindentci < nowindentc; nowindentci++) //填充未知的量
                {
                    nowObjs.Add(new object());
                }
            }
            if (cindent < 0)
            {
                int nowindentAbs = Math.Abs(nowindent); //层级变化量绝对值:要向上级提交的次数
                for (int nowindentAbsi = 0; nowindentAbsi < nowindentAbs; nowindentAbsi++) //向上级提交
                {
                    //获得上级
                    int nowObjsn = nowindent - 1 - nowindentAbsi;
                    object nowObj = nowObjs[nowObjsn];
                    Type nowType = nowObj.GetType();
                    if (nowType == typeof(ArrayList))
                    {
                        ArrayList nowObjArr = (ArrayList)nowObj;
                    }
                    else if (nowType == typeof(Dictionary<string, Object>))
                    {
                        Dictionary<string, Object> nowObjDic = (Dictionary<string, Object>)nowObj;
                    }
                }
            }
            else if(cindent > 0)
            {

            }
            else
            {

            }
            /*
            if (nowindent != 0) //如果层级有变化
            {
                int abs = Math.Abs(nowindent); //层级变化量绝对值
                abs--; //层级变化量绝对值-1
                if (nowindent < 0)
                {
                    nowindent = 0 - abs;
                    //return回上一级
                }
                else
                {
                    nowindent = abs;
                    //new出下一级并添加数据
                }
            }
            else
            {
                //当前级添加数据
            }
            */
        }

        public int check()
        {
            try
            {
                if (instring.Length <= 17)
                {
                    return -2; //长度不正确
                }
                else if (!instring.Substring(0, 5).Equals("YCSDF"))
                {
                    return -3; //格式描述不匹配
                }
                else if (int.Parse(instring.Substring(5, 5)) != 10000)
                {
                    return -4; //版本号不匹配
                }
                else if (!instring.Substring(13, 1).Equals("@"))
                {
                    return -5; //找不到起始符
                }
                else if (!instring.Substring(instring.Length - 1, 1).Equals("@"))
                {
                    return -6; //找不到结束符
                }
                string ss = instring.Substring(10, 3);
                indentstart = ss.Substring(0, 1);
                indentend = ss.Substring(1, 1);
                indentdic = ss.Substring(2, 1);
            }
            catch
            {
                return -1; //意外错误
            }
            return 0;
        }

    }
}
