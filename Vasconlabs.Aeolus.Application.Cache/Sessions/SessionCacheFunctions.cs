using FASTER.core;

namespace Vasconlabs.Aeolus.Application.Cache.Sessions
{
    internal class SessionCacheFunctions : FunctionsBase<ulong, byte[], byte[], byte[], Empty>
    {
        public override bool SingleReader(ref ulong key, ref byte[] input, ref byte[] value, ref byte[] dst, ref ReadInfo readInfo)
        {
            dst = value;

            return base.SingleReader(ref key, ref input, ref value, ref dst, ref readInfo);
        }

        public override bool ConcurrentReader(ref ulong key, ref byte[] input, ref byte[] value, ref byte[] dst, ref ReadInfo readInfo)
        {
            dst = value;

            return base.ConcurrentReader(ref key, ref input, ref value, ref dst, ref readInfo);
        }
    }
}
