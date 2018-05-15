using System;
using System.IO;
using System.Security.Cryptography;

namespace Lidgren.Network
{
    public abstract class NetCryptoProviderBase : NetEncryption, IDisposable
    {
        private bool _disposed = false;

        protected SymmetricAlgorithm m_algorithm;

        [ThreadStatic]
        private ICryptoTransform __encryptor;
        protected ICryptoTransform Encryptor
        {
            get
            {
                ICryptoTransform GetNew() => m_algorithm.CreateEncryptor();

                if (__encryptor == null)
                {
                    __encryptor = GetNew();
                }
                else if (CanTransformBeReused(__encryptor) == false)
                {
                    __encryptor.Dispose();
                    __encryptor = GetNew();
                }

                return __encryptor;
            }
        }

        [ThreadStatic]
        private ICryptoTransform __decryptor;
        protected ICryptoTransform Decryptor
        {
            get
            {
                ICryptoTransform GetNew() => m_algorithm.CreateDecryptor();

                if (__decryptor == null)
                {
                    __decryptor = GetNew();
                }
                else if (CanTransformBeReused(__decryptor) == false)
                {
                    __decryptor.Dispose();
                    __decryptor = GetNew();
                }

                return __decryptor;
            }
        }

        public NetCryptoProviderBase(NetPeer peer, SymmetricAlgorithm algo) : base(peer)
        {
            m_algorithm = algo;
            m_algorithm.GenerateKey();
            m_algorithm.GenerateIV();
        }

        private bool CanTransformBeReused(ICryptoTransform transform)
        {
            return transform.CanReuseTransform && transform.CanTransformMultipleBlocks;
        }

        public override void SetKey(byte[] data, int offset, int count)
        {
            int len = m_algorithm.Key.Length;
            var key = new byte[len];
            for (int i = 0; i < len; i++)
                key[i] = data[offset + (i % count)];
            m_algorithm.Key = key;

            len = m_algorithm.IV.Length;
            key = new byte[len];
            for (int i = 0; i < len; i++)
                key[len - 1 - i] = data[offset + (i % count)];
            m_algorithm.IV = key;
        }

        public override bool Encrypt(NetOutgoingMessage msg)
        {
            int sourceBits = msg.LengthBits;

            int length;
            byte[] data;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, Encryptor, CryptoStreamMode.Write, true))
                    cs.Write(msg.m_data, 0, msg.LengthBytes);

                length = (int)ms.Length;
                data = ms.GetBuffer();
            }

            int neededBufferSize = (length + 4) * 8;
            msg.EnsureBufferSize(neededBufferSize);
            msg.LengthBits = 0; // reset write pointer
            msg.Write((uint)sourceBits);
            msg.Write(data, 0, length);
            msg.LengthBits = neededBufferSize;

            return true;
        }

        public override bool Decrypt(NetIncomingMessage msg)
        {
            int decryptedLenBits = (int)msg.ReadUInt32();
            int storageSize = NetUtility.BytesToHoldBits(decryptedLenBits);
            byte[] result = m_peer.GetStorage(storageSize);

            using (var ms = new MemoryStream(msg.m_data, 4, msg.LengthBytes - 4))
            using (var cs = new CryptoStream(ms, Decryptor, CryptoStreamMode.Read))
                cs.Read(result, 0, storageSize);

            if (m_peer.m_configuration.m_useMessageRecycling)
                m_peer.Recycle(msg.m_data);

            msg.m_data = result;
            msg.m_bitLength = decryptedLenBits;
            msg.m_readPosition = 0;

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {

                }

                __encryptor.Dispose();
                __decryptor.Dispose();
                m_algorithm.Dispose();

                _disposed = true;
            }
        }

        ~NetCryptoProviderBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
