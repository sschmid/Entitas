using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace UnityTest
{

    public class DTOFormatter {
    
        private interface ITransferInterface
        {
            void Transfer(ref ResultDTO.MessageType val);
            void Transfer(ref TestResultState val);
            void Transfer(ref byte val);
            void Transfer(ref bool val);
            void Transfer(ref int val);
            void Transfer(ref float val);
            void Transfer(ref double val);
            void Transfer(ref string val);
        }
        
        private class Writer : ITransferInterface
        {
            private readonly Stream _stream;
            public Writer(Stream stream) { _stream = stream; }
        
            private void WriteConvertedNumber(byte[] bytes)
            {
                if(BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                _stream.Write(bytes, 0, bytes.Length);
            }
        
            public void Transfer(ref ResultDTO.MessageType val) { _stream.WriteByte((byte)val); }
            public void Transfer(ref TestResultState val) { _stream.WriteByte((byte)val); }
            public void Transfer(ref byte val) { _stream.WriteByte(val); }
            public void Transfer(ref bool val) { _stream.WriteByte((byte)(val ? 0x01 : 0x00)); }
            public void Transfer(ref int val) { WriteConvertedNumber(BitConverter.GetBytes(val)); }
            public void Transfer(ref float val) { WriteConvertedNumber(BitConverter.GetBytes(val)); }
            public void Transfer(ref double val) { WriteConvertedNumber(BitConverter.GetBytes(val)); }
            
            public void Transfer(ref string val) 
            {
                var bytes = Encoding.BigEndianUnicode.GetBytes(val);
                int length = bytes.Length;
                Transfer(ref length);
                _stream.Write(bytes, 0, bytes.Length);
            }
        }
    
        private class Reader : ITransferInterface
        {
            private readonly Stream _stream;
            public Reader(Stream stream) { _stream = stream; }
            
            private byte[] ReadConvertedNumber(int size)
            {
                byte[] buffer = new byte[size];
                _stream.Read (buffer, 0, buffer.Length);
                if(BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);
                return buffer;
            }
            
            public void Transfer(ref ResultDTO.MessageType val) { val = (ResultDTO.MessageType)_stream.ReadByte(); }
            public void Transfer(ref TestResultState val) { val = (TestResultState)_stream.ReadByte(); }
            public void Transfer(ref byte val) { val = (byte)_stream.ReadByte(); }
            public void Transfer(ref bool val) { val = (_stream.ReadByte() != 0); }
            public void Transfer(ref int val) { val = BitConverter.ToInt32(ReadConvertedNumber(4), 0); }
            public void Transfer(ref float val) { val = BitConverter.ToSingle(ReadConvertedNumber(4), 0); }
            public void Transfer(ref double val) { val = BitConverter.ToDouble(ReadConvertedNumber(8), 0); }
            
            public void Transfer(ref string val) 
            {
                int length = 0;
                Transfer (ref length);
                var bytes = new byte[length];
                _stream.Read(bytes, 0, length);
                val = Encoding.BigEndianUnicode.GetString(bytes);
            }
        }
        
        private void Transfer(ResultDTO dto, ITransferInterface transfer)
        {
            transfer.Transfer(ref dto.messageType);
            
            transfer.Transfer(ref dto.levelCount);
            transfer.Transfer(ref dto.loadedLevel);
            transfer.Transfer(ref dto.loadedLevelName);
            
            if(dto.messageType == ResultDTO.MessageType.Ping
               || dto.messageType == ResultDTO.MessageType.RunStarted
               || dto.messageType == ResultDTO.MessageType.RunFinished
               || dto.messageType == ResultDTO.MessageType.RunInterrupted
               || dto.messageType == ResultDTO.MessageType.AllScenesFinished)
                return;
                
            transfer.Transfer(ref dto.testName);
            transfer.Transfer(ref dto.testTimeout);
            
            if(dto.messageType == ResultDTO.MessageType.TestStarted)
                return;
            
            if(transfer is Reader)
                dto.testResult = new SerializableTestResult();
            SerializableTestResult str = (SerializableTestResult)dto.testResult;
            
            transfer.Transfer(ref str.resultState);
            transfer.Transfer(ref str.message);
            transfer.Transfer(ref str.executed);
            transfer.Transfer(ref str.name);
            transfer.Transfer(ref str.fullName);
            transfer.Transfer(ref str.id);
            transfer.Transfer(ref str.isSuccess);
            transfer.Transfer(ref str.duration);
            transfer.Transfer(ref str.stackTrace);
        }
    
        public void Serialize (Stream stream, ResultDTO dto)
        {
            Transfer(dto, new Writer(stream));
        }
        
        public object Deserialize (Stream stream)
        {
            var result = (ResultDTO)FormatterServices.GetSafeUninitializedObject(typeof(ResultDTO));
            Transfer (result, new Reader(stream));
            return result;
        }
    }

}