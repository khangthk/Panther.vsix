﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Panther.Windows
{
    public class FolderBrowser
    {
        internal enum FDAP
        {
            FDAP_BOTTOM = 0x00000000,
            FDAP_TOP = 0x00000001
        }

        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }

        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }

        [Flags]
        internal enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040,
            FOS_ALLNONSTORAGEITEMS = 0x00000080,
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_OKBUTTONNEEDSINTERACTION = 0x00200000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000,
            FOS_FORCEPREVIEWPANEON = 0x40000000,
            FOS_SUPPORTSTREAMABLEITEMS = 0x80000000
        }

        internal enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 0x00000001,
            SIATTRIBFLAGS_OR = 0x00000002,
            SIATTRIBFLAGS_APPCOMPAT = 0x00000003,
            SIATTRIBFLAGS_ALLITEMS = 0x00004000
        }

        internal enum SIGDN : uint
        {
            SIGDN_NORMALDISPLAY = 0x00000000,
            SIGDN_PARENTRELATIVEPARSING = 0x80018001,
            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
            SIGDN_PARENTRELATIVEEDITING = 0x80031001,
            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
            SIGDN_FILESYSPATH = 0x80058000,
            SIGDN_URL = 0x80068000,
            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
            SIGDN_PARENTRELATIVE = 0x80080001,
            SIGDN_PARENTRELATIVEFORUI = 0x80094001
        }

        public string Folder { get; set; }

        public string Title { get; set; }

        public DialogResult ShowDialog(Window owner)
        {
            return ShowDialog(owner != null ? new WindowInteropHelper(owner).Handle : IntPtr.Zero);
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            return ShowDialog(owner?.Handle ?? IntPtr.Zero);
        }

        public DialogResult ShowDialog(IntPtr owner)
        {
            IntPtr hwndOwner = owner != IntPtr.Zero ? owner : NativeMethods.GetActiveWindow();
            NativeInterfaces.IFileOpenDialog dialog = (NativeInterfaces.IFileOpenDialog)new FileOpenDialog();

            try
            {
                if (!string.IsNullOrEmpty(Folder))
                {
                    Guid shellItemGuid = typeof(NativeInterfaces.IShellItem).GUID;
                    NativeInterfaces.IShellItem item = (NativeInterfaces.IShellItem)NativeMethods.SHCreateItemFromParsingName(Folder, null, ref shellItemGuid);

                    if (item != null)
                    {
                        dialog.SetFolder(item);
                        Marshal.ReleaseComObject(item);
                    }
                }

                dialog.SetOptions(FOS.FOS_PICKFOLDERS | FOS.FOS_FORCEFILESYSTEM);

                if (!string.IsNullOrEmpty(Title))
                {
                    dialog.SetTitle(Title);
                }

                int hr = dialog.Show(hwndOwner);

                if (hr == NativeMethods.ERROR_CANCELLED)
                {
                    return DialogResult.Cancel;
                }

                if (hr != 0)
                {
                    return DialogResult.Abort;
                }

                dialog.GetResult(out NativeInterfaces.IShellItem itemResult);
                itemResult.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);
                Folder = path;

                Marshal.ReleaseComObject(itemResult);

                return DialogResult.OK;
            }
            finally
            {
                Marshal.ReleaseComObject(dialog);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string pszSpec;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            public Guid fmtid;

            public uint pid;
        }

        internal static class NativeInterfaces
        {
            [ComImport, Guid("42f85136-db7e-439c-85f1-e4075d135fc8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IFileDialog : IModalWindow
            {
                [PreserveSig]
                new int Show([In] IntPtr parent);

                void SetFileTypes([In] uint cFileTypes, [In][MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

                void SetFileTypeIndex([In] uint iFileType);

                void GetFileTypeIndex(out uint piFileType);

                void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

                void Unadvise([In] uint dwCookie);

                void SetOptions([In] FOS fos);

                void GetOptions(out FOS pfos);

                void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

                void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

                void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

                void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

                void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

                void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

                void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

                void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

                void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

                void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

                void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, int alignment);

                void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

                void Close([MarshalAs(UnmanagedType.Error)] int hr);

                void SetClientGuid([In] ref Guid guid);

                void ClearClientData();

                void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
            }

            [ComImport, Guid("973510DB-7D7F-452B-8975-74A85828D354"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IFileDialogEvents
            {
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
                int OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
                int OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_SHAREVIOLATION_RESPONSE pResponse);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_OVERWRITE_RESPONSE pResponse);
            }

            [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IFileOpenDialog : IFileDialog
            {
                void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);

                void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

                void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
            }

            [ComImport, Guid("b4db1657-70d7-485e-8e3e-6fcb5a5c1802"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IModalWindow
            {
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
                uint Show([In] IntPtr parent);
            }

            [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IShellItem
            {
                void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

                void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

                void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

                void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

                void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
            }

            [ComImport, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IShellItemArray
            {
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void GetCount(out uint pdwNumItems);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
            }
        }

        [SuppressUnmanagedCodeSecurity]
        internal static class NativeMethods
        {
            public const int ERROR_CANCELLED = unchecked((int)0x800704C7);

            [DllImport("user32.dll")]
            public static extern IntPtr GetActiveWindow();

            [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
            [return: MarshalAs(UnmanagedType.Interface)]
            public static extern object SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, ref Guid riid);
        }

        [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        private class FileOpenDialog { }
    }
}