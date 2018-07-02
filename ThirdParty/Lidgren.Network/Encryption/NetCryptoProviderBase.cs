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
            byte[] key = new byte[len];
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
            try
            {
                int sourceBits = msg.LengthBits;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, Encryptor, CryptoStreamMode.Write, true))
                        cs.Write(msg.m_data, 0, msg.LengthBytes);

                    int length = (int)ms.Length;
                    int neededBufferSize = (length + 4) * 8;

                    msg.EnsureBufferSize(neededBufferSize);
                    msg.LengthBits = 0; // reset write pointer
                    msg.Write((uint)sourceBits);
                    msg.Write(ms.GetBuffer(), 0, length);
                    msg.LengthBits = neededBufferSize;

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public override bool Decrypt(NetIncomingMessage msg)
        {
            void Recycle(byte[] buffer)
            {
                if (m_peer.m_configuration.m_useMessageRecycling)
                    m_peer.Recycle(buffer);
            }

            bool success = true;
            int decryptedBits = 0;
            byte[] result = null;
            byte[] originalMessageBuffer = msg.m_data;

            try
            {
                decryptedBits = (int)msg.ReadUInt32();
                int decryptedLength = NetUtility.BytesNeededToHoldBits(decryptedBits);
                result = m_peer.GetStorage(decryptedLength);

                using (var ms = new MemoryStream(originalMessageBuffer, 4, msg.LengthBytes - 4))
                using (var cs = new CryptoStream(ms, Decryptor, CryptoStreamMode.Read))
                {
                    if (cs.Read(result, 0, decryptedLength) != decryptedLength)
                        success = false;
                }
            }
            catch
            {
                success = false;
            }

            Recycle(originalMessageBuffer);
            if (success == false)
            {
                if (result != null)
                {
                    Recycle(result);
                    result = null;
                }
                decryptedBits = 0;
            }

            msg.m_data = result;
            msg.m_bitLength = decryptedBits;
            msg.m_readPosition = 0;
            return success;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
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
