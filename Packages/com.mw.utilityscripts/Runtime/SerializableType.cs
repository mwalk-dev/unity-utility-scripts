using UnityEngine;

namespace Runtime
{
    [System.Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _qualifiedName;

        private System.Type _storedType;

        public SerializableType(System.Type typeToStore)
        {
            _storedType = typeToStore;
        }

        public override string ToString()
        {
            if (_storedType == null)
                return string.Empty;
            return _storedType.Name;
        }

        public void OnBeforeSerialize()
        {
            _qualifiedName = _storedType.AssemblyQualifiedName;
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(_qualifiedName) || _qualifiedName == "null")
            {
                _storedType = null;
                return;
            }
            _storedType = System.Type.GetType(_qualifiedName);
        }

        public static implicit operator System.Type(SerializableType t) => t._storedType;

        public static implicit operator SerializableType(System.Type t) => new(t);
    }
}
