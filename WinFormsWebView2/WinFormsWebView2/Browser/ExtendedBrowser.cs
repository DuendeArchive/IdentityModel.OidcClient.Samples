// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace WinFormsWebView2
{ 
    internal class ExtendedWebBrowser : WebBrowser
    {
        private AxHost.ConnectionPointCookie _cookie;
        private ExtendedWebBrowserEventHelper _helper;

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();

            _helper = new ExtendedWebBrowserEventHelper(this);
            _cookie = new AxHost.ConnectionPointCookie(ActiveXInstance, _helper, typeof(DWebBrowserEvents2));
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            if (_cookie != null)
            {
                _cookie.Disconnect();
                _cookie = null;
            }
            base.DetachSink();
        }

        internal event EventHandler<BeforeNavigate2EventArgs> BeforeNavigate2;
        internal event EventHandler<NavigateErrorEventArgs> NavigateError;

        private void OnBeforeNavigate2(object pDisp, ref object url, ref object flags, ref object targetFrameName,
            ref object postData, ref object headers, ref bool cancel)
        {
            var handler = BeforeNavigate2;
            if (handler != null)
            {
                var args = new BeforeNavigate2EventArgs((string)url, (string)targetFrameName, (byte[])postData, (string)headers);
                handler(this, args);
                cancel = args.Cancel;
            }
        }

        private void OnNavigateError(object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel)
        {
            var handler = NavigateError;
            if (handler != null)
            {
                var args = new NavigateErrorEventArgs((string)url, (string)frame, (int)statusCode);
                handler(this, args);
                cancel = args.Cancel;
            }
        }

        internal class BeforeNavigate2EventArgs : EventArgs
        {
            private readonly string _url;
            private readonly string _targetFrameName;
            private readonly byte[] _postData;
            private readonly string _headers;

            public BeforeNavigate2EventArgs(string url, string targetFrameName, byte[] postData, string headers)
            {
                _url = url;
                _targetFrameName = targetFrameName;
                _postData = postData;
                _headers = headers;
                Cancel = false;
            }

            public string Url { get { return _url; } }
            public string TargetFrameName { get { return _targetFrameName; } }
            public byte[] PostData { get { return _postData; } }
            public string Headers { get { return _headers; } }
            public bool Cancel { get; set; }
        }

        internal class NavigateErrorEventArgs : EventArgs
        {
            private readonly string _url;
            private readonly string _frame;
            private int _statusCode;

            public NavigateErrorEventArgs(string url, string frame, int statusCode)
            {
                _url = url;
                _frame = frame;
                _statusCode = statusCode;
                Cancel = false;
            }

            public string Url { get { return _url; } }
            public string Frame { get { return _frame; } }
            public int StatusCode { get { return _statusCode; } }
            public bool Cancel { get; set; }
        }

        [ComImport]
        [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        [TypeLibType(TypeLibTypeFlags.FHidden)]
        private interface DWebBrowserEvents2
        {
            [PreserveSig]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [DispId(250)]
            void BeforeNavigate2([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object URL,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object Flags,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object TargetFrameName,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object PostData,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object Headers,
                             [In] [Out] ref bool Cancel);

            [PreserveSig]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [DispId(271)]
            void NavigateError([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                            [In] [MarshalAs(UnmanagedType.Struct)] ref object URL,
                            [In] [MarshalAs(UnmanagedType.Struct)] ref object Frame,
                            [In] [MarshalAs(UnmanagedType.Struct)] ref object StatusCode,
                            [In] [Out] ref bool Cancel);
        }


        private class ExtendedWebBrowserEventHelper : StandardOleMarshalObject, DWebBrowserEvents2
        {
            readonly ExtendedWebBrowser parent;

            public ExtendedWebBrowserEventHelper(ExtendedWebBrowser parent)
            {
                this.parent = parent;
            }

            public void BeforeNavigate2(object pDisp, ref object URL, ref object Flags,
                ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
            {
                parent.OnBeforeNavigate2(pDisp, ref URL, ref Flags, ref TargetFrameName,
                    ref PostData, ref Headers, ref Cancel);
            }

            public void NavigateError(object pDisp, ref object URL, ref object Frame,
                ref object StatusCode, ref bool Cancel)
            {
                parent.OnNavigateError(pDisp, ref URL, ref Frame, ref StatusCode, ref Cancel);
            }
        }
    }
}