using NPSerialization;
using UnityEngine;
using static NPBehave.Action;

namespace NPVisualEditor_Example
{
    [SerializationID(9999)]
    public partial class FunctionsCase
    {
        public FunctionsCase(long _ID) 
        {
            m_ID = _ID;
        }

        public static void PublicStaticVoidFunc()
        {
            Debug.Log("PublicStaticVoidFunc");
        }

        protected static void ProtectedStaticVoidFunc()
        {
            Debug.Log("ProtectedStaticVoidFunc");
        }

        private static void PrivateStaticVoidFunc()
        {
            Debug.Log("PrivateStaticVoidFunc");
        }

        public static bool PublicStaticBoolFunc()
        {
            Debug.Log("PublicStaticBoolFunc");
            return true;
        }

        protected static bool ProtectedStaticBoolFunc()
        {
            Debug.Log("ProtectedStaticBoolFunc");
            return true;
        }

        private static bool PrivateStaticBoolFunc()
        {
            Debug.Log("PrivateStaticBoolFunc");
            return true;
        }

        public static Result PublicStaticBoolResultFunc(bool _b)
        {
            Debug.Log("PublicStaticBoolResultFunc");
            return Result.SUCCESS;
        }

        protected static Result ProtectedStaticBoolResultFunc(bool _b)
        {
            Debug.Log("ProtectedStaticBoolResultFunc");
            return Result.SUCCESS;
        }

        private static Result PrivateStaticBoolResultFunc(bool _b)
        {
            Debug.Log("PrivateStaticBoolResultFunc");
            return Result.SUCCESS;
        }

        public static Result PublicStaticRequestResultFunc(Request _r)
        {
            Debug.Log("PublicStaticRequestResultFunc");
            return Result.SUCCESS;
        }

        protected static Result ProtectedStaticRequestResultFunc(Request _r)
        {
            Debug.Log("ProtectedStaticRequestResultFunc");
            return Result.SUCCESS;
        }

        private static Result PrivateStaticRequestResultFunc(Request _r)
        {
            Debug.Log("PrivateStaticRequestResultFunc");
            return Result.SUCCESS;
        }

        public void PublicVoidFunc()
        {
            Debug.Log("PublicVoidFunc");
        }

        protected void ProtectedVoidFunc()
        {
            Debug.Log("ProtectedVoidFunc");
        }

        private void PrivateVoidFunc()
        {
            Debug.Log("PrivateVoidFunc");
        }

        public bool PublicBoolFunc()
        {
            Debug.Log("PublicBoolFunc");
            return true;
        }

        protected bool ProtectedBoolFunc()
        {
            Debug.Log("ProtectedBoolFunc");
            return true;
        }

        private bool PrivateBoolFunc()
        {
            Debug.Log("PrivateBoolFunc");
            return true;
        }

        public Result PublicBoolResultFunc(bool _b)
        {
            Debug.Log("PublicBoolResultFunc");
            return Result.SUCCESS;
        }

        protected Result ProtectedBoolResultFunc(bool _b)
        {
            Debug.Log("ProtectedBoolResultFunc");
            return Result.SUCCESS;
        }

        private Result PrivateBoolResultFunc(bool _b)
        {
            Debug.Log("PrivateBoolResultFunc");
            return Result.SUCCESS;
        }

        public Result PublicRequestResultFunc(Request _r)
        {
            Debug.Log("PublicRequestResultFunc");
            return Result.SUCCESS;
        }

        protected Result ProtectedRequestResultFunc(Request _r)
        {
            Debug.Log("ProtectedRequestResultFunc");
            return Result.SUCCESS;
        }

        private Result PrivateRequestResultFunc(Request _r)
        {
            Debug.Log("PrivateRequestResultFunc");
            return Result.SUCCESS;
        }

        public long ID 
        { 
            get => m_ID;  
        }

        private long m_ID;
    }
}