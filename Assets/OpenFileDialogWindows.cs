using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class OpenFileDialogWindows : MonoBehaviour
{
    public Text TextToSet;
    
    
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OPENFILENAME_I ofn);

    // Start is called before the first frame update
    public void OpenFile()
    {
        int FILEBUFSIZE = 1255;

        var FILEBUF = new char[FILEBUFSIZE];

        var address = GCHandle.Alloc(FILEBUF, GCHandleType.Pinned);
        //WndProc hookProcPtr = new WndProc(this.HookProc);
        OPENFILENAME_I ofn = new OPENFILENAME_I();
        try
        {
            //charBuffer = CharBuffer.CreateBuffer(FILEBUFSIZE);
            //if (fileNames != null)
            //{
            //    charBuffer.PutString(fileNames[0]);
            //}
            ofn.lStructSize = Marshal.SizeOf(typeof(OPENFILENAME_I));
            // Degrade to the older style dialog if we're not on Win2K.
            // We do this by setting the struct size to a different value
            //
            if (Environment.OSVersion.Platform != System.PlatformID.Win32NT || Environment.OSVersion.Version.Major < 5)
            {
                ofn.lStructSize = 0x4C;
            }

            //ofn.hwndOwner = hWndOwner;
            //ofn.hInstance = Instance;
            //ofn.lpstrFilter = MakeFilterString(filter, this.DereferenceLinks);
            //ofn.nFilterIndex = filterIndex;
            ofn.lpstrFile = address.AddrOfPinnedObject();
            ofn.nMaxFile = FILEBUFSIZE;
            //ofn.lpstrInitialDir = initialDir;
            //ofn.lpstrTitle = title;
            ofn.Flags = 0x00000008;
            //ofn.Flags = Options | (NativeMethods.OFN_EXPLORER | NativeMethods.OFN_ENABLEHOOK | NativeMethods.OFN_ENABLESIZING);
            //ofn.lpfnHook = hookProcPtr;
            //ofn.FlagsEx =  NativeMethods.OFN_USESHELLITEM;
            //if (defaultExt != null && AddExtension)
            //{
            //    ofn.lpstrDefExt = defaultExt;
            //}

            //Security checks happen here
            bool result = GetOpenFileName(ofn);

            var zero = FILEBUF.TakeWhile(o => o != 0)
                .Count();
            var file = new string(FILEBUF, 0, zero);
            TextToSet.text = file;
            Debug.Log($"lpstrFile '{file.Length}' '{file}'");
        }
        finally
        {
            ////charBuffer = null;
            //if (ofn.lpstrFile != IntPtr.Zero)
            //{
            //    Marshal.FreeCoTaskMem(ofn.lpstrFile);
            //}
            address.Free();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OPENFILENAME_I
    {
        public int lStructSize = Marshal.SizeOf(typeof(OPENFILENAME_I)); //ndirect.DllLib.sizeOf(this);
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter; // use embedded nulls to separate filters
        public IntPtr lpstrCustomFilter = IntPtr.Zero;
        public int nMaxCustFilter = 0;
        public int nFilterIndex;
        public IntPtr lpstrFile;
        public int nMaxFile = 260;
        public IntPtr lpstrFileTitle = IntPtr.Zero;
        public int nMaxFileTitle = 260;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset = 0;
        public short nFileExtension = 0;
        public string lpstrDefExt;
        public IntPtr lCustData = IntPtr.Zero;
        public WndProc lpfnHook;
        public string lpTemplateName = null;
        public IntPtr pvReserved = IntPtr.Zero;
        public int dwReserved = 0;
        public int FlagsEx;
    }

    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}