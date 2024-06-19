using System;
using System.Runtime.InteropServices;

namespace FaultTreeAnalysis
{
    /// <summary>
    /// FTA����ʱ�õ��ĵ�������DLL
    /// </summary>
    public static class Xftar
    {
        //
        // * XFTA version 1.1
        // * xftar
        // * Copyrights (c) 2012 Antoine Rauzy
        // 

        //
        // * Table of Contents
        // * -----------------
        // * 1) Include Files
        // * 2) Main
        // 

        //
        // * 1) Include Files
        // * ----------------
        // 


        //
        // * 2) Main
        // * -------
        // 

        /// <summary>
        /// �������ί����ʲô�ã�������ֱ�ӵ��ã�
        /// </summary>
        /// <param name="NamelessParameter"></param>
        /// <returns></returns>
        public delegate int XFTACommand(string NamelessParameter);

        //---------

        /// <summary>
        /// ִ��dll���ڲ�����
        /// </summary>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        internal static int Execute(int argc, string[] argv)
        {
            XFTACommand evalScriptFile = null;
            int error = 0;

            IntPtr hDLL = new System.IntPtr();
            if (hDLL == null)
            {
                return 1;
            }
            evalScriptFile = XFTA_EvalScriptFile;
            if (evalScriptFile == null)
            {
                return 1;
            }
            for (int i = 0; i < argc && error == 0; i++)
                error = evalScriptFile(argv[i]);
                
            return (error);
        }

        /// <summary>
        /// ������DLL�ṩ�ļ��㷽��
        /// </summary>
        /// <param name="NamelessParameter"></param>
        /// <returns></returns>
        [DllImport(FixedString.DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int XFTA_EvalScriptFile(string NamelessParameter);
    }
}